using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiplaneAI : BiplaneControl
{
	[Header("Biplane AI Properties")]
	[SerializeField] private BiplaneBehaviorCode entryBehavior;
	[SerializeField] private BiplaneBehaviorProfile behaviorProfile;
	public BiplaneBehaviorProfile BehaviorProfile => behaviorProfile;
	[SerializeField] private GameObject target;
	public GameObject Target => target;

	private Dictionary<BiplaneBehaviorCode, BiplaneBehavior> behaviors;
	private BiplaneBehavior currentBehavior;

	private bool takingOff = true;

	public string CurrentBehaviorName => currentBehavior.GetBehaviorName();

	private void Awake()
	{
		behaviors = behaviorProfile.GetBehaviors(this);
		currentBehavior = behaviors[entryBehavior];
	}

	// Start is called before the first frame update
	void Start()
    {
		ApplyInitialConditions();
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

	public void SetAiTarget(GameObject aiTarget)
	{
		this.target = aiTarget;
		foreach (BiplaneBehavior bb in behaviors.Values)
		{
			bb.SetAiTarget(aiTarget);
		}
	}
}
