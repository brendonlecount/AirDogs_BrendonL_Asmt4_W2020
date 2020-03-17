using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeoffBiplaneBehavior : BiplaneBehavior
{
	[SerializeField] private float takeOffAngle;

	public override BiplaneBehaviorCode GetBehaviorCode()
	{
		return BiplaneBehaviorCode.Takeoff;
	}

	public override string GetBehaviorName()
	{
		return "Taking Off";
	}

	public override void EnterBehavior()
	{
		controller.GroundPlane();
		Debug.Log(controller.name + " taking off!");
		controller.Thrust = controller.ThrustMax;
		controller.YawRate = 0f;
		controller.PitchRate = GetAngleRateFromTargetAngle(takeOffAngle, controller.Pitch, controller.PitchRate);
	}

	public override BiplaneBehaviorCode ExecuteBehavior()
	{
		if (controller.IsDead)
		{
			return BiplaneBehaviorCode.Dead;
		}
		if (controller.transform.position.y > behaviorProfile.ElevationMin)
		{
			return GetDefaultBehavior();
		}
		controller.PitchRate = GetAngleRateFromTargetAngle(takeOffAngle, controller.Pitch, controller.PitchRate);
		return BiplaneBehaviorCode.Takeoff;
	}

	public override void ExitBehavior()
	{
	}
}
