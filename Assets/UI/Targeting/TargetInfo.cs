using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Brendon LeCount 3/18/2020
// This script displays target info as part of the targeting HUD.

public class TargetInfo : MonoBehaviour
{
	[SerializeField] private GameObject ratingPrefab;
	[SerializeField] private Transform ratingParent;
	[SerializeField] private Text behaviorText;

	public void SetRating(int rating)
	{
		for(int i = ratingParent.childCount - 1; i >= 0; i--)
		{
			Transform next = ratingParent.GetChild(i);
			next.parent = null;
			Destroy(next.gameObject);
		}
		for(int i = 0; i < rating; i++)
		{
			GameObject.Instantiate(ratingPrefab, ratingParent);
		}
	}

	public void SetBehavior(string behavior)
	{
		behaviorText.text = behavior;
	}

	public void SetShowBehavior(bool showBehavior)
	{
		behaviorText.gameObject.SetActive(showBehavior);
	}
}
