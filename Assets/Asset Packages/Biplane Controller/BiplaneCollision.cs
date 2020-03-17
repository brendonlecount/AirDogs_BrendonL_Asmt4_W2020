using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiplaneCollision : MonoBehaviour
{
	[SerializeField] private BiplaneController controller;

	private void OnCollisionEnter(Collision collision)
	{
		controller.Die();
	}
}
