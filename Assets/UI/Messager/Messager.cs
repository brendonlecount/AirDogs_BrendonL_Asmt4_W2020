using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Messager : MonoBehaviour
{
	[SerializeField] private Text messageText;

	private static Messager instance;

	private void Awake()
	{
		instance = this;
	}

	public static void ShowMessage(string message, float duration)
	{
		instance.messageText.text = message;
		instance.messageText.gameObject.SetActive(true);
		instance.StartCoroutine(instance.DisplayRoutine(duration));
	}

	private IEnumerator DisplayRoutine(float duration)
	{
		yield return new WaitForSeconds(duration);
		messageText.gameObject.SetActive(false);
	}
}
