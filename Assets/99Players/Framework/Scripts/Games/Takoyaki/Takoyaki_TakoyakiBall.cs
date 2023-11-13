using System;
using UnityEngine;
public class Takoyaki_TakoyakiBall : MonoBehaviour
{
	[Serializable]
	public struct IngredientsPattern
	{
		[Header("パタ\u30fcン親オブジェクト")]
		public GameObject patternRootObj;
		[Header("具材のMeshRenderer")]
		public MeshRenderer[] ingredientsMeshRer;
	}
	[SerializeField]
	[Header("たこ焼き(球形)")]
	private MeshRenderer takoyakiBall;
	[SerializeField]
	[Header("たこ焼き(焦げ状態)")]
	private MeshRenderer takoyakiBall_OverBake;
	[SerializeField]
	[Header("たこ焼き(半球形)")]
	private MeshRenderer takoyakiBall_Half;
	[SerializeField]
	[Header("具材のパタ\u30fcンデ\u30fcタ")]
	private IngredientsPattern[] ingredientsPatterns;
	[SerializeField]
	[Header("放物線移動処理")]
	private OrbitCalculation orbitCalc;
	[SerializeField]
	[Header("たこ焼きのトッピング")]
	private MeshRenderer takoyaki_Topping;
	private Takoyaki_Define.UserType userType;
	private float takoBallBakeTime;
	private float[] nowTakoBallBakeTime = new float[2];
	private Takoyaki_Define.TakoBallBakeStatus[] takoBallBakeStatus = new Takoyaki_Define.TakoBallBakeStatus[2];
	private bool isBakingTakoBallHalf = true;
	private bool isBakingDown = true;
	private const float INGREDIENTS_FADE_IN_TIME = 1f;
	private const float TOPPING_FADE_IN_TIME = 0.75f;
	public readonly float TAKOBALL_SPIN_TIME = 0.5f;
	private int ingredientsPatternNo;
	private readonly Vector2 TAKO_BALL_HALF_BAKE_TEX_OFFSET = new Vector2(0f, -0.5f);
	private readonly Vector2 TAKO_BALL_BAKE_TEX_OFFSET = new Vector2(0f, 0f);
	private readonly Vector2 TAKO_BALL_OVER_BAKE_TEX_OFFSET = new Vector2(0.5f, 0f);
	private bool isIngredientsPutIn;
	private bool isIngredientsFadeIn;
	private bool duringSpinAnimation;
	private bool isTakoBallHalfForm;
	private bool isOverBakeAlert;
	private const float OVER_BAKE_TAKO_BALL_FADE_TIME = 2f;
	public OrbitCalculation OrbitCalc => orbitCalc;
	public Takoyaki_Define.TakoBallBakeStatus[] TakoBallBakeStatus => takoBallBakeStatus;
	public bool IsBakingTakoBallHalf => isBakingTakoBallHalf;
	public bool IsIngredientsPutIn => isIngredientsPutIn;
	public bool IsIngredientsFadeIn => isIngredientsFadeIn;
	public bool DuringSpinAnimation => duringSpinAnimation;
	public bool IsTakoBallHalfForm => isTakoBallHalfForm;
	public bool IsOverBakeAlert => isOverBakeAlert;
	public void Init(Takoyaki_Define.UserType _userType, float _bakeTime)
	{
		userType = _userType;
		takoBallBakeTime = _bakeTime;
		base.transform.localPosition = Vector3.zero;
		base.transform.localEulerAngles = Vector3.zero;
		base.transform.localScale = Vector3.one;
		takoyakiBall.gameObject.SetActive(value: false);
		takoyakiBall_OverBake.gameObject.SetActive(value: false);
		takoyakiBall_Half.gameObject.SetActive(value: true);
		Color color;
		Color color2;
		for (int i = 0; i < ingredientsPatterns.Length; i++)
		{
			ingredientsPatterns[i].patternRootObj.SetActive(value: false);
			for (int j = 0; j < ingredientsPatterns[i].ingredientsMeshRer.Length; j++)
			{
				color = ingredientsPatterns[i].ingredientsMeshRer[j].material.GetColor("_Color");
				color2 = ingredientsPatterns[i].ingredientsMeshRer[j].material.GetColor("_OutlineColor");
				color.a = 0f;
				color2.a = 0f;
				ingredientsPatterns[i].ingredientsMeshRer[j].material.SetColor("_Color", color);
				ingredientsPatterns[i].ingredientsMeshRer[j].material.SetColor("_OutlineColor", color2);
				ingredientsPatterns[i].ingredientsMeshRer[j].enabled = false;
			}
		}
		color = takoyaki_Topping.material.GetColor("_Color");
		color2 = takoyaki_Topping.material.GetColor("_OutlineColor");
		color.a = 0f;
		color2.a = 0f;
		takoyaki_Topping.material.SetColor("_Color", color);
		takoyaki_Topping.material.SetColor("_OutlineColor", color2);
		takoyaki_Topping.enabled = false;
		for (int k = 0; k < takoBallBakeStatus.Length; k++)
		{
			takoBallBakeStatus[k] = Takoyaki_Define.TakoBallBakeStatus.HalfBake;
		}
		Material[] materials = takoyakiBall.materials;
		for (int l = 0; l < materials.Length; l++)
		{
			materials[l].SetTextureOffset("_MainTex", TAKO_BALL_HALF_BAKE_TEX_OFFSET);
		}
		materials = takoyakiBall_OverBake.materials;
		foreach (Material material in materials)
		{
			material.SetColor("_Color", new Color(material.GetColor("_Color").r, material.GetColor("_Color").g, material.GetColor("_Color").b, 0f));
			material.SetColor("_OutlineColor", new Color(material.GetColor("_OutlineColor").r, material.GetColor("_OutlineColor").g, material.GetColor("_OutlineColor").b, 0f));
			material.SetTextureOffset("_MainTex", TAKO_BALL_OVER_BAKE_TEX_OFFSET);
		}
		takoyakiBall_Half.materials[0].SetTextureOffset("_MainTex", TAKO_BALL_HALF_BAKE_TEX_OFFSET);
		ingredientsPatternNo = UnityEngine.Random.Range(0, ingredientsPatterns.Length);
		ingredientsPatterns[ingredientsPatternNo].patternRootObj.SetActive(value: true);
	}
	public void ShowIngredients()
	{
		isIngredientsPutIn = true;
		LeanTween.value(0f, 1f, 1f).setOnUpdate(delegate(float value)
		{
			for (int i = 0; i < ingredientsPatterns[ingredientsPatternNo].ingredientsMeshRer.Length; i++)
			{
				Color color = ingredientsPatterns[ingredientsPatternNo].ingredientsMeshRer[i].material.GetColor("_Color");
				Color color2 = ingredientsPatterns[ingredientsPatternNo].ingredientsMeshRer[i].material.GetColor("_OutlineColor");
				color.a = value;
				color2.a = value;
				ingredientsPatterns[ingredientsPatternNo].ingredientsMeshRer[i].material.SetColor("_Color", color);
				ingredientsPatterns[ingredientsPatternNo].ingredientsMeshRer[i].material.SetColor("_OutlineColor", color2);
				ingredientsPatterns[ingredientsPatternNo].ingredientsMeshRer[i].enabled = true;
			}
		}).setOnComplete((Action)delegate
		{
			isIngredientsFadeIn = true;
		});
	}
	public void BakeTakoBall()
	{
		if (duringSpinAnimation)
		{
			return;
		}
		nowTakoBallBakeTime[(!isBakingDown) ? 1 : 0] += Time.deltaTime;
		if (nowTakoBallBakeTime[(!isBakingDown) ? 1 : 0] > takoBallBakeTime * 3f && takoBallBakeStatus[(!isBakingDown) ? 1 : 0] == Takoyaki_Define.TakoBallBakeStatus.Bake)
		{
			takoBallBakeStatus[(!isBakingDown) ? 1 : 0] = Takoyaki_Define.TakoBallBakeStatus.OverBake;
			if (takoBallBakeStatus[0] == Takoyaki_Define.TakoBallBakeStatus.OverBake && takoBallBakeStatus[1] == Takoyaki_Define.TakoBallBakeStatus.OverBake)
			{
				FadeInOverBakeTakoBall();
			}
			else
			{
				takoyakiBall.materials[(!isBakingDown) ? 1 : 0].SetTextureOffset("_MainTex", TAKO_BALL_OVER_BAKE_TEX_OFFSET);
				takoyakiBall_Half.materials[0].SetTextureOffset("_MainTex", TAKO_BALL_OVER_BAKE_TEX_OFFSET);
			}
			if (userType <= Takoyaki_Define.UserType.PLAYER_4 && !SingletonCustom<AudioManager>.Instance.IsSePlaying("se_takoyaki_bake"))
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_takoyaki_bake");
			}
		}
		if (nowTakoBallBakeTime[(!isBakingDown) ? 1 : 0] > takoBallBakeTime * 2f)
		{
			isOverBakeAlert = true;
			if (userType <= Takoyaki_Define.UserType.PLAYER_4 && !SingletonCustom<AudioManager>.Instance.IsSePlaying("se_takoyaki_bake"))
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_takoyaki_bake");
			}
		}
		else if (nowTakoBallBakeTime[(!isBakingDown) ? 1 : 0] > takoBallBakeTime)
		{
			if (takoBallBakeStatus[(!isBakingDown) ? 1 : 0] == Takoyaki_Define.TakoBallBakeStatus.HalfBake)
			{
				takoBallBakeStatus[(!isBakingDown) ? 1 : 0] = Takoyaki_Define.TakoBallBakeStatus.Bake;
				takoyakiBall.materials[(!isBakingDown) ? 1 : 0].SetTextureOffset("_MainTex", TAKO_BALL_BAKE_TEX_OFFSET);
				takoyakiBall_Half.materials[0].SetTextureOffset("_MainTex", TAKO_BALL_BAKE_TEX_OFFSET);
				if (userType <= Takoyaki_Define.UserType.PLAYER_4 && !SingletonCustom<AudioManager>.Instance.IsSePlaying("se_takoyaki_bake"))
				{
					SingletonCustom<AudioManager>.Instance.SePlay("se_takoyaki_bake");
				}
			}
		}
		else if (nowTakoBallBakeTime[0] > takoBallBakeTime / 2f)
		{
			isTakoBallHalfForm = true;
		}
	}
	public void TakoBallSpin()
	{
		isBakingDown = !isBakingDown;
		duringSpinAnimation = true;
		LeanTween.rotateAroundLocal(base.gameObject, Vector3.right, 180f, TAKOBALL_SPIN_TIME).setOnComplete(TakoBallSpinEnd);
	}
	private void TakoBallSpinEnd()
	{
		if (isBakingTakoBallHalf)
		{
			isBakingTakoBallHalf = false;
			takoyakiBall.gameObject.SetActive(value: true);
			takoyakiBall_Half.gameObject.SetActive(value: false);
			for (int i = 0; i < ingredientsPatterns.Length; i++)
			{
				ingredientsPatterns[i].patternRootObj.SetActive(value: false);
			}
		}
		duringSpinAnimation = false;
	}
	public void ShowTopping()
	{
		takoyaki_Topping.transform.SetLocalEulerAnglesZ(base.transform.localEulerAngles.z);
		LeanTween.value(0f, 1f, 0.75f).setOnUpdate(delegate(float value)
		{
			for (int i = 0; i < ingredientsPatterns[ingredientsPatternNo].ingredientsMeshRer.Length; i++)
			{
				Color color = takoyaki_Topping.material.GetColor("_Color");
				Color color2 = takoyaki_Topping.material.GetColor("_OutlineColor");
				color.a = value;
				color2.a = value;
				takoyaki_Topping.material.SetColor("_Color", color);
				takoyaki_Topping.material.SetColor("_OutlineColor", color2);
				takoyaki_Topping.enabled = true;
			}
		});
	}
	public Takoyaki_Define.TakoBallBakeStatus GetTakoBallBakeStatus()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < takoBallBakeStatus.Length; i++)
		{
			if (takoBallBakeStatus[i] == Takoyaki_Define.TakoBallBakeStatus.HalfBake)
			{
				num++;
			}
			else if (takoBallBakeStatus[i] == Takoyaki_Define.TakoBallBakeStatus.Bake)
			{
				num2++;
			}
			else if (takoBallBakeStatus[i] == Takoyaki_Define.TakoBallBakeStatus.OverBake)
			{
				num3++;
			}
		}
		if (num2 == 2)
		{
			return Takoyaki_Define.TakoBallBakeStatus.Bake;
		}
		if (num > 0)
		{
			return Takoyaki_Define.TakoBallBakeStatus.HalfBake;
		}
		return Takoyaki_Define.TakoBallBakeStatus.OverBake;
	}
	public bool IsBakingPlaneBaked()
	{
		return takoBallBakeStatus[(!isBakingDown) ? 1 : 0] == Takoyaki_Define.TakoBallBakeStatus.Bake;
	}
	private void FadeInOverBakeTakoBall()
	{
		takoyakiBall_OverBake.gameObject.SetActive(value: true);
		LeanTween.value(0f, 1f, 2f).setOnUpdate(delegate(float value)
		{
			Material[] materials2 = takoyakiBall_OverBake.materials;
			foreach (Material material2 in materials2)
			{
				material2.SetColor("_Color", new Color(material2.GetColor("_Color").r, material2.GetColor("_Color").g, material2.GetColor("_Color").b, value));
				material2.SetColor("_OutlineColor", new Color(material2.GetColor("_OutlineColor").r, material2.GetColor("_OutlineColor").g, material2.GetColor("_OutlineColor").b, value));
			}
		});
		LeanTween.value(1f, 0f, 2f).setOnUpdate(delegate(float value)
		{
			Material[] materials = takoyakiBall.materials;
			foreach (Material material in materials)
			{
				material.SetColor("_Color", new Color(material.GetColor("_Color").r, material.GetColor("_Color").g, material.GetColor("_Color").b, value));
				material.SetColor("_OutlineColor", new Color(material.GetColor("_OutlineColor").r, material.GetColor("_OutlineColor").g, material.GetColor("_OutlineColor").b, value));
			}
		});
		LeanTween.delayedCall(2f, (Action)delegate
		{
			takoyakiBall.gameObject.SetActive(value: false);
		});
	}
}
