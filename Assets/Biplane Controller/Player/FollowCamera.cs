using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	[SerializeField] private Vector3 cameraOffset;
	[SerializeField] private float lookAheadAngle;
	[SerializeField] private float lerpFactor;
	[SerializeField] private float collisionRadius;

	private Transform target;
	private Quaternion lookAheadRotation;
	private float offsetDistance;
	private Vector3 offsetDirection;

	private void Start()
    {
		target = PlayerInput.Instance.transform;
		lookAheadRotation = Quaternion.AngleAxis(lookAheadAngle, Vector3.left);
		offsetDirection = cameraOffset.normalized;
		offsetDistance = cameraOffset.magnitude;
		transform.position = GetPositionTarget();
		transform.rotation = GetRotationTarget();
    }

	private void FixedUpdate()
    {
		transform.position = Vector3.Lerp(transform.position, GetPositionTarget(), lerpFactor * Time.deltaTime);

		transform.rotation = Quaternion.Lerp(transform.rotation, GetRotationTarget(), lerpFactor * Time.deltaTime);
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
