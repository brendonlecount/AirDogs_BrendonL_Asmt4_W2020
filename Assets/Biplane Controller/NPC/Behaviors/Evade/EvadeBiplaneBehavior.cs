using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script implements the evasion biplane AI state.

public class EvadeBiplaneBehavior : BiplaneBehavior
{
	[SerializeField] private float evadeTimeMin;
	[SerializeField] private float evadeTimeMax;
	private float RandomEvadeTime => Random.Range(evadeTimeMin, evadeTimeMax);

	private float RandomRawRate => Random.Range(-behaviorProfile.HandlingLimit * controller.PitchYawRateLimit, behaviorProfile.HandlingLimit * controller.PitchYawRateLimit);

	private float evadeElevation;

	public override BiplaneBehaviorCode GetBehaviorCode()
	{
		return BiplaneBehaviorCode.Evade;
	}

	public override string GetBehaviorName()
	{
		return "Evading";
	}

	public override void EnterBehavior()
	{
		StartCoroutine(EvadeRoutine());
		Debug.Log(controller.name + " is evading.");
	}

	public override BiplaneBehaviorCode ExecuteBehavior()
	{
		if (controller.IsDead)
		{
			return BiplaneBehaviorCode.Dead;
		}
		BiplaneControl aggroTarget = TargetManager.GetTargetNearest(biplaneTransform.position, behaviorProfile.LoseAggroRadius, !biplaneAI.IsPlayerFaction);
		if (aggroTarget == null || aggroTarget.Controller.IsDead)
		{
			return GetDefaultBehavior();
		}
		else if (IsInFront(aggroTarget) || GetRange(aggroTarget) > behaviorProfile.StopEvadeRadius)
		{
			return BiplaneBehaviorCode.Attack;
		}
		else
		{
			controller.PitchRate = GetPitchRateFromElevation(evadeElevation);
			return BiplaneBehaviorCode.Evade;
		}
	}

	public override void ExitBehavior()
	{
		StopAllCoroutines();
	}

	private IEnumerator EvadeRoutine()
	{
		while (true)
		{
			evadeElevation = behaviorProfile.ElevationRandom;
			controller.YawRate = RandomRawRate;
			yield return new WaitForSeconds(RandomEvadeTime);
		}
	}
}
