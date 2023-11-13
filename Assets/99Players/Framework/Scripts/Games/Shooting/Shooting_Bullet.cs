using UnityEngine;
public class Shooting_Bullet : MonoBehaviour
{
	private int gunNo;
	private int playerNo;
	[SerializeField]
	private Rigidbody rigid;
	[SerializeField]
	private Collider collider;
	private bool isContact;
	private Vector3 prev;
	private Vector3 defScale;
	private Vector3 shotDir;
	private readonly float SHOT_SCALE_MAX_MAG = 10f;
	[SerializeField]
	[Header("軌道")]
	private TrailRenderer trail;
	private bool flg;
	private bool isPlayer;
	private Transform aiTarget;
	public Vector3 acceleration;
	private Vector3 velocity;
	private Vector3 position;
	private float trackingTime;
	private const float trackingMaxTime = 1f;
	private bool isTracking;
	private const float period = 0.25f;
	public int GunNo => gunNo;
	public int PlayerNo => playerNo;
	public Rigidbody Rigid => rigid;
	public Collider Collider => collider;
	public bool IsPleryer
	{
		get
		{
			return isPlayer;
		}
		set
		{
			isPlayer = value;
		}
	}
	public Transform AITarget
	{
		get
		{
			return aiTarget;
		}
		set
		{
			aiTarget = value;
		}
	}
	public TrailRenderer Trail => trail;
	private void Awake()
	{
		defScale = base.transform.localScale;
		trackingTime = 0f;
	}
	private void Start()
	{
		position = base.gameObject.transform.position;
	}
	private void Update()
	{
		if (!isContact && !(rigid == null))
		{
			Vector3 forward = rigid.position - prev;
			if ((double)forward.magnitude > 0.01)
			{
				rigid.rotation = Quaternion.LookRotation(forward);
			}
			prev = rigid.position;
			if (flg)
			{
				base.transform.rotation = base.transform.parent.transform.rotation;
			}
		}
	}
	public void Shot(int _gunNo, Vector3 _vec, int _playerNo)
	{
		gunNo = _gunNo;
		playerNo = _playerNo;
		prev = rigid.position;
		base.gameObject.SetActive(value: true);
		LeanTween.scale(base.gameObject, defScale * SHOT_SCALE_MAX_MAG, 0.2f);
		if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			rigid.AddForce(_vec * 120f * 1.5f, ForceMode.VelocityChange);
		}
		else
		{
			rigid.AddForce(_vec * 120f, ForceMode.VelocityChange);
		}
		shotDir = _vec.normalized;
	}
	private void OnCollisionEnter(Collision collision)
	{
		trail.gameObject.transform.parent = SingletonCustom<Shooting_BulletManager>.Instance.gameObject.transform;
		trail.emitting = false;
		if (collision.gameObject.CompareTag("Ninja"))
		{
			isContact = true;
		}
	}
}
