using UnityEngine;
public class Shooting_HitPointAnchor : MonoBehaviour
{
	private Shooting_HitPoint[] arrayHitPointAnchor;
	[SerializeField]
	[Header("HitPoint用アンカ\u30fc（Weak）")]
	private Shooting_HitPoint[] arrayHitPointAnchor_Weak;
	[SerializeField]
	[Header("HitPoint用アンカ\u30fc（Normal）")]
	private Shooting_HitPoint[] arrayHitPointAnchor_Normal;
	[SerializeField]
	[Header("HitPoint用アンカ\u30fc（Bad）")]
	private Shooting_HitPoint[] arrayHitPointAnchor_Bad;
	public Shooting_HitPoint[] ArrayHitPointAnchor => arrayHitPointAnchor;
	public void Init()
	{
		int num = 0;
		arrayHitPointAnchor = new Shooting_HitPoint[arrayHitPointAnchor_Weak.Length + arrayHitPointAnchor_Normal.Length + arrayHitPointAnchor_Bad.Length];
		for (int i = 0; i < arrayHitPointAnchor_Weak.Length; i++)
		{
			arrayHitPointAnchor[num] = arrayHitPointAnchor_Weak[i];
			num++;
		}
		for (int j = 0; j < arrayHitPointAnchor_Normal.Length; j++)
		{
			arrayHitPointAnchor[num] = arrayHitPointAnchor_Normal[j];
			num++;
		}
		for (int k = 0; k < arrayHitPointAnchor_Bad.Length; k++)
		{
			arrayHitPointAnchor[num] = arrayHitPointAnchor_Bad[k];
			num++;
		}
	}
}
