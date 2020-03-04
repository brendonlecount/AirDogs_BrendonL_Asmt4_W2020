using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiplaneControl : MonoBehaviour
{
	[Header("Biplane Control Components")]
	[SerializeField] private BiplaneController controller;
	[SerializeField] private WingGun[] wingGuns;
	[Header("Biplane Control Properties")]
	[SerializeField] private bool isPlayerFaction;

	public bool IsPlayerFaction => isPlayerFaction;
	public BiplaneController Controller => controller;
	public WingGun[] WingGuns => wingGuns;

	public virtual void SetIsPlayerFaction(bool isPlayerFaction)
	{
		this.isPlayerFaction = isPlayerFaction;
	}
}
