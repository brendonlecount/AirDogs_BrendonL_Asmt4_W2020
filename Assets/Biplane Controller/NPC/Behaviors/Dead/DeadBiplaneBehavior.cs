using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBiplaneBehavior : BiplaneBehavior
{
	public override void EnterBehavior()
	{
		Debug.Log(controller.name + " is now dead.");
		controller.Thrust = 0f;
	}

	public override BiplaneBehaviorCode ExecuteBehavior()
	{
		return BiplaneBehaviorCode.Dead;
	}

	public override void ExitBehavior()
	{
	}

	public override BiplaneBehaviorCode GetBehaviorCode()
	{
		return BiplaneBehaviorCode.Dead;
	}
}
