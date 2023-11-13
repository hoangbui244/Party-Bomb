using UnityEngine;
public class MakingPotion_Water : MonoBehaviour
{
	[SerializeField]
	private MakingPotion_PlayerScript player;
	private MeshRenderer waterMesh;
	private Color mainColor = Color.white;
	private float red = 1f;
	private float green = 1f;
	private float blue = 1f;
	private float alpha = 1f;
	public void Init()
	{
	}
	private void ColorChange()
	{
		switch (player.NowSugarColorType)
		{
		case MakingPotion_PlayerScript.SugarColorType.Red:
			red = Mathf.Clamp01(red + Time.deltaTime);
			green = Mathf.Clamp01(green - Time.deltaTime);
			blue = Mathf.Clamp01(blue - Time.deltaTime);
			break;
		case MakingPotion_PlayerScript.SugarColorType.Green:
			red = Mathf.Clamp01(red - Time.deltaTime);
			green = Mathf.Clamp01(green + Time.deltaTime);
			blue = Mathf.Clamp01(blue - Time.deltaTime);
			break;
		case MakingPotion_PlayerScript.SugarColorType.Blue:
			red = Mathf.Clamp01(red - Time.deltaTime);
			green = Mathf.Clamp01(green - Time.deltaTime);
			blue = Mathf.Clamp01(blue + Time.deltaTime);
			break;
		case MakingPotion_PlayerScript.SugarColorType.Yellow:
			red = Mathf.Clamp01(red + Time.deltaTime);
			green = Mathf.Clamp01(green + Time.deltaTime);
			blue = Mathf.Clamp01(blue - Time.deltaTime);
			break;
		}
		mainColor = new Color(red, green, blue, alpha);
	}
	public void WaterReset()
	{
		red = 1f;
		green = 1f;
		blue = 1f;
	}
}
