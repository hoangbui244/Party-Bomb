using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Scuba_UiManager : SingletonCustom<Scuba_UiManager>
{
	[Serializable]
	public class FoundView
	{
		public SpriteRenderer[] sprites;
		public Vector2 rightTop;
		public Vector2 leftBottom;
		public void SetLocalPos(int _spriteNo, Vector2 _viewportPoint)
		{
			sprites[_spriteNo].transform.localPosition = new Vector2(Mathf.Lerp(leftBottom.x, rightTop.x, _viewportPoint.x), Mathf.Lerp(leftBottom.y, rightTop.y, _viewportPoint.y));
		}
		public void SetLocalScale(int _spriteNo, float _scale)
		{
			sprites[_spriteNo].transform.localScale = new Vector3(_scale, _scale, 1f);
		}
	}
	[Serializable]
	public class GetPointUiArray
	{
		public Scuba_GetPointUi[] getPoints;
	}
	private static readonly string[] CHARA_DEFAULT_SPRITE_NAMES = new string[8]
	{
		"character_yuto_02",
		"character_hina_02",
		"character_ituki_02",
		"character_souta_02",
		"character_takumi_02",
		"character_rin_02",
		"character_akira_02",
		"character_rui_02"
	};
	private const string CPU_PLAYER_ICON_SPRITE_NAME = "_screen_cp";
	private const float SINGLE_FOUND_VIEW_MAX_SCALE = 0.28f;
	private const float SINGLE_FOUND_VIEW_MIN_SCALE = 0.1f;
	private const float MULTI_FOUND_VIEW_MAX_SCALE = 0.21f;
	private const float MULTI_FOUND_VIEW_MIN_SCALE = 0.075f;
	private const float FOUND_VIEW_MAX_SCALE_VIEWPORT_Z = 0.1f;
	private const float FOUND_VIEW_MIN_SCALE_VIEWPORT_Z = 1.5f;
	private const float OK_SPRITE_SCALE = 1f;
	private const float FOUND_SPRITE_SCALE = 0.8f;
	private static readonly Vector3 SINGLE_PUCTURE_BIG_SIZE = new Vector3(1920f, 1080f, 1f);
	private static readonly Vector3 SINGLE_PUCTURE_NORMAL_SIZE = new Vector3(1344f, 756f, 1f);
	private static readonly Vector3 MULTI_PUCTURE_BIG_SIZE = new Vector3(960f, 540f, 1f);
	private static readonly Vector3 MULTI_PUCTURE_NORMAL_SIZE = new Vector3(624f, 351f, 1f);
	private static readonly Vector3 SINGLE_PUCTURE_BIG_POS = new Vector3(0f, 0f, 0f);
	private static readonly Vector3 SINGLE_PUCTURE_NORMAL_POS = new Vector3(0f, 0f, 0f);
	private static readonly Vector3[] MULTI_PUCTURE_BIG_POSES = new Vector3[4]
	{
		new Vector3(-480f, 270f, 0f),
		new Vector3(480f, 270f, 0f),
		new Vector3(-480f, -270f, 0f),
		new Vector3(480f, -270f, 0f)
	};
	private static readonly Vector3[] MULTI_PUCTURE_NORMAL_POSES = new Vector3[4]
	{
		new Vector3(-480f, 270f, 0f),
		new Vector3(480f, 270f, 0f),
		new Vector3(-480f, -270f, 0f),
		new Vector3(480f, -270f, 0f)
	};
	private Camera uiCamera;
	[SerializeField]
	private GameObject singleObj;
	[SerializeField]
	private GameObject multiObj;
	[SerializeField]
	[Header("1人用UI")]
	private CommonWaterPistolBattleUILayout singleUiLayout;
	[SerializeField]
	private FoundView singleFoundView;
	[SerializeField]
	private Transform singleStaminaGaugeAnchor;
	[SerializeField]
	private GameObject singleActionCtrlObj;
	[SerializeField]
	private Canvas singleCanvas;
	[SerializeField]
	private RawImage singleCameraFrameImage;
	[SerializeField]
	private Scuba_ShutterAnimation singleShutterAnimation;
	[SerializeField]
	private GameObject singlePictureObj;
	[SerializeField]
	private Transform singlePictureRightTop;
	[SerializeField]
	private Transform singlePictureLeftBottom;
	[SerializeField]
	private Scuba_GetPointUi[] singleGetPoints;
	[SerializeField]
	[Header("複数人用UI")]
	private GameObject multiPartition;
	[SerializeField]
	private SpriteRenderer[] multiCharaIcons;
	[SerializeField]
	private SpriteRenderer[] multiPlayerIcons;
	[SerializeField]
	private CommonGameTimeUI_Font_Time multiTime;
	[SerializeField]
	private SpriteNumbers[] multiScores;
	[SerializeField]
	private FoundView[] multiFoundViews;
	[SerializeField]
	private Transform[] multiStaminaGaugeAnchors;
	[SerializeField]
	private GameObject[] multiCtrlViewObjs;
	[SerializeField]
	private GameObject[] multiActionCtrlObjs;
	[SerializeField]
	private Canvas multiCanvas;
	[SerializeField]
	private RawImage[] multiCameraFrameImages;
	[SerializeField]
	private Scuba_ShutterAnimation[] multiShutterAnimations;
	[SerializeField]
	private GameObject[] multiPictureObjs;
	[SerializeField]
	private Transform[] multiPictureRightTops;
	[SerializeField]
	private Transform[] multiPictureLeftBottoms;
	[SerializeField]
	private GameObject[] multiPictureFrames;
	[SerializeField]
	private GetPointUiArray[] multiGetPointsArray;
	[SerializeField]
	private Sprite okSprite;
	[SerializeField]
	private Sprite foundSprite;
	[SerializeField]
	private Color[] cameraFrameColors;
	[SerializeField]
	private Color[] getPointColors;
	private bool isSingle;
	public void Init()
	{
		uiCamera = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>();
		int playerNum = SingletonCustom<Scuba_CharacterManager>.Instance.PlayerNum;
		isSingle = (playerNum == 1);
		if (isSingle)
		{
			singleObj.SetActive(value: true);
			multiObj.SetActive(value: false);
			singleUiLayout.Init(120f, SingletonCustom<GameSettingManager>.Instance.PlayerGroupList);
			singleCanvas.worldCamera = uiCamera;
			singleCameraFrameImage.color = cameraFrameColors[SingletonCustom<Scuba_CharacterManager>.Instance.GetChara(0).StyleCharaNo];
			singleCameraFrameImage.gameObject.SetActive(value: false);
			for (int i = 0; i < singleGetPoints.Length; i++)
			{
				singleGetPoints[i].Init();
			}
		}
		else
		{
			singleObj.SetActive(value: false);
			multiObj.SetActive(value: true);
			multiPartition.transform.parent = base.transform;
			multiTime.SetTime(120f);
			for (int j = 0; j < multiCharaIcons.Length; j++)
			{
				int num = (j < playerNum) ? SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[j] : SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[4 + j - playerNum];
				multiCharaIcons[j].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, CHARA_DEFAULT_SPRITE_NAMES[num]);
			}
			int num2 = 1;
			for (int k = playerNum; k < multiPlayerIcons.Length; k++)
			{
				multiPlayerIcons[k].sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_screen_cp" + num2.ToString());
				num2++;
			}
			for (int l = playerNum; l < multiCtrlViewObjs.Length; l++)
			{
				multiCtrlViewObjs[l].SetActive(value: false);
			}
			multiCanvas.worldCamera = uiCamera;
			for (int m = 0; m < multiCameraFrameImages.Length; m++)
			{
				multiCameraFrameImages[m].color = cameraFrameColors[SingletonCustom<Scuba_CharacterManager>.Instance.GetChara(m).StyleCharaNo];
				multiCameraFrameImages[m].gameObject.SetActive(value: false);
			}
			for (int n = 0; n < multiGetPointsArray.Length; n++)
			{
				for (int num3 = 0; num3 < multiGetPointsArray[n].getPoints.Length; num3++)
				{
					multiGetPointsArray[n].getPoints[num3].Init();
					multiGetPointsArray[n].getPoints[num3].SetAnchorScale(0.75f);
				}
			}
		}
		SetScoreArray(new int[4]);
		int playerNum2 = SingletonCustom<Scuba_CharacterManager>.Instance.PlayerNum;
	}
	public void UpdateMethod()
	{
		StaminaGaugeUpdate();
	}
	public void SetTime(float _time)
	{
		if (isSingle)
		{
			singleUiLayout.SetTime(_time);
		}
		else
		{
			multiTime.SetTime(_time);
		}
	}
	public void SetScore(int _charaNo, int _score)
	{
		if (isSingle)
		{
			singleUiLayout.SetScore(_charaNo, _score);
		}
		else
		{
			multiScores[_charaNo].Set(_score);
		}
	}
	public void SetScoreArray(int[] _scores)
	{
		if (isSingle)
		{
			singleUiLayout.SetScoreArray(_scores);
			return;
		}
		for (int i = 0; i < multiScores.Length; i++)
		{
			multiScores[i].Set(_scores[i]);
		}
	}
	public void SetFoundView(int _charaNo, Scuba_ItemObject[] _items)
	{
		Camera camera = SingletonCustom<Scuba_CharacterManager>.Instance.GetCamera(_charaNo);
		Vector3 position = camera.transform.position;
		Vector3 forward = camera.transform.forward;
		if (isSingle)
		{
			for (int i = 0; i < singleFoundView.sprites.Length; i++)
			{
				if (i < _items.Length)
				{
					singleFoundView.sprites[i].gameObject.SetActive(value: true);
					singleFoundView.sprites[i].sprite = (_items[i].GetIsFound(_charaNo) ? foundSprite : okSprite);
					Vector3 vector = camera.WorldToViewportPoint(_items[i].GetUiPos(position, forward));
					singleFoundView.SetLocalPos(i, vector);
					float t = Mathf.InverseLerp(1.5f, 0.1f, vector.z);
					float num = _items[i].GetEasyMagZ();
					if (num < 1f)
					{
						num = Mathf.Lerp(1f, num, t);
					}
					if (_items[i].GetIsFound(_charaNo))
					{
						num *= 0.8f;
					}
					singleFoundView.SetLocalScale(i, Mathf.Lerp(0.1f, 0.28f, t) * num);
				}
				else
				{
					singleFoundView.sprites[i].gameObject.SetActive(value: false);
				}
			}
			return;
		}
		for (int j = 0; j < multiFoundViews[_charaNo].sprites.Length; j++)
		{
			if (j < _items.Length)
			{
				multiFoundViews[_charaNo].sprites[j].gameObject.SetActive(value: true);
				multiFoundViews[_charaNo].sprites[j].sprite = (_items[j].GetIsFound(_charaNo) ? foundSprite : okSprite);
				Vector3 vector2 = camera.WorldToViewportPoint(_items[j].GetUiPos(position, forward));
				multiFoundViews[_charaNo].SetLocalPos(j, vector2);
				float t2 = Mathf.InverseLerp(1.5f, 0.1f, vector2.z);
				float num2 = _items[j].GetEasyMagZ();
				if (num2 < 1f)
				{
					num2 = Mathf.Lerp(1f, num2, t2);
				}
				if (_items[j].GetIsFound(_charaNo))
				{
					num2 *= 0.8f;
				}
				multiFoundViews[_charaNo].SetLocalScale(j, Mathf.Lerp(0.075f, 0.21f, t2) * num2);
			}
			else
			{
				multiFoundViews[_charaNo].sprites[j].gameObject.SetActive(value: false);
			}
		}
	}
	private void StaminaGaugeUpdate()
	{
		if (isSingle)
		{
			singleStaminaGaugeAnchor.SetLocalScaleX(SingletonCustom<Scuba_CharacterManager>.Instance.GetChara(0).GetStaminaPer());
			return;
		}
		for (int i = 0; i < multiStaminaGaugeAnchors.Length; i++)
		{
			multiStaminaGaugeAnchors[i].SetLocalScaleX(SingletonCustom<Scuba_CharacterManager>.Instance.GetChara(i).GetStaminaPer());
		}
	}
	public void SetActionCtrlView(int _playerNo, bool _active)
	{
		if (isSingle)
		{
			if (_playerNo == 0)
			{
				singleActionCtrlObj.SetActive(_active);
			}
		}
		else
		{
			multiActionCtrlObjs[_playerNo].SetActive(_active);
		}
	}
	public void SetCameraFrameView(int _playerNo, bool _active)
	{
		if (isSingle)
		{
			singleCameraFrameImage.gameObject.SetActive(_active);
		}
		else
		{
			multiCameraFrameImages[_playerNo].gameObject.SetActive(_active);
		}
	}
	public void ShutterAnimationPlay(int _playerNo)
	{
		if (isSingle)
		{
			ShutterAnimationDirection(singleShutterAnimation);
		}
		else
		{
			ShutterAnimationDirection(multiShutterAnimations[_playerNo]);
		}
	}
	private void ShutterAnimationDirection(Scuba_ShutterAnimation _shutterAnim)
	{
		_shutterAnim.gameObject.SetActive(value: true);
		_shutterAnim.SetAmount(0f);
	}
	public void ShutterAnimationEnd(int _playerNo)
	{
		if (isSingle)
		{
			singleShutterAnimation.gameObject.SetActive(value: false);
		}
		else
		{
			multiShutterAnimations[_playerNo].gameObject.SetActive(value: false);
		}
	}
	public void PictureView(int _playerNo)
	{
		if (isSingle)
		{
			StartCoroutine(_PictureViewDirection(_playerNo));
		}
		else
		{
			StartCoroutine(_PictureViewDirection(_playerNo));
		}
	}
	private IEnumerator _PictureViewDirection(int _playerNo)
	{
		GameObject pictureObj = isSingle ? singlePictureObj : multiPictureObjs[_playerNo];
		GameObject pictureFrame = isSingle ? null : multiPictureFrames[_playerNo];
		bool isFps = SingletonCustom<Scuba_CharacterManager>.Instance.GetIsCameraFps(_playerNo);
		Vector3 localPos = pictureObj.transform.localPosition;
		Vector3 localScale = pictureObj.transform.localScale;
		Vector3 frameLocalScale = Vector3.one;
		if (isFps)
		{
			if (isSingle)
			{
				pictureObj.transform.localPosition = SINGLE_PUCTURE_BIG_POS;
				pictureObj.transform.localScale = SINGLE_PUCTURE_BIG_SIZE;
			}
			else
			{
				pictureObj.transform.localPosition = MULTI_PUCTURE_BIG_POSES[_playerNo];
				pictureObj.transform.localScale = MULTI_PUCTURE_BIG_SIZE;
				frameLocalScale = pictureFrame.transform.localScale;
				pictureFrame.transform.localScale = Vector3.one;
			}
			pictureObj.transform.SetLocalPositionZ(localPos.z);
		}
		else
		{
			pictureObj.transform.localScale = new Vector3(0f, 0f, 1f);
		}
		pictureObj.SetActive(value: true);
		yield return new WaitForSeconds(0.5f);
		LeanTween.moveLocal(pictureObj, localPos, 0.3f);
		LeanTween.scale(pictureObj, localScale, 0.3f);
		if (isFps && !isSingle)
		{
			LeanTween.scale(pictureFrame, frameLocalScale, 0.05f);
		}
		yield return new WaitForSeconds(0.5f);
		ViewGetPoint(_playerNo);
		yield return new WaitForSeconds(1f);
		pictureObj.SetActive(value: false);
	}
	private void ViewGetPoint(int _playerNo)
	{
		Vector3 vector = isSingle ? singlePictureRightTop.position : multiPictureRightTops[_playerNo].position;
		Vector3 vector2 = isSingle ? singlePictureLeftBottom.position : multiPictureLeftBottoms[_playerNo].position;
		Scuba_CharacterManager.PictureViewPointData[] pictureViewPointDataArray = SingletonCustom<Scuba_CharacterManager>.Instance.GetPictureViewPointDataArray(_playerNo);
		Scuba_GetPointUi[] array = isSingle ? singleGetPoints : multiGetPointsArray[_playerNo].getPoints;
		int num = Mathf.Min(pictureViewPointDataArray.Length, array.Length);
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < num; i++)
		{
			array[i].SetColor(getPointColors[pictureViewPointDataArray[i].GetColorTypeIndex()]);
			zero.x = Mathf.Lerp(vector2.x, vector.x, pictureViewPointDataArray[i].viewport.x);
			zero.y = Mathf.Lerp(vector2.y, vector.y, pictureViewPointDataArray[i].viewport.y);
			zero.z = pictureViewPointDataArray[i].viewport.z;
			array[i].Show(pictureViewPointDataArray[i].point, zero);
		}
	}
	public Color GetPointColor(int _idx)
	{
		return getPointColors[_idx];
	}
	public Color[] GetPointColors()
	{
		return getPointColors;
	}
	public void SetUIActive(bool _active)
	{
		if (isSingle)
		{
			singleObj.SetActive(_active);
		}
		else
		{
			multiObj.SetActive(_active);
		}
	}
	public void CloseGameUI()
	{
		singleObj.SetActive(value: false);
		multiObj.SetActive(value: false);
	}
}
