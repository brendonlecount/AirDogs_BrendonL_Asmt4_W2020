using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBiplaneBehavior : BiplaneBehavior
{
	[SerializeField] private float attackDistance;
	[SerializeField] private float fireRange;
	[SerializeField] private float fireAngle;

	private BiplaneControl aggroTarget;

	public override BiplaneBehaviorCode GetBehaviorCode()
	{
		return BiplaneBehaviorCode.Attack;
	}

	public override void EnterBehavior()
	{
		aggroTarget = TargetManager.GetTargetNearest(biplaneTransform.position, behaviorProfile.LoseAggroRadius, !biplaneAI.IsPlayerFaction);
		if (aggroTarget == null)
		{
			Debug.Log(controller.name + " attacking null target!");
		}
		else
		{
			Debug.Log(controller.name + " attacking " + aggroTarget.name);
		}
	}

	public override BiplaneBehaviorCode ExecuteBehavior()
	{
		if (aggroTarget != null && !aggroTarget.Controller.IsDead)
		{
			float range = GetRange(aggroTarget);
			if (range > behaviorProfile.LoseAggroRadius)
			{
				return GetDefaultBehavior();
			}
			else
			{
				if (IsInFront(aggroTarget))
				{
					Vector3 aimPoint = GetAimPoint(aggroTarget);
					if (range < fireRange && GetFireAngle(aimPoint) < fireAngle)
					{
						SetFireWingGuns(true);
					}
					else
					{
						SetFireWingGuns(false);
					}
					controller.YawRate = GetYawRateFromHeading(aimPoint - transform.position);
					controller.PitchRate = GetPitchRateFromHeading(aimPoint - transform.position);
					controller.Thrust = GetFollowThrust(aggroTarget, attackDistance);
					return BiplaneBehaviorCode.Attack;
				}
				else
				{
					return BiplaneBehaviorCode.Evade;
				}
			}
		}
		else
		{
			return GetDefaultBehavior();
		}
	}

	public override void ExitBehavior()
	{
	}
}
