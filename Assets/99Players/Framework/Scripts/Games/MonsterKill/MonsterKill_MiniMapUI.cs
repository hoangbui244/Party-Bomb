using UnityEngine;
using UnityEngine.Experimental.Rendering;
public class MonsterKill_MiniMapUI : MonoBehaviour
{
	[SerializeField]
	[Header("レンダ\u30fcテクスチャ\u30fc用カメラ")]
	private Camera rtCamera;
	[SerializeField]
	[Header("レンダ\u30fcテクスチャ\u30fc用のマテリアル")]
	private Material rtMaterial;
	private RenderTexture rt;
	[SerializeField]
	[Header("ミニマップ用のアイコンの色（プレイヤ\u30fc）")]
	private Color[] arrayMiniMapIconColor_Player;
	[SerializeField]
	[Header("ミニマップ用のアイコンの色（モンスタ\u30fc）")]
	private Color arrayMiniMapIconColor_Enemy;
	public void Init()
	{
		rt = new RenderTexture(512, 512, 24, GraphicsFormat.R8G8B8A8_UNorm);
		rt.Create();
		rtCamera.targetTexture = rt;
		rtMaterial.mainTexture = rt;
	}
	public Color GetMiniMapIconColor(int _userType = -1)
	{
		if (_userType == -1)
		{
			return arrayMiniMapIconColor_Enemy;
		}
		return arrayMiniMapIconColor_Player[_userType];
	}
	private void OnDisable()
	{
		rtCamera.targetTexture = null;
		rtMaterial.mainTexture = null;
		rt.Release();
	}
}
