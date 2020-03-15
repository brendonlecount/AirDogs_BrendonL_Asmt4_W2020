using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BiplaneBehaviorCode { Takeoff, Follow, Patrol, Evade, Attack, Dead }

public abstract class BiplaneBehavior : MonoBehaviour
{
	private const float MAX_CORRECTION_ANGLE = 10f;
	private const float CORRECTION_ANGLE_PER_METER = 0.25f;
	private const float CORRECTION_ANGLE_RATE_PER_DEGREE = 0.1f;		
	private const float CORRECTION_ANGLE_RATE_DAMPING = 0.6f;
	private const float FOLLOW_THRUST_PER_METER = 0.01f;
	private const float FOLLOW_THRUST_PER_MS = 0.1f;
	
	protected BiplaneAI biplaneAI;
	protected BiplaneController controller;
	protected Transform biplaneTransform;
	protected WingGun[] wingGuns;
	protected GameObject aiTarget;
	protected BiplaneControl aiTargetControl = null;
	protected PatrolNode aiTargetPatrolNode = null;
	protected BiplaneBehaviorProfile behaviorProfile;

	public abstract BiplaneBehaviorCode GetBehaviorCode();
	public abstract string GetBehaviorName();
	public virtual void InitializeBehavior(BiplaneAI biplaneAI)
	{
		this.biplaneAI = biplaneAI;
		this.controller = biplaneAI.Controller;
		biplaneTransform = controller.transform;
		this.wingGuns = biplaneAI.WingGuns;
		this.behaviorProfile = biplaneAI.BehaviorProfile;
		SetAiTarget(biplaneAI.Target);
	}
	public abstract void EnterBehavior();
	public abstract BiplaneBehaviorCode ExecuteBehavior();
	public abstract void ExitBehavior();

	public virtual void SetAiTarget(GameObject aiTarget)
	{
		this.aiTarget = aiTarget;
		if (aiTarget != null)
		{
			aiTargetControl = aiTarget.GetComponent<BiplaneControl>();
			aiTargetPatrolNode = aiTarget.GetComponent<PatrolNode>();
		}
	}


	protected float GetPitchRateFromElevation(float targetElevation)
	{
		float altitudeError = biplaneTransform.position.y - targetElevation;
		float targetPitch = Mathf.Clamp(altitudeError * CORRECTION_ANGLE_PER_METER, -MAX_CORRECTION_ANGLE, MAX_CORRECTION_ANGLE);
		float targetPitchRate = GetAngleRateFromTargetAngle(targetPitch, controller.Pitch, controller.PitchRate);
		// I was calculating pitch with the wrong sign initially (damn Unity's left hand rule), which made this very obnoxious to debug...
		// When I finally fixed it the AI started wrecking me lol
//		Debug.Log(controller.name);
//		Debug.Log("Elevation: " + biplaneTransform.position.y + " Target: " + targetElevation.ToString("N0"));
//		Debug.Log("Pitch: " + controller.Pitch.ToString("N1") + " Target: " + targetPitch.ToString("N1"));
//		Debug.Log("Rate: " + targetPitchRate.ToString("N1"));
		return targetPitchRate;
	}

	protected float GetPitchRateFromHeading(Vector3 targetHeading, bool applyElevationConstraints = true)
	{
		if (applyElevationConstraints && biplaneTransform.position.y < behaviorProfile.ElevationMin || biplaneTransform.position.y > behaviorProfile.ElevationMax)
		{
//			Debug.Log("Limited Pitch Correction:");
			return GetPitchRateFromElevation((behaviorProfile.ElevationMax + behaviorProfile.ElevationMin) * 0.5f);
		}
		float targetPitch = -Mathf.Asin(targetHeading.normalized.y) * Mathf.Rad2Deg;
//		Debug.Log("Pitch Correction:");
		return GetAngleRateFromTargetAngle(targetPitch, controller.Pitch, controller.PitchRate);
	}

	protected float GetYawRateFromHeading(Vector3 targetHeading)
	{
		float targetYaw = Vector3.SignedAngle(Vector3.forward, targetHeading, Vector3.up);
//		Debug.Log("Yaw Correction:");
		float yawRate = GetAngleRateFromTargetAngle(targetYaw, controller.Yaw, controller.YawRate);
		return yawRate;
	}

	protected float GetAngleRateFromTargetAngle(float targetAngle, float currentAngle, float currentRate)
	{
		targetAngle = ConditionAngle(targetAngle, currentAngle);
		float angleError = currentAngle - targetAngle;
//		Debug.Log((angleError * CORRECTION_ANGLE_RATE_PER_DEGREE).ToString("N2") + "  |  " + (currentRate * CORRECTION_ANGLE_RATE_DAMPING).ToString("N2"));
		float rate = Mathf.Clamp(-SRoot(angleError * CORRECTION_ANGLE_RATE_PER_DEGREE) - currentRate * CORRECTION_ANGLE_RATE_DAMPING,
			-behaviorProfile.HandlingLimit * controller.PitchYawRateLimit,
			behaviorProfile.HandlingLimit * controller.PitchYawRateLimit);
//		Debug.Log(rate.ToString("N2"));
		return rate;
	}

	private float SRoot(float value)
	{
		return Mathf.Sign(value) * Mathf.Sqrt(Mathf.Abs(value));
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
		Vector3 currentHeading = controller.transform.forward;
		return Vector3.Dot(currentHeading, targetHeading) > 0f;
	}

	protected bool AmInFront(BiplaneControl target)
	{
		Vector3 targetHeading = biplaneTransform.position - target.transform.position;
		return Vector3.Dot(target.transform.forward, targetHeading) > 0f;
	}

	protected Vector3 GetAimPoint(BiplaneControl target)
	{
		return target.transform.position + target.Controller.Rb.velocity * GetRange(target) / biplaneAI.ProjectileSpeed;
	}

	protected float GetRange(BiplaneControl target)
	{
		return GetRange(target.transform.position);
	}

	protected float GetRange(Vector3 targetPosition)
	{
		return (targetPosition - biplaneTransform.position).magnitude;
	}

	protected float GetHorizontalRange(Vector3 targetPosition)
	{
		Vector3 offset = targetPosition - biplaneTransform.position;
		offset.y = 0f;
		return offset.magnitude;
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

	protected bool IsShotBlocked(Vector3 targetPoint, BiplaneControl target)
	{
		RaycastHit hit;
		Vector3 targetHeading = targetPoint - controller.LineOfSightNode.position;
		if (Physics.SphereCast(controller.LineOfSightNode.position, controller.LineOfSightRadius, controller.LineOfSightNode.forward, out hit, targetHeading.magnitude, LayerMaskManager.BlocksProjectileMask))
		{
			if (hit.collider.CompareTag("Target"))
			{
				TargetTrigger tt = hit.collider.GetComponent<TargetTrigger>();
				if (tt != null)
				{
					if (tt.Controller == controller)
					{
						Debug.Log("Not working. Move LOS node forward :/");
						return true;
					}
					return tt.Controller != target.Controller;
				}
				else
				{
					Debug.Log("Target Trigger " + hit.collider.name + " missing ");
					return true;
				}
			}
			else
			{
				return true;
			}
		}
		else
		{
			return false;
		}
	}

	protected bool IsOnCollisionCourse(BiplaneControl target)
	{
		RaycastHit hit;
		if (Physics.SphereCast(biplaneTransform.position, controller.LineOfSightRadius, controller.Rb.velocity, out hit, controller.Speed * controller.PredictiveCollisionTime, LayerMaskManager.PredictiveCollisionMask))
		{
			if (hit.collider == controller.PredictiveCollisionCol)
			{
				Debug.Log("This isn't going to work.");
				return false;
			}
			if (hit.collider == target.Controller.PredictiveCollisionCol)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		else
		{
			return false;
		}
	}
}
