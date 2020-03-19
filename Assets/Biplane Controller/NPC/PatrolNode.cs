using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script implements a patrol node used by BiplaneAI patroling.

public class PatrolNode : MonoBehaviour
{
	[SerializeField] private PatrolNode nextNode;
	public PatrolNode NextNode => nextNode;

	public Vector3 Position => transform.position;

	private void OnDrawGizmos()
	{
		if (nextNode == null)
		{
			Gizmos.color = Color.red;
		}
		else
		{
			Gizmos.color = Color.green;
		}
		Gizmos.DrawWireSphere(transform.position, 10f);
	}
}
