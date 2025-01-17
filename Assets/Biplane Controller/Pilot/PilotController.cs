﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brendon LeCount 3/18/2020
// This script is a character controller for a parachuting pilot that has been shot down.

public class PilotController : MonoBehaviour
{
	[SerializeField] private Animator animator;
	[SerializeField] private float parachuteDrag;
	[SerializeField] private GameObject parachute;
	[SerializeField] private float mouseLookSensitivity;

	private const float G = 9.81f;


	private Vector3 velocity;
	private bool ejected = false;

	private void Start()
	{
		parachute.SetActive(false);
	}

	private void Update()
	{
		FollowCamera.OrbitPitch -= Input.GetAxis("Mouse Y") * mouseLookSensitivity;
		FollowCamera.OrbitYaw += Input.GetAxis("Mouse X") * mouseLookSensitivity;
	}

	// Update is called once per frame
	void FixedUpdate()
    {
		if (ejected)
		{
			velocity += Vector3.down * Time.deltaTime * G;
			velocity -= velocity * velocity.magnitude * parachuteDrag * Time.deltaTime;
			transform.position = transform.position + velocity * Time.deltaTime;
			transform.rotation = Quaternion.FromToRotation(transform.rotation * Vector3.down, velocity) * transform.rotation;
			if (transform.position.y < -10f)
			{
				gameObject.SetActive(false);
			}
		}
	}

	public void Eject(Vector3 velocity)
	{
		transform.parent = null;
		this.velocity = velocity;
		parachute.SetActive(true);
		animator.SetTrigger("StartParachuting");
		ejected = true;
	}
}
