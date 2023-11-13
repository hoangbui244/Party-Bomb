using System;
using UnityEngine;
public class RA_CharacterAnim : MonoBehaviour {
    public enum BodyPartsList {
        HEAD,
        BODY,
        HIP,
        SHOULDER_L,
        SHOULDER_R,
        ARM_L,
        ARM_R,
        LEG_L,
        LEG_R,
        ROOT,
        MAX
    }
    [Serializable]
    public struct BodyParts {
        public MeshRenderer[] rendererList;
        public Transform Parts(BodyPartsList _parts) {
            return rendererList[(int)_parts].transform;
        }
        public Transform Parts(int _parts) {
            return rendererList[_parts].transform;
        }
        public void SetMat(Material _mat) {
            for (int i = 0; i < rendererList.Length; i++) {
                rendererList[i].GetComponent<MeshRenderer>().material = _mat;
            }
        }
    }
    public enum AnimType {
        NONE,
        STANDBY,
        WALK,
        CROWN_GET,
        WINNER,
        LOSE,
        JUMP,
        SOMERSAULT_WAIT,
        SOMERSAULT,
        STICK
    }
    [SerializeField]
    [Header("太鼓バチ")]
    private GameObject[] arrayStick;
    [SerializeField]
    [Header("体のパ\u30fcツ")]
    private BodyParts bodyParts;
    private AnimType animType = AnimType.WALK;
    private float animationTime;
    private float animationSpeed = 0.85f;
    private float[] calcRot;
    private float rightAddSpeedTime;
    private float leftAddSpeedTime;
    public void SetAnim(AnimType _type, bool _isStick = false) {
        if (_type == animType) {
            return;
        }
        animType = _type;
        animationTime = UnityEngine.Random.Range(0f, 10f);
        switch (_type) {
            case AnimType.WINNER:
                if (_isStick) {
                    animationSpeed = 0.95f;
                    for (int i = 0; i < arrayStick.Length; i++) {
                        LeanTween.scale(arrayStick[i], Vector3.one, 0.15f);
                    }
                }
                break;
            case AnimType.SOMERSAULT:
                LeanTween.rotateAroundLocal(bodyParts.Parts(BodyPartsList.HIP).gameObject, bodyParts.Parts(BodyPartsList.HIP).right, 720f, 0.6f);
                break;
        }
    }
    private void Awake() {
        calcRot = new float[bodyParts.rendererList.Length];
    }
    private void Update() {
        if (Time.timeScale == 0f) {
            return;
        }
        animationTime += Time.deltaTime * animationSpeed;
        switch (animType) {
            case AnimType.STANDBY:
                calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.2f);
                bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
                bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
                calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.2f);
                bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
                bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
                calcRot[3] = Mathf.SmoothStep(calcRot[3], 0f, 0.2f);
                bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
                calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.2f);
                bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
                calcRot[4] = Mathf.SmoothStep(calcRot[4], 0f, 0.2f);
                bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
                calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.2f);
                bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
                calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.2f);
                bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
                calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.2f);
                bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
                break;
            case AnimType.WALK:
                calcRot[2] = Mathf.Sin(animationTime * animationSpeed * (float)Math.PI) * 2.5f - 1.25f;
                bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
                bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(Mathf.Sin(animationTime * animationSpeed * (float)Math.PI) * 0.01f);
                calcRot[1] = Mathf.SmoothStep(calcRot[1], 3f, 0.2f);
                bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
                bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(Mathf.Sin(animationTime * animationSpeed * 1.25f * (float)Math.PI) * 2.5f - 1.25f);
                calcRot[5] = Mathf.SmoothStep(calcRot[5], -50f, 0.2f);
                bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
                if (animationSpeed >= 3.5f) {
                    calcRot[3] = Mathf.SmoothStep(calcRot[3], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 80f * animationSpeed, 0.2f);
                } else {
                    calcRot[3] = Mathf.SmoothStep(calcRot[3], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 60f * animationSpeed, 0.2f);
                }
                bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
                calcRot[6] = Mathf.SmoothStep(calcRot[6], -50f, 0.2f);
                bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
                if (animationSpeed >= 3.5f) {
                    calcRot[4] = Mathf.SmoothStep(calcRot[4], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -80f * animationSpeed, 0.2f);
                } else {
                    calcRot[4] = Mathf.SmoothStep(calcRot[4], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -60f * animationSpeed, 0.2f);
                }
                bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
                if (animationSpeed >= 3.5f) {
                    calcRot[7] = Mathf.SmoothStep(calcRot[7], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -90f * animationSpeed, 0.2f);
                } else {
                    calcRot[7] = Mathf.SmoothStep(calcRot[7], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -70f * animationSpeed, 0.2f);
                }
                bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
                if (animationSpeed >= 3.5f) {
                    calcRot[8] = Mathf.SmoothStep(calcRot[8], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 90f * animationSpeed, 0.2f);
                } else {
                    calcRot[8] = Mathf.SmoothStep(calcRot[8], Mathf.Sin(animationTime * (float)Math.PI * 2f) * 70f * animationSpeed, 0.2f);
                }
                bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
                break;
            case AnimType.CROWN_GET:
                calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
                bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
                calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
                bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
                calcRot[3] = Mathf.SmoothStep(calcRot[3], -170f, 0.15f);
                bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
                calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
                calcRot[4] = Mathf.SmoothStep(calcRot[4], -170f, 0.15f);
                bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
                calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
                calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
                calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
                bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPosition(0.054f, -0.0483f, 0f);
                break;
            case AnimType.JUMP:
                calcRot[2] = Mathf.SmoothStep(calcRot[2], -2.5f, 0.15f);
                bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
                bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
                calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
                bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
                calcRot[3] = Mathf.SmoothStep(calcRot[3], -90f, 0.15f);
                bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
                calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
                calcRot[4] = Mathf.SmoothStep(calcRot[4], -90f, 0.15f);
                bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
                calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
                calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
                calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
                bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPosition(0.054f, -0.0483f, 0f);
                break;
            case AnimType.SOMERSAULT_WAIT:
                calcRot[2] = Mathf.SmoothStep(calcRot[2], 0f, 0.2f);
                bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
                bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
                calcRot[1] = Mathf.SmoothStep(calcRot[1], 15f, 0.2f);
                bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
                bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
                calcRot[3] = Mathf.SmoothStep(calcRot[3], 15f, 0.2f);
                bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
                calcRot[5] = Mathf.SmoothStep(calcRot[5], -45f, 0.2f);
                bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
                calcRot[4] = Mathf.SmoothStep(calcRot[4], 15f, 0.2f);
                bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
                calcRot[6] = Mathf.SmoothStep(calcRot[6], -45f, 0.2f);
                bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
                calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.2f);
                bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
                calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.2f);
                bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
                break;
            case AnimType.SOMERSAULT:
                bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
                calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.2f);
                bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
                bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
                calcRot[3] = Mathf.SmoothStep(calcRot[3], -180f, 0.2f);
                bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, calcRot[3] * 0.25f);
                calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.2f);
                bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
                calcRot[4] = Mathf.SmoothStep(calcRot[4], -180f, 0.2f);
                bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, calcRot[3] * -0.25f);
                calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.2f);
                bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
                calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.2f);
                bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
                calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.2f);
                bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
                break;
            case AnimType.WINNER:
                calcRot[2] = Mathf.SmoothStep(calcRot[2], -2.5f, 0.15f);
                bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
                bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
                calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
                bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
                calcRot[3] = Mathf.SmoothStep(calcRot[3], -100f, 0.15f);
                bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], calcRot[3] * 0.1f, calcRot[3] * 0.1f);
                calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
                calcRot[4] = Mathf.SmoothStep(calcRot[4], -100f, 0.15f);
                bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], calcRot[4] * -0.1f, calcRot[3] * -0.1f);
                calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
                calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
                calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
                bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPosition(0.054f, -0.0483f, 0f);
                break;
            case AnimType.LOSE:
                calcRot[2] = Mathf.SmoothStep(calcRot[2], 2.5f, 0.15f);
                bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(calcRot[2], 0f, 0f);
                calcRot[0] = Mathf.SmoothStep(calcRot[0], 10f, 0.15f);
                bodyParts.Parts(BodyPartsList.HEAD).SetLocalEulerAngles(calcRot[0], 0f, 0f);
                bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
                calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
                bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
                calcRot[3] = Mathf.SmoothStep(calcRot[3], 10f, 0.15f);
                bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
                calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
                calcRot[4] = Mathf.SmoothStep(calcRot[4], 10f, 0.15f);
                bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
                calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
                calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
                calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.15f);
                bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
                bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPosition(0.054f, -0.0483f, 0f);
                break;
            case AnimType.STICK: {
                    calcRot[2] = Mathf.Sin(animationTime * animationSpeed * 2.5f * (float)Math.PI) * 5f - 2.5f;
                    bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, calcRot[2], 0f);
                    bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalPositionY(0f);
                    calcRot[1] = Mathf.SmoothStep(calcRot[1], 0f, 0.15f);
                    bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(calcRot[1], 0f, 0f);
                    bodyParts.Parts(BodyPartsList.BODY).transform.SetLocalEulerAnglesY(0f);
                    calcRot[5] = Mathf.SmoothStep(calcRot[5], 0f, 0.2f);
                    bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
                    float num = Mathf.Sin((animationTime + rightAddSpeedTime) * (float)Math.PI * 2f);
                    float num2 = 0.95f;
                    if (num < 0f) {
                        num2 = 1.5f;
                        rightAddSpeedTime = 0f;
                    } else {
                        rightAddSpeedTime += Time.deltaTime * 0.15f;
                    }
                    calcRot[3] = Mathf.SmoothStep(calcRot[3], num * 60f * animationSpeed * 1.2f * num2 - 90f, 0.2f);
                    bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
                    calcRot[6] = Mathf.SmoothStep(calcRot[6], 0f, 0.2f);
                    bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAngles(calcRot[6], 0f, 0f);
                    float num3 = Mathf.Sin((animationTime + leftAddSpeedTime) * (float)Math.PI * 2f);
                    float num4 = 0.95f;
                    if (num3 > 0f) {
                        num4 = 1.5f;
                        leftAddSpeedTime = 0f;
                    } else {
                        leftAddSpeedTime += Time.deltaTime * 0.15f;
                    }
                    calcRot[4] = Mathf.SmoothStep(calcRot[4], num3 * -60f * animationSpeed * 1.2f * num4 - 90f, 0.2f);
                    bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
                    calcRot[7] = Mathf.SmoothStep(calcRot[7], 0f, 0.15f);
                    bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAngles(calcRot[7], 0f, 0f);
                    calcRot[8] = Mathf.SmoothStep(calcRot[8], 0f, 0.15f);
                    bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAngles(calcRot[8], 0f, 0f);
                    bodyParts.Parts(BodyPartsList.LEG_R).SetLocalPosition(0.054f, -0.0483f, 0f);
                    break;
                }
        }
    }
}
