using UnityEngine;
public class Common_SpeedUpEffectController : MonoBehaviour {
    [SerializeField]
    [Header("矢印エフェクト")]
    private ParticleSystem[] arrowEffects;
    [SerializeField]
    [Header("足元の波エフェクト")]
    private ParticleSystem waveEffect;
    [SerializeField]
    [Header("矢印エフェクトの位置(X,Z)")]
    private float arrowPosRadius;
    [SerializeField]
    [Header("矢印エフェクトの位置範囲(Y)")]
    private float arrowPosHeightMin;
    [SerializeField]
    private float arrowPosHeightMax;
    [SerializeField]
    [Header("矢印エフェクトを出す間隔(秒)")]
    private float arrowInterval;
    [SerializeField]
    [Header("カメラのX角度")]
    private float cameraAngleX;
    [SerializeField]
    [Header("カメラのY角度(カメラ向きが特殊なゲ\u30fcムのみ変更)")]
    private float cameraAngleY;
    private int arrowNo;
    private float arrowTimer;
    private bool isArrowRight;
    private bool isPlay;
    private Vector3 rightDir;
    private Quaternion rotation;
    private void Awake() {
        isArrowRight = (UnityEngine.Random.Range(0, 2) == 1);
        rotation = Quaternion.Euler(0f, cameraAngleY, 0f);
        rightDir = rotation * Vector3.right;
        for (int i = 0; i < arrowEffects.Length; i++) {
            arrowEffects[i].transform.SetLocalEulerAnglesX(cameraAngleX);
        }
    }
    private void Update() {
        if (!isPlay) {
            return;
        }
        base.transform.rotation = rotation;
        arrowTimer += Time.deltaTime;
        if (arrowTimer > arrowInterval) {
            Vector3 vector = rightDir * arrowPosRadius;
            if (!isArrowRight) {
                vector *= -1f;
            }
            vector = Quaternion.Euler(0f, UnityEngine.Random.Range(-45f, 45f), 0f) * vector;
            vector.y = UnityEngine.Random.Range(arrowPosHeightMin, arrowPosHeightMax);
            arrowEffects[arrowNo].transform.position = base.transform.position + vector;
            arrowEffects[arrowNo].Play();
            isArrowRight = !isArrowRight;
            arrowNo++;
            if (arrowNo >= arrowEffects.Length) {
                arrowNo = 0;
            }
            arrowTimer = 0f;
        }
    }
    public void Play() {
        isPlay = true;
        waveEffect.Play();
        arrowTimer = arrowInterval;
    }
    public void Stop() {
        isPlay = false;
        waveEffect.Stop();
    }
    public void SetCameraAngleX(float _cameraAngleX) {
        cameraAngleX = _cameraAngleX;
        for (int i = 0; i < arrowEffects.Length; i++) {
            arrowEffects[i].transform.SetLocalEulerAnglesX(cameraAngleX);
        }
    }
}
