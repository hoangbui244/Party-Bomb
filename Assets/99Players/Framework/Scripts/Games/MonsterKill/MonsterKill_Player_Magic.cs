using UnityEngine;
public class MonsterKill_Player_Magic : MonoBehaviour {
    private MonsterKill_Player player;
    [SerializeField]
    [Header("魔法準備エフェクト")]
    private ParticleSystem castEffect;
    [SerializeField]
    [Header("魔法弾プレハブ")]
    private MonsterKill_Player_MagicBullet bulletEffectPref;
    public void Init(MonsterKill_Player _player) {
        player = _player;
        bulletEffectPref.gameObject.SetActive(value: false);
    }
    public void SetColor(Gradient _color) {
        //??castEffect.colorOverLifetime.color = _color;
        bulletEffectPref.SetColor(_color);
    }
    public void PlayCastEffect() {
        castEffect.Play();
    }
    public void AttackStart() {
        castEffect.Stop();
        MonsterKill_Player_MagicBullet monsterKill_Player_MagicBullet = UnityEngine.Object.Instantiate(bulletEffectPref, SingletonCustom<MonsterKill_PlayerManager>.Instance.GetMagicBulletEffectAnchor());
        monsterKill_Player_MagicBullet.Init(player);
        monsterKill_Player_MagicBullet.transform.SetPosition(castEffect.transform.position.x, player.transform.position.y + 0.3f, castEffect.transform.position.z);
        monsterKill_Player_MagicBullet.SetMoveDir(player.transform.forward);
        monsterKill_Player_MagicBullet.gameObject.SetActive(value: true);
        monsterKill_Player_MagicBullet.PlayEffect();
        if (!player.GetIsCpu()) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_magic_bullet", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
            SingletonCustom<HidVibration>.Instance.SetCommonVibration((int)player.GetUserType());
        }
    }
    public void AttackEnd() {
        castEffect.Stop();
    }
}
