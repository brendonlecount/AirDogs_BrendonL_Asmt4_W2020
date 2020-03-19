using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script implements biplane bombing.

public class Bomber : MonoBehaviour
{
	[SerializeField] private Bomb bombPrefab;
	[SerializeField] private Transform bombNode;
	[SerializeField] private GameObject bombStatic;
	[SerializeField] private Rigidbody parentRb;
	[SerializeField] private float armDelay;

	private Coroutine bombDelayRoutine;

	public bool IsArmed => bombDelayRoutine == null;

	public bool DropBomb()
	{
		if (!IsArmed)
		{
			return false;
		}
		bombDelayRoutine = StartCoroutine(BombDelayRoutine());
		bombStatic.SetActive(false);
		Rigidbody rb = Instantiate(bombPrefab, bombNode.position, bombNode.rotation).GetComponent<Rigidbody>();
		rb.velocity = parentRb.velocity;
		return true;
	}

	private IEnumerator BombDelayRoutine()
	{
		yield return new WaitForSeconds(armDelay);
		bombStatic.SetActive(true);
		bombDelayRoutine = null;
	}
}
