using System;
using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_ThrowingShuriken : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("リジッドボディ")]
	private Rigidbody rigidbody;
	[SerializeField]
	[DisplayName("レンダラ\u30fc")]
	private MeshRenderer renderer;
	[SerializeField]
	[DisplayName("軌跡")]
	private TrailRenderer trail;
	[SerializeField]
	[DisplayName("コンフィグ")]
	private Shuriken_ThrowingShurikenConfig config;
	[Header("デバッグ表示")]
	[SerializeField]
	[Disable(false)]
	[DisplayName("投げたプレイヤ\u30fcID")]
	private int throwPlayerNo;
	[SerializeField]
	[Disable(false)]
	[DisplayName("使用中")]
	private bool used;
	private Transform cachedTransform;
	private Transform child;
	public bool Used => used;
	public void Initialize()
	{
		cachedTransform = base.transform;
		child = cachedTransform.GetChild(0);
		rigidbody.useGravity = false;
		Deactivate();
	}
	public void Throw(int playerNo, Vector3 originPoint, Vector3 throwVector)
	{
		Activate();
		throwPlayerNo = playerNo;
		cachedTransform.position = originPoint;
		cachedTransform.LookAt(originPoint + throwVector);
		trail.Clear();
		int num = SingletonMonoBehaviour<Shuriken_GameMain>.Instance.CharacterIndexes[playerNo];
		Color color = config.TrailColors[num];
		trail.startColor = color;
		trail.endColor = color.A(trail.endColor.a);
		renderer.sharedMaterial = config.ShurikenMaterials[num];
		rigidbody.AddForce(throwVector * config.ThrowPower, ForceMode.VelocityChange);
		this.CoffeeBreak().Keep(-1f, (Action<float>)delegate
		{
			child.Rotate(Vector3.up, 360f * Time.deltaTime);
		});
	}
	private void Activate()
	{
		used = true;
		base.gameObject.SetActive(value: true);
	}
	private void Deactivate()
	{
		used = false;
		rigidbody.useGravity = false;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.velocity = Vector3.zero;
		base.gameObject.SetActive(value: false);
		trail.Clear();
	}
	private void OnCollisionEnter(Collision other)
	{
		Shuriken_BasicTarget component = other.collider.GetComponent<Shuriken_BasicTarget>();
		if ((bool)component)
		{
			if (component.AddArmor > 0)
			{
				SingletonMonoBehaviour<Shuriken_Audio>.Instance.PlaySfx(config.HitSfxNames[0], throwPlayerNo);
				component.AddArmor--;
				component.Shake();
				SingletonMonoBehaviour<Shuriken_HitEffectCache>.Instance.Play(other.GetContact(0).point);
			}
			else
			{
				SingletonMonoBehaviour<Shuriken_Players>.Instance.AddScore(throwPlayerNo, component.Score);
				SingletonMonoBehaviour<Shuriken_UI>.Instance.ShowGetScoreUI(component.Score, throwPlayerNo, component.AimHint);
				component.Hide();
				SingletonMonoBehaviour<Shuriken_Audio>.Instance.PlaySfx(config.HitSfxNames[0], throwPlayerNo);
				SingletonMonoBehaviour<Shuriken_HitEffectCache>.Instance.Play(other.GetContact(0).point);
			}
		}
		Deactivate();
	}
}
