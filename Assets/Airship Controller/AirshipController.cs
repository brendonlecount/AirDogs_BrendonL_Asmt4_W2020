using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipController : MonoBehaviour, IDamageable
{
	[SerializeField] private Rigidbody rb;
	[SerializeField] private Transform bombNode;
	[SerializeField] private Transform[] deathExplosionNodes;
	[SerializeField] private AirshipPatrolPoint patrolPoint;
	[SerializeField] private GameObject bombPrefab;
	[SerializeField] private GameObject deathExplosionPrefab;
	[SerializeField] private GameObject deathExplosionAudioPrefab;
	[SerializeField] private float approachTolerance;
	[SerializeField] private float accelCoeff;
	[SerializeField] private float rotationRate;
	[SerializeField] private float bombInterval;
	[SerializeField] private float maxHealth;
	[SerializeField] private float damageReduction;
	[SerializeField] private float deathExplosionDelayMin;
	[SerializeField] private float deathExplosionDelayMax;
	[SerializeField] private float finalExplosionScale;

	public float HealthFraction => health / maxHealth;

	private static AirshipController instance;
	public static AirshipController Instance => instance;

	private Coroutine holdRoutine;
	private Coroutine bombRoutine;
	private Vector3 targetVelocity;
	private float targetYaw;
	private float health;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		health = maxHealth;
	}

	

	private void FixedUpdate()
	{
		if (patrolPoint == null)
		{
			targetVelocity = Vector3.zero;
		}
		else if (holdRoutine == null)
		{
			Vector3 targetHeading = patrolPoint.Position - transform.position;
			if (targetHeading.sqrMagnitude < approachTolerance * approachTolerance)
			{
				holdRoutine = StartCoroutine(HoldRoutine(patrolPoint.HoldTime));
				targetVelocity = Vector3.zero;
			}
			else
			{
				targetVelocity = targetHeading.normalized * patrolPoint.ApproachSpeed;
				targetYaw = Quaternion.LookRotation(rb.velocity).eulerAngles.y;
			}
		}
		rb.AddForce((targetVelocity - rb.velocity) * accelCoeff, ForceMode.Acceleration);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, targetYaw, 0f), rotationRate * Time.deltaTime);
	}

	private void DropBomb()
	{
		Bomb bomb = Instantiate(bombPrefab, bombNode.position, bombNode.rotation).GetComponent<Bomb>();
		bomb.Rb.velocity = rb.velocity;
	}

	private IEnumerator HoldRoutine(float duration)
	{
		if (patrolPoint.DropBombs)
		{
			bombRoutine = StartCoroutine(BombRoutine());
		}

		yield return new WaitForSeconds(duration);

		if (bombRoutine != null)
		{
			StopCoroutine(bombRoutine);
			bombRoutine = null;
		}
		holdRoutine = null;
		patrolPoint = patrolPoint.NextPoint;
	}

	private void Die()
	{
		Debug.Log("Airship destroyed!");
		StartCoroutine(DeathRoutine());
	}

	private IEnumerator BombRoutine()
	{
		while (true)
		{
			DropBomb();
			yield return new WaitForSeconds(bombInterval);
		}
	}

	public void TakeDamage(float damage)
	{
		damage -= damageReduction;
		if (damage > 0f)
		{
			Debug.Log("Airship damaged! " + health.ToString("N0"));
			health -= damage;
			if (health < 0f)
			{
				Die();
			}
		}
	}

	private IEnumerator DeathRoutine()
	{
		foreach(Transform node in deathExplosionNodes)
		{
			Instantiate(deathExplosionPrefab, node.position, Quaternion.identity);
			Instantiate(deathExplosionAudioPrefab, node.position, Quaternion.identity);
			yield return new WaitForSeconds(Random.Range(deathExplosionDelayMin, deathExplosionDelayMax));
		}
		gameObject.SetActive(false);
		GameObject go = Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
		go.transform.localScale = go.transform.localScale * finalExplosionScale;
		Instantiate(deathExplosionAudioPrefab, transform.position, Quaternion.identity);
	}
}
