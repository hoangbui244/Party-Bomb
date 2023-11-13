using System;
using TMPro;
using UnityEngine;
public class MikoshiRaceFirstCtrlUI : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer[] slices;
	[SerializeField]
	private SpriteRenderer[] sprites;
	[SerializeField]
	private TextMeshPro[] texts;
	private void OnEnable()
	{
		Color white = Color.white;
		white.a = 0f;
		for (int i = 0; i < slices.Length; i++)
		{
			slices[i].color = white;
		}
		for (int j = 0; j < sprites.Length; j++)
		{
			sprites[j].color = white;
		}
		for (int k = 0; k < texts.Length; k++)
		{
			texts[k].color = white;
		}
		float num = 1.5f;
		if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1)
		{
			num += 1.6f;
		}
		LeanTween.delayedCall(base.gameObject, num, (Action)delegate
		{
			LeanTween.value(base.gameObject, 0f, 1f, 0.25f).setOnUpdate(delegate(float val)
			{
				Color white3 = Color.white;
				white3.a = val;
				for (int num2 = 0; num2 < slices.Length; num2++)
				{
					slices[num2].color = white3;
				}
				for (int num3 = 0; num3 < sprites.Length; num3++)
				{
					sprites[num3].color = white3;
				}
				for (int num4 = 0; num4 < texts.Length; num4++)
				{
					texts[num4].color = white3;
				}
			}).setOnComplete((Action)delegate
			{
				LeanTween.delayedCall(base.gameObject, 3.5f, (Action)delegate
				{
					LeanTween.value(base.gameObject, 1f, 0f, 0.5f).setOnUpdate(delegate(float val)
					{
						Color white2 = Color.white;
						white2.a = val;
						for (int l = 0; l < slices.Length; l++)
						{
							slices[l].color = white2;
						}
						for (int m = 0; m < sprites.Length; m++)
						{
							sprites[m].color = white2;
						}
						for (int n = 0; n < texts.Length; n++)
						{
							texts[n].color = white2;
						}
					}).setOnComplete((Action)delegate
					{
						base.gameObject.SetActive(value: false);
					});
				});
			});
		});
	}
}
