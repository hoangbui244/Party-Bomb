using UnityEngine;
public class FireworksPoint : MonoBehaviour
{
	[SerializeField]
	[Header("数値")]
	private SpriteNumbers number;
	[SerializeField]
	[Header("表示スプライト")]
	private SpriteRenderer spRenderer;
	[SerializeField]
	[Header("ポイント150スプライト")]
	private Sprite[] arrayPoint150;
	[SerializeField]
	[Header("ポイント200スプライト")]
	private Sprite[] arrayPoint200;
	[SerializeField]
	[Header("ポイント250スプライト")]
	private Sprite[] arrayPoint250;
	[SerializeField]
	[Header("ポイント300スプライト")]
	private Sprite[] arrayPoint300;
	private readonly int MAX_LENGTH = 4;
	private float waitTime = 2f;
	public void Set(FireworksDefine.UserType _userType, int _score)
	{
		number.Set(_score);
		UnityEngine.Debug.Log("set:" + _userType.ToString());
		switch (_score)
		{
		case 150:
			spRenderer.sprite = arrayPoint150[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)_userType]];
			break;
		case 200:
			spRenderer.sprite = arrayPoint200[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)_userType]];
			break;
		case 250:
			spRenderer.sprite = arrayPoint250[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)_userType]];
			break;
		case 300:
			spRenderer.sprite = arrayPoint300[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)_userType]];
			break;
		}
	}
	private void Update()
	{
		waitTime -= Time.deltaTime;
		if (waitTime <= 1.5f)
		{
			base.transform.AddLocalPositionY(Time.deltaTime * 50f);
		}
		spRenderer.SetAlpha(waitTime * 2f);
		if (waitTime <= 0f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
