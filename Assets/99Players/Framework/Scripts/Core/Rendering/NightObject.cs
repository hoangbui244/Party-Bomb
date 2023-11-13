using System.Collections.Generic;
using UnityEngine;
namespace Satbox.Rendering {
    public class NightObject : MonoBehaviour {
        private Renderer renderer;
        private static Dictionary<Material, Material> materials = new Dictionary<Material, Material>();
        private void Start() {
            ReplaceMaterial();
        }
        private void ReplaceMaterial() {
            if (renderer == null) {
                renderer = GetComponent<Renderer>();
            }
            if (!(renderer == null)) {
                Material[] sharedMaterials = renderer.sharedMaterials;
                for (int i = 0; i < sharedMaterials.Length; i++) {
                    sharedMaterials[i] = GetMaterial(sharedMaterials[i]);
                }
                renderer.sharedMaterials = sharedMaterials;
            }
        }
        private Material GetMaterial(Material source) {
            if (!materials.TryGetValue(source, out Material value)) {
                value = UnityEngine.Object.Instantiate(source);
                materials.Add(source, value);
            }
            return value;
        }
    }
}
