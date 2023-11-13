using UnityEngine;
public class SmeltFishing_BaitDepthUI : MonoBehaviour
{
	[SerializeField]
	private SmeltFishing_FishingConfig config;
	[SerializeField]
	private Transform depthArrow;
	[SerializeField]
	private SpriteRenderer properDepthRange;
	[SerializeField]
	private Color[] properDepthColor;
	[SerializeField]
	private SpriteRenderer fishMark;
	[SerializeField]
	private LineRenderer fisihngLine;
	[SerializeField]
	private Transform fisihngLineAnchor;
	[SerializeField]
	private ParticleSystem hitEffect;
	[SerializeField]
	private GameObject fishIcon;
	[SerializeField]
	private float minDepthPositionY = 247f;
	[SerializeField]
	private float maxDepthPositionY = -247f;
	private float PROPER_MOVE_LIMIT_MIN_POS_Y;
	private float PROPER_MOVE_LIMIT_MAX_POS_Y;
	private float PROPER_MAX_SIZE_Y = 180f;
	private float PROPER_MIN_SIZE_Y = 40f;
	private readonly float CHANGE_DEPTH_PROPER_MOVE_DIR_TIME = 1.5f;
	private float changeDepthProperMoveDirTime;
	private int depthProperMoveDir;
	private int depthArrowMoveDir;
	private float MOVE_PROPER_SPEED = 50f;
	private float MOVE_ARROW_SPEED = 100f;
	private bool _beforeProperMoveNoting;
	[SerializeField]
	[Header("魚のアイコンを揺らす幅")]
	private float fishIconShakeRange;
	private float targetDepth;
	public void Init()
	{
		Hide();
	}
	public void Show(float _smeltValue)
	{
		properDepthRange.enabled = true;
		fishMark.enabled = true;
		depthArrow.transform.SetLocalPositionY(minDepthPositionY * 0.8f);
		PROPER_MOVE_LIMIT_MIN_POS_Y = 0f - properDepthRange.size.y / 2f;
		PROPER_MOVE_LIMIT_MAX_POS_Y = maxDepthPositionY + properDepthRange.size.y / 2f;
		properDepthRange.transform.SetLocalPositionY(UnityEngine.Random.Range(PROPER_MOVE_LIMIT_MIN_POS_Y, PROPER_MOVE_LIMIT_MAX_POS_Y));
		properDepthRange.color = properDepthColor[(!(_smeltValue >= 0.5f)) ? 1 : 0];
		changeDepthProperMoveDirTime = UnityEngine.Random.Range(CHANGE_DEPTH_PROPER_MOVE_DIR_TIME - 0.5f, CHANGE_DEPTH_PROPER_MOVE_DIR_TIME + 0.5f);
		depthProperMoveDir = ((UnityEngine.Random.Range(0, 2) == 0) ? 1 : (-1));
		depthArrowMoveDir = -1;
		Vector3 vector = fisihngLine.transform.InverseTransformPoint(fisihngLineAnchor.position);
		vector.x = 0f;
		if (vector.y > 0f)
		{
			vector.y = 0f;
		}
		vector.z = 0f;
		UnityEngine.Debug.Log("line y " + vector.y.ToString());
		fisihngLine.SetPosition(1, vector);
		float num = fishIconShakeRange / 2f;
		fishIcon.transform.SetLocalEulerAnglesZ(0f - num);
		LeanTween.rotateLocal(fishIcon, new Vector3(0f, 0f, num), 0.5f).setLoopPingPong();
		base.gameObject.SetActive(value: true);
	}
	public void Hide()
	{
		LeanTween.cancel(fishIcon);
		base.gameObject.SetActive(value: false);
	}
	public void SetProperDepth(float properDepth)
	{
	}
	public void SetDepth(float depth)
	{
	}
	public void SetDepthImmediate(float depth)
	{
	}
	public void SetProperDepthRangeActive(bool _isActive)
	{
		properDepthRange.enabled = _isActive;
		fishMark.enabled = _isActive;
	}
	public void SetDepthArrowMoveDir(bool _isRollUp)
	{
		depthArrowMoveDir = (_isRollUp ? 1 : (-1));
	}
	public bool IsRollUpComplete()
	{
		return Mathf.Approximately(depthArrow.transform.localPosition.y, minDepthPositionY + 0.1f);
	}
	public bool IsArrowGaugeCenterDown()
	{
		return depthArrow.transform.localPosition.y <= minDepthPositionY * 0.25f;
	}
	public bool IsArrowMaxDepthPosition()
	{
		return Mathf.Approximately(depthArrow.transform.localPosition.y, maxDepthPositionY);
	}
	public bool IsDownDepthArrowToProperDepth()
	{
		return depthArrow.transform.localPosition.y <= properDepthRange.transform.localPosition.y;
	}
	public bool CheckHitRange(float _diff)
	{
		if (depthArrow.transform.localPosition.y <= properDepthRange.transform.localPosition.y + _diff)
		{
			return depthArrow.transform.localPosition.y >= properDepthRange.transform.localPosition.y - _diff;
		}
		return false;
	}
	public bool CheckHitRange()
	{
		if (depthArrow.transform.localPosition.y <= properDepthRange.transform.localPosition.y + properDepthRange.size.y / 2f)
		{
			return depthArrow.transform.localPosition.y >= properDepthRange.transform.localPosition.y - properDepthRange.size.y / 2f;
		}
		return false;
	}
	public void PlayHitEffect()
	{
		hitEffect.Play();
	}
	public void SetProperSize(float _smeltValue, float _time)
	{
		float num = PROPER_MAX_SIZE_Y - PROPER_MIN_SIZE_Y;
		Vector2 size = properDepthRange.size;
		size.y = PROPER_MIN_SIZE_Y + num * _smeltValue;
		size.y = Mathf.Clamp(size.y, PROPER_MIN_SIZE_Y, PROPER_MAX_SIZE_Y);
		LeanTween.value(properDepthRange.gameObject, properDepthRange.size, size, _time).setOnUpdate(delegate(Vector2 value)
		{
			properDepthRange.size = value;
			PROPER_MOVE_LIMIT_MIN_POS_Y = 0f - properDepthRange.size.y / 2f;
			PROPER_MOVE_LIMIT_MAX_POS_Y = maxDepthPositionY + properDepthRange.size.y / 2f;
		});
	}
	private void Update()
	{
		UpdateProperPosition();
		UpdateArrowPosition();
	}
	private void UpdateProperPosition()
	{
		if (changeDepthProperMoveDirTime > 0f && !Mathf.Approximately(properDepthRange.transform.localPosition.y, PROPER_MOVE_LIMIT_MIN_POS_Y) && !Mathf.Approximately(properDepthRange.transform.localPosition.y, PROPER_MOVE_LIMIT_MAX_POS_Y))
		{
			changeDepthProperMoveDirTime -= Time.deltaTime;
		}
		else
		{
			changeDepthProperMoveDirTime = UnityEngine.Random.Range(CHANGE_DEPTH_PROPER_MOVE_DIR_TIME - 0.5f, CHANGE_DEPTH_PROPER_MOVE_DIR_TIME + 0.5f);
			if (!_beforeProperMoveNoting)
			{
				if (UnityEngine.Random.Range(0, 2) == 0)
				{
					_beforeProperMoveNoting = true;
					depthProperMoveDir = 0;
					changeDepthProperMoveDirTime = 0.5f;
				}
				else
				{
					depthProperMoveDir *= -1;
				}
			}
			else
			{
				_beforeProperMoveNoting = false;
				if (Mathf.Approximately(properDepthRange.transform.localPosition.y, PROPER_MOVE_LIMIT_MIN_POS_Y))
				{
					depthProperMoveDir = -1;
				}
				else if (Mathf.Approximately(properDepthRange.transform.localPosition.y, PROPER_MOVE_LIMIT_MAX_POS_Y))
				{
					depthProperMoveDir = 1;
				}
				else
				{
					depthProperMoveDir = ((UnityEngine.Random.Range(0, 2) == 0) ? 1 : (-1));
				}
			}
		}
		float y = properDepthRange.transform.localPosition.y;
		y += (float)depthProperMoveDir * Time.deltaTime * MOVE_PROPER_SPEED;
		y = Mathf.Clamp(y, PROPER_MOVE_LIMIT_MAX_POS_Y, PROPER_MOVE_LIMIT_MIN_POS_Y);
		properDepthRange.transform.SetLocalPositionY(y);
	}
	private void UpdateArrowPosition()
	{
		float y = depthArrow.transform.localPosition.y;
		y += (float)depthArrowMoveDir * Time.deltaTime * MOVE_ARROW_SPEED;
		y = Mathf.Clamp(y, maxDepthPositionY, minDepthPositionY + 0.1f);
		depthArrow.SetLocalPositionY(y);
		Vector3 vector = fisihngLine.transform.InverseTransformPoint(fisihngLineAnchor.position);
		vector.x = 0f;
		if (vector.y > 0f)
		{
			vector.y = 0f;
		}
		vector.z = 0f;
		fisihngLine.SetPosition(1, vector);
	}
}
