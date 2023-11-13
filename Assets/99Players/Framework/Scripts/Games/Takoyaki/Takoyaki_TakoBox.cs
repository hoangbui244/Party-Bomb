using System;
using System.Collections.Generic;
using UnityEngine;
public class Takoyaki_TakoBox : MonoBehaviour
{
	[SerializeField]
	[Header("たこ焼きを置くポイント")]
	private Transform[] takoBallPutPoints;
	private Transform takoBoxHideAnchor;
	private Transform takoBoxShowAnchor;
	private Transform takoBoxToppingHideAnchor;
	private Transform takoBoxToppingShowAnchor;
	private List<Takoyaki_TakoyakiBall> putOnTakoBallList = new List<Takoyaki_TakoyakiBall>();
	private ParticleSystem goodTextFX;
	private ParticleSystem badTextFX;
	private const int MAX_PUT_ON_TAKOBALL = 6;
	private int putOnTakoCount;
	private bool isShowBox;
	private bool isMaxPutOn;
	private const float HIDE_BOX_TIME = 2f;
	private float nowHideBoxTime;
	private Takoyaki_Define.UserType userType;
	private const float MAX_PUT_ON_BOX_DESTROY_TIME = 2f;
	private float remainDestroyTime;
	public bool IsMaxPutOn => isMaxPutOn;
	public void Init(Takoyaki_Define.UserType _userType, Transform _hideAnchor, Transform _showAnchor, Transform _toppingHideAnchor, Transform _toppingShowAnchor, ParticleSystem _goodTextFx, ParticleSystem _badTextFX)
	{
		userType = _userType;
		takoBoxHideAnchor = _hideAnchor;
		takoBoxShowAnchor = _showAnchor;
		takoBoxToppingHideAnchor = _toppingHideAnchor;
		takoBoxToppingShowAnchor = _toppingShowAnchor;
		goodTextFX = _goodTextFx;
		badTextFX = _badTextFX;
		base.transform.position = takoBoxHideAnchor.position;
		base.transform.SetEulerAnglesY(90f);
	}
	private void Update()
	{
		if (isMaxPutOn)
		{
			if (remainDestroyTime > 2f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				remainDestroyTime += Time.deltaTime;
			}
		}
		else if (isShowBox)
		{
			if (nowHideBoxTime < 0f)
			{
				isShowBox = false;
				LeanTween.moveLocal(base.gameObject, takoBoxHideAnchor.localPosition, 0.1f);
			}
			else
			{
				nowHideBoxTime -= Time.deltaTime;
			}
		}
	}
	public void TakoBallPutOnBox(Takoyaki_TakoyakiBall _takoBall)
	{
		nowHideBoxTime = 2f;
		putOnTakoBallList.Add(_takoBall);
		putOnTakoCount++;
		if (!isShowBox)
		{
			isShowBox = true;
			LeanTween.cancel(base.gameObject);
			LeanTween.moveLocal(base.gameObject, takoBoxShowAnchor.localPosition, 0.01f).setOnComplete((Action)delegate
			{
				_takoBall.transform.parent = base.transform;
				_takoBall.OrbitCalc.SetEndPoint(takoBallPutPoints[putOnTakoCount - 1].position);
				_takoBall.OrbitCalc.StartOrbitMove(delegate
				{
					TakoBallEvalution(_takoBall);
				});
			});
		}
		else
		{
			_takoBall.transform.parent = base.transform;
			_takoBall.OrbitCalc.SetEndPoint(takoBallPutPoints[putOnTakoCount - 1].position);
			_takoBall.OrbitCalc.StartOrbitMove(delegate
			{
				TakoBallEvalution(_takoBall);
			});
		}
		if (putOnTakoCount == 6)
		{
			isMaxPutOn = true;
		}
	}
	public void ToppingTakoBox(Takoyaki_SauceBrush _sauceBrush)
	{
		for (int i = 0; i < putOnTakoBallList.Count; i++)
		{
			putOnTakoBallList[i].ShowTopping();
		}
		LeanTween.moveLocal(base.gameObject, takoBoxToppingShowAnchor.localPosition, 0.1f).setOnComplete((Action)delegate
		{
			_sauceBrush.PlaySauceBrushAnimation(base.transform);
		});
		LeanTween.moveLocal(base.gameObject, takoBoxToppingHideAnchor.localPosition, 0.1f).setDelay(1f);
	}
	private void TakoBallEvalution(Takoyaki_TakoyakiBall _takoBall)
	{
		CalcManager.mCalcVector3.x = Takoyaki_Define.PM.GetUserCamera(userType).WorldToScreenPoint(_takoBall.transform.position).x;
		CalcManager.mCalcVector3.y = Takoyaki_Define.PM.GetUserCamera(userType).WorldToScreenPoint(_takoBall.transform.position).y;
		CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
		if (_takoBall.GetTakoBallBakeStatus() == Takoyaki_Define.TakoBallBakeStatus.Bake)
		{
			ParticleSystem particleSystem = UnityEngine.Object.Instantiate(goodTextFX, Vector3.zero, Quaternion.identity, _takoBall.transform);
			particleSystem.transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y, 0f);
			particleSystem.transform.SetLocalPositionZ(0f);
			particleSystem.Play();
			Takoyaki_Define.PM.SetTakoyakiCnt(userType, 1);
			if (userType <= Takoyaki_Define.UserType.PLAYER_4)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_good");
			}
		}
		else
		{
			ParticleSystem particleSystem2 = UnityEngine.Object.Instantiate(badTextFX, Vector3.zero, Quaternion.identity, _takoBall.transform);
			particleSystem2.transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y, 0f);
			particleSystem2.transform.SetLocalPositionZ(0f);
			particleSystem2.Play();
			if (userType <= Takoyaki_Define.UserType.PLAYER_4)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_gauge_bad");
			}
		}
	}
}
