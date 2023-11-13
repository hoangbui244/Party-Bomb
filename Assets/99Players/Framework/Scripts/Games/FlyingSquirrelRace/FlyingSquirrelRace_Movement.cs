using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_Movement : DecoratedMonoBehaviour
{
	private const float MinPosY = -0.75f;
	private const float MaxPosY = 0.75f;
	private const float MinAngle = -20f;
	private const float MaxAngle = 20f;
	[SerializeField]
	[DisplayName("速度補正")]
	private float maxDelta = 1f;
	[SerializeField]
	[DisplayName("上昇速度")]
	private float riseSpeed = 2f;
	[SerializeField]
	[DisplayName("下降速度")]
	private float fallSpeed = -1f;
	[SerializeField]
	[DisplayName("下降加速度")]
	private float fallAcceleration = 2f;
	[SerializeField]
	[DisplayName("上昇時のアングル速度")]
	private float riseAngleSpeed = 160f;
	[SerializeField]
	[DisplayName("下降時のアングル速度")]
	private float fallAngleSpeed = 80f;
	private FlyingSquirrelRace_Player owner;
	private Transform cachedTransform;
	private bool isPressButtonA;
	private float targetY;
	private float fallTime;
	private float angle;
	public void Initialize(FlyingSquirrelRace_Player player)
	{
		owner = player;
		cachedTransform = base.transform;
		targetY = cachedTransform.localPosition.y;
	}
	public void UpdateMethod()
	{
		isPressButtonA = SingletonCustom<FlyingSquirrelRace_Input>.Instance.IsHoldButtonA(owner.Controller);
		UpdatePositionY();
		UpdateRotate();
	}
	public void UpdateForAIMethod(FlyingSquirrelRace_AI ai)
	{
		isPressButtonA = ai.IsPressButtonA;
		UpdatePositionY();
		UpdateRotate();
	}
	public bool MoveTargetHeight(float height)
	{
		Vector3 localPosition = cachedTransform.localPosition;
		if (Mathf.Approximately(localPosition.y, height))
		{
			return true;
		}
		if (localPosition.y < height)
		{
			angle += Time.deltaTime * riseAngleSpeed;
		}
		else if (localPosition.y > height)
		{
			angle -= Time.deltaTime * fallAngleSpeed;
		}
		angle = Mathf.Clamp(angle, -20f, 20f);
		cachedTransform.LocalEulerAngles((Vector3 angles) => angles.Y(angle));
		localPosition.y = Mathf.MoveTowards(localPosition.y, height, Time.fixedDeltaTime * maxDelta);
		cachedTransform.localPosition = localPosition;
		return Mathf.Approximately(localPosition.y, height);
	}
	public bool RotateToBaseRotation()
	{
		angle = Mathf.MoveTowards(angle, 0f, Time.fixedDeltaTime * fallAngleSpeed);
		cachedTransform.LocalEulerAngles((Vector3 angles) => angles.Y(angle));
		return Mathf.Approximately(angle, 0f);
	}
	private void UpdatePositionY()
	{
		if (isPressButtonA)
		{
			targetY += riseSpeed * Time.fixedDeltaTime;
			fallTime = 0f;
		}
		else
		{
			fallTime += Time.fixedDeltaTime;
			float num = 1f + Mathf.Pow(fallTime, fallAcceleration);
			targetY += fallSpeed * Time.fixedDeltaTime * num;
		}
		Vector3 localPosition = cachedTransform.localPosition;
		targetY = Mathf.Clamp(targetY, -0.75f, 0.75f);
		localPosition.y = targetY;
		cachedTransform.localPosition = localPosition;
	}
	private void UpdateRotate()
	{
		bool flag = Mathf.Approximately(cachedTransform.localPosition.y, -0.75f);
		if (isPressButtonA)
		{
			angle += Time.deltaTime * riseAngleSpeed;
		}
		else if (!flag)
		{
			angle -= Time.deltaTime * fallAngleSpeed;
		}
		else
		{
			angle = Mathf.MoveTowards(angle, 0f, Time.fixedDeltaTime * fallAngleSpeed);
		}
		angle = Mathf.Clamp(angle, -20f, 20f);
		Vector3 localEulerAngles = cachedTransform.localEulerAngles;
		localEulerAngles.x = angle;
		cachedTransform.localEulerAngles = localEulerAngles;
	}
}
