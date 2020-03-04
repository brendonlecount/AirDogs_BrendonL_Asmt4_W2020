using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BiplaneBehaviorCode { Takeoff, Patrol, Evade, Attack, Dead }

public abstract class BiplaneBehavior : MonoBehaviour
{
	protected BiplaneController controller;
	protected WingGun[] wingGuns;
	protected BiplaneBehaviorProfile behaviorProfile;

	public abstract BiplaneBehaviorCode GetBehaviorCode();
	public virtual void InitializeBehavior(BiplaneController controller, WingGun[] wingGuns, BiplaneBehaviorProfile behaviorProfile)
	{
		this.controller = controller;
		this.wingGuns = wingGuns;
		this.behaviorProfile = behaviorProfile;
	}
	public abstract void EnterBehavior();
	public abstract BiplaneBehaviorCode ExecuteBehavior();
	public abstract void ExitBehavior();

}
