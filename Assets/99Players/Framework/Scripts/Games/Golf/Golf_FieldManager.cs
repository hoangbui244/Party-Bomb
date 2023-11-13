using System;
using UnityEngine;
public class Golf_FieldManager : SingletonCustom<Golf_FieldManager>
{
	public enum GroundType
	{
		Fairway,
		Rough,
		Bunker,
		Green,
		Cup,
		OB,
		TeeGround
	}
	[Serializable]
	private struct GroundAttenuation
	{
		public GroundType groundType;
		public float drag;
		public float angularDrag;
		public float velocity;
		public float bounce;
	}
	[Header("デバッグ：地面の種類を固定するかどうか")]
	public bool isDebugGroundType;
	[Header("デバッグ：地面の種類を固定")]
	public GroundType debugGroundType;
	[Header("デバッグ：ホ\u30fcルの種類を固定するかどうか")]
	public bool isDebugHole;
	[Header("デバッグ：ホ\u30fcルの種類を固定")]
	public int debugHoleIdx;
	[SerializeField]
	[Header("ホ\u30fcル")]
	private Golf_Hole[] arrayHole;
	private Golf_Hole hole;
	[SerializeField]
	[Header("生成したホ\u30fcルを格納するル\u30fcト")]
	private Transform holeRoot;
	private Vector3 teeMarkCenter;
	[SerializeField]
	[Header("ティ\u30fcクラス")]
	private Golf_Tee tee;
	[SerializeField]
	[Header("カップ")]
	private GameObject cup;
	[SerializeField]
	[Header("旗ル\u30fcト")]
	private GameObject flagRoot;
	[SerializeField]
	[Header("旗")]
	private MeshRenderer flagMesh;
	[SerializeField]
	[Header("ボ\u30fcルが地面に接触した時の減衰量")]
	private GroundAttenuation[] arrayGroundAttenuation;
	[SerializeField]
	[Header("ボ\u30fcルが地面に接触した時のエフェクトを生成するのル\u30fcト")]
	private Transform groundEffectRoot;
	[SerializeField]
	[Header("ボ\u30fcルが地面に接触した時の芝エフェクト")]
	private ParticleSystem prefGroundEffectLawn;
	[SerializeField]
	[Header("ボ\u30fcルが地面に接触した時の砂エフェクト")]
	private ParticleSystem prefGroundEffectSand;
	public void Init()
	{
		isDebugGroundType = false;
		isDebugHole = false;
		int num = 0;
		do
		{
			num = UnityEngine.Random.Range(0, arrayHole.Length);
		}
		while (num == Golf_Define.BEFORE_HOLE_IDX);
		Golf_Define.BEFORE_HOLE_IDX = num;
		hole = UnityEngine.Object.Instantiate(arrayHole[num], holeRoot);
		hole.Init();
		cup.transform.position = hole.GetCupPos();
		flagMesh.enabled = false;
		LeanTween.delayedCall(base.gameObject, 0.1f, (Action)delegate
		{
			flagMesh.enabled = true;
		});
		InitPlay();
	}
	public void InitPlay()
	{
		flagRoot.transform.SetLocalEulerAnglesY(90f + SingletonCustom<Golf_WindManager>.Instance.GetWindDir());
	}
	public Golf_Hole GetHole()
	{
		return hole;
	}
	public float GetConversionYardDistance(float _distance)
	{
		return Mathf.Floor(_distance * Golf_Define.CONVERSION_YARD * 100f) / 100f;
	}
	public void SetTeeMarkCenterPos(Vector3 _pos)
	{
		teeMarkCenter = _pos;
	}
	public void SetTeePos(Vector3 _pos)
	{
		tee.transform.position = _pos;
	}
	public Vector3 GetReadyBallPos()
	{
		return tee.GetPutBallPos();
	}
	public Vector3 GetShotVec()
	{
		Vector3 normalized = (teeMarkCenter - tee.GetPutBallPos()).normalized;
		normalized.y = 0f;
		return normalized;
	}
	public Vector3 GetTeeMarkCenterToBallVec()
	{
		Vector3 normalized = (tee.GetPutBallPos() - teeMarkCenter).normalized;
		normalized.y = 0f;
		return normalized;
	}
	public float GetGroundAttenuationDrag(GroundType _groundType)
	{
		return arrayGroundAttenuation[(int)_groundType].drag;
	}
	public float GetGroundAttenuationAngularDrag(GroundType _groundType)
	{
		return arrayGroundAttenuation[(int)_groundType].angularDrag;
	}
	public float GetGroundAttenuationVelocity(GroundType _groundType)
	{
		return arrayGroundAttenuation[(int)_groundType].velocity;
	}
	public float GetGroundAttenuationBounce(GroundType _groundType)
	{
		return arrayGroundAttenuation[(int)_groundType].bounce;
	}
	public void PlayGroundEffect(GroundType _groundType, Vector3 _pos)
	{
		ParticleSystem particleSystem = null;
		switch (_groundType)
		{
		case GroundType.Fairway:
		case GroundType.Rough:
		case GroundType.Green:
		case GroundType.TeeGround:
			particleSystem = UnityEngine.Object.Instantiate(prefGroundEffectLawn, groundEffectRoot);
			break;
		case GroundType.Bunker:
			particleSystem = UnityEngine.Object.Instantiate(prefGroundEffectSand, groundEffectRoot);
			break;
		}
		if (particleSystem != null)
		{
			particleSystem.transform.position = _pos;
			particleSystem.gameObject.SetActive(value: true);
			particleSystem.Play();
		}
	}
	public void PlayGroundEffectSE(GroundType _groundType, float _volume = 1f)
	{
		switch (_groundType)
		{
		case GroundType.Cup:
		case GroundType.OB:
			break;
		case GroundType.Fairway:
		case GroundType.Green:
		case GroundType.TeeGround:
			SingletonCustom<AudioManager>.Instance.SePlay("se_golf_green", _loop: false, 0f, _volume);
			break;
		case GroundType.Rough:
			SingletonCustom<AudioManager>.Instance.SePlay("se_golf_lawn", _loop: false, 0f, _volume);
			break;
		case GroundType.Bunker:
			SingletonCustom<AudioManager>.Instance.SePlay("se_golf_sand", _loop: false, 0f, _volume);
			break;
		}
	}
}
