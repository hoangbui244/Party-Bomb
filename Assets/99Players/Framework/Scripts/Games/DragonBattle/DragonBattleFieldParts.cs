using UnityEngine;
public class DragonBattleFieldParts : MonoBehaviour
{
	private bool isBossField;
	[SerializeField]
	[Header("落下判定コライダ\u30fc")]
	private BoxCollider checkCollider;
	[SerializeField]
	[Header("復活位置アンカ\u30fc")]
	private Transform[] arrayRespawnAnchor;
	[SerializeField]
	[Header("有効化ボ\u30fcダ\u30fc")]
	private Transform activeBorderAnchor;
	[SerializeField]
	[Header("敵リスト")]
	[ContextMenuItem("AutoSetting", "AutoSetting")]
	private DragonBattleEnemyNinja[] enemys;
	private DragonBattlePlayer[] players;
	private int step;
	private bool isEnemyActive;
	private bool isFieldActive;
	[SerializeField]
	private GameObject moveBlock;
	public bool IsBossField => isBossField;
	public int Step => step;
	public Transform[] ArrayRespawnAnchor => arrayRespawnAnchor;
	public Transform ActiveBorderAnchor => activeBorderAnchor;
	public DragonBattleEnemyNinja[] Enemys => enemys;
	public bool IsEnemyActive => isEnemyActive;
	public bool IsFieldActive => isFieldActive;
	public void Init(int _step)
	{
		players = SingletonCustom<DragonBattlePlayerManager>.Instance.GetArrayPlayer();
		for (int i = 0; i < enemys.Length; i++)
		{
			enemys[i].Init();
			if (enemys[i].IsBoss)
			{
				isBossField = true;
			}
		}
		SetStep(_step);
	}
	public void SetStep(int _step)
	{
		step = _step;
		if ((bool)checkCollider)
		{
			checkCollider.center = new Vector3(checkCollider.center.x, checkCollider.center.y - (float)step * 0f * 0.2f, checkCollider.center.z);
		}
	}
	public void UpdateMethod()
	{
		if (!base.enabled || !activeBorderAnchor)
		{
			return;
		}
		if (isFieldActive)
		{
			if (!isEnemyActive || !IsBossField)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < enemys.Length; i++)
			{
				if ((bool)enemys[i])
				{
					flag = false;
				}
			}
			if (flag)
			{
				SingletonCustom<DragonBattleCameraMover>.Instance.FinishBossBattle();
				SingletonCustom<DragonBattleUIManager>.Instance.HideBossHp();
				base.enabled = false;
				if ((bool)moveBlock)
				{
					moveBlock.SetActive(value: false);
				}
			}
		}
		else
		{
			if (SingletonCustom<DragonBattleFieldManager>.Instance.IsBossBattle || !(SingletonCustom<DragonBattleCameraMover>.Instance.GetAnchor().position.z >= ActiveBorderAnchor.position.z))
			{
				return;
			}
			SingletonCustom<DragonBattleFieldManager>.Instance.SetNowParts(this);
			isFieldActive = true;
			if (IsBossField)
			{
				SingletonCustom<DragonBattleCameraMover>.Instance.StartBossBattle();
				UnityEngine.Debug.Log("ボスエリアに到達した");
				return;
			}
			for (int j = 0; j < enemys.Length; j++)
			{
				enemys[j].Active();
			}
			isEnemyActive = true;
		}
	}
	public void EnemyActive()
	{
		if (!isEnemyActive)
		{
			for (int i = 0; i < enemys.Length; i++)
			{
				enemys[i].Active();
			}
			isEnemyActive = true;
		}
	}
	private void AutoSetting()
	{
		DragonBattleEnemyNinja[] componentsInChildren = base.transform.GetComponentsInChildren<DragonBattleEnemyNinja>();
		enemys = new DragonBattleEnemyNinja[componentsInChildren.Length];
		for (int i = 0; i < enemys.Length; i++)
		{
			enemys[i] = componentsInChildren[i];
		}
	}
}
