using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	[SerializeField] private Camera cam;
	[SerializeField] private Vector3 cameraOffset;
	[SerializeField] private float lookAheadAngle;
	[SerializeField] private float lerpFactor;
	[SerializeField] private float collisionRadius;
	[SerializeField] private float firstPersonFOV;
	[SerializeField] private float transitionTime;

	private Transform target;
	private Transform firstPersonNode;
	private Quaternion lookAheadRotation;
	private float offsetDistance;
	private Vector3 offsetDirection;
	private bool isFirstPerson = false;
	private float lerpTimer;
	private bool physicsUpdated = false;
	private float deltaTime;
	private float transitionTimer = 0f;
	private Vector3 transitionPosition;
	private Quaternion transitionRotation;
	private float followFOV;


	private void Start()
    {
		followFOV = cam.fieldOfView;
		transitionTimer = transitionTime;
		target = PlayerInput.Instance.transform;
		firstPersonNode = PlayerInput.Instance.Controller.FirstPersonNode;
		lookAheadRotation = Quaternion.AngleAxis(lookAheadAngle, Vector3.left);
		offsetDirection = cameraOffset.normalized;
		offsetDistance = cameraOffset.magnitude;
		transform.position = GetPositionTarget();
		transform.rotation = GetRotationTarget();
    }

	private void Update()
	{
		if (Input.GetButtonDown("Fire2") && transitionTimer >= transitionTime)
		{
			isFirstPerson = !isFirstPerson;
			transitionTimer = 0f;
		}
	}

	private void FixedUpdate()
    {
		deltaTime = Time.deltaTime;
		physicsUpdated = true;
	}

	private void LateUpdate()
	{
		if (physicsUpdated)
		{
			physicsUpdated = false;
			LateFixedUpdate();
		}
	}

	private void LateFixedUpdate()
	{
		if (isFirstPerson)
		{
			if (transitionTimer < transitionTime)
			{
				transitionTimer += deltaTime;
				float t = GetEaseInOutT();
				transform.position = Vector3.Lerp(transitionPosition, firstPersonNode.position, t);
				transform.rotation = Quaternion.Lerp(transitionRotation, firstPersonNode.rotation, t);
				cam.fieldOfView = Mathf.Lerp(followFOV, firstPersonFOV, t);
			}
			else
			{
				transform.position = firstPersonNode.position;
				transform.rotation = firstPersonNode.rotation;
				transitionPosition = transform.position;
				transitionRotation = transform.rotation;
			}
		}
		else
		{
			if (transitionTimer < transitionTime)
			{
				transitionTimer += deltaTime;
				float t = GetEaseInOutT();
				transform.position = Vector3.Lerp(transitionPosition, GetPositionTarget(), t);
				transform.rotation = Quaternion.Lerp(transitionRotation, GetRotationTarget(), t);
				cam.fieldOfView = Mathf.Lerp(firstPersonFOV, followFOV, t);
			}
			transform.position = Vector3.Lerp(transform.position, GetPositionTarget(), lerpFactor * deltaTime);
			transform.rotation = Quaternion.Lerp(transform.rotation, GetRotationTarget(), lerpFactor * deltaTime);
			transitionPosition = transform.position;
			transitionRotation = transform.rotation;
		}
	}

	private Vector3 GetPositionTarget()
	{
		Quaternion targetRotation = Quaternion.LookRotation(target.forward, Vector3.up);
		RaycastHit hit;
		if (Physics.SphereCast(target.position, collisionRadius, targetRotation * offsetDirection, out hit, offsetDistance, LayerMaskManager.BlocksCameraMask, QueryTriggerInteraction.Ignore))
		{
			return target.position + targetRotation * offsetDirection * hit.distance;
		}
		else
		{
			return target.position + targetRotation * cameraOffset;
		}
	}

	private Quaternion GetRotationTarget()
	{
		return Quaternion.LookRotation(target.position - transform.position, Vector3.up) * lookAheadRotation;
	}

	private float GetEaseInOutT()
	{
		float t = transitionTimer / transitionTime;
		if (t < 0.5f)
		{
			t = t * t;
		}
		else
		{
			t = -2f * (1 - t) * (1 - t) + 1f;
		}
		return t;
	}
}
