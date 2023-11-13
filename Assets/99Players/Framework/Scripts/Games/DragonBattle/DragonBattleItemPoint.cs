using UnityEngine;
public class DragonBattleItemPoint : MonoBehaviour
{
	public enum RareType
	{
		RANDOM,
		NORMAL,
		RARE,
		RARE_S,
		RARE_H,
		MAX
	}
	[SerializeField]
	private RareType rareType;
	[SerializeField]
	private MeshRenderer obj;
	private float rotSpeed = 50f;
	private float moveSpeed = 2.5f;
	private float moveRange = 0.5f;
	private float moveTime;
	public RareType Type => rareType;
	private void Start()
	{
		Init();
	}
	public void Init()
	{
		if (rareType == RareType.RANDOM)
		{
			rareType = (RareType)UnityEngine.Random.Range(1, 5);
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(SingletonCustom<DragonBattleResources>.Instance.ObjectList.jewel[(int)rareType], base.transform);
		gameObject.transform.SetLocalPosition(0f, 0f, 0f);
		gameObject.transform.SetLocalEulerAngles(0f, 0f, 0f);
		gameObject.SetActive(value: true);
		obj = gameObject.GetComponentInChildren<MeshRenderer>();
		obj.material.color = SingletonCustom<DragonBattleResources>.Instance.ColorList.jewel[(int)rareType];
		moveTime = 0f;
	}
	private void Update()
	{
		obj.transform.AddLocalEulerAnglesY(rotSpeed * Time.deltaTime);
		moveTime += moveSpeed * Time.deltaTime;
		obj.transform.SetLocalPositionY(Mathf.Sin(moveTime) * moveRange);
	}
	private void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.layer == DragonBattleDefine.ConvertLayerNo("Character"))
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_goeraser_item_0");
			DragonBattlePlayer dragonBattlePlayer = SingletonCustom<DragonBattlePlayerManager>.Instance.CheckPlayer(collision.gameObject);
			if (dragonBattlePlayer != null)
			{
				int value = DragonBattleDefine.SCORE_ITEM[(int)(Type - 1)];
				dragonBattlePlayer.AddScore(value, _isShowValue: true, _isVibration: false);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
