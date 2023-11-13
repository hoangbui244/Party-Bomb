using System;
using UnityEngine;
public class Biathlon_PlayerUI : MonoBehaviour
{
	[SerializeField]
	private Biathlon_PlayerIcon icon;
	[SerializeField]
	private Biathlon_LapUI lapUI;
	[SerializeField]
	private Biathlon_PlacementUI placementUI;
	[SerializeField]
	private Biathlon_ReverseRunUI reverseRunUI;
	[SerializeField]
	private Biathlon_FadeUI fadeUI;
	[SerializeField]
	private Biathlon_Reticle reticle;
	[SerializeField]
	private Biathlon_LastLapUI finalLapUI;
	[SerializeField]
	private Biathlon_PlacementUI resultPlacementUI;
	[SerializeField]
	private ResultPlacementAnimation resultPlacementAnimation;
	[SerializeField]
	private Biathlon_OperationUI operationUI;
	[SerializeField]
	private CourseMapUI courseMapUI;
	[SerializeField]
	private Biathlon_UIGroup runModeGroup;
	[SerializeField]
	private Biathlon_UIGroup shootingModeGroup;
	public void Init(int no)
	{
		if ((bool)icon)
		{
			icon.Init(no);
		}
		lapUI.Init();
		placementUI.Init(no);
		reverseRunUI.Init();
		reticle.Init(no);
		finalLapUI.Init();
		resultPlacementUI.Init(no);
		operationUI.Init(no);
		if (no == 0)
		{
			courseMapUI.Init();
			Biathlon_Course current = SingletonCustom<Biathlon_Courses>.Instance.Current;
			courseMapUI.SetWorldRangeAnchor(current.RightTopAnchor, current.LeftBottomAnchor);
			UpdateCharactersPosition();
		}
		shootingModeGroup.Deactivate();
		runModeGroup.Activate();
	}
	public void UpdatePlacement(int placement)
	{
		placementUI.SetPlacement(placement);
	}
	public void HidePlacement()
	{
		placementUI.Hide();
	}
	public void SetPlacementAlpha(float alpha)
	{
		placementUI.SetAlpha(alpha);
	}
	public void SetLapLabel(int lap)
	{
		lapUI.SetLap(lap);
	}
	public void PlayFade(float duration, float keep, Action fadeIn, Action fadeOut)
	{
		fadeUI.PlayFade(duration, keep, fadeIn, fadeOut);
	}
	public void ActivateRunModeUI()
	{
		shootingModeGroup.Deactivate();
		runModeGroup.Activate();
	}
	public void ActivateShootingModeUI()
	{
		runModeGroup.Deactivate();
		shootingModeGroup.Activate();
	}
	public void UpdateReticlePosition(Vector3 aimPosition)
	{
		reticle.UpdatePosition(aimPosition);
	}
	public void ShowReloadGauge(float reloadTime)
	{
		reticle.ShowReloadUI(reloadTime);
	}
	public void ShowFinalLap()
	{
		finalLapUI.Show();
	}
	public void ShowHit()
	{
		reticle.ShowHit();
	}
	public void ShowMiss()
	{
		reticle.ShowMiss();
	}
	public void ShowResultPlacement(int placement)
	{
		resultPlacementUI.SetPlacementImmediate(placement);
		resultPlacementAnimation.Play();
	}
	public void ShowReverseRun()
	{
		reverseRunUI.Show();
	}
	public void HideReverseRun()
	{
		reverseRunUI.Hide();
	}
	public void UpdateCharactersPosition()
	{
		if (!(courseMapUI == null))
		{
			courseMapUI.UpdateMethod();
		}
	}
}
