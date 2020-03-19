using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script was going to manage patrol points, but was not ultimately used.

public class PatrolPointManager : MonoBehaviour
{
	private static PatrolPointManager instance = null;

	private void Awake()
	{
		instance = this;
	}
}
