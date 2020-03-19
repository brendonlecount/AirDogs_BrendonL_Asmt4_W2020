using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This manager creates a centralized gameobject where layer masks can be maintained, similar to the layer collision settings matrix in project settings.

public class LayerMaskManager : MonoBehaviour
{
	[SerializeField] private LayerMask blocksProjectileMask;
	public static LayerMask BlocksProjectileMask => instance.blocksProjectileMask;

	[SerializeField] private LayerMask blocksCameraMask;
	public static LayerMask BlocksCameraMask => instance.blocksCameraMask;

	[SerializeField] private LayerMask groundMask;
	public static LayerMask GroundMask => instance.groundMask;

	[SerializeField] private LayerMask predictiveCollisionMask;
	public static LayerMask PredictiveCollisionMask => instance.predictiveCollisionMask;

	[SerializeField] private LayerMask targetTriggerMask;
	public static LayerMask TargetTriggerMask => instance.targetTriggerMask;

	private static LayerMaskManager instance = null;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
