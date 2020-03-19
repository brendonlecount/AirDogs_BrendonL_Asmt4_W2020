using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Brendon LeCount 3/18/2020
// This script displays the number of remaining towns in the HUD.

public class TownCounter : MonoBehaviour
{
	[SerializeField] private Text counterText;

	private void Start()
	{
		TownDestructionHandler(AirshipController.Instance.TargetTownCount);
		AirshipController.Instance.onTownDestroyed += TownDestructionHandler;
	}

	private void TownDestructionHandler(int townsRemaining)
	{
		counterText.text = townsRemaining.ToString("N0") + " Towns Remain";
	}
}
