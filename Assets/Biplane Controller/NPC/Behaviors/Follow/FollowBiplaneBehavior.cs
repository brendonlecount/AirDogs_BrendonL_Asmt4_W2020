using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBiplaneBehavior : BiplaneBehavior
{
	[SerializeField] private float followDistance;

	public override void EnterBehavior()
	{
		Debug.Log(controller.name + " following " + aiTarget.name);
	}

	public override BiplaneBehaviorCode ExecuteBehavior()
	{
		if (aiTargetControl == null || aiTargetControl.Controller.IsDead)
		{
			return BiplaneBehaviorCode.Patrol;
		}
		else
		{
			BiplaneControl aggroTarget = TargetManager.GetTargetNearest(biplaneTransform.position, behaviorProfile.AggroRadius, !biplaneAI.IsPlayerFaction);
			if (aggroTarget != null && !aggroTarget.Controller.IsDead)
			{
				if (IsInFront(aggroTarget))
				{
					return BiplaneBehaviorCode.Attack;
				}
				else
				{
					return BiplaneBehaviorCode.Evade;
				}
			}
			else
			{
				Vector3 followTargetHeading = aiTarget.transform.position - biplaneTransform.position;
				controller.YawRate = GetYawRateFromHeading(followTargetHeading);
				controller.PitchRate = GetPitchRateFromHeading(followTargetHeading);
				controller.Thrust = GetFollowThrust(aiTargetControl, followDistance);
				return BiplaneBehaviorCode.Follow;
			}
		}
	}

	public override void ExitBehavior()
	{
	}

	public override BiplaneBehaviorCode GetBehaviorCode()
	{
		return BiplaneBehaviorCode.Follow;
	}

	public override string GetBehaviorName()
	{
		return "Following";
	}
}
