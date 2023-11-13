using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Extension;
public class SmeltFishing_IcePlate : MonoBehaviour
{
	[SerializeField]
	[DisplayName("穴を開けることが可能")]
	private bool canMakeHole = true;
	[SerializeField]
	[DisplayName("コンフィグ")]
	private SmeltFishing_IcePlateConfig config;
	[SerializeField]
	[DisplayName("メッシュフィルタ\u30fc")]
	private MeshFilter filter;
	[SerializeField]
	[DisplayName("メッシュレンダラ\u30fc")]
	private MeshRenderer mesh;
	private CapsuleCollider holeCollider;
	private CapsuleCollider areaCollider;
	private NavMeshObstacle obstacle;
	[SerializeField]
	[Header("魚影")]
	private SmeltFishing_SmeltShadow[] arraySmeltShadow;
	private int showSmeltSadowCnt;
	private SmeltFishing_Character useCharacter;
	public bool CanMakeHole => canMakeHole;
	public bool IsUsing
	{
		get;
		private set;
	}
	public float SmeltValue
	{
		get;
		private set;
	}
	public Vector3 Position => base.transform.position;
	public void Init()
	{
		filter.sharedMesh = config.IcePlateMesh;
		arraySmeltShadow = CalcManager.ShuffleList(arraySmeltShadow);
		for (int i = 0; i < arraySmeltShadow.Length; i++)
		{
			if (canMakeHole)
			{
				arraySmeltShadow[i].Init();
				arraySmeltShadow[i].Activate();
			}
			else
			{
				arraySmeltShadow[i].gameObject.SetActive(value: false);
			}
		}
	}
	public void InitShowSmeltSadowCnt(bool _isHoleIcePlate)
	{
		if (_isHoleIcePlate)
		{
			showSmeltSadowCnt = arraySmeltShadow.Length;
		}
		else
		{
			showSmeltSadowCnt = UnityEngine.Random.Range(0, 3);
		}
		for (int i = 0; i < arraySmeltShadow.Length; i++)
		{
			arraySmeltShadow[i].gameObject.SetActive(i < showSmeltSadowCnt);
		}
	}
	public void UpdateMethod()
	{
		float num = config.RecoveryAmount * Time.deltaTime;
		if (IsUsing)
		{
			num *= config.UsingCorrections;
		}
		SmeltValue += num;
		SmeltValue = Mathf.Clamp01(SmeltValue);
	}
	public void UpdateMethodSmeltShadow(bool _isHoleIcePlate)
	{
		if (_isHoleIcePlate)
		{
			if (SmeltValue >= 0.8f)
			{
				showSmeltSadowCnt = arraySmeltShadow.Length;
			}
			else if (SmeltValue >= 0.4f)
			{
				showSmeltSadowCnt = arraySmeltShadow.Length - 1;
			}
			else
			{
				showSmeltSadowCnt = arraySmeltShadow.Length - 2;
			}
			showSmeltSadowCnt = Mathf.Clamp(showSmeltSadowCnt, 0, arraySmeltShadow.Length);
		}
		for (int i = 0; i < arraySmeltShadow.Length; i++)
		{
			arraySmeltShadow[i].gameObject.SetActive(i < showSmeltSadowCnt);
			arraySmeltShadow[i].UpdateMethod();
			if (arraySmeltShadow[i].gameObject.activeSelf && !arraySmeltShadow[i].IsShowing)
			{
				arraySmeltShadow[i].Show();
			}
		}
	}
	public void SetAsHoledIcePlate()
	{
		filter.sharedMesh = config.HoledIcePlateMesh;
		holeCollider = base.gameObject.AddComponent<CapsuleCollider>();
		holeCollider.height = 2f;
		holeCollider.radius = 0.1f;
		areaCollider = base.gameObject.AddComponent<CapsuleCollider>();
		areaCollider.height = 2f;
		areaCollider.radius = 0.45f;
		areaCollider.isTrigger = true;
		SmeltValue = Mathf.Lerp(0.4f, 85f, UnityEngine.Random.value);
	}
	public void Use(SmeltFishing_Character character)
	{
		if (canMakeHole && !IsUsing)
		{
			IsUsing = true;
			useCharacter = character;
		}
	}
	public void UnUse(SmeltFishing_Character character)
	{
		if (canMakeHole && IsUsing && !(character != useCharacter))
		{
			IsUsing = false;
			useCharacter = null;
		}
	}
	public void SubtractValue(int smeltFish)
	{
		SmeltValue -= (float)smeltFish * config.SubtractAmount;
	}
	private void OnTriggerEnter(Collider other)
	{
		GameObject gameObject = other.attachedRigidbody.gameObject;
		UnityEngine.Debug.Log("プレイヤ\u30fcが侵入 name:" + gameObject.name);
	}
}
