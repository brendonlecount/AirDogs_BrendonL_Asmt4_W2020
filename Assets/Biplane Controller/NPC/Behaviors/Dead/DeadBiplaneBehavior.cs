using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script implements the biplane death behaviour AI state.

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

	public override string GetBehaviorName()
	{
		return "Dead";
	}
}
