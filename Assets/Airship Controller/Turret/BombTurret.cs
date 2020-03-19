using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script is a character controller and AI for a turret that launches bombs.

public class BombTurret : MonoBehaviour, IDamageable
{
	[SerializeField] private GameObject deathExplosionPrefab;
	[SerializeField] private GameObject explosionAudioPrefab;
	[SerializeField] private TurretBomb turretBombPrefab;
	[SerializeField] private Transform bombNode;
	[SerializeField] private Transform pivotNode;
	[SerializeField] private float fireDelayMin;
	[SerializeField] private float fireDelayMax;
	private float fireDelay => Random.Range(fireDelayMin, fireDelayMax);
	[SerializeField] private Rigidbody parentRb;
	[SerializeField] private float aggroRadius;
	[SerializeField] private float loseAggroRadius;
	[SerializeField] private float rotationRate;
	[SerializeField] private float pitchMin;
	[SerializeField] private float pitchMax;
	[SerializeField] private float fireAngleMax;
	[SerializeField] private float maxHealth;

	private BiplaneControl aggroTarget = null;
	private bool isOnTarget = false;
	private bool isDead = false;
	private float health;

    // Start is called before the first frame update
    void Start()
    {
		health = maxHealth;
		StartCoroutine(FireRoutine());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if (aggroTarget != null && (aggroTarget.transform.position - transform.position).sqrMagnitude > loseAggroRadius * loseAggroRadius)
		{
			aggroTarget = null;
		}
		if (aggroTarget == null)
		{
			aggroTarget = TargetManager.GetTargetNearest(transform.position, aggroRadius, true);
		}
		if (aggroTarget != null)
		{
			TrackTarget();
		}
    }

	private void TrackTarget()
	{
		float timeToTarget = (aggroTarget.transform.position - transform.position).magnitude / turretBombPrefab.Speed;
		Vector3 aimPoint = aggroTarget.transform.position + aggroTarget.Controller.Rb.velocity * timeToTarget;
		Quaternion aimRotation = Quaternion.LookRotation(aimPoint - pivotNode.position, Vector3.up);
		Vector3 aimEulers = aimRotation.eulerAngles;
		aimRotation = Quaternion.Euler(Mathf.Clamp(aimEulers.x, pitchMin, pitchMax), aimEulers.y, aimEulers.z);
		pivotNode.rotation = Quaternion.RotateTowards(pivotNode.rotation, aimRotation, rotationRate * Time.deltaTime);
		isOnTarget = Vector3.Angle(pivotNode.forward, aimPoint - pivotNode.position) <= fireAngleMax;
	}

	private void Fire()
	{
		TurretBomb tb = Instantiate(turretBombPrefab, bombNode.position, bombNode.rotation).GetComponent<TurretBomb>();
		tb.StartTrajectory(parentRb.velocity);
	}

	IEnumerator FireRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(fireDelay);
			if (aggroTarget != null)
			{
				if (isOnTarget)
				{
					Fire();
				}
			}
		}
	}

	public void TakeDamage(float damage)
	{
		if (health > damage)
		{
			health -= damage;
		}
		else
		{
			health = 0f;
			gameObject.SetActive(false);
			Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
			Instantiate(explosionAudioPrefab, transform.position, Quaternion.identity);
		}
	}
}
