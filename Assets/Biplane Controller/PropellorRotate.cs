using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellorRotate : MonoBehaviour
{
	public enum SpinState { Stopped, SpinningUp, Spinning, SpinningDown }

	[SerializeField] private Transform propellorRoot;
	[SerializeField] private float rotationRate;
	[SerializeField] private float spinUpTime;
	[SerializeField] private SpinState spinState;

	private float rotationRateCurrent;
	private float spinUpRate;

    // Start is called before the first frame update
    void Start()
    {
		spinUpRate = rotationRate / spinUpTime;
		SetSpinState(spinState);
    }

    // Update is called once per frame
    void Update()
    {
		switch (spinState)
		{
			case SpinState.SpinningUp:
				rotationRateCurrent = Mathf.Min(rotationRateCurrent + spinUpRate * Time.deltaTime, rotationRate);
				if (rotationRateCurrent >= rotationRate)
				{
					spinState = SpinState.Spinning;
				}
				RotatePropellor();
				break;
			case SpinState.Spinning:
				RotatePropellor();
				break;
			case SpinState.SpinningDown:
				rotationRateCurrent = Mathf.Max(rotationRate - spinUpRate * Time.deltaTime, 0f);
				if (rotationRateCurrent <= 0f)
				{
					spinState = SpinState.Stopped;
				}
				RotatePropellor();
				break;
		}
    }

	public void SetSpinState(SpinState spinState)
	{
		switch (spinState)
		{
			case SpinState.Stopped:
				rotationRateCurrent = 0f;
				break;
			case SpinState.Spinning:
				rotationRateCurrent = rotationRate;
				break;
		}
		this.spinState = spinState;
	}

	private void RotatePropellor()
	{
		propellorRoot.Rotate(Vector3.left, rotationRateCurrent * Time.deltaTime, Space.Self);
	}
}
