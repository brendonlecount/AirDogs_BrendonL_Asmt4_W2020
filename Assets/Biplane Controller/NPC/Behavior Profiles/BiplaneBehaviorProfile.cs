﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script implements a scriptable object containing data defining an AI profile.

[CreateAssetMenu(fileName = "BiplaneBehaviorProfile", menuName = "ScriptableObjects/Biplane Behavior Profile", order = 1)]
public class BiplaneBehaviorProfile : ScriptableObject
{
	[Header("Properties")]
	[SerializeField] private int skillRating;
	public int SkillRating => skillRating;

	[SerializeField] private float collisionCourseCheckTime;
	public float CollisionCourseCheckTime => collisionCourseCheckTime;

	[SerializeField] private float elevationMin;
	public float ElevationMin => elevationMin;

	[SerializeField] private float elevationMax;
	public float ElevationMax => elevationMax;
	
	public float ElevationRandom => Random.Range(elevationMin, elevationMax);

	[SerializeField] private float aggroRadius;
	public float AggroRadius => aggroRadius;

	[SerializeField] private float loseAggroRadius;
	public float LoseAggroRadius => loseAggroRadius;

	[SerializeField] private float evadeRadius;
	public float EvadeRadius => evadeRadius;

	[SerializeField] private float stopEvadeRadius;
	public float StopEvadeRadius => stopEvadeRadius;
	
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
	[SerializeField] private AttackBiplaneBehavior attackBehavior;
	[SerializeField] private EvadeBiplaneBehavior evadeBehavior;
	[SerializeField] private FollowBiplaneBehavior followBehavior;
	[SerializeField] private DeadBiplaneBehavior deadBehavior;

	public Dictionary<BiplaneBehaviorCode, BiplaneBehavior> GetBehaviors(BiplaneAI biplaneAI)
	{
		GameObject container = new GameObject();
		container.name = biplaneAI.name + " AI Behaviors";
		Dictionary<BiplaneBehaviorCode, BiplaneBehavior> behaviors = new Dictionary<BiplaneBehaviorCode, BiplaneBehavior>();

		CreateBehavior(takeoffBehavior, biplaneAI, behaviors, container.transform);
		CreateBehavior(patrolBehavior, biplaneAI, behaviors, container.transform);
		CreateBehavior(attackBehavior, biplaneAI, behaviors, container.transform);
		CreateBehavior(evadeBehavior, biplaneAI, behaviors, container.transform);
		CreateBehavior(followBehavior, biplaneAI, behaviors, container.transform);
		CreateBehavior(deadBehavior, biplaneAI, behaviors, container.transform);

		return behaviors;
	}

	private void CreateBehavior(BiplaneBehavior behavior, BiplaneAI biplaneAI, Dictionary<BiplaneBehaviorCode, BiplaneBehavior> behaviors, Transform container)
	{
		BiplaneBehavior newBehavior = GameObject.Instantiate(behavior, container).GetComponent<BiplaneBehavior>();
		newBehavior.InitializeBehavior(biplaneAI);
		behaviors.Add(newBehavior.GetBehaviorCode(), newBehavior);
	}
}
