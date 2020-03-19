using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script destroys an audio source prefab when it is done playing.

public class KillAudio : MonoBehaviour
{
	[SerializeField] private AudioSource source;

    void Update()
    {
		if (!source.isPlaying)
		{
			gameObject.SetActive(false);
			Destroy(gameObject);
		}
    }
}
