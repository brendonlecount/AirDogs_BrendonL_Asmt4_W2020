using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[SerializeField] private float resetDelay;
	[SerializeField] private float messageDuration;

    // Start is called before the first frame update
    void Start()
    {
		AirshipController.Instance.onTownDestroyed += TownDestructionHandler;
		AirshipController.Instance.onAirshipDestroyed += AirshipDestructionHandler;
		PlayerInput.Instance.Controller.onDeath += PlayerDestructionHandler;
		Messager.ShowMessage("Destroy the enemy airship before it bombs all of the towns!", 5f);
    }

	private void TownDestructionHandler(int townsRemaining)
	{
		if (townsRemaining <= 0)
		{
			Messager.ShowMessage("All towns have been destroyed. You have lost!", messageDuration);
			StartCoroutine(ResetRoutine());
		}
	}

	private void AirshipDestructionHandler()
	{
		Messager.ShowMessage("Enemy airship destroyed. You have won the day!", messageDuration);
		StartCoroutine(ResetRoutine());
	}

	private void PlayerDestructionHandler(PilotController pilot)
	{
		Messager.ShowMessage("You have been defeated!", messageDuration);
		StartCoroutine(ResetRoutine());
	}

	private IEnumerator ResetRoutine()
	{
		yield return new WaitForSeconds(resetDelay);
		SceneManager.LoadScene(0);
	}
}
