using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This manager creates a centralized manager where layer masks can be maintained.

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
