using UnityEngine;
namespace Satbox.Rendering {
    [ExecuteAlways]
    [RequireComponent(typeof(Renderer))]
    public class AtlasRenderer : MonoBehaviour {
        [SerializeField]
        private Vector2Int tiles = new Vector2Int(4, 4);
        [SerializeField]
        private int index;
        [SerializeField]
        private float padding;
        [SerializeField]
        private bool flipU;
        private Renderer renderer;
        private MaterialPropertyBlock propertyBlock;
        private static int _MainTex = Shader.PropertyToID("_MainTex");
        private static int _MainTex_ST = Shader.PropertyToID("_MainTex_ST");
        private void Awake() {
            if (renderer == null) {
                renderer = GetComponent<Renderer>();
            }
            if (propertyBlock == null) {
                propertyBlock = new MaterialPropertyBlock();
            }
        }
        private void OnEnable() {
            EnableMaterialPropertyBlock();
        }
        private Vector4 CalcuateUV() {
            Material sharedMaterial = renderer.sharedMaterial;
            if (sharedMaterial == null) {
                return new Vector4(1f, 1f, 0f, 0f);
            }
            Texture texture = sharedMaterial.GetTexture(_MainTex);
            if (texture == null) {
                return new Vector4(1f, 1f, 0f, 0f);
            }
            int num = index % Mathf.Max(1, tiles.x);
            if (flipU) {
                num = tiles.x - 1 - num;
            }
            int num2 = index / Mathf.Max(1, tiles.x);
            float num3 = 1f / (float)texture.width * padding;
            float num4 = 1f / (float)texture.height * padding;
            float num5 = 1f / Mathf.Max(1f, tiles.x);
            float num6 = 1f / Mathf.Max(1f, tiles.y);
            return new Vector4((num5 - num3 * 2f) * (flipU ? (-1f) : 1f), num6 - num4 * 2f, ((float)num * num5 + num3) * (flipU ? (-1f) : 1f) + (flipU ? 1f : 0f), (float)num2 * num6 + num4);
        }
        private void EnableMaterialPropertyBlock() {
            propertyBlock.SetVector(_MainTex_ST, CalcuateUV());
            renderer.SetPropertyBlock(propertyBlock);
        }
        private void DisablePropertyBlock() {
            if (!(renderer == null)) {
                renderer.SetPropertyBlock(null);
            }
        }
    }
}
