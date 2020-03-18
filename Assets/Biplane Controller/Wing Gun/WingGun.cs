using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingGun : MonoBehaviour
{
	[SerializeField] private Rigidbody rb;
	[SerializeField] private GameObject projectilePrefab;
	[SerializeField] private Light muzzleFlashEffect;
	[SerializeField] private AudioSource fireAudioSource;
	[SerializeField] private float fireDelay;
	[Range(0f, 1f)]
	[SerializeField] private float spread;
	[SerializeField] private float flashFadeRate;

	private Coroutine fireRoutine;
	private float flashIntensity;
	private Projectile projectile;

	public float ProjectileSpeed => projectile.Speed;

	private void Start()
	{
		flashIntensity = muzzleFlashEffect.intensity;
		projectile = projectilePrefab.GetComponent<Projectile>();
	}

	private void Update()
	{
		muzzleFlashEffect.intensity = Mathf.Lerp(muzzleFlashEffect.intensity, 0f, flashFadeRate * Time.deltaTime);
	}

	public bool Firing
	{
		get { return firing; }

		set
		{
			firing = value;
			if (firing && fireRoutine == null)
			{
				fireAudioSource.Play();
				fireRoutine = StartCoroutine(FireRoutine());
			}
			else if (!firing && fireRoutine != null)
			{
				fireAudioSource.Stop();
				StopCoroutine(fireRoutine);
				fireRoutine = null;
			}
		}
	}
	private bool firing;

	private IEnumerator FireRoutine()
	{
		while (true)
		{
			Fire();
			yield return new WaitForSeconds(fireDelay);
		}
	}

	private void Fire()
	{
		Projectile p = ProjectileManager.GetProjectile();
		Quaternion spreadRotation = Quaternion.Lerp(Quaternion.identity, Quaternion.FromToRotation(Vector3.forward, Random.onUnitSphere), Random.Range(0f, spread));
		p.FireProjectile(transform.position, spreadRotation * transform.rotation, rb.velocity);
		muzzleFlashEffect.intensity = flashIntensity;
	}
}
