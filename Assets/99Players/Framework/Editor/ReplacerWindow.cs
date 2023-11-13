using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class PrefabReplacer : EditorWindow {
    private GameObject m_Prefab;
    private bool m_UsePrefabName;
    private string m_Name;
    [MenuItem("99Players/PrefabReplacer")]
    public static void ShowWindow() {
        GetWindow<PrefabReplacer>("PrefabReplacer");
    }
    private void OnGUI() {
        m_Prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", m_Prefab, typeof(GameObject), false);
        string prefabName = "";
        if (m_Prefab != null) {
            prefabName = m_Prefab.name;
        }
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        // Use prefab name
        if (m_UsePrefabName) {
            EditorGUI.BeginDisabledGroup(m_UsePrefabName);
            EditorGUILayout.TextField("Name", prefabName);
            EditorGUI.EndDisabledGroup();
        }
        else {
            m_Name = EditorGUILayout.TextField("Name", m_Name);
        }
        m_UsePrefabName = EditorGUILayout.Toggle(m_UsePrefabName, GUILayout.Width(15f));
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Replace", GUILayout.Width(100), GUILayout.Height(30))) {
            // Replace logic
            Transform[] selectedTransform = Selection.transforms;
            Transform[] replacedTransform = new Transform[Selection.transforms.Length];
            for (var i = 0; i < selectedTransform.Length; i++) {
                Transform transform = selectedTransform[i];
                GameObject prefabInstance = PrefabUtility.InstantiatePrefab(m_Prefab, transform.parent) as GameObject;
                if (prefabInstance == null) {
                    continue;
                }
                // Transfer transform properties
                prefabInstance.transform.SetPositionAndRotation(transform.position, transform.rotation);
                prefabInstance.transform.localScale = transform.localScale;
                // TODO BKB: Hardcoded naming prefix
                string[] splitStr = transform.gameObject.name.Split('_');
                string prefix = splitStr.Length > 1 ? "_" + splitStr.Last() : "";
                if (m_UsePrefabName) {
                    prefabInstance.name = prefabName + prefix;
                }
                else {
                    prefabInstance.name = m_Name + prefix;
                }
                // Marked old game object by disabling them
                transform.gameObject.SetActive(false);
                transform.gameObject.name += "(Replaced)";
                replacedTransform[i] = prefabInstance.transform;
            }
            // Sort
            Array.Sort(replacedTransform, (left, right) => String.Compare(left.gameObject.name, right.gameObject.name, StringComparison.Ordinal));
            foreach (var transform in replacedTransform) {
                transform.SetAsLastSibling();
            }
            foreach (var transform in selectedTransform) {
                transform.SetAsFirstSibling();
            }
        }
    }
}