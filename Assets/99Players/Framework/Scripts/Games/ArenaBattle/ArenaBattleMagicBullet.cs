using UnityEngine;
public class ArenaBattleMagicBullet : MonoBehaviour {
    [SerializeField]
    [Header("ル\u30fcトプレイヤ\u30fc")]
    private ArenaBattlePlayer rootPlayer;
    [SerializeField]
    [Header("弾丸パ\u30fcティクル")]
    private ParticleSystem psEffect;
    [SerializeField]
    [Header("爆発パ\u30fcティクル")]
    private ParticleSystem psHit;
    [SerializeField]
    [Header("全パ\u30fcティクル")]
    private ParticleSystem[] arrayAllParticle;
    [SerializeField]
    [Header("リジッドボディ")]
    private Rigidbody rigid;
    [SerializeField]
    [Header("コライダ\u30fc")]
    private SphereCollider col;
    private Vector3 moveDir;
    private bool isHit;
    private ArenaBattlePlayer player;
    private ArenaBattleMagicBullet bullet;
    private float destroyTime = 10f;
    public ArenaBattlePlayer RootPlayer => rootPlayer;
    public void Init(Vector3 _dir, Gradient _color) {
        moveDir = _dir.normalized * 0.125f;
        psEffect.Play();
        for (int i = 0; i < arrayAllParticle.Length; i++) {
            //??arrayAllParticle[i].colorOverLifetime.color = _color;
        }
    }
    private void Update() {
        if (!isHit) {
            base.transform.AddPosition(moveDir.x, moveDir.y, moveDir.z);
        }
        destroyTime -= Time.deltaTime;
        if (destroyTime <= 0f) {
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (isHit) {
            return;
        }
        if (rootPlayer == null) {
            player = other.gameObject.GetComponent<ArenaBattlePlayer>();
            bool flag = player != null;
        } else if (other.gameObject != rootPlayer.gameObject) {
            player = other.gameObject.GetComponent<ArenaBattlePlayer>();
            if (player != null && !player.IsDodge && player.CurrentState != ArenaBattlePlayer.State.SWORD_ATTACK_SP) {
                player.KnockBack((player.transform.position - rootPlayer.transform.position).normalized, 1.5f, 0.05f);
                RootPlayer.SetVibration();
                RootPlayer.AddSp();
                isHit = true;
                psEffect.Stop();
                psHit.Play();
                SingletonCustom<AudioManager>.Instance.SePlay("se_magic_hit", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
                UnityEngine.Object.Destroy(rigid);
                UnityEngine.Object.Destroy(col);
            }
            bullet = other.gameObject.GetComponent<ArenaBattleMagicBullet>();
            if (bullet != null) {
                isHit = true;
                psEffect.Stop();
                psHit.Play();
                SingletonCustom<AudioManager>.Instance.SePlay("se_magic_hit", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
                UnityEngine.Object.Destroy(rigid);
                UnityEngine.Object.Destroy(col);
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("Field")) {
                isHit = true;
                psEffect.Stop();
                psHit.Play();
                SingletonCustom<AudioManager>.Instance.SePlay("se_magic_hit", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
                UnityEngine.Object.Destroy(rigid);
                UnityEngine.Object.Destroy(col);
            }
        }
    }
}
