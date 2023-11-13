using System;
using System.Collections.Generic;
using UnityEngine;
public class GS_FieldMap : MonoBehaviour
{
	public class TargetData
	{
		public int idx;
		public float distance;
		public TargetData(int _idx, float _distance)
		{
			idx = _idx;
			distance = _distance;
		}
	}
	public enum State
	{
		NONE,
		MOVE
	}
	public enum MoveType
	{
		UP,
		DOWN,
		LEFT,
		RIGHT
	}
	private readonly Vector2 MAP_LIMIT_BOTTOM = new Vector2(800f, -500f);
	private readonly Vector2 MAP_LIMIT_TOP = new Vector2(-898f, 402f);
	[SerializeField]
	[Header("マップオブジェクト")]
	private GameObject objFieldMap;
	[SerializeField]
	[Header("マップ移動キャラ")]
	private GameObject objMoveCharacter;
	[SerializeField]
	[Header("マップ停止判定キャラアンカ\u30fc")]
	private Transform characterStopAnchor;
	[SerializeField]
	[Header("マップ移動カ\u30fcソル")]
	private GameObject moveCursor;
	[SerializeField]
	[Header("移動ポイント")]
	private Transform[] arrayMovePoint;
	[SerializeField]
	[Header("ノ\u30fcド情報")]
	private GS_MapNode[] arrayNode;
	[SerializeField]
	[Header("移動マ\u30fcカ\u30fc")]
	private GameObject moveMarker;
	[SerializeField]
	[Header("移動先アイコン")]
	private GameObject[] arrayIcons;
	[SerializeField]
	[Header("停止判定オブジェクト")]
	private Transform[] arrayStopAnchor;
	[SerializeField]
	[Header("DLC1追加ボタン")]
	private GameObject[] arrayDLC1AddButtons;
	[SerializeField]
	[Header("[標準先頭]段ボ\u30fcル滑りボタン")]
	private CursorButtonObject buttonCardboardSlide;
	[SerializeField]
	[Header("[標準最後]水鉄砲バトルボタン")]
	private CursorButtonObject buttonWaterPistolBattle;
	[SerializeField]
	[Header("カ\u30fcソルキャラ")]
	private SpriteRenderer spRenderCursorChar;
	[SerializeField]
	[Header("カ\u30fcソルキャラ画像")]
	private Sprite[] arraySpRenderCursorChar;
	[SerializeField]
	[Header("カ\u30fcソルフレ\u30fcム")]
	private SpriteRenderer spRenderCursorFrame;
	[SerializeField]
	[Header("カ\u30fcソルフレ\u30fcム画像")]
	private Sprite[] arraySpRenderCursorFrame;
	[SerializeField]
	[Header("鳥エフェクト")]
	private ParticleSystem psBird;
	private bool isMoveInput;
	private int currentIdx;
	private int targetIdx;
	private int moveTempIdx;
	private bool isMove;
	private bool isUpdateTarget;
	private List<int> moveLog = new List<int>();
	private List<int> searchLog = new List<int>();
	private readonly Vector3 BUTTON_SIZE = new Vector3(256f, 64f, 0f);
	private readonly Vector3 POS_INIT_CURSOR = new Vector3(68f, 224f, 0f);
	private readonly float CURSOR_SPEED = 15f;
	private Vector3 cursorMoveOffset;
	private List<TargetData> listTarget = new List<TargetData>();
	private List<int> listAttract = new List<int>();
	private State currentState;
	public int TargetIdx => targetIdx;
	public void Init(int _cursorIdx, bool _isInitTween = true)
	{
		LeanTween.cancel(objMoveCharacter);
		currentState = State.NONE;
		if (_cursorIdx == -1)
		{
			_cursorIdx = 0;
			Move(_cursorIdx, _isSe: false);
		}
		currentIdx = _cursorIdx;
		targetIdx = currentIdx;
		objMoveCharacter.transform.localPosition = arrayIcons[targetIdx].transform.localPosition;
		cursorMoveOffset = arrayIcons[targetIdx].transform.localPosition - POS_INIT_CURSOR;
		isMove = false;
		isUpdateTarget = false;
		moveMarker.SetActive(value: true);
		moveMarker.transform.SetPositionX(arrayIcons[_cursorIdx].transform.position.x);
		moveMarker.transform.SetPositionY(arrayIcons[_cursorIdx].transform.position.y);
		moveMarker.transform.SetLocalScale(1.5f, 1.5f, 1.5f);
		arrayIcons[targetIdx].transform.SetLocalScale(1f, 1f, 1f);
		LeanTween.cancel(arrayIcons[targetIdx]);
		LeanTween.scale(arrayIcons[targetIdx], Vector3.one * 1.15f, 0.9f).setEaseInOutQuad().setLoopPingPong();
		spRenderCursorChar.sprite = arraySpRenderCursorChar[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[0]];
		spRenderCursorFrame.sprite = arraySpRenderCursorFrame[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[0]];
		MapInit();
		isMoveInput = false;
	}
	public bool IsUpdateTarget()
	{
		return isUpdateTarget;
	}
	public void SetDefaultMap()
	{
	}
	public void MapInit()
	{
	}
	private void OnEnable()
	{
		Init(targetIdx, _isInitTween: false);
	}
	public int GetMoveIdx()
	{
		if (listTarget.Count <= 0)
		{
			return targetIdx;
		}
		return listTarget[0].idx;
	}
	public int InputMove(Vector3 _dir)
	{
		CalcManager.mCalcVector3 = objMoveCharacter.transform.position;
		CalcManager.mCalcVector3.y -= 62f;
		listTarget.Clear();
		for (int i = 0; i < GS_Define.FIRST_GAME_NUM; i++)
		{
			if (i != currentIdx)
			{
				UnityEngine.Debug.Log("i:" + i.ToString() + " angle:" + Vector3.Angle(_dir, arrayMovePoint[i].position - CalcManager.mCalcVector3).ToString());
				if (Vector3.Angle(_dir, arrayMovePoint[i].position - CalcManager.mCalcVector3) < 27f)
				{
					UnityEngine.Debug.Log("add:" + i.ToString());
					listTarget.Add(new TargetData(i, (arrayMovePoint[i].position - CalcManager.mCalcVector3).sqrMagnitude));
				}
			}
		}
		if (listTarget.Count <= 0)
		{
			return -1;
		}
		listTarget.Sort((TargetData a, TargetData b) => (int)a.distance - (int)b.distance);
		SingletonCustom<AudioManager>.Instance.SePlay("se_cursor_move");
		Move(listTarget[0].idx);
		return listTarget[0].idx;
	}
	public void Move(int _cursorIdx, bool _isSe = true)
	{
		if (targetIdx != _cursorIdx)
		{
			UnityEngine.Debug.Log("stop:" + targetIdx.ToString() + " c:" + _cursorIdx.ToString());
			for (int i = 0; i < arrayIcons.Length; i++)
			{
				LeanTween.cancel(arrayIcons[i]);
				arrayIcons[i].transform.SetLocalScale(1f, 1f, 1f);
			}
			isUpdateTarget = true;
			targetIdx = _cursorIdx;
			UnityEngine.Debug.Log("目標設定:" + targetIdx.ToString());
			moveMarker.SetActive(value: true);
			moveMarker.transform.SetPositionX(arrayIcons[_cursorIdx].transform.position.x);
			moveMarker.transform.SetPositionY(arrayIcons[_cursorIdx].transform.position.y);
			moveMarker.transform.SetLocalScale(1.5f, 1.5f, 1.5f);
			LeanTween.cancel(moveMarker);
			LeanTween.scale(moveMarker, Vector3.one * 2f, 0.5f).setLoopPingPong();
			if (_isSe)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_cursor_move");
			}
			LeanTween.cancel(arrayIcons[targetIdx]);
			LeanTween.scale(arrayIcons[targetIdx], Vector3.one * 1.15f, 0.9f).setEaseInOutQuad().setLoopPingPong();
		}
	}
	private void SetMovePos()
	{
		moveTempIdx = targetIdx;
		int num = 0;
		bool flag = false;
		searchLog.Clear();
		if (moveLog.Count == 0)
		{
			return;
		}
		while (moveTempIdx != moveLog[0])
		{
			int idx = arrayNode[moveTempIdx].ArrayRelatedPoint[arrayNode[moveTempIdx].ArrayRelatedPoint.Length - 1].idx;
			if (idx == moveLog[0])
			{
				break;
			}
			UnityEngine.Debug.Log("tempMoveIdx:" + moveTempIdx.ToString() + "movePoint:" + idx.ToString() + " targetIdx:" + targetIdx.ToString());
			if ((searchLog.Contains(idx) || idx == targetIdx) && arrayNode[moveTempIdx].ArrayRelatedPoint.Length > 1)
			{
				idx = arrayNode[moveTempIdx].ArrayRelatedPoint[1].idx;
			}
			for (int i = 0; i < arrayNode[moveTempIdx].ArrayRelatedPoint.Length; i++)
			{
				if (arrayNode[moveTempIdx].ArrayRelatedPoint[i].idx == moveLog[moveLog.Count - 1])
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
			for (int j = 0; j < arrayNode[moveTempIdx].ArrayRelatedPoint.Length; j++)
			{
				if (arrayNode[moveTempIdx].ArrayRelatedPoint[j].idx != idx && arrayNode[moveTempIdx].ArrayRelatedPoint[j].idx != targetIdx && !searchLog.Contains(arrayNode[moveTempIdx].ArrayRelatedPoint[j].idx) && !moveLog.Contains(arrayNode[moveTempIdx].ArrayRelatedPoint[j].idx))
				{
					if (arrayNode[moveTempIdx].ArrayRelatedPoint[j].idx == moveLog[0])
					{
						break;
					}
					if (CalcManager.Length(arrayMovePoint[arrayNode[moveTempIdx].ArrayRelatedPoint[j].idx].position, arrayMovePoint[moveLog[0]].position) < CalcManager.Length(arrayMovePoint[idx].position, arrayMovePoint[moveLog[0]].position))
					{
						idx = arrayNode[moveTempIdx].ArrayRelatedPoint[j].idx;
					}
				}
			}
			moveTempIdx = idx;
			searchLog.Add(moveTempIdx);
			UnityEngine.Debug.Log("moveTemp_Idx:" + moveTempIdx.ToString());
			num++;
			if (num >= 1000)
			{
				UnityEngine.Debug.Log("Break!!!:" + moveTempIdx.ToString());
				break;
			}
		}
		moveLog.Add(moveTempIdx);
		if (moveTempIdx == -1)
		{
			moveLog.Clear();
			if (moveTempIdx != targetIdx)
			{
				moveLog.Add(currentIdx);
			}
			SetMovePos();
			return;
		}
		isMove = true;
		if (moveTempIdx == targetIdx)
		{
			LeanTween.cancel(objMoveCharacter);
			LeanTween.moveLocal(objMoveCharacter, new Vector3(arrayMovePoint[moveTempIdx].localPosition.x, arrayMovePoint[moveTempIdx].localPosition.y + 62f, objMoveCharacter.transform.localPosition.z), Mathf.Clamp(CalcManager.Length(arrayMovePoint[moveTempIdx].position, objMoveCharacter.transform.position) * 0.0025f, 0.3f, 1f)).setOnComplete((Action)delegate
			{
				currentIdx = moveTempIdx;
				isMove = false;
				if (isUpdateTarget)
				{
					moveLog.Clear();
					if (moveTempIdx != targetIdx)
					{
						moveLog.Add(currentIdx);
					}
					SetMovePos();
					isUpdateTarget = false;
				}
				else
				{
					LeanTween.cancel(arrayIcons[targetIdx]);
					LeanTween.scale(arrayIcons[targetIdx], Vector3.one * 1.15f, 0.9f).setEaseInOutQuad().setLoopPingPong();
				}
			}).setEaseLinear();
		}
		else
		{
			LeanTween.cancel(objMoveCharacter);
			LeanTween.moveLocal(objMoveCharacter, new Vector3(arrayMovePoint[moveTempIdx].localPosition.x, arrayMovePoint[moveTempIdx].localPosition.y + 62f, objMoveCharacter.transform.localPosition.z), Mathf.Clamp(CalcManager.Length(arrayMovePoint[moveTempIdx].position, objMoveCharacter.transform.position) * 0.0025f, 0.3f, 1f)).setOnComplete((Action)delegate
			{
				currentIdx = moveTempIdx;
				isMove = false;
				if (isUpdateTarget)
				{
					moveLog.Clear();
					if (moveTempIdx != targetIdx)
					{
						moveLog.Add(currentIdx);
					}
					isUpdateTarget = false;
				}
				SetMovePos();
			}).setEaseLinear();
		}
	}
	public void SetMoveInput()
	{
		isMoveInput = true;
	}
	private void Update()
	{
		if (!SingletonCustom<SceneManager>.Instance.IsFade && !SingletonCustom<GS_Setting>.Instance.IsActive && base.gameObject.activeSelf && isMoveInput && !SingletonCustom<GS_GameSelectManager>.Instance.IsMapFade)
		{
			isUpdateTarget = false;
		}
	}
	public void MoveLeft()
	{
		int num = targetIdx - 1;
		if (num < 0)
		{
			num = arrayIcons.Length - 1;
		}
		Move(num);
		cursorMoveOffset = arrayIcons[num].transform.localPosition - POS_INIT_CURSOR;
		objMoveCharacter.transform.localPosition = POS_INIT_CURSOR + cursorMoveOffset;
	}
	public void MoveRight()
	{
		int num = targetIdx + 1;
		if (num >= arrayIcons.Length)
		{
			num = 0;
		}
		Move(num);
		cursorMoveOffset = arrayIcons[num].transform.localPosition - POS_INIT_CURSOR;
		objMoveCharacter.transform.localPosition = POS_INIT_CURSOR + cursorMoveOffset;
	}
	private void CheckAttract(bool _isMove)
	{
		listAttract.Clear();
		bool flag = false;
		for (int i = 0; i < arrayIcons.Length; i++)
		{
			CalcManager.mCalcVector3 = arrayStopAnchor[i].position;
			CalcManager.mCalcVector3.z = characterStopAnchor.position.z;
			if (Vector3.Distance(characterStopAnchor.position, CalcManager.mCalcVector3) <= 80f)
			{
				if (targetIdx != i)
				{
					Move(i);
				}
				flag = true;
			}
			if (Vector3.Distance(characterStopAnchor.position, CalcManager.mCalcVector3) <= 130f)
			{
				listAttract.Add(i);
			}
		}
		if (!flag && targetIdx != -1)
		{
			moveMarker.SetActive(value: false);
			LeanTween.cancel(moveMarker);
			for (int j = 0; j < arrayIcons.Length; j++)
			{
				LeanTween.cancel(arrayIcons[j]);
				arrayIcons[j].transform.SetLocalScale(1f, 1f, 1f);
			}
			targetIdx = -1;
		}
		if (_isMove || listAttract.Count <= 0)
		{
			return;
		}
		float num = 1000f;
		int num2 = 0;
		for (int k = 0; k < listAttract.Count; k++)
		{
			if (Vector3.Distance(objMoveCharacter.transform.localPosition, arrayIcons[listAttract[k]].transform.localPosition) < num)
			{
				num = Vector3.Distance(objMoveCharacter.transform.localPosition, arrayIcons[listAttract[k]].transform.localPosition);
				num2 = listAttract[k];
			}
		}
		cursorMoveOffset = Vector3.Slerp(cursorMoveOffset, arrayIcons[num2].transform.localPosition - POS_INIT_CURSOR, 0.1f);
	}
	private bool IsSquareIn(Vector3 _point, Vector3 _squarePos, Vector3 _squareSize)
	{
		if (_point.x >= _squarePos.x - _squareSize.x * 0.5f && _point.x <= _squarePos.x + _squareSize.x * 0.5f && _point.y >= _squarePos.y - _squareSize.y * 0.5f && _point.y <= _squarePos.y + _squareSize.y * 0.5f)
		{
			return true;
		}
		return false;
	}
	private void OnDestroy()
	{
		LeanTween.cancel(objMoveCharacter);
		LeanTween.cancel(moveMarker);
		isMove = false;
		for (int i = 0; i < arrayIcons.Length; i++)
		{
			LeanTween.cancel(arrayIcons[i]);
		}
	}
	public Vector3 GetMoveDir(int _no)
	{
		float num = 0f;
		float num2 = 0f;
		int playerIdx = (!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? _no : 0;
		JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(playerIdx);
		num = axisInput.Stick_L.x;
		num2 = axisInput.Stick_L.y;
		return (Vector2)new Vector3(num, num2);
	}
}
