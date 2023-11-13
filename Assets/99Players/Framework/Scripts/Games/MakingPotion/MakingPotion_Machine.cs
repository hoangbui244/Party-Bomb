using System;
using UnityEngine;
public class MakingPotion_Machine : MonoBehaviour {
    private readonly float ITEM_DROP_TIME_INTERVAL = 1f;
    private float itemDorpCountTime;
    [SerializeField]
    private Transform centerAnchor;
    [SerializeField]
    private GameObject parentObj;
    [SerializeField]
    private MeshRenderer[] cauldronRenderers;
    [SerializeField]
    private GameObject redObj;
    [SerializeField]
    private GameObject blueObj;
    [SerializeField]
    private GameObject greenObj;
    [SerializeField]
    private GameObject yellowObj;
    [SerializeField]
    private ParticleSystem fastEffect;
    [SerializeField]
    private ParticleSystem goodEffect;
    [SerializeField]
    private ParticleSystem slowEffect;
    [SerializeField]
    private ParticleSystem dropSugarEffect;
    private ParticleSystemRenderer dropSugarRenderer;
    [SerializeField]
    [Header("わたがし機のメッシュレンダラ\u30fc")]
    private MeshRenderer[] bodyRenderers_Opaque;
    [SerializeField]
    [Header("不透明と透明の2つマテリアルがついているメッシュレンダラ\u30fc")]
    private MeshRenderer[] bodyRenderers_MixTransparent;
    private int charaNo;
    private bool isDrop;
    private int fastCnt;
    private int goodCnt;
    private int slowCnt;
    private GameObject obj;
    private GameObject[] objArray = new GameObject[10];
    public void Init(int _charaNo) {
        charaNo = _charaNo;
        dropSugarRenderer = dropSugarEffect.GetComponent<ParticleSystemRenderer>();
        SettingBodyMaterial();
    }
    public Vector3 GetCenterPos() {
        return centerAnchor.position;
    }
    public void SettingBodyMaterial() {
        Material machineOpaqueMaterial = SingletonCustom<MakingPotion_PlayerManager>.Instance.GetMachineOpaqueMaterial(charaNo);
        Material machineTransparentMaterial = SingletonCustom<MakingPotion_PlayerManager>.Instance.GetMachineTransparentMaterial(charaNo);
        for (int i = 0; i < bodyRenderers_Opaque.Length; i++) {
            bodyRenderers_Opaque[i].sharedMaterial = machineOpaqueMaterial;
        }
        for (int j = 0; j < bodyRenderers_MixTransparent.Length; j++) {
            bodyRenderers_MixTransparent[j].sharedMaterials = new Material[2]
            {
                machineTransparentMaterial,
                machineOpaqueMaterial
            };
        }
    }
    public void MachineEffectPlay() {
    }
    public void MachineEffectStop() {
    }
    public void SetMachineEffectColor(Color _color) {
        LeanTween.cancel(base.gameObject);
    }
    public void SetMachineEffectColor(Color _color, float _tweenTime) {
        LeanTween.cancel(base.gameObject);
    }
    public void SetFastEffectColor(Color _color) {
        //??fastEffect.main.startColor = _color;
    }
    public void SetCauldronMaterial(Material _mat) {
        for (int i = 0; i < cauldronRenderers.Length; i++) {
            cauldronRenderers[i].sharedMaterial = _mat;
        }
    }
    public void DropSugarEffectPlay(MakingPotion_PlayerScript.SugarColorType _colorType) {
        int idx;
        for (idx = 0; idx < objArray.Length && objArray[idx] != null; idx++) {
        }
        if (idx != objArray.Length) {
            switch (_colorType) {
                case MakingPotion_PlayerScript.SugarColorType.Red:
                    objArray[idx] = UnityEngine.Object.Instantiate(redObj, parentObj.transform);
                    break;
                case MakingPotion_PlayerScript.SugarColorType.Yellow:
                    objArray[idx] = UnityEngine.Object.Instantiate(yellowObj, parentObj.transform);
                    break;
                case MakingPotion_PlayerScript.SugarColorType.Green:
                    objArray[idx] = UnityEngine.Object.Instantiate(greenObj, parentObj.transform);
                    break;
                case MakingPotion_PlayerScript.SugarColorType.Blue:
                    objArray[idx] = UnityEngine.Object.Instantiate(blueObj, parentObj.transform);
                    break;
            }
            if (objArray[idx] != null) {
                objArray[idx].transform.localPosition = Vector3.zero;
                LeanTween.moveLocalY(objArray[idx], objArray[idx].transform.localPosition.y - 1f, 0.5f).setOnComplete((Action)delegate {
                    if (objArray[idx] != null) {
                        UnityEngine.Object.Destroy(objArray[idx]);
                        objArray[idx] = null;
                    }
                });
            }
        }
    }
    public void SpeedEffectSetting(MakingPotion_PlayerScript.SpinSpeedType _speedType) {
        if (fastEffect != null) {
            if (fastEffect.isPlaying && _speedType != MakingPotion_PlayerScript.SpinSpeedType.Fast && _speedType != MakingPotion_PlayerScript.SpinSpeedType.TooFast) {
                fastEffect.Stop();
            } else if (!fastEffect.isPlaying && (_speedType == MakingPotion_PlayerScript.SpinSpeedType.Fast || _speedType == MakingPotion_PlayerScript.SpinSpeedType.TooFast)) {
                fastCnt += ((_speedType != MakingPotion_PlayerScript.SpinSpeedType.TooFast) ? 1 : 2);
                if (fastCnt > 2) {
                    fastCnt = 0;
                    fastEffect.Play();
                }
            } else {
                fastCnt = 0;
            }
        }
        if (goodEffect != null) {
            if (goodEffect.isPlaying && _speedType != MakingPotion_PlayerScript.SpinSpeedType.Normal) {
                goodEffect.Stop();
            } else if (!goodEffect.isPlaying && _speedType == MakingPotion_PlayerScript.SpinSpeedType.Normal) {
                goodCnt++;
                if (goodCnt == 2) {
                    goodCnt = 0;
                    goodEffect.Play();
                }
            } else {
                goodCnt = 0;
            }
        }
        if (!(slowEffect != null)) {
            return;
        }
        if (slowEffect.isPlaying && _speedType != MakingPotion_PlayerScript.SpinSpeedType.Slow && _speedType != MakingPotion_PlayerScript.SpinSpeedType.TooSlow) {
            slowEffect.Stop();
        } else if (!slowEffect.isPlaying && (_speedType == MakingPotion_PlayerScript.SpinSpeedType.Slow || _speedType == MakingPotion_PlayerScript.SpinSpeedType.TooSlow)) {
            slowCnt += ((_speedType != MakingPotion_PlayerScript.SpinSpeedType.TooSlow) ? 1 : 2);
            if (slowCnt > 2) {
                slowCnt = 0;
                slowEffect.Play();
            }
        } else {
            slowCnt = 0;
        }
    }
}
