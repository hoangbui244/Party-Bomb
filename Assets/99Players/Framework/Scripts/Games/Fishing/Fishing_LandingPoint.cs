using UnityEngine;
using UnityEngine.U2D;
public class Fishing_LandingPoint : MonoBehaviour
{
	[SerializeField]
	[Header("ウキ")]
	private Fishing_RodUki fishingUki;
	[SerializeField]
	[Header("着地点のカ\u30fcソル")]
	private SpriteRenderer targetCursor;
	[SerializeField]
	[Header("タ\u30fcゲットカ\u30fcソルの距離")]
	private float targetCursorDistance = 1f;
	private RaycastHit hit;
	private string raycastHitObjectName = "";
	public void Init(FishingDefinition.User user)
	{
		CheckLandingPoint();
		SpriteAtlas commonAtlas = SingletonCustom<Fishing_SpriteAtlasCache>.Instance.GetCommonAtlas();
		switch (user)
		{
		case FishingDefinition.User.Player1:
			targetCursor.sprite = commonAtlas.GetSprite("tex_target_cursor_0");
			break;
		case FishingDefinition.User.Player2:
			targetCursor.sprite = commonAtlas.GetSprite("tex_target_cursor_1");
			break;
		case FishingDefinition.User.Player3:
			targetCursor.sprite = commonAtlas.GetSprite("tex_target_cursor_2");
			break;
		case FishingDefinition.User.Player4:
			targetCursor.sprite = commonAtlas.GetSprite("tex_target_cursor_3");
			break;
		case FishingDefinition.User.Cpu1:
		case FishingDefinition.User.Cpu2:
		case FishingDefinition.User.Cpu3:
		case FishingDefinition.User.Cpu4:
		case FishingDefinition.User.Cpu5:
			targetCursor.SetAlpha(0f);
			break;
		}
	}
	public void UpdateMethod()
	{
		base.transform.SetLocalPositionZ(targetCursorDistance);
		CheckLandingPoint();
	}
	private void CheckLandingPoint()
	{
		UnityEngine.Debug.DrawRay(base.transform.position, Vector3.down, Color.red);
		if (Physics.Raycast(base.transform.position, Vector3.down, out hit, float.PositiveInfinity, LayerMask.GetMask(FishingDefinition.LayerField)))
		{
			if (hit.collider.gameObject.tag == FishingDefinition.TagObject)
			{
				fishingUki.SetTargetCursorActive(_isActive: true);
				fishingUki.GetTargetCursorAnchor().SetPositionY(hit.point.y);
			}
			else if (hit.collider.gameObject.tag == FishingDefinition.TagNoHit)
			{
				fishingUki.SetTargetCursorActive(_isActive: false);
			}
			else
			{
				fishingUki.SetTargetCursorActive(_isActive: false);
			}
			raycastHitObjectName = hit.collider.gameObject.name;
		}
		else
		{
			raycastHitObjectName = "";
		}
	}
	public bool IsActiveLandingPoint()
	{
		return fishingUki.IsTargetCursorActive();
	}
}
