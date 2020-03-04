using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeoffBiplaneBehavior : BiplaneBehavior
{
	[SerializeField] private float targetElevation;

	public override BiplaneBehaviorCode GetBehaviorCode()
	{
		return BiplaneBehaviorCode.Takeoff;
	}

	public override void EnterBehavior()
	{
		controller.Thrust = controller.ThrustMax;
		controller.YawRate = 0f;
		controller.PitchRate = 0f;
	}

	public override BiplaneBehaviorCode ExecuteBehavior()
	{
		if (controller.transform.position.y > targetElevation)
		{
			return BiplaneBehaviorCode.Patrol;
		}
		return BiplaneBehaviorCode.Takeoff;
	}

	public override void ExitBehavior()
	{
	}
}
