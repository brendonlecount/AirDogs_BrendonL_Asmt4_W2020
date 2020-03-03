using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
	private static TargetManager instance = null;

	private List<BiplaneController> enemyBiplanes = new List<BiplaneController>();

	private void Awake()
	{
		instance = this;
		BiplaneController[] allBiplanes = FindObjectsOfType<BiplaneController>();
		foreach (BiplaneController bc in allBiplanes)
		{
			if (bc.GetComponent<EnemyAI>() != null)
			{
				enemyBiplanes.Add(bc);
			}
		}
	}

	public static List<BiplaneController> GetEnemyBiplanes()
	{
		return instance.enemyBiplanes;
	}
}
