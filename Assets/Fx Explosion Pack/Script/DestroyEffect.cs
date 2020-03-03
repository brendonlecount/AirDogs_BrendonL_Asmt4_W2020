using UnityEngine;
using System.Collections;

// Brendon LeCount 3/3/2020
// Rewritten to destroy the particle effect once all systems have finished playing.

public class DestroyEffect : MonoBehaviour {
	private ParticleSystem[] systems;

	private void Start()
	{
		systems = GetComponentsInChildren<ParticleSystem>();
	}

	void Update ()
	{

		//		if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.C))
		//		   Destroy(transform.gameObject);

		bool allDead = true;
		foreach(ParticleSystem ps in systems)
		{
			if (ps.isPlaying)
			{
				allDead = false;
				break;
			}
		}
		if (allDead)
		{
			Destroy(gameObject);
		}
	}
}
