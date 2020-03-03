using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
	[SerializeField] private Text speedometerText;

	private const float MS_TO_MPH = 2.23684f;

	private BiplaneController controller;
    
	// Start is called before the first frame update
    void Start()
    {
		controller = PlayerInput.Controller;
	}

    // Update is called once per frame
    void Update()
    {
		speedometerText.text = (controller.AxialSpeed * MS_TO_MPH).ToString("N0") + " mph";
    }
}
