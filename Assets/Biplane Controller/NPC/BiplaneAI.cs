using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiplaneAI : BiplaneControl
{
	[Header("Biplane AI Properties")]
	[SerializeField] private BiplaneBehaviorCode entryBehavior;
	[SerializeField] private BiplaneBehaviorProfile behaviorProfile;

	private Dictionary<BiplaneBehaviorCode, BiplaneBehavior> behaviors;
	private BiplaneBehavior currentBehavior;

	private bool takingOff = true;

    // Start is called before the first frame update
    void Start()
    {
		behaviors = behaviorProfile.GetBehaviors(Controller, WingGuns);
		currentBehavior = behaviors[entryBehavior];
		currentBehavior.EnterBehavior();
    }

	private void FixedUpdate()
	{
		BiplaneBehaviorCode nextBehaviorCode = currentBehavior.ExecuteBehavior();
		if (nextBehaviorCode != currentBehavior.GetBehaviorCode())
		{
			currentBehavior.ExitBehavior();
			currentBehavior = behaviors[nextBehaviorCode];
			currentBehavior.EnterBehavior();
		}
	}

	private void StartFiringGuns()
	{
		foreach (WingGun wg in WingGuns)
		{
			wg.Firing = true;
		}
	}

	private void StopFiringGuns()
	{
		foreach (WingGun wg in WingGuns)
		{
			wg.Firing = false;
		}
	}
}
