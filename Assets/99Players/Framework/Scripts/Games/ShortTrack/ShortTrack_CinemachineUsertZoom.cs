using Cinemachine;
using System;
using UnityEngine;
public class ShortTrack_CinemachineUsertZoom : CinemachineExtension
{
	[SerializeField]
	private ShortTrack_Character character;
	[SerializeField]
	[Range(1f, 179f)]
	private float _minFOV = 10f;
	[SerializeField]
	[Range(1f, 179f)]
	private float _maxFOV = 90f;
	[SerializeField]
	private float _smoothTime = 0.1f;
	[SerializeField]
	private float _maxSpeed = float.PositiveInfinity;
	private float _scrollDelta;
	private float _targetAdjustFOV;
	private float _currentAdjustFOV;
	private float _currentAdjustFOVVelocity;
	private bool isStartWait;
	protected override void Awake()
	{
		base.Awake();
		LeanTween.delayedCall(base.gameObject, 0.1f, (Action)delegate
		{
			isStartWait = true;
		});
	}
	protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
	{
		if (isStartWait && SHORTTRACK.MGM.IsRunCharacter() && stage == CinemachineCore.Stage.Aim)
		{
			LensSettings lens = state.Lens;
			if (character.GetTotalSpeed() >= character.chara_full_power_run_speed)
			{
				_scrollDelta = 5f;
				_targetAdjustFOV = Mathf.Clamp(_targetAdjustFOV - _scrollDelta, _minFOV - lens.FieldOfView, _maxFOV - lens.FieldOfView);
				_scrollDelta = 0f;
			}
			else
			{
				_scrollDelta = 5f;
				_targetAdjustFOV = Mathf.Clamp(_targetAdjustFOV + _scrollDelta, _minFOV - lens.FieldOfView, _maxFOV - lens.FieldOfView);
				_scrollDelta = 0f;
			}
			_currentAdjustFOV = Mathf.SmoothDamp(_currentAdjustFOV, _targetAdjustFOV, ref _currentAdjustFOVVelocity, _smoothTime, _maxSpeed, deltaTime);
			lens.FieldOfView += _currentAdjustFOV;
			state.Lens = lens;
		}
	}
}
