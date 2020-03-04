using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StallIndicator : MonoBehaviour
{
	[SerializeField] private CanvasGroup indicatorGroup;
	[SerializeField] private float fadeRate;

	private BiplaneController controller;

    // Start is called before the first frame update
    void Start()
    {
		controller = PlayerInput.Instance.Controller;
	}

	// Update is called once per frame
	void Update()
    {
        if (controller.IsStalling && indicatorGroup.alpha < 1f)
		{
			indicatorGroup.alpha = Mathf.Min(indicatorGroup.alpha + fadeRate * Time.deltaTime, 1f);
		}
		else if (!controller.IsStalling && indicatorGroup.alpha > 0f)
		{
			indicatorGroup.alpha = Mathf.Max(indicatorGroup.alpha - fadeRate * Time.deltaTime, 0f);
		}
    }
}
