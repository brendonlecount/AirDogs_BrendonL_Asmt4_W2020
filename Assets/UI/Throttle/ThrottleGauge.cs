using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Brendon LeCount 3/18/2020
// This script implements a throttle gauge displayed on the HUD.

public class ThrottleGauge : MonoBehaviour
{
	[SerializeField] private Image throttleImage;
	[SerializeField] private float lerpFactor;

	private BiplaneController controller;

    // Start is called before the first frame update
    void Start()
    {
		controller = PlayerInput.Instance.Controller;
	}

	// Update is called once per frame
	void Update()
    {
		throttleImage.fillAmount = Mathf.Lerp(throttleImage.fillAmount, controller.Thrust / controller.ThrustMax, lerpFactor * Time.deltaTime);
    }
}
