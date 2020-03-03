using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotController : MonoBehaviour
{
	[SerializeField] private float parachuteDrag;
	[SerializeField] private GameObject parachute;

	private const float G = 9.81f;


	private Vector3 velocity;
	private bool ejected = false;

	private void Start()
	{
		parachute.SetActive(false);
	}

	// Update is called once per frame
	void Update()
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
		ejected = true;
	}
}
