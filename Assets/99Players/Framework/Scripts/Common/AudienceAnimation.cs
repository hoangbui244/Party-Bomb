using System;
using UnityEngine;
public class AudienceAnimation : MonoBehaviour {
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
        CHEER,
        CHEER_TWO_HAND,
        RANDOM
    }
    [SerializeField]
    [Header("体のパ\u30fcツ")]
    private BodyParts bodyParts;
    [SerializeField]
    [Header("アニメ種別")]
    private AnimType animType;
    [SerializeField]
    [Header("アニメ速度")]
    private float animationSpeed = 1f;
    private float animationTime;
    private float[] calcRot;
    private void Awake() {
        calcRot = new float[bodyParts.rendererList.Length];
        if (animType == AnimType.RANDOM) {
            animType = (AnimType)UnityEngine.Random.Range(0, 2);
        }
    }
    private void Update() {
        animationTime += Time.deltaTime * animationSpeed;
        switch (animType) {
            case AnimType.CHEER:
                calcRot[5] = Mathf.SmoothStep(calcRot[5], -90f, 0.2f);
                bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAngles(calcRot[5], 0f, 0f);
                calcRot[4] = Mathf.SmoothStep(calcRot[4], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -20f * animationSpeed - 150f, 0.2f);
                bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
                break;
            case AnimType.CHEER_TWO_HAND:
                calcRot[3] = Mathf.SmoothStep(calcRot[3], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -20f * animationSpeed - 150f, 0.2f);
                bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAngles(calcRot[3], 0f, 0f);
                calcRot[4] = Mathf.SmoothStep(calcRot[4], Mathf.Sin(animationTime * (float)Math.PI * 2f) * -20f * animationSpeed - 150f, 0.2f);
                bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAngles(calcRot[4], 0f, 0f);
                break;
        }
    }
}
