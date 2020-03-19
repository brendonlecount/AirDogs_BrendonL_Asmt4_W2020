using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script implements the IDamageable interface for a destructible building.

public class Building : MonoBehaviour, IDamageable
{
	[SerializeField] private GameObject deathExplosionPrefab;
	[SerializeField] private GameObject deathExplosionAudioPrefab;
	[SerializeField] private float deathExplosionScale;

	public void TakeDamage(float damage)
	{
		if (damage > 50f)
		{
			GameObject go = Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
			go.transform.localScale = go.transform.localScale * deathExplosionScale;
			Instantiate(deathExplosionAudioPrefab, transform.position, Quaternion.identity);
			gameObject.SetActive(false);
		}
	}
}
