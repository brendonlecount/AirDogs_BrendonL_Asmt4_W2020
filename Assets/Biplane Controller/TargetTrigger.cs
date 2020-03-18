using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
