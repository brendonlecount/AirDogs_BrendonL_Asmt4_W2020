using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBiplaneBehavior : BiplaneBehavior
{
	[SerializeField] private float patrolRadius;
	[SerializeField] private float patrolElevationMin;
	[SerializeField] private float patrolElevationMax;

	private float patrolElevation;
	private Vector3 patrolOrigin;

	public override BiplaneBehaviorCode GetBehaviorCode()
	{
		return BiplaneBehaviorCode.Patrol;
	}

	public override void EnterBehavior()
	{
		if (aiTarget == null)
		{
			patrolOrigin = biplaneTransform.position;
			Debug.Log(controller.name + " patrolling about " + patrolOrigin);
		}
		else
		{
			patrolOrigin = aiTarget.transform.position;
			Debug.Log(controller.name + " patrolling about " + aiTarget.name);
		}
		controller.Thrust = controller.ThrustMax * Random.Range(behaviorProfile.ThrustMin, behaviorProfile.ThrustMax);
		patrolElevation = Random.Range(patrolElevationMin, patrolElevationMax);
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
			float range = GetRange(patrolOrigin);
			float approachYawRate = GetYawRateFromHeading(patrolOrigin - biplaneTransform.position);
			float circleYawRate = GetYawRateFromHeading(Vector3.Cross(patrolOrigin - biplaneTransform.position, Vector3.up));
			controller.YawRate = Mathf.Lerp(circleYawRate, approachYawRate, (range - patrolRadius * 0.75f) / (patrolRadius * 0.5f));

			controller.PitchRate = GetPitchRateFromElevation(patrolElevation);
			return BiplaneBehaviorCode.Patrol;
		}
	}

	public override void ExitBehavior()
	{
	}
}
