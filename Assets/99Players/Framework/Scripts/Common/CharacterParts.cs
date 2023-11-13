using System;
using UnityEngine;
using UnityEngine.Rendering;
public class CharacterParts : MonoBehaviour {
    public enum BodyPartsList {
        HEAD,
        BODY,
        HIP,
        SHOULDER_L,
        SHOULDER_R,
        ARM_L,
        ARM_R,
        LEG_L,
        LEG_R
    }
    [Serializable]
    public struct BodyParts {
        public MeshRenderer[] rendererList;
        public Transform RendererParts(BodyPartsList _parts) {
            return rendererList[(int)_parts].transform;
        }
        public Transform RendererParts(int _parts) {
            return rendererList[_parts].transform;
        }
        public int GetRendererListLength() {
            return rendererList.Length;
        }
        public void SetMaterial(Material _mat) {
            for (int i = 0; i < rendererList.Length; i++) {
                Material[] materials = rendererList[i].materials;
                for (int j = 0; j < materials.Length; j++) {
                    materials[j] = _mat;
                }
                rendererList[i].materials = materials;
            }
        }
        public void SetShadowCasting(bool _isShadowOn) {
            for (int i = 0; i < rendererList.Length; i++) {
                rendererList[i].shadowCastingMode = (_isShadowOn ? ShadowCastingMode.On : ShadowCastingMode.Off);
            }
        }
        public void SetLayer(int _layer) {
            for (int i = 0; i < rendererList.Length; i++) {
                rendererList[i].gameObject.layer = _layer;
            }
        }
        public void SetLayer(int _layer, BodyPartsList _parts, bool _isIncludeActive = false) {
            MeshRenderer[] componentsInChildren = rendererList[(int)_parts].GetComponentsInChildren<MeshRenderer>(_isIncludeActive);
            for (int i = 0; i < componentsInChildren.Length; i++) {
                componentsInChildren[i].gameObject.layer = _layer;
            }
        }
    }
    [SerializeField]
    [Header("体のパ\u30fcツ")]
    private BodyParts bodyParts;
    public BodyParts Parts => bodyParts;
}
