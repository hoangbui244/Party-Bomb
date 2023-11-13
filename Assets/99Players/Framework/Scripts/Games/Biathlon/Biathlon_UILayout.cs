using System;
using UnityEngine;
public class Biathlon_UILayout : MonoBehaviour
{
	[SerializeField]
	private GameObject[] uiRoots;
	[SerializeField]
	private CommonGameTimeUI_Font_Time[] timeUI;
	[SerializeField]
	private Biathlon_PlayerUI[] playersUI;
	public void Init()
	{
		Enable();
		for (int i = 0; i < playersUI.Length; i++)
		{
			playersUI[i].Init(i);
		}
	}
	public void Enable()
	{
		GameObject[] array = uiRoots;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
	}
	public void Disable()
	{
		GameObject[] array = uiRoots;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
	}
	public void UpdateGameTime(float[] times)
	{
		for (int i = 0; i < timeUI.Length; i++)
		{
			timeUI[i].SetTime(times[i], i);
		}
	}
	public void UpdatePlacement(int no, int placement)
	{
		if (no < playersUI.Length)
		{
			playersUI[no].UpdatePlacement(placement);
		}
	}
	public void HidePlacement(int no)
	{
		if (no < playersUI.Length)
		{
			playersUI[no].HidePlacement();
		}
	}
	public void ShowPlacement()
	{
		LeanTween.value(base.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate(float alpha)
		{
			for (int i = 0; i < playersUI.Length; i++)
			{
				SetPlacementAlpha(i, alpha);
			}
		});
	}
	public void SetPlacementAlpha(int no, float alpha)
	{
		if (no < playersUI.Length)
		{
			playersUI[no].SetPlacementAlpha(alpha);
		}
	}
	public void SetLapLabel(int no, int lap)
	{
		if (no < playersUI.Length)
		{
			playersUI[no].SetLapLabel(lap);
		}
	}
	public void PlayFade(int no, float duration, float keep, Action fadeIn, Action fadeOut)
	{
		if (no >= playersUI.Length)
		{
			LeanTween.delayedCall(base.gameObject, duration, (Action)delegate
			{
				fadeOut?.Invoke();
				LeanTween.delayedCall(base.gameObject, duration + keep, (Action)delegate
				{
					fadeIn?.Invoke();
				});
			});
		}
		else
		{
			playersUI[no].PlayFade(duration, keep, fadeIn, fadeOut);
		}
	}
	public void ActivateShootingModeUI(int no)
	{
		if (no < playersUI.Length)
		{
			playersUI[no].ActivateShootingModeUI();
		}
	}
	public void ActivateRunModeUI(int no)
	{
		if (no < playersUI.Length)
		{
			playersUI[no].ActivateRunModeUI();
		}
	}
	public void UpdateReticlePosition(int no, Vector3 aimPosition)
	{
		if (no < playersUI.Length)
		{
			playersUI[no].UpdateReticlePosition(aimPosition);
		}
	}
	public void ShowReloadGauge(int no, float reloadTime)
	{
		if (no < playersUI.Length)
		{
			playersUI[no].ShowReloadGauge(reloadTime);
		}
	}
	public void ShowHit(int no)
	{
		if (no < playersUI.Length)
		{
			playersUI[no].ShowHit();
		}
	}
	public void ShowMiss(int no)
	{
		if (no < playersUI.Length)
		{
			playersUI[no].ShowMiss();
		}
	}
	public void ShowFinalLap(int no)
	{
		if (no < playersUI.Length)
		{
			playersUI[no].ShowFinalLap();
		}
	}
	public void ShowResultPlacement(int no, int placement)
	{
		if (no < playersUI.Length)
		{
			playersUI[no].ShowResultPlacement(placement);
		}
	}
	public void ShowReverseRun(int no)
	{
		if (no < playersUI.Length)
		{
			playersUI[no].ShowReverseRun();
		}
	}
	public void HideReverseRun(int no)
	{
		if (no < playersUI.Length)
		{
			playersUI[no].HideReverseRun();
		}
	}
	public void UpdateCharactersPosition(int no)
	{
		if (no <= 0)
		{
			playersUI[no].UpdateCharactersPosition();
		}
	}
}
