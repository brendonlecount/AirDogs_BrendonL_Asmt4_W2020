using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
