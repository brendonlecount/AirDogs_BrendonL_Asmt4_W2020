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

	[Header("Prefabs")]
	[SerializeField] private GameObject deathExplosionPrefab;
	[SerializeField] private GameObject explosionAudioPrefab;

	[Header("Parameters")]
	[SerializeField] private float torqueFactor;
	[SerializeField] private float rightingTorqueFactor;
	[SerializeField] private float rightingTorqueDamping;
	[SerializeField] private float thrustMax;
	public float ThrustMax => thrustMax;
	[SerializeField] private float healthMax;
	public float HealthMax => healthMax;
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

    // Start is called before the first frame update
    void Start()
    {
		Health = healthMax;
		rb.centerOfMass = Vector3.zero;
		smokeEmission = damageSmoke.emission;
		maxSmoke = smokeEmission.rateOverTimeMultiplier;
		smokeEmission.rateOverTimeMultiplier = 0f;
    }

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(LineOfSightNode.position, LineOfSightRadius);
	}

	// Update is called once per frame
	void FixedUpdate()
    {
		SetPhysicsProperties();
		Vector3 accel = GetThrustAccel() + GetLiftAccel() + Vector3.ClampMagnitude(GetTurnAccel() + GetDriftCounterAccel(), AxialSpeed * AxialSpeed * turnTrackingCoeff);
		Vector3 force = GetDragForce();
		rb.AddForce(accel * rb.mass + force);
		rb.AddTorque(GetTorque(), ForceMode.Acceleration);
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

	private Vector3 GetTorque()
	{
		float rollTarget = Mathf.Clamp(-RelativeAngularVelocity.y * rollFactor, -maxRoll, maxRoll);
		float pitchRate = PitchRate;
		if (Pitch > maxPitch)
		{
			pitchRate = Mathf.Max((maxPitch - Pitch) * pitchLimitFactor, -pitchYawRateLimit);
		}
		else if (Pitch < minPitch)
		{
			pitchRate = Mathf.Min((minPitch - Pitch) * pitchLimitFactor, pitchYawRateLimit);
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
			transform.position -= Vector3.down * (hit.distance - 1.02f);
		}

		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}

	private IEnumerator ExplosionDelayRoutine()
	{
		yield return new WaitForSeconds(explosionDelay);
		gameObject.SetActive(false);
		Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
		Instantiate(explosionAudioPrefab, transform.position, Quaternion.identity);
	}


}
