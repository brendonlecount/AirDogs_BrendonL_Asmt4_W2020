using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBiplaneBehavior : BiplaneBehavior
{
	[SerializeField] private float aggroRadius;
	[SerializeField] private float patrolRadius;
	[SerializeField] private float patrolElevationMin;
	[SerializeField] private float patrolElevationMax;

	private float patrolElevation;

	public override BiplaneBehaviorCode GetBehaviorCode()
	{
		return BiplaneBehaviorCode.Patrol;
	}

	public override void EnterBehavior()
	{
		controller.Thrust = controller.ThrustMax * Random.Range(behaviorProfile.ThrustMin, behaviorProfile.ThrustMax);
		controller.YawRate = 0.3f;
		patrolElevation = Random.Range(patrolElevationMin, patrolElevationMax);
	}

	public override BiplaneBehaviorCode ExecuteBehavior()
	{
		float altitudeError = transform.position.y - patrolElevation;
		float targetPitch = Mathf.Lerp(0f, 10f, -altitudeError * 0.5f);
		float pitchError = controller.Pitch - targetPitch;
		controller.PitchRate = Mathf.LerpUnclamped(0f, 10f, pitchError * 0.01f) - controller.PitchRate * 0.7f;
		return BiplaneBehaviorCode.Patrol;
	}

	public override void ExitBehavior()
	{
	}
}
