using UnityEngine;
public class MikoshiRaceMapUI : MonoBehaviour
{
	private const float AnchorOffset = 300f;
	[SerializeField]
	[Header("プレイヤ\u30fc番号")]
	private int playerNo;
	[SerializeField]
	[Header("マップアンカ\u30fc")]
	private GameObject mapAnchor;
	[SerializeField]
	[Header("マップ上のラジコンアンカ\u30fc")]
	private Transform[] mapCarAnchor;
	[SerializeField]
	[Header("ラジコンアンカ\u30fcのル\u30fcト")]
	private Transform rootCarAnchor;
	[SerializeField]
	[Header("マップのコ\u30fcス画像")]
	private SpriteRenderer mapCourseSprite;
	[SerializeField]
	[Header("マップ上の右上と左下の両端アンカ\u30fc")]
	private Transform mapRightTop;
	[SerializeField]
	private Transform mapLeftBottom;
	[SerializeField]
	[Header("コ\u30fcス上の右上と左下の両端アンカ\u30fc")]
	private Transform courseRightTop;
	[SerializeField]
	private Transform courseLeftBottom;
	[SerializeField]
	[Header("複製するマップ")]
	private MikoshiRaceMapUI[] copyMaps;
	private int carNum;
	public void MapInit()
	{
		carNum = SingletonCustom<MikoshiRaceGameManager>.Instance.CarNum;
		carNum = 4;
		for (int i = 0; i < mapCarAnchor.Length; i++)
		{
			mapCarAnchor[i].gameObject.SetActive(i < carNum);
		}
		for (int j = 0; j < copyMaps.Length; j++)
		{
			copyMaps[j].MapInit();
		}
	}
	public void MapUpdate()
	{
		if (!mapAnchor.activeSelf)
		{
			return;
		}
		for (int i = 0; i < carNum; i++)
		{
			MikoshiRaceCarScript car = SingletonCustom<MikoshiRaceCarManager>.Instance.GetCar(i);
			MapView(mapCarAnchor[i], Stage2Map01Vec2(car.GetPos()));
			for (int j = 0; j < copyMaps.Length; j++)
			{
				Vector3 localPosition = mapCarAnchor[i].localPosition;
				localPosition.z = copyMaps[j].mapCarAnchor[i].localPosition.z;
				copyMaps[j].mapCarAnchor[i].localPosition = localPosition;
			}
		}
	}
	private Vector2 Stage2Map01Vec2(Vector3 _stagePos)
	{
		Vector2 zero = Vector2.zero;
		zero.x = Mathf.InverseLerp(courseLeftBottom.position.x, courseRightTop.position.x, _stagePos.x);
		zero.y = Mathf.InverseLerp(courseLeftBottom.position.z, courseRightTop.position.z, _stagePos.z);
		return zero;
	}
	private void MapView(Transform _trans, Vector2 _xy01)
	{
		Vector3 localPosition = mapLeftBottom.localPosition;
		Vector3 vector = mapRightTop.localPosition - mapLeftBottom.localPosition;
		localPosition.x += vector.x * _xy01.x;
		localPosition.y += vector.y * _xy01.y;
		localPosition.z = _trans.localPosition.z;
		_trans.localPosition = localPosition;
	}
	public void SetMapActive(bool _active)
	{
		mapAnchor.SetActive(_active);
		for (int i = 0; i < copyMaps.Length; i++)
		{
			copyMaps[i].mapAnchor.SetActive(_active);
		}
	}
	public void SetCourseAnchor(Transform _rightTop, Transform _leftBottom)
	{
		courseRightTop = _rightTop;
		courseLeftBottom = _leftBottom;
		for (int i = 0; i < copyMaps.Length; i++)
		{
			copyMaps[i].SetCourseAnchor(_rightTop, _leftBottom);
		}
	}
	public void SetMapSprite(string _textureName)
	{
		for (int i = 0; i < copyMaps.Length; i++)
		{
			copyMaps[i].SetMapSprite(_textureName);
		}
		mapRightTop.SetLocalPosition(mapCourseSprite.transform.localPosition.x + 300f, mapCourseSprite.transform.localPosition.y + 300f, mapRightTop.localPosition.z);
		mapLeftBottom.SetLocalPosition(mapCourseSprite.transform.localPosition.x - 300f, mapCourseSprite.transform.localPosition.y - 300f, mapLeftBottom.localPosition.z);
	}
	public void SetMapAnchorPos(Vector2 _pos)
	{
		mapAnchor.transform.SetPositionX(_pos.x);
		mapAnchor.transform.SetPositionY(_pos.y);
	}
	public void SetMapAnchorScale(float _scale)
	{
		mapAnchor.transform.SetLocalScaleX(_scale);
		mapAnchor.transform.SetLocalScaleY(_scale);
	}
}
