using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : BiplaneControl
{
	[Header("Settings")]
	[SerializeField] private float thrustSensitivity;
	[SerializeField] private float mouseSensitivity;
	[SerializeField] private bool invertMouse;

	public static float ProjectileSpeed => instance.Controller.AxialSpeed + instance.WingGuns[0].ProjectileSpeed;

	private static PlayerInput instance;
	public static PlayerInput Instance => instance;

    private void Awake()
    {
		instance = this;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
		FireWingGuns();
		SetYawRate();
		SetPitchRate();
		SetThrust();
    }

	private void SetYawRate()
	{
		Controller.YawRate = Input.GetAxis("Mouse X") * mouseSensitivity / Time.deltaTime;
	}

	private void SetPitchRate()
	{
		Controller.PitchRate = (invertMouse ? -1f : 1f) * Input.GetAxis("Mouse Y") * mouseSensitivity / Time.deltaTime;
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
}
