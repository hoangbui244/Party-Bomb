using System.Collections;
using UnityEngine;
public class SmeltFishing_CharacterMovingAction : MonoBehaviour
{
	private SmeltFishing_Character playingCharacter;
	private SmeltFishing_IcePlate icePlate;
	private SmeltFishing_IcePlate beforeIcePlate;
	public void Init(SmeltFishing_Character character)
	{
		playingCharacter = character;
	}
	public void UpdateMethod()
	{
		if (playingCharacter.IsPlayer)
		{
			SitDown();
		}
		else
		{
			SitDownForAI();
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("HoledIcePlate"))
		{
			return;
		}
		UnityEngine.Debug.Log("釣り穴に接近\u3000釣り穴:" + other.name);
		icePlate = other.GetComponent<SmeltFishing_IcePlate>();
		if (icePlate.IsUsing)
		{
			return;
		}
		if (!playingCharacter.IsPlayer)
		{
			if (playingCharacter.Movement.TargetIcePlate != icePlate)
			{
				icePlate = null;
				return;
			}
			if (icePlate == beforeIcePlate)
			{
				icePlate = null;
				return;
			}
			beforeIcePlate = icePlate;
		}
		if (!playingCharacter.Camera.InTransition)
		{
			SingletonCustom<SmeltFishing_UI>.Instance.SetClipPosition(playingCharacter.PlayerNo, icePlate.Position);
			SingletonCustom<SmeltFishing_UI>.Instance.ShowSitDownUI(playingCharacter.PlayerNo);
		}
		else
		{
			StartCoroutine(WaitCameraInTransition());
		}
	}
	private IEnumerator WaitCameraInTransition()
	{
		yield return new WaitWhile(() => playingCharacter.Camera.InTransition);
		if (!(icePlate == null) && playingCharacter.GetState() != SmeltFishing_Character.State.Fishing)
		{
			SingletonCustom<SmeltFishing_UI>.Instance.SetClipPosition(playingCharacter.PlayerNo, icePlate.Position);
			SingletonCustom<SmeltFishing_UI>.Instance.ShowSitDownUI(playingCharacter.PlayerNo);
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("HoledIcePlate") && !(icePlate == null))
		{
			SmeltFishing_IcePlate component = other.GetComponent<SmeltFishing_IcePlate>();
			if (!(icePlate != component))
			{
				UnityEngine.Debug.Log("釣り穴から離れる\u3000釣り穴:" + other.name);
				SingletonCustom<SmeltFishing_UI>.Instance.HideSitDownUI(playingCharacter.PlayerNo);
				icePlate = null;
			}
		}
	}
	private void SitDown()
	{
		if (playingCharacter.IsMoving && !(icePlate == null))
		{
			if (icePlate.IsUsing)
			{
				SingletonCustom<SmeltFishing_UI>.Instance.HideSitDownUI(playingCharacter.PlayerNo);
			}
			else if (SingletonCustom<SmeltFishing_Input>.Instance.IsPressButtonA(playingCharacter.ControlUser))
			{
				playingCharacter.SitDown(icePlate);
				SingletonCustom<SmeltFishing_UI>.Instance.HideSitDownUI(playingCharacter.PlayerNo);
			}
		}
	}
	private void SitDownForAI()
	{
		if (playingCharacter.IsMoving && !(icePlate == null) && !icePlate.IsUsing)
		{
			playingCharacter.SitDown(icePlate);
		}
	}
}
