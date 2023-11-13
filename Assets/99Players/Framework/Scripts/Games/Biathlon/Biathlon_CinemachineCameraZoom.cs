using Cinemachine;
using UnityEngine;
public class Biathlon_CinemachineCameraZoom : CinemachineExtension
{
	[SerializeField]
	private float fov = 10f;
	[SerializeField]
	private float minFov = 10f;
	[SerializeField]
	private float maxFov = 60f;
	public float Fov
	{
		get
		{
			return fov;
		}
		set
		{
			fov = value;
		}
	}
	protected override void Awake()
	{
		base.Awake();
		Fov = base.VirtualCamera.State.Lens.FieldOfView;
	}
	protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
	{
		LensSettings lens = state.Lens;
		lens.FieldOfView = Mathf.Clamp(Fov, minFov, maxFov);
		state.Lens = lens;
	}
}
