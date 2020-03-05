using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPointManager : MonoBehaviour
{
	private static PatrolPointManager instance = null;

	private void Awake()
	{
		instance = this;
	}
}
