using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script associates a gameobject implementing the IDamageable interface with a trigger collider used for taking damage from explosions and projectiles.

public interface IDamageable
{
	void TakeDamage(float damage);
}

public class TargetTrigger : MonoBehaviour
{
	[SerializeField] private GameObject damageableObject;

	private IDamageable damageable;
	public IDamageable Damageable => damageable;

	private void Start()
	{
		damageable = damageableObject.GetComponent<IDamageable>();
		if (damageable == null)
		{
			Debug.Log("IDamageable not found on " + damageableObject.name);
		}
	}
}
