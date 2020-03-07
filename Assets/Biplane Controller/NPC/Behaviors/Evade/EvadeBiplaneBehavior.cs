using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		BiplaneControl aggroTarget = TargetManager.GetTargetNearest(biplaneTransform.position, behaviorProfile.LoseAggroRadius, !biplaneAI.IsPlayerFaction);
		if (aggroTarget == null || aggroTarget.Controller.IsDead)
		{
			return GetDefaultBehavior();
		}
		else if (IsInFront(aggroTarget))
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
