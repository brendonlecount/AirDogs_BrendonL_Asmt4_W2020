using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script was an early attempt at causing a biplane to explode upon colliding, not ultimately used (I'm pretty sure...)

public class BiplaneCollision : MonoBehaviour
{
	[SerializeField] private BiplaneController controller;

	private void OnCollisionEnter(Collision collision)
	{
		controller.Die();
	}
}
