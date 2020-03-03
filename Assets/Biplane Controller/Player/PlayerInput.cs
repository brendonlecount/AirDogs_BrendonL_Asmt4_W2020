using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	[Header("Components")]
	[SerializeField] private BiplaneController controller;
	[SerializeField] private WingGun[] wingGuns;

	[Header("Settings")]
	[SerializeField] private float thrustSensitivity;
	[SerializeField] private float mouseSensitivity;
	[SerializeField] private bool invertMouse;

	public static float ProjectileSpeed => instance.controller.AxialSpeed + instance.wingGuns[0].ProjectileSpeed;

	private static PlayerInput instance;

	public static BiplaneController Controller => instance.controller;

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
		controller.YawRate = Input.GetAxis("Mouse X") * mouseSensitivity / Time.deltaTime;
	}

	private void SetPitchRate()
	{
		controller.PitchRate = (invertMouse ? -1f : 1f) * Input.GetAxis("Mouse Y") * mouseSensitivity / Time.deltaTime;
	}

	private void SetThrust()
	{
		controller.Thrust += controller.ThrustMax * Input.GetAxis("Mouse ScrollWheel") * thrustSensitivity;
	}

	private void FireWingGuns()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			foreach (WingGun wg in wingGuns)
			{
				wg.Firing = true;
			}
		}
		if (Input.GetButtonUp("Fire1"))
		{
			foreach (WingGun wg in wingGuns)
			{
				wg.Firing = false;
			}
		}
	}
}
