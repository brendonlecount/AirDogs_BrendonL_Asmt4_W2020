using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField] private float speed;
	[SerializeField] private float damage;
	[SerializeField] private float drag;
	[SerializeField] private float lifetime;
	[SerializeField] private TrailRenderer trail;
	[SerializeField] private bool useGravity;
	[SerializeField] private bool useDrag;

	public float Speed => speed;
	public float Damage => damage;

	public bool Fired { get; private set; } = false;

	private float lifeTimer;
	private Vector3 velocity;
	private float lastSpeed;
	private const float gravity = 9.81f;

	private void Awake()
	{
		gameObject.SetActive(false);
	}

	private void FixedUpdate()
    {
        if (Fired)
		{
			lifeTimer += Time.deltaTime;
			if (lifeTimer >= lifetime)
			{
				KillProjectile();
			}
			else
			{
				CastMove();
			}
		}
    }

	public void FireProjectile(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity)
	{
		transform.position = initialPosition;
		transform.rotation = initialRotation;
		velocity = initialVelocity + initialRotation * Vector3.forward * speed;
		lastSpeed = velocity.magnitude;
		lifeTimer = 0f;
		Fired = true;
		trail.Clear();
		gameObject.SetActive(true);
	}

	private void CastMove()
	{
		velocity += (useGravity ? Vector3.down * gravity * Time.deltaTime : Vector3.zero) - (useDrag ? velocity * lastSpeed * drag : Vector3.zero);
		lastSpeed = velocity.magnitude;
		RaycastHit hit;
		if (Physics.Raycast(transform.position, velocity, out hit, lastSpeed * Time.deltaTime, LayerMaskManager.BlocksProjectileMask, QueryTriggerInteraction.Collide))
		{
			if (hit.collider.CompareTag("Target"))
			{
				TargetTrigger tt = hit.collider.GetComponent<TargetTrigger>();
				if (tt != null)
				{
					if (tt.Damageable != null)
					{
						//Debug.Log(tt.name + " took damage.");
						tt.Damageable.TakeDamage(damage);
					}
					else
					{
						Debug.Log("Controller not set on Target Trigger " + hit.collider.name);
					}
				}
				else
				{
					Debug.Log("Collider " + hit.collider.name + " tagged Target missing TargetTrigger component!");
				}
			}
			KillProjectile();
		}
		else
		{
			transform.position += velocity * Time.deltaTime;
		}
	}

	public void KillProjectile()
	{
		gameObject.SetActive(false);
		Fired = false;
	}
}
