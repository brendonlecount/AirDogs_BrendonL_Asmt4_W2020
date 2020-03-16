using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, IDamageable
{
	[SerializeField] private GameObject explosionPrefab;
	[SerializeField] private GameObject explosionAudioPrefab;
	[SerializeField] private Rigidbody rb;
	public Rigidbody Rb => rb;
	[SerializeField] private TargetTrigger myTargetTrigger;
	[SerializeField] private float armTime;
	[SerializeField] private float damage;
	[SerializeField] private float explosionRadius;

	private float armTimer = 0f;
	private bool exploded = false;

	private void FixedUpdate()
	{
		transform.LookAt(transform.position + Rb.velocity);
		armTimer += Time.deltaTime;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (armTimer >= armTime)
		{
			Explode();
		}
	}


	private void Explode()
	{
		if (!exploded)
		{
			exploded = true;
			GameObject.Instantiate(explosionPrefab, transform.position, Quaternion.identity);
			GameObject.Instantiate(explosionAudioPrefab, transform.position, Quaternion.identity);
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
