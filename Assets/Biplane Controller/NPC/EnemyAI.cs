using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	[SerializeField] private BiplaneController controller;
	[SerializeField] private WingGun[] wingGuns;

	public BiplaneController Controller => controller;

	private bool takingOff = true;

    // Start is called before the first frame update
    void Start()
    {
		controller.Thrust = controller.ThrustMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (takingOff)
		{
			if (transform.position.y > 20f)
			{
				takingOff = false;
				controller.YawRate = .3f;
				controller.Thrust = controller.ThrustMax * 0.75f;
				Debug.Log("Took off!");
			}
		}
		else
		{
			float altitudeError = transform.position.y - 20f;
			float targetPitch = Mathf.Lerp(0f, 10f, -altitudeError * 0.5f);
			float pitchError = controller.Pitch - targetPitch;
			controller.PitchRate = Mathf.LerpUnclamped(0f, 10f, pitchError * 0.01f) - controller.PitchRate * 0.7f;
		}
    }

	private void StartFiringGuns()
	{
		foreach (WingGun wg in wingGuns)
		{
			wg.Firing = true;
		}
	}

	private void StopFiringGuns()
	{
		foreach (WingGun wg in wingGuns)
		{
			wg.Firing = false;
		}
	}
}
