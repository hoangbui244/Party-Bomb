using UnityEngine;
public class SmeltFishing_SitDownUI : MonoBehaviour
{
	[SerializeField]
	private Vector3 offset = new Vector3(0f, 0.3f, 0f);
	private Vector3 clipPosition;
	private Camera mainCamera;
	private Camera camera;
	private float mainPixelWidth;
	private float mainPixelHeight;
	private float cameraPixelWidth;
	private float cameraPixelHeight;
	[SerializeField]
	private GameObject sitDown_JP;
	[SerializeField]
	private GameObject sitDown_EN;
	public void Init()
	{
		if (Localize_Define.Language == Localize_Define.LanguageType.Japanese)
		{
			sitDown_JP.gameObject.SetActive(value: true);
			sitDown_EN.gameObject.SetActive(value: false);
		}
		else
		{
			sitDown_JP.gameObject.SetActive(value: false);
			sitDown_EN.gameObject.SetActive(value: true);
		}
		Hide();
		mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
		mainPixelWidth = mainCamera.pixelWidth;
		mainPixelHeight = mainCamera.pixelHeight;
	}
	public void SetClipPosition(Vector3 position)
	{
		clipPosition = position;
	}
	public void Show(int no)
	{
		if (camera == null)
		{
			camera = SingletonCustom<SmeltFishing_CameraRegistry>.Instance.GetCamera(no).GetComponent<Camera>();
		}
		UpdatePosition();
		base.gameObject.SetActive(value: true);
	}
	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}
	private void Update()
	{
		UpdatePosition();
	}
	private void UpdatePosition()
	{
		if (!(camera == null))
		{
			Vector3 position = clipPosition + offset;
			Vector3 vector = camera.WorldToViewportPoint(position);
			Rect rect = camera.rect;
			float x = 1920f * rect.width * vector.x;
			float y = 1080f * rect.height * vector.y;
			base.transform.localPosition = new Vector3(x, y, 0f);
		}
	}
}
