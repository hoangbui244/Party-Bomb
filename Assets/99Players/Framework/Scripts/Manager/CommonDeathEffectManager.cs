using UnityEngine;
public class CommonDeathEffectManager : SingletonCustom<CommonDeathEffectManager> {
    [SerializeField]
    private ParticleSystem[] deathEffects;
    public void SetScale(float _scale) {
        Vector3 localScale = Vector3.one * _scale;
        for (int i = 0; i < deathEffects.Length; i++) {
            deathEffects[i].transform.localScale = localScale;
        }
    }
    public void SetScale(int _playerNo, float _scale) {
        deathEffects[_playerNo].transform.localScale = Vector3.one * _scale;
    }
    public void PlayEffect(int _playerNo, Vector3 _pos, Vector3 _dir) {
        deathEffects[_playerNo].transform.position = _pos;
        deathEffects[_playerNo].transform.forward = _dir;
        deathEffects[_playerNo].Play();
        SingletonCustom<AudioManager>.Instance.SePlay("se_goeraser_syo_metu");
    }
}
