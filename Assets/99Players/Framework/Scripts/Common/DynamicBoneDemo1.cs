using UnityEngine;
public class DynamicBoneDemo1 : MonoBehaviour {
    public GameObject m_Player;
    private float m_weight = 1f;
    private void Update() {
        m_Player.transform.Rotate(new Vector3(0f, UnityEngine.Input.GetAxis("Horizontal") * Time.deltaTime * 200f, 0f));
        m_Player.transform.Translate(base.transform.forward * UnityEngine.Input.GetAxis("Vertical") * Time.deltaTime * 4f);
    }
    private void OnGUI() {
        float num = 50f;
        float num2 = 50f;
        float width = 100f;
        float width2 = 200f;
        float num3 = 24f;
        GUI.Label(new Rect(num, num2, width2, num3), "Press arrow key to move");
        Animation componentInChildren = m_Player.GetComponentInChildren<Animation>();
        num2 += num3;
        componentInChildren.enabled = GUI.Toggle(new Rect(num, num2, width2, num3), componentInChildren.enabled, "Play animation");
        num2 += num3 * 2f;
        DynamicBone[] components = m_Player.GetComponents<DynamicBone>();
        GUI.Label(new Rect(num, num2, width2, num3), "Choose dynamic bone:");
        num2 += num3;
        components[0].enabled = GUI.Toggle(new Rect(num, num2, width, num3), components[0].enabled, "Breasts");
        num2 += num3;
        components[1].enabled = GUI.Toggle(new Rect(num, num2, width, num3), components[1].enabled, "Tail");
        num2 += num3;
        GUI.Label(new Rect(num, num2, width2, num3), "Weight");
        m_weight = GUI.HorizontalSlider(new Rect(num + 50f, num2 + 5f, width, num3), m_weight, 0f, 1f);
        DynamicBone[] array = components;
        for (int i = 0; i < array.Length; i++) {
            array[i].SetWeight(m_weight);
        }
    }
}
