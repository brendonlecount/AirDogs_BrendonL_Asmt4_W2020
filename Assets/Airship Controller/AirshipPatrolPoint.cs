using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipPatrolPoint : MonoBehaviour
{
	[SerializeField] private AirshipPatrolPoint nextPoint;
	public AirshipPatrolPoint NextPoint => nextPoint;

	[SerializeField] private float approachSpeed;
	public float ApproachSpeed => approachSpeed;

	[SerializeField] private float holdTime;
	public float HoldTime => holdTime;

	[SerializeField] private bool dropBombs;
	public bool DropBombs => dropBombs;

	public Vector3 Position => transform.position;
}
