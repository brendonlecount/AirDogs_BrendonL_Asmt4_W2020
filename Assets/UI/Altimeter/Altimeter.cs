using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Altimeter : MonoBehaviour
{
	[SerializeField] private Text altimeterText;

	private const float M_TO_FEET = 3.28084f;

	private BiplaneController controller;

	// Start is called before the first frame update
	void Start()
	{
		controller = PlayerInput.Instance.Controller;
	}

	// Update is called once per frame
	void Update()
	{
		altimeterText.text = (controller.Elevation * M_TO_FEET).ToString("N0") + " feet";
	}
}
