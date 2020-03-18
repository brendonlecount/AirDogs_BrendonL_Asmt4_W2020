using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
