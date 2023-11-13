using System.Collections;
using UnityEngine;
public class FireworksLaunchTubeRack : MonoBehaviour
{
	[SerializeField]
	[Header("対応している玉タイプ")]
	private FireworksBall.ItemType colorType;
	[SerializeField]
	[Header("発射時エフェクト")]
	private ParticleSystem[] arrayLaunchEffect;
	[SerializeField]
	[Header("筒配列")]
	private GameObject[] arrayLaunchTube;
	[SerializeField]
	[Header("花火管理クラス")]
	private FireWorksManager fireworksManager;
	public FireworksBall.ItemType ColorType => colorType;
	public GameObject[] ArrayLaunchTube => arrayLaunchTube;
	public void Set(int _idx)
	{
		StartCoroutine(_Set(_idx));
	}
	private IEnumerator _Set(int _idx)
	{
		LeanTween.cancel(arrayLaunchTube[_idx].gameObject);
		arrayLaunchTube[_idx].transform.SetLocalScale(1f, 1f, 1f);
		yield return new WaitForSeconds(0.35f);
		SingletonCustom<FireworksUIManager>.Instance.ShowPoint(SingletonCustom<FireworksPlayerManager>.Instance.GetPlayerAtIdx(_idx), base.transform.position, FireworksDefine.SCORE_SET, _isTopViewCamera: false);
		arrayLaunchTube[_idx].transform.SetLocalScale(1.2f, 1.2f, 1.2f);
		LeanTween.scale(arrayLaunchTube[_idx].gameObject, Vector3.one, 0.5f).setOnStart(delegate
		{
		}).setEaseOutBack();
		arrayLaunchEffect[_idx].Play();
		yield return new WaitForSeconds(0.35f);
	}
	private void OnDestroy()
	{
		for (int i = 0; i < arrayLaunchTube.Length; i++)
		{
			LeanTween.cancel(arrayLaunchTube[i].gameObject);
		}
	}
}
