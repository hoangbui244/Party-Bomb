using UnityEngine;
public class GS_CharacterManager : MonoBehaviour
{
	[SerializeField]
	[Header("キャラ画像")]
	private SpriteRenderer[] arrayCharacterSp;
	[SerializeField]
	[Header("キャラクラス")]
	private GS_Character[] arrayCharacterMover;
	private int[] arrayCharacterUseIdx;
	public void Init()
	{
		for (int i = 0; i < arrayCharacterMover.Length; i++)
		{
			arrayCharacterMover[i].Stop();
		}
		if (arrayCharacterUseIdx == null)
		{
			arrayCharacterUseIdx = new int[arrayCharacterMover.Length];
			for (int j = 0; j < arrayCharacterUseIdx.Length; j++)
			{
				arrayCharacterUseIdx[j] = j;
			}
		}
		Shuffle(arrayCharacterUseIdx);
	}
	public void Shuffle(int[] arr)
	{
		for (int num = arr.Length - 1; num > 0; num--)
		{
			int num2 = UnityEngine.Random.Range(0, num + 1);
			int num3 = arr[num];
			arr[num] = arr[num2];
			arr[num2] = num3;
		}
	}
}
