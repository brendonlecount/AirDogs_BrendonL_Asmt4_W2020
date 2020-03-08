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
	[SerializeField] private float mouseSensitivity;

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
	private Vector3 followPosition;
	private Quaternion followRotation;
	private float followFOV;
	private Transform pilot = null;
	private float currentFOV;
	private float pilotPitch = 0f;
	private float pilotYaw = 0f;


	private void Start()
    {
		followFOV = cam.fieldOfView;
		transitionTimer = transitionTime;
		target = PlayerInput.Instance.transform;
		firstPersonNode = PlayerInput.Instance.Controller.FirstPersonNode;
		lookAheadRotation = Quaternion.AngleAxis(lookAheadAngle, Vector3.left);
		PlayerInput.Instance.Controller.onDeath += DeathHandler;
		offsetDirection = cameraOffset.normalized;
		offsetDistance = cameraOffset.magnitude;

		GetPositionAndRotationTarget(out followPosition, out followRotation);
		transform.position = followPosition;
		transform.rotation = followRotation;
    }

	private void Update()
	{
		if (pilot != null)
		{
			pilotPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
			pilotYaw += Input.GetAxis("Mouse X") * mouseSensitivity;
		}
		else if (Input.GetButtonDown("Fire2") && transitionTimer >= transitionTime)
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
		if (pilot != null)
		{
			Vector3 pilotPositionTarget;
			Quaternion pilotRotationTarget;
			GetPilotPositionAndRotationTarget(out pilotPositionTarget, out pilotRotationTarget);
			if (transitionTimer < transitionTime)
			{
				transitionTimer += deltaTime;
				float t = GetEaseInOutT();
				transform.position = Vector3.Lerp(followPosition, pilotPositionTarget, t);
				transform.rotation = Quaternion.Lerp(followRotation, pilotRotationTarget, t);
				cam.fieldOfView = Mathf.Lerp(currentFOV, followFOV, t);
			}
			else
			{
				transform.position = pilotPositionTarget;
				transform.rotation = pilotRotationTarget;
			}
			return;
		}

		Vector3 followPositionTarget;
		Quaternion followRotationTarget;
		GetPositionAndRotationTarget(out followPositionTarget, out followRotationTarget);
		followPosition = Vector3.Lerp(followPosition, followPositionTarget, lerpFactor * deltaTime);
		followRotation = Quaternion.Lerp(followRotation, followRotationTarget, lerpFactor * deltaTime);

		if (isFirstPerson)
		{
			if (transitionTimer < transitionTime)
			{
				transitionTimer += deltaTime;
				float t = GetEaseInOutT();
				transform.position = Vector3.Lerp(followPosition, firstPersonNode.position, t);
				transform.rotation = Quaternion.Lerp(followRotation, firstPersonNode.rotation, t);
				cam.fieldOfView = Mathf.Lerp(followFOV, firstPersonFOV, t);
			}
			else
			{
				transform.position = firstPersonNode.position;
				transform.rotation = firstPersonNode.rotation;
			}
		}
		else
		{
			if (transitionTimer < transitionTime)
			{
				transitionTimer += deltaTime;
				float t = GetEaseInOutT();
				transform.position = Vector3.Lerp(firstPersonNode.position, followPosition, t);
				transform.rotation = Quaternion.Lerp(firstPersonNode.rotation, followRotation, t);
				cam.fieldOfView = Mathf.Lerp(firstPersonFOV, followFOV, t);
			}
			else
			{
				transform.position = followPosition;
				transform.rotation = followRotation;
			}
		}
	}

	private void GetPilotPositionAndRotationTarget(out Vector3 positionTarget, out Quaternion rotationTarget)
	{
		Vector3 flatForward = pilot.forward.y > 0.5f ? -pilot.up : pilot.forward;
		flatForward.y = 0f;
		Quaternion targetRotation = Quaternion.LookRotation(flatForward, Vector3.up);
		RaycastHit hit;
		if (Physics.SphereCast(pilot.position, collisionRadius, targetRotation * offsetDirection, out hit, offsetDistance, LayerMaskManager.BlocksCameraMask, QueryTriggerInteraction.Ignore))
		{
			positionTarget = pilot.position + targetRotation * Quaternion.Euler(0f, pilotYaw, 0f) * Quaternion.Euler(pilotPitch, 0f, 0f) * offsetDirection * hit.distance;
		}
		else
		{
			positionTarget = pilot.position + targetRotation * Quaternion.Euler(0f, pilotYaw, 0f) * Quaternion.Euler(pilotPitch, 0f, 0f) * cameraOffset;
		}
		rotationTarget = Quaternion.LookRotation(pilot.position - positionTarget, Vector3.up);

	}

	private void GetPositionAndRotationTarget(out Vector3 positionTarget, out Quaternion rotationTarget)
	{
		Quaternion targetRotation = Quaternion.LookRotation(target.forward, Vector3.up);
		RaycastHit hit;
		if (Physics.SphereCast(target.position, collisionRadius, targetRotation * offsetDirection, out hit, offsetDistance, LayerMaskManager.BlocksCameraMask, QueryTriggerInteraction.Ignore))
		{
			positionTarget = target.position + targetRotation * offsetDirection * hit.distance;
		}
		else
		{
			positionTarget = target.position + targetRotation * cameraOffset;
		}
		rotationTarget = Quaternion.LookRotation(target.position - positionTarget, Vector3.up) * lookAheadRotation;
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

	private void DeathHandler(PilotController pilot)
	{
		this.pilot = pilot.transform;
		transitionTimer = 0f;
		currentFOV = cam.fieldOfView;
	}
}
