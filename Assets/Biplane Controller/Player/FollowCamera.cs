using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script implements a first/third/orbit camera that follows a biplane controlled by a PlayerInput script.

public enum CameraModes { First, Follow, Orbit, Pilot }

public class FollowCamera : MonoBehaviour
{
	private class CameraTarget
	{
		public Vector3 position;
		public Quaternion rotation;
		public float fieldOfView;
	}

	[SerializeField] private Camera cam;
	[SerializeField] private Vector3 cameraOffset;
	[SerializeField] private float lookAheadAngle;
	[SerializeField] private float lerpFactor;
	[SerializeField] private float collisionRadius;
	[SerializeField] private float firstPersonFOV;
	[SerializeField] private float transitionTime;
	[SerializeField] private CameraModes startingCameraMode;

	private static FollowCamera instance = null;

	private Transform target;
	private Transform firstPersonNode;
	private Quaternion lookAheadRotation;
	private float offsetDistance;
	private Vector3 offsetDirection;
	private bool physicsUpdated = false;
	private float deltaTime;
	private float transitionTimer = 0f;
	private Transform pilot = null;
	public static float OrbitPitch
	{
		get { return instance.orbitPitch; }
		set
		{
			instance.orbitPitch = Mathf.Clamp(value, -90f, 90f);
		}
	}
	private float orbitPitch = 0f;
	public static float OrbitYaw = 0f;
	private CameraModes cameraMode;
	public static CameraModes CameraMode => instance.cameraMode;
	private CameraModes lastCameraMode;
	private Dictionary<CameraModes, CameraTarget> cameraTargets = new Dictionary<CameraModes, CameraTarget>();

	public static bool Transitioning => instance.transitionTimer < instance.transitionTime;

	public static void SetCameraMode(CameraModes cameraMode)
	{
		instance.lastCameraMode = instance.cameraMode;
		instance.cameraMode = cameraMode;
		instance.transitionTimer = 0f;
	}

	private void Awake()
	{
		instance = this;

		cameraTargets.Add(CameraModes.First, new CameraTarget());
		cameraTargets[CameraModes.First].fieldOfView = firstPersonFOV;

		cameraTargets.Add(CameraModes.Follow, new CameraTarget());
		cameraTargets[CameraModes.Follow].fieldOfView = cam.fieldOfView;

		cameraTargets.Add(CameraModes.Orbit, new CameraTarget());
		cameraTargets[CameraModes.Orbit].fieldOfView = cam.fieldOfView;

		cameraTargets.Add(CameraModes.Pilot, new CameraTarget());
		cameraTargets[CameraModes.Pilot].fieldOfView = cam.fieldOfView;

		OrbitPitch = 0f;
		OrbitYaw = 0f;
	}

	private void Start()
    {
		transitionTimer = transitionTime;
		target = PlayerInput.Instance.transform;
		firstPersonNode = PlayerInput.Instance.Controller.FirstPersonNode;
		lookAheadRotation = Quaternion.AngleAxis(lookAheadAngle, Vector3.left);
		PlayerInput.Instance.Controller.onDeath += DeathHandler;
		offsetDirection = cameraOffset.normalized;
		offsetDistance = cameraOffset.magnitude;

		GetFirstPositionAndRotationTarget(out cameraTargets[CameraModes.First].position, out cameraTargets[CameraModes.First].rotation);
		GetFollowPositionAndRotationTarget(out cameraTargets[CameraModes.Follow].position, out cameraTargets[CameraModes.Follow].rotation);
		GetOrbitPositionAndRotationTarget(out cameraTargets[CameraModes.Orbit].position, out cameraTargets[CameraModes.Orbit].rotation);

		if (startingCameraMode == CameraModes.Pilot)
		{
			cameraMode = CameraModes.Orbit;
		}
		else
		{
			cameraMode = startingCameraMode;
		}

		lastCameraMode = cameraMode;
		transitionTimer = transitionTime * 2f;
		transform.position = cameraTargets[cameraMode].position;
		transform.rotation = cameraTargets[cameraMode].rotation;
		cam.fieldOfView = cameraTargets[cameraMode].fieldOfView;
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
		UpdateCameraTargets();
		if (Transitioning)
		{
			ApplyCameraTransition();
		}
		else
		{
			ApplyCameraTarget();
		}
	}

	private void UpdateCameraTargets()
	{
		Vector3 targetPosition;
		Quaternion targetRotation;
		
		GetFirstPositionAndRotationTarget(out cameraTargets[CameraModes.First].position, out cameraTargets[CameraModes.First].rotation);

		GetFollowPositionAndRotationTarget(out targetPosition, out targetRotation);
		cameraTargets[CameraModes.Follow].position = Vector3.Lerp(cameraTargets[CameraModes.Follow].position, targetPosition, lerpFactor * deltaTime);
		cameraTargets[CameraModes.Follow].rotation = Quaternion.Lerp(cameraTargets[CameraModes.Follow].rotation, targetRotation, lerpFactor * deltaTime);

		GetOrbitPositionAndRotationTarget(out targetPosition, out targetRotation);
		cameraTargets[CameraModes.Orbit].position = Vector3.Lerp(cameraTargets[CameraModes.Orbit].position, targetPosition, lerpFactor * deltaTime);
		cameraTargets[CameraModes.Orbit].rotation = Quaternion.Lerp(cameraTargets[CameraModes.Orbit].rotation, targetRotation, lerpFactor * deltaTime);

		if (pilot != null)
		{
			GetPilotPositionAndRotationTarget(out targetPosition, out targetRotation);
			cameraTargets[CameraModes.Pilot].position = Vector3.Lerp(cameraTargets[CameraModes.Pilot].position, targetPosition, lerpFactor * deltaTime);
			cameraTargets[CameraModes.Pilot].rotation = Quaternion.Lerp(cameraTargets[CameraModes.Pilot].rotation, targetRotation, lerpFactor * deltaTime);
		}
	}

	private void ApplyCameraTransition()
	{
		transitionTimer += deltaTime;
		float t = GetEaseInOutT();
		transform.position = Vector3.Lerp(cameraTargets[lastCameraMode].position, cameraTargets[cameraMode].position, t);
		transform.rotation = Quaternion.Lerp(cameraTargets[lastCameraMode].rotation, cameraTargets[cameraMode].rotation, t);
		cam.fieldOfView = Mathf.Lerp(cameraTargets[lastCameraMode].fieldOfView, cameraTargets[cameraMode].fieldOfView, t);
	}

	private void ApplyCameraTarget()
	{
		transform.position = cameraTargets[cameraMode].position;
		transform.rotation = cameraTargets[cameraMode].rotation;
	}

	private void GetPilotPositionAndRotationTarget(out Vector3 positionTarget, out Quaternion rotationTarget)
	{
		Vector3 flatForward = pilot.forward.y > 0.5f ? -pilot.up : pilot.forward;
		flatForward.y = 0f;
		Quaternion targetRotation = Quaternion.LookRotation(flatForward, Vector3.up);
		RaycastHit hit;
		if (Physics.SphereCast(pilot.position, collisionRadius, targetRotation * offsetDirection, out hit, offsetDistance, LayerMaskManager.BlocksCameraMask, QueryTriggerInteraction.Ignore))
		{
			positionTarget = pilot.position + targetRotation * Quaternion.Euler(0f, OrbitYaw, 0f) * Quaternion.Euler(OrbitPitch, 0f, 0f) * offsetDirection * hit.distance;
		}
		else
		{
			positionTarget = pilot.position + targetRotation * Quaternion.Euler(0f, OrbitYaw, 0f) * Quaternion.Euler(OrbitPitch, 0f, 0f) * cameraOffset;
		}
		rotationTarget = Quaternion.LookRotation(pilot.position - positionTarget, Vector3.up);
	}

	private void GetOrbitPositionAndRotationTarget(out Vector3 positionTarget, out Quaternion rotationTarget)
	{
		Vector3 flatForward = target.forward;
		flatForward.y = 0f;
		Quaternion targetRotation = Quaternion.LookRotation(flatForward, Vector3.up);
		RaycastHit hit;
		if (Physics.SphereCast(target.position, collisionRadius, targetRotation * offsetDirection, out hit, offsetDistance, LayerMaskManager.BlocksCameraMask, QueryTriggerInteraction.Ignore))
		{
			positionTarget = target.position + targetRotation * Quaternion.Euler(0f, OrbitYaw, 0f) * Quaternion.Euler(OrbitPitch, 0f, 0f) * offsetDirection * hit.distance;
		}
		else
		{
			positionTarget = target.position + targetRotation * Quaternion.Euler(0f, OrbitYaw, 0f) * Quaternion.Euler(OrbitPitch, 0f, 0f) * cameraOffset;
		}
		rotationTarget = Quaternion.LookRotation(target.position - positionTarget, Vector3.up);
	}

	private void GetFollowPositionAndRotationTarget(out Vector3 positionTarget, out Quaternion rotationTarget)
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

	private void GetFirstPositionAndRotationTarget(out Vector3 positionTarget, out Quaternion rotationTarget)
	{
		positionTarget = firstPersonNode.position;
		rotationTarget = firstPersonNode.rotation;
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
		GetPilotPositionAndRotationTarget(out cameraTargets[CameraModes.Pilot].position, out cameraTargets[CameraModes.Pilot].rotation);
		SetCameraMode(CameraModes.Pilot);
	}
}
