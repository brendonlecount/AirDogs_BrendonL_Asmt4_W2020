using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script is a character controller for a turret bomb that explodes once it's within range of a TargetTrigger.

public class TurretBomb : MonoBehaviour, IDamageable
{
	[SerializeField] private Rigidbody rb;
	[SerializeField] private float speed;
	[SerializeField] private float damage;
	[SerializeField] private float explosionRadius;
	[SerializeField] private float rangeMin = 20f;
	[SerializeField] private float rangeMax = 500f;
	[SerializeField] private GameObject explosionEffect;
	[SerializeField] private TargetTrigger myTargetTrigger;

	public float Speed => speed;

	private float lifetime = Mathf.Infinity;
	private float armTime = Mathf.Infinity;
	private float lifeTimer = 0f;
	private bool exploded = false;

	public void StartTrajectory(Vector3 initialVelocity)
	{
		armTime = rangeMin / speed;
		lifetime = rangeMax / speed;
		rb.velocity = transform.forward * speed + initialVelocity;
	}

	private void FixedUpdate()
	{
		lifeTimer += Time.deltaTime;
		if (lifeTimer >= lifetime)
		{
			Explode();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (lifeTimer >= armTime)
		{
			Explode();
		}
	}

	private void Explode()
	{
		if (!exploded)
		{
			exploded = true;
			GameObject.Instantiate(explosionEffect, transform.position, Quaternion.identity);
			List<IDamageable> damageTargets = new List<IDamageable>();
			Collider[] targetTriggers = Physics.OverlapSphere(transform.position, explosionRadius, LayerMaskManager.TargetTriggerMask);
			foreach (Collider collider in targetTriggers)
			{
				TargetTrigger tt = collider.GetComponent<TargetTrigger>();
				if (tt != null && tt != myTargetTrigger)
				{
					if (!damageTargets.Contains(tt.Damageable))
					{
						damageTargets.Add(tt.Damageable);
					}
				}
			}
			foreach (IDamageable damageable in damageTargets)
			{
				damageable.TakeDamage(damage);
			}
			Destroy(gameObject);
		}
	}

	public void TakeDamage(float damage)
	{
		Explode();
	}
}
