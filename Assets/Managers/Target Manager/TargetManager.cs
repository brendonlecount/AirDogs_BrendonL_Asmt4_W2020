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

	public static BiplaneControl GetTargetNearest(Vector3 position, float maxRange, bool isPlayerFaction)
	{
		List<BiplaneControl> targets = isPlayerFaction ? instance.friendlyBiplanes : instance.enemyBiplanes;
		float r2 = maxRange * maxRange;
		float d2Min = Mathf.Infinity;
		BiplaneControl bestTarget = null;
		foreach (BiplaneControl bc in targets)
		{
			float d2New = (bc.transform.position - position).sqrMagnitude;
			if (d2New < d2Min && d2New < r2)
			{
				d2Min = d2New;
				bestTarget = bc;
			}
		}
		return bestTarget;
	}
}
