using UnityEngine;
using UnityEngine.Extension;
public class Biathlon_CharacterRenderer : MonoBehaviour
{
	[SerializeField]
	[DisplayName("キャラクタ\u30fcの見た目")]
	private CharacterStyle style;
	[SerializeField]
	private MeshFilter leftSnowBoard;
	[SerializeField]
	private MeshFilter rightSnowBoard;
	[SerializeField]
	private Mesh[] leftSnowBoardMeshes;
	[SerializeField]
	private Mesh[] rightSnowBoardMeshes;
	[SerializeField]
	private MeshFilter leftPole;
	[SerializeField]
	private MeshFilter rightPole;
	[SerializeField]
	private Mesh[] poleMeshes;
	[SerializeField]
	private GameObject[] crossCountryParts;
	[SerializeField]
	private GameObject[] standRifleShootingParts;
	[SerializeField]
	private GameObject[] lieRifleShootingParts;
	[SerializeField]
	private MeshRenderer[] guns;
	[SerializeField]
	private Material[] gunMaterials;
	public void Init(Biathlon_Character character)
	{
		int controlUser = (int)character.ControlUser;
		int num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[controlUser];
		style.SetGameStyle(GS_Define.GameType.DELIVERY_ORDER, controlUser);
		leftSnowBoard.sharedMesh = leftSnowBoardMeshes[num];
		rightSnowBoard.sharedMesh = rightSnowBoardMeshes[num];
		MeshRenderer[] array = guns;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sharedMaterial = gunMaterials[num];
		}
		Mesh mesh3 = leftPole.sharedMesh = (rightPole.sharedMesh = poleMeshes[num]);
		SetAsCrossCountryMode();
	}
	public void SetAsCrossCountryMode()
	{
		GameObject[] array = standRifleShootingParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = lieRifleShootingParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = crossCountryParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
	}
	public void SetAsStandRifleShootingMode()
	{
		GameObject[] array = crossCountryParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = lieRifleShootingParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = standRifleShootingParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
	}
	public void SetAsLieRifleShootingMode()
	{
		GameObject[] array = crossCountryParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = standRifleShootingParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = lieRifleShootingParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
	}
}
