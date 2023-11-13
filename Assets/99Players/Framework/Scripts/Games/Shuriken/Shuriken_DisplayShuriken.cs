using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_DisplayShuriken : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("コンフィグ")]
	private Shuriken_DisplayShurikenConfig config;
	[SerializeField]
	[DisplayName("手裏剣レンダラ")]
	private MeshRenderer renderer;
	private Vector3 basePosition;
	public Vector3 Position => base.transform.position;
	public void Initialize(int playerNo)
	{
		basePosition = config.DisplayShurikenPosition[playerNo];
		int num = SingletonMonoBehaviour<Shuriken_GameMain>.Instance.CharacterIndexes[playerNo];
		renderer.sharedMaterial = config.ShurikenMaterials[num];
		base.transform.localPosition = basePosition;
	}
	public void Show(float duration)
	{
		Vector3 popPosition = basePosition.Y((float y) => y + config.PopupBasePosition);
		base.transform.localPosition = popPosition;
		base.gameObject.SetActive(value: true);
		this.CoffeeBreak().Keep(duration, delegate(float x)
		{
			base.transform.localPosition = Vector3.Lerp(popPosition, basePosition, x);
		}).Start();
	}
	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}
}
