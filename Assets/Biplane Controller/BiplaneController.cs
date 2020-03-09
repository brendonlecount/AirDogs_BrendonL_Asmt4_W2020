using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiplaneController : MonoBehaviour
{
	[Header("Components")]
	[SerializeField] private Rigidbody rb;
	public Rigidbody Rb => rb;
	[SerializeField] private PilotController pilot;
	[SerializeField] private ParticleSystem damageSmoke;
	[SerializeField] private AudioSource propAudio;
	[SerializeField] private Transform firstPersonNode;
	public Transform FirstPersonNode => firstPersonNode;
	[SerializeField] private Transform lineOfSightNode;
	public Transform LineOfSightNode => lineOfSightNode;
	[SerializeField] private SphereCollider[] gearColliders;
	[SerializeField] private Transform centerOfGravityNode;
	[SerializeField] private Transform centerOfLiftNode;
	[SerializeField] private Transform centerOfDragNode;
	[SerializeField] private Transform centerOfThrustNode;

	[Header("Prefabs")]
	[SerializeField] private GameObject deathExplosionPrefab;
	[SerializeField] private GameObject explosionAudioPrefab;
	[SerializeField] private GameObject predictiveCollisionPrefab;

	[Header("Parameters")]
	[SerializeField] private float torqueFactor;
	[SerializeField] private float rightingTorqueFactor;
	[SerializeField] private float rightingTorqueDamping;
	[SerializeField] private float thrustMax;
	public float ThrustMax => thrustMax;
	[SerializeField] private float healthMax;
	public float HealthMax => healthMax;
	[SerializeField] private float damageVelocityMin;
	[SerializeField] private float damageVelocityMax;
	[SerializeField] private float liftCoefficient;
	[SerializeField] private float turnTrackingCoeff;
	[SerializeField] private float dragCoeff;
	[SerializeField] private float driftCoeff;
	[Range(1f, 5f)]
	[SerializeField] private float neutralLift;
	[SerializeField] private float rollFactor;
	[SerializeField] private float maxRoll;
	[SerializeField] private float maxPitch;
	[SerializeField] private float minPitch;
	[SerializeField] private float pitchLimitFactor;
	[SerializeField] private float pitchYawRateLimit;
	public float PitchYawRateLimit => pitchYawRateLimit;
	[SerializeField] private float lineOfSightRadius;
	public float LineOfSightRadius => lineOfSightRadius;
	[SerializeField] private float explosionDelay;
	[SerializeField] private float predictiveCollisionTime;
	public float PredictiveCollisionTime => predictiveCollisionTime;

	[Header("Sound")]
	[Range(0f, 1f)]
	[SerializeField] private float propVolumeMin;
	[Range(0f, 1f)]
	[SerializeField] private float propVolumeMax;
	[Range(1f, 3f)]
	[SerializeField] private float propPitchMin;
	[Range(1f, 3f)]
	[SerializeField] private float propPitchMax;

	private const float G = 9.81f;

	// Input variables
	public float YawRate
	{
		get { return yawRate; }
		set
		{
			yawRate = Mathf.Clamp(value, -pitchYawRateLimit, pitchYawRateLimit);
		}
	}
	private float yawRate = 0f;

	public float PitchRate
	{
		get { return pitchRate; }
		set
		{
			pitchRate = Mathf.Clamp(value, -pitchYawRateLimit, pitchYawRateLimit);
		}
	}
	private float pitchRate = 0f;

	public float Thrust
	{
		get
		{
			return thrust;
		}
		set
		{
			thrust = Mathf.Clamp(value, 0f, thrustMax);
			propAudio.volume = Mathf.Lerp(propVolumeMin, propVolumeMax, thrust / thrustMax);
			propAudio.pitch = Mathf.Lerp(propPitchMin, propPitchMax, thrust / thrustMax);
		}
	}
	private float thrust = 0f;

	// physics properties
	public float Speed { get; private set; } = 0f;
	public Vector3 RelativeVelocity { get; private set; } = Vector3.zero;
	public float AxialSpeed => RelativeVelocity.z;
	public float LateralSpeed => RelativeVelocity.x;
	public float VerticalSpeed => RelativeVelocity.y;
	public Vector3 RelativeAngularVelocity { get; private set; } = Vector3.zero;
	public float Roll { get; private set; }
	public float Pitch { get; private set; }
	public float Yaw { get; private set; }
	public float Elevation { get; private set; }

	public float Health { get; private set; }
	public bool IsDead { get; private set; } = false;
	public bool IsStalling { get; private set; } = true;

	private float maxSmoke;
	private ParticleSystem.EmissionModule smokeEmission;

	public delegate void DeathDelegate(PilotController pilot);
	public event DeathDelegate onDeath;

	private bool wasGrounded = false;
	private Vector3 lastVelocity = Vector3.zero;
	private Transform predictiveCollision;
	private CapsuleCollider predictiveCollisionCol;
	private Vector3 predictiveCollisionScale;

	public CapsuleCollider PredictiveCollisionCol => predictiveCollisionCol;


	private void Awake()
	{
		Health = healthMax;
		rb.centerOfMass = centerOfGravityNode.localPosition;
		smokeEmission = damageSmoke.emission;
		maxSmoke = smokeEmission.rateOverTimeMultiplier;
		smokeEmission.rateOverTimeMultiplier = 0f;
		predictiveCollision = Instantiate(predictiveCollisionPrefab).transform;
		predictiveCollisionCol = predictiveCollision.GetComponent<CapsuleCollider>();
		PredictiveCollisionCol.height = lineOfSightRadius * 2f;
		PredictiveCollisionCol.radius = lineOfSightRadius;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(LineOfSightNode.position, LineOfSightRadius);
	}

	// Update is called once per frame
	void FixedUpdate()
    {
		if (wasGrounded)
		{
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			PitchRate = 0f;
			YawRate = 0f;
			wasGrounded = false;
		}
		SetPhysicsProperties();
		UpdatePredictiveCollision();
		CheckForCrash();
		rb.AddForceAtPosition(GetThrustAccel(), centerOfThrustNode.position, ForceMode.Acceleration);
		rb.AddForceAtPosition(GetLiftAccel(), centerOfLiftNode.position, ForceMode.Acceleration);
		rb.AddForceAtPosition(Vector3.ClampMagnitude(GetTurnAccel() + GetDriftCounterAccel(), AxialSpeed * AxialSpeed * turnTrackingCoeff), centerOfGravityNode.position, ForceMode.Acceleration);
		rb.AddForceAtPosition(GetDragForce(), centerOfDragNode.position, ForceMode.Force);
		rb.AddTorque(GetTorque(), ForceMode.Acceleration);
	}

	private void UpdatePredictiveCollision()
	{
		predictiveCollisionCol.height = Speed * predictiveCollisionTime + 2f * LineOfSightRadius;
		predictiveCollision.position = transform.position + 0.5f * rb.velocity * predictiveCollisionTime;
		predictiveCollision.rotation = Quaternion.LookRotation(rb.velocity);
	}

	private void SetPhysicsProperties()
	{
		RelativeVelocity = Quaternion.Inverse(transform.rotation) * rb.velocity;
		Speed = RelativeVelocity.magnitude;
		RelativeAngularVelocity = Quaternion.Inverse(transform.rotation) * rb.angularVelocity;
		Roll = Vector3.SignedAngle(Vector3.up, transform.up, transform.forward);
		float pitch = -Mathf.Asin(transform.forward.y) * Mathf.Rad2Deg;
		while(pitch > 180f)
		{
			pitch -= 360f;
		}
		while (pitch < -180f)
		{
			pitch += 360f;
		}
		Pitch = pitch;
		Yaw = Vector3.SignedAngle(Vector3.forward, transform.forward, Vector3.up);
		Elevation = transform.position.y;
	}

	private void CheckForCrash()
	{
		Vector3 deltaVelocity = rb.velocity - lastVelocity;
		lastVelocity = rb.velocity;
		float velocityDelta = deltaVelocity.magnitude;
		if (velocityDelta > damageVelocityMin)
		{
			Debug.Log(name + " crashed with delta " + velocityDelta);
			TakeDamage(Mathf.Lerp(0f, healthMax, (velocityDelta - damageVelocityMin) / (damageVelocityMax - damageVelocityMin)));
		}
	}

	private Vector3 GetTorque()
	{
		float rollTarget = Mathf.Clamp(-RelativeAngularVelocity.y * rollFactor, -maxRoll, maxRoll);
		float pitchRate = PitchRate;
		if (Pitch > maxPitch)
		{
			pitchRate = Mathf.Max((0.9f * maxPitch - Pitch) * pitchLimitFactor, -pitchYawRateLimit);
		}
		else if (Pitch < minPitch)
		{
			pitchRate = Mathf.Min((0.9f * minPitch - Pitch) * pitchLimitFactor, pitchYawRateLimit);
		}
		Vector3 targetAngularVelocity = new Vector3(pitchRate, YawRate, (rollTarget - Roll) * rightingTorqueFactor - RelativeAngularVelocity.z * rightingTorqueDamping);
		return transform.rotation * ((targetAngularVelocity - RelativeAngularVelocity) * torqueFactor);
	}

	private Vector3 GetTurnAccel()
	{
		Vector3 axialVelocity = Vector3.forward * AxialSpeed;
		return transform.rotation * Vector3.Cross(RelativeAngularVelocity, axialVelocity);
	}

	private Vector3 GetDriftCounterAccel()
	{
		Vector3 drift = new Vector3(LateralSpeed, VerticalSpeed, 0f);
		return transform.rotation * (-driftCoeff * drift);
	}

	private Vector3 GetThrustAccel()
	{
		return Thrust * G * transform.forward;
	}

	private Vector3 GetLiftAccel()
	{
		Vector3 lift = transform.up;
		lift *= 1f / lift.y;
		float liftLimit = AxialSpeed * AxialSpeed * liftCoefficient;
		float liftMagnitude = lift.magnitude;
		IsStalling = liftLimit < liftMagnitude;
		if (IsStalling)
		{
			lift *= liftLimit / liftMagnitude;
		}
		lift *= G;
		return lift;
	}

	private Vector3 GetDragForce()
	{
		return -dragCoeff * Speed * rb.velocity;
	}

	public void TakeDamage(float damage)
	{
		if (!IsDead && damage > 0f)
		{
			if (damage >= Health)
			{
				Die();
			}
			else
			{
				Health -= damage;
				Debug.Log(name + " damaged! " + Health);
				smokeEmission.rateOverTimeMultiplier = Mathf.Lerp(maxSmoke, 0f, Health / healthMax);
			}
		}
	}

	public void Die()
	{
		Health = 0f;
		IsDead = true;
		Debug.Log(name + " died!");
		pilot.Eject(rb.velocity);
		onDeath?.Invoke(pilot);
		StartCoroutine(ExplosionDelayRoutine());
	}

	public void GroundPlane()
	{
		Debug.Log("Grounding plane " + name);
		Vector3 eulers = transform.rotation.eulerAngles;
		eulers.x = 0f;
		eulers.z = 0f;
		transform.rotation = Quaternion.Euler(eulers);
		Vector3 gearOffset = 0.5f * (gearColliders[1].transform.position + gearColliders[2].transform.position) - gearColliders[0].transform.position;
		Quaternion groundedRotation = Quaternion.FromToRotation(gearOffset, transform.forward);
		Debug.Log(groundedRotation.eulerAngles);
		transform.rotation = groundedRotation * transform.rotation;

		RaycastHit hit;
		if (Physics.SphereCast(gearColliders[0].transform.position + Vector3.up, gearColliders[0].radius, Vector3.down, out hit, Mathf.Infinity, LayerMaskManager.GroundMask))
		{
			transform.position -= Vector3.down * (hit.distance - 1f);
		}

		wasGrounded = true;
	}

	private IEnumerator ExplosionDelayRoutine()
	{
		yield return new WaitForSeconds(explosionDelay);
		gameObject.SetActive(false);
		Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
		Instantiate(explosionAudioPrefab, transform.position, Quaternion.identity);
	}


}
