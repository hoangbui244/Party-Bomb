using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_Camera : SingletonMonoBehaviour<Shuriken_Camera>
{
	[SerializeField]
	[DisplayName("3Dカメラ")]
	private Camera camera;
	public Vector3 FullHdScreenPointToScreenPoint(Vector2 position)
	{
		Vector3 result = position;
		float num = (float)camera.pixelWidth / 1920f;
		float num2 = (float)camera.pixelHeight / 1080f;
		result.x *= num;
		result.y *= num2;
		return result;
	}
	public Vector3 WorldToFullHdScreenPoint(Vector3 worldPoint)
	{
		Vector3 result = camera.WorldToScreenPoint(worldPoint);
		float num = 1920f / (float)camera.pixelWidth;
		float num2 = 1080f / (float)camera.pixelHeight;
		result.x *= num;
		result.y *= num2;
		result.z = 0f;
		return result;
	}
	public Ray FullHdScreenPointToRay(Vector2 position)
	{
		Vector3 pos = FullHdScreenPointToScreenPoint(position);
		return camera.ScreenPointToRay(pos);
	}
}
