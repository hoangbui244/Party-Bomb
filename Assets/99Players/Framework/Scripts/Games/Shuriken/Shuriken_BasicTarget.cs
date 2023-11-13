using System;
using UnityEngine;
using UnityEngine.Extension;
using UnityEngine.Extension.CoffeeBreakTime;
public class Shuriken_BasicTarget : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("コンフィグ")]
	private Shuriken_TargetConfig config;
	[SerializeField]
	[DisplayName("コライダ\u30fc")]
	private Collider collider;
	[SerializeField]
	[DisplayName("スコア倍率")]
	private float scoreMultiplier = 1f;
	[Header("上書き設定")]
	[SerializeField]
	[DisplayName("表示角度の上書き")]
	[AfterDraw]
	private bool useOverwriteShowAngle;
	[SerializeField]
	[Hide("useOverwriteShowAngle", false)]
	[DisplayName("表示角度(上書き)")]
	[AfterDraw]
	private float overwriteShowAngle;
	[SerializeField]
	[DisplayName("非表示角度の上書き")]
	[AfterDraw]
	private bool useOverwriteHideAngle;
	[SerializeField]
	[Hide("useOverwriteHideAngle", false)]
	[DisplayName("非表示角度(上書き)")]
	[AfterDraw]
	private float overwriteHideAngle;
	[SerializeField]
	[DisplayName("表示時間の上書き")]
	[AfterDraw]
	private bool useOverwriteShowTime;
	[SerializeField]
	[Hide("useOverwriteShowTime", false)]
	[DisplayName("表示時間(上書き)")]
	[AfterDraw]
	private float overwriteShowTime;
	[SerializeField]
	[Header("ラッシュ時出現フラグ")]
	private bool isRushShow;
	[SerializeField]
	[Header("加算耐久値")]
	private int addArmor;
	private Shuriken_TargetGenerator generator;
	private Vector3 showAngles;
	private Vector3 hideAngles;
	private CoffeeBreak autoHide;
	private Vector3 initPos;
	public int AddArmor
	{
		get
		{
			return addArmor;
		}
		set
		{
			addArmor = value;
		}
	}
	public int Score => Mathf.FloorToInt((float)config.Score * scoreMultiplier);
	public bool IsActive
	{
		get;
		private set;
	}
	public Vector3 AimHint => collider.bounds.center.Y(collider.bounds.min.y + collider.bounds.size.y * 0.75f);
	protected Collider Collider => collider;
	public bool IsTargeted
	{
		get;
		private set;
	}
	public void Initialize(Shuriken_TargetGenerator owner)
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		float x = useOverwriteShowAngle ? overwriteShowAngle : config.ShowEulerAnglesX;
		float x2 = useOverwriteHideAngle ? overwriteHideAngle : config.HideEulerAnglesX;
		showAngles = localEulerAngles.X(x);
		hideAngles = localEulerAngles.X(x2);
		generator = owner;
		base.transform.localEulerAngles = hideAngles;
		collider.enabled = false;
		OnInitialize();
		if (isRushShow)
		{
			base.gameObject.SetActive(value: false);
		}
		initPos = base.transform.localPosition;
	}
	public void Shake(float _time = 0.25f)
	{
		initPos = base.transform.localPosition;
		LeanTween.value(base.gameObject, 0f, 1f, _time).setOnUpdate((Action<float>)delegate
		{
			base.transform.localPosition = initPos + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(0f, 0.2f), 0f);
		}).setOnComplete((Action)delegate
		{
			base.transform.localPosition = initPos;
		});
	}
	public void Show()
	{
		if ((!isRushShow || SingletonMonoBehaviour<Shuriken_GameMain>.Instance.IsRushTime) && (!isRushShow || addArmor > 0) && base.gameObject.activeSelf)
		{
			IsTargeted = false;
			float delayTime = Mathf.Lerp(config.MinShowDelayTime, config.MaxShowDelayTime, UnityEngine.Random.value);
			this.CoffeeBreak().DelayCall(delayTime, delegate
			{
				SingletonMonoBehaviour<Shuriken_Audio>.Instance.PlaySfx("se_shuriken_target_stand_0");
				ShowMethod();
			}).Start();
		}
	}
	public void Hide()
	{
		HideMethod();
		generator.NotifyHide(this);
		IsTargeted = false;
	}
	protected virtual void OnInitialize()
	{
	}
	protected virtual void ShowMethod()
	{
		if (!IsActive)
		{
			this.CoffeeBreak().Keep(config.ShowDuration, delegate(float t)
			{
				Shuriken_BasicTarget shuriken_BasicTarget = this;
				base.transform.LocalEulerAngles((Vector3 ea) => Vector3.Lerp(shuriken_BasicTarget.hideAngles, shuriken_BasicTarget.showAngles, t));
			}).DelayCall(0f, delegate
			{
				base.transform.localEulerAngles = showAngles;
				IsActive = true;
				collider.enabled = true;
			})
				.Start();
			if (SingletonMonoBehaviour<Shuriken_GameMain>.Instance.IsDuringGame)
			{
				float num = useOverwriteShowTime ? overwriteShowTime : config.ShowTime;
				num += Mathf.Lerp(0f - config.ShowTimeRange, config.ShowTimeRange, UnityEngine.Random.value);
				autoHide = this.CoffeeBreak().DelayCall(num, Hide).Start();
			}
		}
	}
	protected virtual void HideMethod()
	{
		Shake(0.05f);
		IsActive = false;
		LeanTween.delayedCall(0.075f, (Action)delegate
		{
			CoffeeBreak coffeeBreak = this.CoffeeBreak();
			coffeeBreak.Keep(config.HideDuration, delegate(float t)
			{
				Shuriken_BasicTarget shuriken_BasicTarget = this;
				base.transform.LocalEulerAngles((Vector3 ea) => Vector3.Lerp(shuriken_BasicTarget.showAngles, shuriken_BasicTarget.hideAngles, t));
			});
			coffeeBreak.DelayCall(0f, delegate
			{
				base.transform.localEulerAngles = hideAngles;
				collider.enabled = false;
			});
			coffeeBreak.Start();
		});
		if (autoHide != null)
		{
			autoHide.Stop();
			autoHide = null;
		}
	}
	public void Targeting()
	{
		IsTargeted = true;
	}
	public void ReleaseTargeting()
	{
		IsTargeted = false;
	}
	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
