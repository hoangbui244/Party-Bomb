using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_Backgrounds : DecoratedMonoBehaviour
{
	private const float GlobalScaleFactor = 0.1f;
	[SerializeField]
	[DisplayName("スピ\u30fcドに対する補正")]
	private float scaleFactor = 1f;
	[SerializeField]
	[DisplayName("テクスチャレイヤ\u30fc")]
	private FlyingSquirrelRace_Backgrounds_TextureLayer[] textureLayers = new FlyingSquirrelRace_Backgrounds_TextureLayer[0];
	[SerializeField]
	[DisplayName("オブジェクトレイヤ\u30fc")]
	private FlyingSquirrelRace_Backgrounds_ObjectLayer[] objectLayers = new FlyingSquirrelRace_Backgrounds_ObjectLayer[0];
	public void Initialize()
	{
		FlyingSquirrelRace_Backgrounds_TextureLayer[] array = textureLayers;
		foreach (FlyingSquirrelRace_Backgrounds_TextureLayer flyingSquirrelRace_Backgrounds_TextureLayer in array)
		{
			if (!(flyingSquirrelRace_Backgrounds_TextureLayer == null))
			{
				flyingSquirrelRace_Backgrounds_TextureLayer.Initialize();
			}
		}
		FlyingSquirrelRace_Backgrounds_ObjectLayer[] array2 = objectLayers;
		foreach (FlyingSquirrelRace_Backgrounds_ObjectLayer flyingSquirrelRace_Backgrounds_ObjectLayer in array2)
		{
			if (!(flyingSquirrelRace_Backgrounds_ObjectLayer == null))
			{
				flyingSquirrelRace_Backgrounds_ObjectLayer.Initialize();
			}
		}
	}
	public void UpdateMethod(float speed)
	{
		FlyingSquirrelRace_Backgrounds_TextureLayer[] array = textureLayers;
		foreach (FlyingSquirrelRace_Backgrounds_TextureLayer flyingSquirrelRace_Backgrounds_TextureLayer in array)
		{
			if (!(flyingSquirrelRace_Backgrounds_TextureLayer == null))
			{
				flyingSquirrelRace_Backgrounds_TextureLayer.UpdateMethod(speed * scaleFactor * Time.deltaTime);
			}
		}
		FlyingSquirrelRace_Backgrounds_ObjectLayer[] array2 = objectLayers;
		foreach (FlyingSquirrelRace_Backgrounds_ObjectLayer flyingSquirrelRace_Backgrounds_ObjectLayer in array2)
		{
			if (!(flyingSquirrelRace_Backgrounds_ObjectLayer == null))
			{
				flyingSquirrelRace_Backgrounds_ObjectLayer.UpdateMethod(speed * scaleFactor * Time.deltaTime);
			}
		}
	}
	public void UpdateGoalAnimation(float scroll)
	{
		base.transform.LocalPosition((Vector3 v) => v.Y((float y) => y + scroll));
	}
}
