using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Brendon LeCount 3/18/2020
// This script controls a health gauge for the enemy airship.

public class AirshipHealth : MonoBehaviour
{
	[SerializeField] private Image[] subGauges;


	private AirshipController ac;
	float gaugeHealth;

    // Start is called before the first frame update
    void Start()
    {
		ac = AirshipController.Instance;
		gaugeHealth = 1f / subGauges.Length;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < subGauges.Length; i++)
		{
			if (ac.HealthFraction >= (i + 1) * gaugeHealth)
			{
				subGauges[i].fillAmount = 1f;
			}
			else if (ac.HealthFraction <= i * gaugeHealth)
			{
				subGauges[i].fillAmount = 0f;
			}
			else
			{
				subGauges[i].fillAmount = (ac.HealthFraction - gaugeHealth * i) / gaugeHealth;
			}
		}
    }
}
