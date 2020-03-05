using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	[SerializeField] private Vector3 cameraOffset;
	[SerializeField] private float lookAheadAngle;
	[SerializeField] private float lerpFactor;
	[SerializeField] private float collisionRadius;
	[SerializeField] private float lerpTime;

	private Transform target;
	private Transform firstPersonNode;
	private Quaternion lookAheadRotation;
	private float offsetDistance;
	private Vector3 offsetDirection;
	private bool isFirstPerson = false;
	private float lerpTimer;

	private void Start()
    {
		target = PlayerInput.Instance.transform;
		firstPersonNode = PlayerInput.Instance.Controller.FirstPersonNode;
		lookAheadRotation = Quaternion.AngleAxis(lookAheadAngle, Vector3.left);
		offsetDirection = cameraOffset.normalized;
		offsetDistance = cameraOffset.magnitude;
		transform.position = GetPositionTarget();
		transform.rotation = GetRotationTarget();
    }

	private void Update()
	{
		if (Input.GetButtonDown("Fire2"))
		{
			isFirstPerson = !isFirstPerson;
		}
	}

	private void FixedUpdate()
    {
		if (!isFirstPerson)
		{
			transform.position = Vector3.Lerp(transform.position, GetPositionTarget(), lerpFactor * Time.deltaTime);

			transform.rotation = Quaternion.Lerp(transform.rotation, GetRotationTarget(), lerpFactor * Time.deltaTime);
		}
	}

	private void LateUpdate()
	{
		if (isFirstPerson)
		{
			transform.position = firstPersonNode.position;
			transform.rotation = firstPersonNode.rotation;
		}
	}

	private Vector3 GetPositionTarget()
	{
		Quaternion targetRotation = Quaternion.LookRotation(target.forward, Vector3.up);
		RaycastHit hit;
		if (Physics.SphereCast(target.position, collisionRadius, targetRotation * offsetDirection, out hit, offsetDistance, LayerMaskManager.BlocksCameraMask, QueryTriggerInteraction.Ignore))
		{
			return target.position + targetRotation * offsetDirection * hit.distance;
		}
		else
		{
			return target.position + targetRotation * cameraOffset;
		}
	}

	private Quaternion GetRotationTarget()
	{
		return Quaternion.LookRotation(target.position - transform.position, Vector3.up) * lookAheadRotation;
	}
}
