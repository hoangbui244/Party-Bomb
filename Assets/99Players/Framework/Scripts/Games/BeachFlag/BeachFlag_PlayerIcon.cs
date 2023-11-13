using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class BeachFlag_PlayerIcon : MonoBehaviour
{
	private enum PositionState
	{
		FRONT,
		UP,
		RIGHT
	}
	private enum ICON_TYPE
	{
		HAED_ICON,
		OUT_ICON
	}
	[SerializeField]
	[Header("基準とするカメラ")]
	private CinemachineBrain maincamera;
	[SerializeField]
	[Header("対象となるプレイヤ\u30fc")]
	private BeachFlag_Player player;
	[SerializeField]
	[Header("使用するアイコンのリスト")]
	private Sprite[] icons;
	[SerializeField]
	[Header("画面外アイコン下地")]
	private Image out_view;
	[SerializeField]
	[Header("表示するアイコンの種類")]
	private ICON_TYPE iCON;
	private Image img_component;
	private Vector2 distance = Vector2.zero;
	private Coroutine cor;
	private Vector3 screenPos;
	public CinemachineBrain MainCam => maincamera;
	private void Start()
	{
		SetIcon();
	}
	public void SetIcon()
	{
		img_component = GetComponent<Image>();
		Quaternion rotation = new Quaternion(0f, 0f, 0f, 0f);
		img_component.color = new Color(1f, 1f, 1f, 1f);
		base.transform.rotation = rotation;
		cor = null;
		img_component.sprite = icons[(int)player.UserType];
		switch (iCON)
		{
		case ICON_TYPE.HAED_ICON:
			img_component.color = new Color(1f, 1f, 1f, 0f);
			img_component.enabled = true;
			out_view.enabled = false;
			break;
		case ICON_TYPE.OUT_ICON:
			img_component.color = new Color(1f, 1f, 1f, 0f);
			img_component.enabled = false;
			out_view.enabled = true;
			switch (player.UserType)
			{
			case BeachFlag_Define.UserType.PLAYER_1:
				out_view.color = new Color(0f, 1f, 0f);
				break;
			case BeachFlag_Define.UserType.PLAYER_2:
				out_view.color = new Color(1f, 0f, 0f);
				break;
			case BeachFlag_Define.UserType.PLAYER_3:
				out_view.color = new Color(0f, 0f, 1f);
				break;
			case BeachFlag_Define.UserType.PLAYER_4:
				out_view.color = new Color(1f, 1f, 0f);
				break;
			}
			break;
		}
		img_component.SetNativeSize();
	}
	private void Update()
	{
		UnityEngine.Debug.Log("GAME STATE LOG " + BeachFlag_Define.GM.GetGameState().ToString());
		if (player.isover)
		{
			img_component.enabled = false;
		}
		else
		{
			img_component.enabled = true;
		}
		switch (iCON)
		{
		case ICON_TYPE.HAED_ICON:
		{
			switch (BeachFlag_Define.GM.GetGameState())
			{
			case BeachFlag_GameManager.GameState.GAME_START_STANDBY:
			case BeachFlag_GameManager.GameState.GAME_START_WAIT:
			case BeachFlag_GameManager.GameState.ROUND_START_STANDBY:
			case BeachFlag_GameManager.GameState.ROUND_CONTINUE_START_STANDBY:
			case BeachFlag_GameManager.GameState.ROUND_START_WAIT:
				if (player.isPlay)
				{
					img_component.enabled = true;
				}
				else
				{
					img_component.enabled = false;
				}
				if (BeachFlag_Define.CaM.UIDisplay && img_component.color.a <= 0f)
				{
					LeanTween.alpha(base.gameObject.GetComponent<RectTransform>(), 1f, 0.4f);
				}
				else if (!BeachFlag_Define.CaM.UIDisplay && img_component.color.a >= 1f)
				{
					LeanTween.alpha(base.gameObject.GetComponent<RectTransform>(), 0f, 0.4f);
				}
				break;
			case BeachFlag_GameManager.GameState.DURING_GAME:
				LeanTween.alpha(base.gameObject.GetComponent<RectTransform>(), 0f, 0.4f);
				break;
			case BeachFlag_GameManager.GameState.ROUND_END:
			case BeachFlag_GameManager.GameState.ROUND_END_WAIT:
			case BeachFlag_GameManager.GameState.ROUND_CONTINUE_WAIT:
			case BeachFlag_GameManager.GameState.GAME_END:
			case BeachFlag_GameManager.GameState.GAME_END_WAIT:
			case BeachFlag_GameManager.GameState.ANIMATION_WAIT:
			case BeachFlag_GameManager.GameState.NONE:
				img_component.color = new Color(1f, 1f, 1f, 1f);
				img_component.enabled = false;
				break;
			}
			distance = GetUIDistance(SetAndGetPositionState());
			screenPos = maincamera.GetComponent<Camera>().WorldToScreenPoint(player.Chara.Base_obj.transform.position);
			Vector3 vector = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToViewportPoint(screenPos);
			vector = new Vector3(Mathf.Clamp(1920f * vector.x, 300f, 1020f), Mathf.Clamp(1080f * vector.y, 300f, 880f), vector.z);
			string str2 = player.UserType.ToString();
			Vector3 vector2 = vector;
			UnityEngine.Debug.Log(str2 + " <color=red>" + vector2.ToString() + "</color>");
			Vector3 target = new Vector3(vector.x + distance.x, vector.y + distance.y, vector.z);
			GetComponent<RectTransform>().anchoredPosition = Vector3.MoveTowards(GetComponent<RectTransform>().anchoredPosition, target, 100f);
			break;
		}
		case ICON_TYPE.OUT_ICON:
			if (!player.isover && BeachFlag_Define.GM.GetGameState() >= BeachFlag_GameManager.GameState.DURING_GAME && BeachFlag_Define.GM.GetGameState() < BeachFlag_GameManager.GameState.ROUND_END)
			{
				screenPos = maincamera.GetComponent<Camera>().WorldToScreenPoint(player.Chara.Base_obj.transform.position);
				int num = (player.Chara.PlayerNo == 0) ? 1 : 0;
				if (Vector3.Magnitude(player.Chara.Base_obj.transform.position - BeachFlag_Define.PM.Players[num].Chara.Base_obj.transform.position) > 0f)
				{
					bool flag = !CheckScreenOut(screenPos);
					string str = flag ? "visible" : "invisible";
					UnityEngine.Debug.Log("The player " + player.UserType.ToString() + " is " + str);
					if (!flag && img_component.color.a < 1f)
					{
						out_view.gameObject.GetComponent<RectTransform>().position = base.gameObject.GetComponent<RectTransform>().position;
						LeanTween.alpha(base.gameObject.GetComponent<RectTransform>(), 1f, 0.4f);
						LeanTween.alpha(out_view.gameObject.GetComponent<RectTransform>(), 1f, 0.4f);
					}
					else if (flag && img_component.color.a > 0f)
					{
						LeanTween.alpha(base.gameObject.GetComponent<RectTransform>(), 0f, 0.4f);
						LeanTween.alpha(out_view.gameObject.GetComponent<RectTransform>(), 0f, 0.4f);
					}
				}
			}
			else
			{
				img_component.color = new Color(1f, 1f, 1f, 0f);
				out_view.color = new Color(out_view.color.r, out_view.color.g, out_view.color.b, 0f);
			}
			break;
		}
	}
	private PositionState SetAndGetPositionState()
	{
		PositionState result = PositionState.FRONT;
		switch (BeachFlag_Define.GM.GetGameState())
		{
		case BeachFlag_GameManager.GameState.GAME_START_STANDBY:
		case BeachFlag_GameManager.GameState.GAME_START_WAIT:
		case BeachFlag_GameManager.GameState.ROUND_START_STANDBY:
		case BeachFlag_GameManager.GameState.ROUND_CONTINUE_START_STANDBY:
		case BeachFlag_GameManager.GameState.ROUND_START_WAIT:
		case BeachFlag_GameManager.GameState.DURING_GAME:
			result = PositionState.UP;
			break;
		}
		return result;
	}
	private Vector2 GetUIDistance(PositionState _state)
	{
		switch (_state)
		{
		case PositionState.FRONT:
			return Vector2.zero;
		case PositionState.UP:
			return new Vector2(0f, 200f);
		case PositionState.RIGHT:
			return new Vector2(200f, 0f);
		default:
			return Vector2.zero;
		}
	}
	private bool CheckScreenOut(Vector3 _pos)
	{
		string[] obj = new string[5]
		{
			"CHECK SCREEN OUT ",
			player.UserType.ToString(),
			" <color=red>",
			null,
			null
		};
		Vector3 mCalcVector = CalcManager.mCalcVector3;
		obj[3] = mCalcVector.ToString();
		obj[4] = "</color>";
		UnityEngine.Debug.Log(string.Concat(obj));
		CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToViewportPoint(_pos);
		if (CalcManager.mCalcVector3.x < -0f || CalcManager.mCalcVector3.x > 1f || CalcManager.mCalcVector3.y < -0f || CalcManager.mCalcVector3.y > 1f)
		{
			return true;
		}
		return false;
	}
}
