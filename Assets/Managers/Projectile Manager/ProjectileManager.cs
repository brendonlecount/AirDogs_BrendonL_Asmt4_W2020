using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script manages an expandable pool of projectiles.

public class ProjectileManager : MonoBehaviour
{
	[SerializeField] private GameObject projectilePrefab;
	[SerializeField] private int maxProjectiles;

	private static ProjectileManager instance = null;

	private List<Projectile> projectiles = new List<Projectile>();

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			SpawnProjectiles();
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public static Projectile GetProjectile()
	{
		foreach (Projectile p in instance.projectiles)
		{
			if (!p.Fired)
			{
				return p;
			}
		}
		return instance.SpawnProjectile();
	}

	private void SpawnProjectiles()
	{
		for (int i = 0; i < maxProjectiles; i++)
		{
			SpawnProjectile();
		}
	}

	private Projectile SpawnProjectile()
	{
		Projectile p = GameObject.Instantiate(projectilePrefab, transform).GetComponent<Projectile>();
		projectiles.Add(p);
		return p;
	}
}
