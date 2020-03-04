﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Targeting : MonoBehaviour
{
	private class TargetIcon
	{
		private CanvasGroup targetGroup;
		private CanvasGroup aimGroup;
		private RectTransform targetTransform;
		private RectTransform aimTransform;
		private RectTransform parentTransform;
		private BiplaneController controller;
		private Transform target;
		private Camera cam;
		float maxAimRange;
		float maxRange;
		float minAlpha;

		public TargetIcon(GameObject targetIconPrefab, GameObject aimIconPrefab, BiplaneController controller, RectTransform iconParent, float maxAimRange, float maxTargetRange, float minIconAlpha)
		{
			this.maxAimRange = maxAimRange;
			maxRange = maxTargetRange;
			minAlpha = minIconAlpha;
			parentTransform = iconParent;
			this.controller = controller;
			target = controller.transform;
			cam = Camera.main;

			targetTransform = GameObject.Instantiate(targetIconPrefab, iconParent).GetComponent<RectTransform>();
			targetGroup = targetTransform.GetComponent<CanvasGroup>();
			targetTransform.gameObject.SetActive(false);

			aimTransform = GameObject.Instantiate(aimIconPrefab, iconParent).GetComponent<RectTransform>();
			aimGroup = aimTransform.GetComponent<CanvasGroup>();
			aimTransform.gameObject.SetActive(false);
		}

		// https://answers.unity.com/questions/799616/unity-46-beta-19-how-to-convert-from-world-space-t.html
		public void UpdateTargetIcon()
		{
			if (!controller.IsDead)
			{
				Vector2 canvasPosition;
				if (GetCanvasPosition(target.position, out canvasPosition))
				{
					targetTransform.localPosition = canvasPosition;
					if (!targetTransform.gameObject.activeSelf)
					{
						targetTransform.gameObject.SetActive(true);
					}
					float range = (PlayerInput.Controller.transform.position - controller.transform.position).magnitude;
					targetGroup.alpha = Mathf.Lerp(1f, minAlpha, range / maxRange);
					if (range <= maxAimRange)
					{
						Vector3 aimPoint = target.position + controller.Rb.velocity * range / PlayerInput.ProjectileSpeed;
						if (GetCanvasPosition(aimPoint, out canvasPosition))
						{
							if (!aimTransform.gameObject.activeSelf)
							{
								aimTransform.gameObject.SetActive(true);
							}
							aimTransform.localPosition = canvasPosition;
							aimGroup.alpha = Mathf.Lerp(1f, minAlpha, range / maxRange);
						}
						else if (aimTransform.gameObject.activeSelf)
						{
							aimTransform.gameObject.SetActive(false);
						}
					}
					else if (aimTransform.gameObject.activeSelf)
					{
						aimTransform.gameObject.SetActive(false);
					}
				}
				else if (targetTransform.gameObject.activeSelf)
				{
					targetTransform.gameObject.SetActive(false);
					aimTransform.gameObject.SetActive(false);
				}
			}
			else if (targetTransform.gameObject.activeInHierarchy)
			{
				targetTransform.gameObject.SetActive(false);
				aimTransform.gameObject.SetActive(false);
			}
		}

		private bool GetCanvasPosition(Vector3 worldPosition, out Vector2 canvasPosition)
		{
			Vector3 viewportPoint = cam.WorldToViewportPoint(worldPosition);
			if (viewportPoint.z < 0)
			{
				canvasPosition = Vector3.zero;
				return false;
			}
			Vector2 size = new Vector2(parentTransform.rect.width, parentTransform.rect.height);
			Vector2 centerOffset = new Vector2(size.x * 0.5f, size.y * 0.5f);
			canvasPosition = new Vector2(viewportPoint.x * size.x, viewportPoint.y * size.y) - centerOffset;
			return true;
		}
	}

	[SerializeField] private GameObject targetIconPrefab;
	[SerializeField] private GameObject aimIconPrefab;
	[SerializeField] private RectTransform targetIconParent;
	[SerializeField] private float maxAimRange;
	[SerializeField] private float targetDistanceMax;
	[SerializeField] private float iconAlphaMin;

	private List<TargetIcon> targetIcons = new List<TargetIcon>();
	
	// Start is called before the first frame update
	void Start()
    {
        foreach(BiplaneController bc in TargetManager.GetEnemyBiplanes())
		{
			targetIcons.Add(new TargetIcon(targetIconPrefab, aimIconPrefab, bc, targetIconParent, maxAimRange, targetDistanceMax, iconAlphaMin));
		}
    }

    // Update is called once per frame
    void Update()
    {
        foreach (TargetIcon ti in targetIcons)
		{
			ti.UpdateTargetIcon();
		}
    }
}