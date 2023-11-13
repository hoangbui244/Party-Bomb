using UnityEngine;
public class ShortTrack_CameraManager : SingletonCustom<ShortTrack_CameraManager>
{
	[SerializeField]
	[Header("カメラアンカ\u30fc")]
	private Transform[] cameraAnchor;
	[SerializeField]
	[Header("カメラ本体")]
	private Camera[] mainCam;
	private int cameraTotalNum = 1;
	private Rect[][] cameraRect = new Rect[4][]
	{
		new Rect[1]
		{
			new Rect(0f, 0f, 1f, 1f)
		},
		new Rect[2]
		{
			new Rect(0f, 0f, 0.5f, 1f),
			new Rect(0.5f, 0f, 1f, 1f)
		},
		new Rect[3]
		{
			new Rect(0f, 0.5f, 0.5f, 1f),
			new Rect(0.5f, 0.5f, 1f, 1f),
			new Rect(0f, 0f, 0.5f, 0.5f)
		},
		new Rect[4]
		{
			new Rect(0f, 0.5f, 0.5f, 1f),
			new Rect(0.5f, 0.5f, 1f, 1f),
			new Rect(0f, 0f, 0.5f, 0.5f),
			new Rect(0.5f, 0f, 1f, 0.5f)
		}
	};
	public void Init()
	{
		for (int i = 0; i < cameraAnchor.Length; i++)
		{
			cameraAnchor[i].gameObject.SetActive(value: false);
		}
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3 || SingletonCustom<GameSettingManager>.Instance.PlayerNum == 4)
		{
			cameraTotalNum = 4;
		}
		else
		{
			cameraTotalNum = SingletonCustom<GameSettingManager>.Instance.PlayerNum;
		}
		for (int j = 0; j < cameraTotalNum; j++)
		{
			cameraAnchor[j].gameObject.SetActive(value: true);
			mainCam[j].rect = cameraRect[cameraTotalNum - 1][j];
		}
	}
}
