using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
	private static TargetManager instance = null;

	private List<BiplaneControl> enemyBiplanes = new List<BiplaneControl>();
	private List<BiplaneControl> friendlyBiplanes = new List<BiplaneControl>();

	private void Awake()
	{
		instance = this;
		BiplaneControl[] allBiplanes = FindObjectsOfType<BiplaneControl>();
		foreach (BiplaneControl bc in allBiplanes)
		{
			if (!bc.IsPlayerFaction)
			{
				enemyBiplanes.Add(bc);
			}
			else
			{
				friendlyBiplanes.Add(bc);
			}
		}
	}

	public static List<BiplaneControl> GetEnemyBiplanes()
	{
		return instance.enemyBiplanes;
	}

	public static List<BiplaneControl> GetFriendlyBiplanes()
	{
		return instance.friendlyBiplanes;
	}
}
