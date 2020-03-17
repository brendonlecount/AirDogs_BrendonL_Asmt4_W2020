using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispController : MonoBehaviour
{
	[SerializeField] private ParticleSystem wispSystem;
	[SerializeField] private float minWispElevation;
	[SerializeField] private float maxWispElevation;

	private ParticleSystem.EmissionModule wispEmission;
	private float maxEmission;

    // Start is called before the first frame update
    void Start()
    {
		wispEmission = wispSystem.emission;
		maxEmission = wispEmission.rateOverDistanceMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
		wispEmission.rateOverDistanceMultiplier = Mathf.Lerp(0f, maxEmission, (transform.position.y - minWispElevation) / (maxWispElevation - minWispElevation));
    }
}
