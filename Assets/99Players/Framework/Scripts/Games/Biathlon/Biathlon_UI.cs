using System;
using UnityEngine;
public class Biathlon_UI : SingletonCustom<Biathlon_UI>
{
	[SerializeField]
	private Biathlon_UILayout singleLayout;
	[SerializeField]
	private Biathlon_UILayout duoLayout;
	[SerializeField]
	private Biathlon_UILayout multiLayout;
	private bool allowChangePlacement;
	private Biathlon_UILayout ActiveUILayout
	{
		get;
		set;
	}
	public void Init()
	{
		switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum)
		{
		case 1:
			singleLayout.Init();
			duoLayout.Disable();
			multiLayout.Disable();
			ActiveUILayout = singleLayout;
			break;
		case 2:
			duoLayout.Init();
			singleLayout.Disable();
			multiLayout.Disable();
			ActiveUILayout = duoLayout;
			break;
		case 3:
		case 4:
			multiLayout.Init();
			duoLayout.Disable();
			singleLayout.Disable();
			ActiveUILayout = multiLayout;
			break;
		}
	}
	public void GameStart()
	{
		LeanTween.delayedCall(base.gameObject, 1f, (Action)delegate
		{
			allowChangePlacement = true;
		});
	}
	public void UpdateGameTime(float[] times)
	{
		ActiveUILayout.UpdateGameTime(times);
	}
	public void UpdatePlacement(int no, int placement)
	{
		ActiveUILayout.UpdatePlacement(no, placement);
	}
	public void ShowPlacement()
	{
		ActiveUILayout.ShowPlacement();
	}
	public void HidePlacement(int no)
	{
		ActiveUILayout.HidePlacement(no);
	}
	public void SetPlacementAlpha(int no, float alpha)
	{
		ActiveUILayout.SetPlacementAlpha(no, alpha);
	}
	public void SetLapLabel(int no, int lap)
	{
		ActiveUILayout.SetLapLabel(no, lap + 1);
	}
	public void HideUI()
	{
		ActiveUILayout.Disable();
	}
	public void PlayFade(int no, float duration, float keep, Action fadeIn = null, Action fadeOut = null)
	{
		ActiveUILayout.PlayFade(no, duration, keep, fadeIn, fadeOut);
	}
	public void ActivateShootingModeUI(int no)
	{
		ActiveUILayout.ActivateShootingModeUI(no);
	}
	public void ActivateCrossCountryModeUI(int no)
	{
		ActiveUILayout.ActivateRunModeUI(no);
	}
	public void UpdateReticlePosition(int no, Vector3 aimPosition)
	{
		ActiveUILayout.UpdateReticlePosition(no, aimPosition);
	}
	public void ShowReloadGauge(int no, float reloadTime)
	{
		ActiveUILayout.ShowReloadGauge(no, reloadTime);
	}
	public void ShowHit(int no)
	{
		ActiveUILayout.ShowHit(no);
	}
	public void ShowMiss(int no)
	{
		ActiveUILayout.ShowMiss(no);
	}
	public void ShowFinalLap(int no)
	{
		ActiveUILayout.ShowFinalLap(no);
	}
	public void ShowResultPlacement(int no, int placement)
	{
		ActiveUILayout.ShowResultPlacement(no, placement);
	}
	public void ShowReverseRun(int no)
	{
		ActiveUILayout.ShowReverseRun(no);
	}
	public void HideReverseRun(int no)
	{
		ActiveUILayout.HideReverseRun(no);
	}
	public void UpdateCharactersPositionAll()
	{
		ActiveUILayout.UpdateCharactersPosition(0);
	}
}
