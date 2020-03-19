using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
