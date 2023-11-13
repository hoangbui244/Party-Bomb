using UnityEngine;
public class MonsterKill_Player_MagicBullet : MonoBehaviour {
    private MonsterKill_Player player;
    [SerializeField]
    [Header("攻撃用コライダ\u30fc")]
    private MonsterKill_AttackCollider attackCollider;
    [SerializeField]
    [Header("弾丸エフェクト")]
    private ParticleSystem bulletEffect;
    [SerializeField]
    [Header("爆発エフェクト")]
    private ParticleSystem hitEffect;
    private ParticleSystem[] arrayAllEffect;
    private Vector3 moveDir;
    private bool isHit;
    private float destroyTime = 10f;
    public void Init(MonsterKill_Player _player) {
        player = _player;
        attackCollider.Init(player, this);
    }
    public void SetColor(Gradient _color) {
        arrayAllEffect = base.transform.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < arrayAllEffect.Length; i++) {
            //??arrayAllEffect[i].colorOverLifetime.color = _color;
        }
    }
    public void SetMoveDir(Vector3 _dir) {
        moveDir = _dir.normalized * 0.125f;
    }
    public void PlayEffect() {
        bulletEffect.Play();
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
    public void Hit() {
        isHit = true;
        attackCollider.gameObject.SetActive(value: false);
        bulletEffect.Stop();
        hitEffect.Play();
        if (!player.GetIsCpu()) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_magic_hit", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
        }
    }
}
