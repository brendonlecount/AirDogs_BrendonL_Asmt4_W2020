﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiplaneControl : MonoBehaviour
{
	[Header("Biplane Control Components")]
	[SerializeField] private BiplaneController controller;
	[SerializeField] private WingGun[] wingGuns;
	[SerializeField] private Bomber biplaneBomber;
	[Header("Biplane Control Properties")]
	[SerializeField] private bool isPlayerFaction;
	[SerializeField] private bool groundAtStart;
	[SerializeField] private float initialSpeed;

	public bool IsPlayerFaction => isPlayerFaction;
	public BiplaneController Controller => controller;
	public WingGun[] WingGuns => wingGuns;
	public Bomber BiplaneBomber => biplaneBomber;
	public float ProjectileSpeed => Controller.AxialSpeed + WingGuns[0].ProjectileSpeed;

	protected void ApplyInitialConditions()
	{
		if (groundAtStart)
		{
			controller.GroundPlane();
		}
		else
		{
			controller.Rb.velocity = controller.transform.forward * initialSpeed;
		}
	}


	public virtual void SetIsPlayerFaction(bool isPlayerFaction)
	{
		this.isPlayerFaction = isPlayerFaction;
	}
}