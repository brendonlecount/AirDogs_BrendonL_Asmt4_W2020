using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
