using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script implements biplane AI attack behavior.

public class AttackBiplaneBehavior : BiplaneBehavior
{
	[SerializeField] private float attackDistance;
	[SerializeField] private float fireRange;
	[SerializeField] private float fireAngle;
	[SerializeField] private float chickenRange;

	private BiplaneControl aggroTarget;
	private bool isChickening = false;

	public override BiplaneBehaviorCode GetBehaviorCode()
	{
		return BiplaneBehaviorCode.Attack;
	}

	public override string GetBehaviorName()
	{
		return "Attacking";
	}

	public override void EnterBehavior()
	{
		isChickening = false;
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
		if (controller.IsDead)
		{
			return BiplaneBehaviorCode.Dead;
		}
		if (aggroTarget != null && !aggroTarget.Controller.IsDead)
		{
			float range = GetRange(aggroTarget);
			if (range > behaviorProfile.LoseAggroRadius)
			{
				return GetDefaultBehavior();
			}
			else
			{
				if (IsInFront(aggroTarget) || range > behaviorProfile.EvadeRadius)
				{
					if (range < chickenRange && AmInFront(aggroTarget))
					{
						if (!isChickening)
						{
							// pull up (or down) to avoid ramming target headon
							controller.YawRate = 0f;
							float elevationError = biplaneTransform.position.y - 0.5f * (behaviorProfile.ElevationMax + behaviorProfile.ElevationMin);
							controller.PitchRate = (elevationError > 0f ? 1f : -1f) * behaviorProfile.HandlingLimit * controller.PitchYawRateLimit;
							isChickening = true;
						}
						return BiplaneBehaviorCode.Attack;
					}
					else
					{
						isChickening = false;
						Vector3 aimPoint = GetAimPoint(aggroTarget);
						if (!IsOnCollisionCourse(aggroTarget))
						{
							if (range < fireRange && GetFireAngle(aimPoint) < fireAngle && !IsShotBlocked(aimPoint, aggroTarget))
							{
								SetFireWingGuns(true);
							}
							else
							{
								SetFireWingGuns(false);
							}
							controller.YawRate = GetYawRateFromHeading(aimPoint - biplaneTransform.position);
							controller.PitchRate = GetPitchRateFromHeading(aimPoint - biplaneTransform.position);
							controller.Thrust = GetFollowThrust(aggroTarget, attackDistance);
							return BiplaneBehaviorCode.Attack;
						}
						else
						{
							controller.PitchRate = -behaviorProfile.HandlingLimit * controller.PitchYawRateLimit;
							return BiplaneBehaviorCode.Attack;
						}
					}
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
		SetFireWingGuns(false);
	}
}
