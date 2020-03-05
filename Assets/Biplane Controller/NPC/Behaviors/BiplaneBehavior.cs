using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BiplaneBehaviorCode { Takeoff, Follow, Patrol, Evade, Attack, Dead }

public abstract class BiplaneBehavior : MonoBehaviour
{
	private const float MAX_CORRECTION_ANGLE = 10f;
	private const float CORRECTION_ANGLE_PER_METER = 2f;
	private const float CORRECTION_ANGLE_RATE_PER_DEGREE = 0.2f;
	private const float CORRECTION_ANGLE_RATE_DAMPING = 5f;
	private const float FOLLOW_THRUST_PER_METER = 0.1f;
	private const float FOLLOW_THRUST_PER_MS = 0.01f;
	
	protected BiplaneAI biplaneAI;
	protected BiplaneController controller;
	protected Transform biplaneTransform;
	protected WingGun[] wingGuns;
	protected GameObject aiTarget;
	protected BiplaneControl aiTargetControl = null;
	protected BiplaneBehaviorProfile behaviorProfile;

	public abstract BiplaneBehaviorCode GetBehaviorCode();
	public virtual void InitializeBehavior(BiplaneAI biplaneAI)
	{
		this.biplaneAI = biplaneAI;
		this.controller = biplaneAI.Controller;
		biplaneTransform = controller.transform;
		this.wingGuns = biplaneAI.WingGuns;
		this.aiTarget = biplaneAI.Target;
		if (aiTarget != null)
		{
			aiTargetControl = aiTarget.GetComponent<BiplaneControl>();
		}
		this.behaviorProfile = biplaneAI.BehaviorProfile;
	}
	public abstract void EnterBehavior();
	public abstract BiplaneBehaviorCode ExecuteBehavior();
	public abstract void ExitBehavior();


	protected float GetPitchRateFromElevation(float targetElevation)
	{
		float altitudeError = biplaneTransform.position.y - targetElevation;
		float targetPitch = Mathf.Clamp(altitudeError * CORRECTION_ANGLE_RATE_PER_DEGREE, -MAX_CORRECTION_ANGLE, MAX_CORRECTION_ANGLE);
		return GetAngleRateFromTargetAngle(targetPitch, controller.Pitch, controller.PitchRate);
	}

	protected float GetPitchRateFromHeading(Vector3 targetHeading)
	{
		float targetPitch = -Mathf.Asin(targetHeading.normalized.y) * Mathf.Rad2Deg;
		return GetAngleRateFromTargetAngle(targetPitch, controller.Pitch, controller.PitchRate);
	}

	protected float GetYawRateFromHeading(Vector3 targetHeading)
	{
		float targetYaw = Vector3.SignedAngle(Vector3.forward, targetHeading, Vector3.up);
		float yawRate = GetAngleRateFromTargetAngle(targetYaw, controller.Yaw, controller.YawRate);
		return yawRate;
	}

	protected float GetAngleRateFromTargetAngle(float targetAngle, float currentAngle, float currentRate)
	{
		targetAngle = ConditionAngle(targetAngle, currentAngle);
		float angleError = currentAngle - targetAngle;
		return Mathf.Clamp(-angleError * CORRECTION_ANGLE_RATE_PER_DEGREE - currentRate * CORRECTION_ANGLE_RATE_DAMPING,
			-behaviorProfile.HandlingLimit * controller.PitchYawRateLimit,
			behaviorProfile.HandlingLimit * controller.PitchYawRateLimit);
	}

	private float ConditionAngle(float angle, float referenceAngle)
	{
		while (angle - referenceAngle > 180f)
		{
			angle -= 360f;
		}
		while (angle - referenceAngle < -180f)
		{
			angle += 360f;
		}
		return angle;
	}

	protected bool IsInFront(BiplaneControl target)
	{
		Vector3 targetHeading = target.transform.position - biplaneTransform.position;
		Vector3 currentHeading = controller.Rb.velocity;
		return Vector3.Dot(currentHeading, targetHeading) > 0f;
	}

	protected Vector3 GetAimPoint(BiplaneControl target)
	{
		return target.transform.position + target.Controller.Rb.velocity * GetRange(target) / biplaneAI.ProjectileSpeed;
	}

	protected float GetRange(BiplaneControl target)
	{
		return (target.transform.position - biplaneAI.transform.position).magnitude;
	}

	protected BiplaneBehaviorCode GetDefaultBehavior()
	{
		if (aiTargetControl == null || aiTargetControl.Controller.IsDead)
		{
			return BiplaneBehaviorCode.Patrol;
		}
		else
		{
			return BiplaneBehaviorCode.Follow;
		}
	}

	protected float GetFireAngle(Vector3 aimPoint)
	{
		return Vector3.Angle(biplaneTransform.forward, aimPoint - biplaneTransform.position);
	}

	protected void SetFireWingGuns(bool fireWingGuns)
	{
		foreach (WingGun wg in wingGuns)
		{
			wg.Firing = fireWingGuns;
		}
	}

	protected float GetFollowThrust(BiplaneControl target, float followDistance)
	{
		float speedError = controller.AxialSpeed - target.Controller.AxialSpeed;
		float distanceError = GetRange(target) - followDistance;
		float followThrust = distanceError * FOLLOW_THRUST_PER_METER - speedError * FOLLOW_THRUST_PER_MS;
		return Mathf.Clamp(followThrust, behaviorProfile.ThrustMin * controller.ThrustMax, behaviorProfile.ThrustMax * controller.ThrustMax);
	}
}
