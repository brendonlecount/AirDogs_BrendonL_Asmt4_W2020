using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script drives a BiplaneController via player input.

public class PlayerInput : BiplaneControl
{
	[Header("Settings")]
	[SerializeField] private float thrustSensitivity;
	[SerializeField] private float mouseSensitivity;
	[SerializeField] private float mouseLookSensitivity;
	[SerializeField] private bool invertMouse;
	[SerializeField] private float cameraSwitchHoldTime;

	private static PlayerInput instance;
	public static PlayerInput Instance => instance;

	private Coroutine cameraSwitchHeldRoutine = null;

    private void Awake()
    {
		instance = this;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = true;
    }

	private void Start()
	{
		ApplyInitialConditions();
	}

	// Update is called once per frame
	void Update()
    {
		if (FollowCamera.CameraMode == CameraModes.Pilot)
		{
			Controller.YawRate = 0f;
			Controller.PitchRate = 0f;
			Controller.Thrust = 0f;
		}
		else
		{
			CheckCameraSwitch();
			FireWingGuns();
			DropBomb();
			if (FollowCamera.CameraMode == CameraModes.Orbit)
			{
				Controller.YawRate = 0f;
				Controller.PitchRate = 0f;
				SetCameraPitch();
				SetCameraYaw();
			}
			else
			{
				SetYawRate();
				SetPitchRate();
			}
			SetThrust();
		}
	}

	private CameraModes lastCameraMode;

	private void CheckCameraSwitch()
	{
		switch (FollowCamera.CameraMode)
		{
			case CameraModes.First:
			case CameraModes.Follow:
				if (Input.GetButtonDown("Fire2") && cameraSwitchHeldRoutine == null)
				{
					cameraSwitchHeldRoutine = StartCoroutine(CameraSwitchHeldRoutine());
				}
				else if (Input.GetButtonUp("Fire2") && cameraSwitchHeldRoutine != null)
				{
					FollowCamera.SetCameraMode(FollowCamera.CameraMode == CameraModes.Follow ? CameraModes.First : CameraModes.Follow);
				}
				else if (Input.GetButton("Fire2") && cameraSwitchHeldRoutine == null)
				{
					lastCameraMode = FollowCamera.CameraMode;
					FollowCamera.SetCameraMode(CameraModes.Orbit);
				}
				break;
			case CameraModes.Orbit:
				if (!Input.GetButton("Fire2"))
				{
					FollowCamera.SetCameraMode(lastCameraMode);
				}
				break;
			case CameraModes.Pilot:
				break;
		}
	}

	private void SetYawRate()
	{
		Controller.YawRate = Input.GetAxis("Mouse X") * mouseSensitivity / Time.deltaTime;
	}

	private void SetPitchRate()
	{
		Controller.PitchRate = (invertMouse ? -1f : 1f) * Input.GetAxis("Mouse Y") * mouseSensitivity / Time.deltaTime;
	}

	private void SetCameraPitch()
	{
		FollowCamera.OrbitPitch -= Input.GetAxis("Mouse Y") * mouseLookSensitivity;
	}

	private void SetCameraYaw()
	{
		FollowCamera.OrbitYaw += Input.GetAxis("Mouse X") * mouseLookSensitivity;
	}

	private void SetThrust()
	{
		if (Input.GetAxisRaw("Mouse ScrollWheel") > 0.1f || Input.GetAxisRaw("Mouse ScrollWheel") < -0.1f)
		{
			Controller.Thrust += Controller.ThrustMax * Input.GetAxis("Mouse ScrollWheel") * thrustSensitivity;
		}
	}

	private void FireWingGuns()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			foreach (WingGun wg in WingGuns)
			{
				wg.Firing = true;
			}
		}
		if (Input.GetButtonUp("Fire1"))
		{
			foreach (WingGun wg in WingGuns)
			{
				wg.Firing = false;
			}
		}
	}

	private void DropBomb()
	{
		if (Input.GetButton("Bomb"))
		{
			BiplaneBomber.DropBomb();
		}
	}

	private IEnumerator CameraSwitchHeldRoutine()
	{
		yield return new WaitForSeconds(cameraSwitchHoldTime);
		cameraSwitchHeldRoutine = null;
	}
}
