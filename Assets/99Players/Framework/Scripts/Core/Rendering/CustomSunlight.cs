using UnityEngine;
namespace Satbox.Rendering {
    [ExecuteAlways]
    public class CustomSunlight : MonoBehaviour {
        private static int _WorldSpaceCustomLightPos0 = Shader.PropertyToID("_WorldSpaceCustomLightPos0");
        private void Start() {
            ApplyLightPos();
        }
        private void ApplyLightPos() {
            Vector3 vector = -base.transform.forward;
            Shader.SetGlobalVector(_WorldSpaceCustomLightPos0, new Vector4(vector.x, vector.y, vector.z, 1f));
        }
    }
}
