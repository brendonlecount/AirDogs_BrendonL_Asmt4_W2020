using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBiplaneBehavior : BiplaneBehavior
{
	[SerializeField] private float patrolRadius;
	[SerializeField] private float reachedRadius;

	private const float CIRCLE_TOLERANCE = 0.25f;

	private float patrolElevation;
	private Vector3 patrolOrigin;
	private bool circling;

	public override BiplaneBehaviorCode GetBehaviorCode()
	{
		return BiplaneBehaviorCode.Patrol;
	}

	public override string GetBehaviorName()
	{
		return "Patroling";
	}

	public override void EnterBehavior()
	{
		SetPatrolOrigin();
	}

	public override BiplaneBehaviorCode ExecuteBehavior()
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
			float range = GetHorizontalRange(patrolOrigin);
			if (!circling)
			{
				if (range < reachedRadius)
				{
					// patrol point reached, go to next
					biplaneAI.SetAiTarget(aiTargetPatrolNode.NextNode.gameObject);
					SetPatrolOrigin();
				}
				else
				{
					// approach patrol point
					controller.YawRate = GetYawRateFromHeading(patrolOrigin - biplaneTransform.position);
					controller.PitchRate = GetPitchRateFromElevation(patrolElevation);
				}
			}
			else
			{
				float approachYawRate = GetYawRateFromHeading(patrolOrigin - biplaneTransform.position);
				float circleYawRate;
				if (approachYawRate >= 0f)
				{
					// patrol point is to the right
					circleYawRate = GetYawRateFromHeading(Vector3.Cross(patrolOrigin - biplaneTransform.position, Vector3.up));
				}
				else
				{
					// patrol point is to the left
					circleYawRate = GetYawRateFromHeading(Vector3.Cross(patrolOrigin - biplaneTransform.position, Vector3.down));
				}
				controller.YawRate = Mathf.Lerp(circleYawRate, approachYawRate, (range - patrolRadius * (1f - CIRCLE_TOLERANCE)) / (patrolRadius * 2f * CIRCLE_TOLERANCE));

				controller.PitchRate = GetPitchRateFromElevation(patrolElevation);
			}
			return BiplaneBehaviorCode.Patrol;
		}
	}

	public override void ExitBehavior()
	{
	}

	private void SetPatrolOrigin()
	{
		if (aiTarget == null)
		{
			patrolOrigin = biplaneTransform.position;
			circling = true;
			Debug.Log(controller.name + " patrolling about " + patrolOrigin);
		}
		else if (aiTargetPatrolNode == null)
		{
			patrolOrigin = aiTarget.transform.position;
			circling = true;
			Debug.Log(controller.name + " patrolling about " + aiTarget.name);
		}
		else
		{
			patrolOrigin = aiTargetPatrolNode.Position;
			if (aiTargetPatrolNode.NextNode == null)
			{
				circling = true;
				Debug.Log(controller.name + " patrolling about " + aiTarget.name);
			}
			else
			{
				circling = false;
				Debug.Log(controller.name + " patrolling to " + aiTarget.name);
			}
		}
		controller.Thrust = controller.ThrustMax * behaviorProfile.ThrustMax;
		patrolElevation = Mathf.Clamp(patrolOrigin.y, behaviorProfile.ElevationMin, behaviorProfile.ElevationMax);
	}
}
