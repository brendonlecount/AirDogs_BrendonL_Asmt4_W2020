using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiplaneBehaviorProfile", menuName = "ScriptableObjects/Biplane Behavior Profile", order = 1)]
public class BiplaneBehaviorProfile : ScriptableObject
{
	[Header("Properties")]
	[SerializeField] private int skillRating;
	public int SkillRating => skillRating;
	
	[Range(0f, 1f)]
	[SerializeField] private float handlingLimit;
	public float HandlingLimit => handlingLimit;

	[Range(0f, 1f)]
	[SerializeField] private float thrustMin;
	public float ThrustMin => thrustMin;

	[Range(0f, 1f)]
	[SerializeField] private float thrustMax;
	public float ThrustMax => thrustMax;

	[Header("Behavior Prefabs")]
	[SerializeField] private TakeoffBiplaneBehavior takeoffBehavior;
	[SerializeField] private PatrolBiplaneBehavior patrolBehavior;

	public Dictionary<BiplaneBehaviorCode, BiplaneBehavior> GetBehaviors(BiplaneController controller, WingGun[] wingGuns)
	{
		GameObject container = new GameObject();
		container.name = controller.name + " AI Behaviors";
		Dictionary<BiplaneBehaviorCode, BiplaneBehavior> behaviors = new Dictionary<BiplaneBehaviorCode, BiplaneBehavior>();

		CreateBehavior(takeoffBehavior, controller, wingGuns, behaviors, container.transform);
		CreateBehavior(patrolBehavior, controller, wingGuns, behaviors, container.transform);

		return behaviors;
	}

	private void CreateBehavior(BiplaneBehavior behavior, BiplaneController controller, WingGun[] wingGuns, Dictionary<BiplaneBehaviorCode, BiplaneBehavior> behaviors, Transform container)
	{
		BiplaneBehavior newBehavior = GameObject.Instantiate(behavior, container).GetComponent<BiplaneBehavior>();
		newBehavior.InitializeBehavior(controller, wingGuns, this);
		behaviors.Add(newBehavior.GetBehaviorCode(), newBehavior);
	}
}
