using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
[System.Serializable]
public class ReplaceHelper:EditorWindow {
    private Mesh targetMesh;
    private string question;
    private Vector3 positionMod;
    private Vector3 scale;
    private Material targetMaterial1;
    private Material targetMaterial2;
    [MenuItem("Tools/ReplaceHelper")]
    public static void ShowWindow() {
        GetWindow<ReplaceHelper>("ReplaceHelper");
    }
    public void OnGUI() {
        EditorGUI.BeginChangeCheck();
        targetMesh = (Mesh)EditorGUILayout.ObjectField("Script Location",targetMesh,typeof(Mesh),false);
        targetMaterial1 = (Material)EditorGUILayout.ObjectField("Script Location1",targetMaterial1,typeof(Material),false);
        targetMaterial2 = (Material)EditorGUILayout.ObjectField("Script Location2",targetMaterial2,typeof(Material),false);
        question = EditorGUILayout.TextField("Mesh",question);
        positionMod = EditorGUILayout.Vector3Field("Position Modification",positionMod);
        scale = EditorGUILayout.Vector3Field("Scale Modification",scale);
        if(GUILayout.Button("Replace",GUILayout.Width(100),GUILayout.Height(30))) {
            ReplaceThings();
        }
        if(GUI.changed) {
            //EditorUtility.SetDirty(castedTarget);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
    private void OnInspectorUpdate() {
        this.Repaint();
    }
    private void ReplaceThings() {
        foreach(GameObject gobj in Selection.gameObjects) {
            gobj.GetComponent<MeshFilter>().mesh = targetMesh;
            gobj.transform.localPosition = new Vector3(positionMod.x,positionMod.y,positionMod.z);
            gobj.transform.localScale = scale;
            var temp = gobj.transform.GetComponent<MeshRenderer>().sharedMaterials[0];
            //gobj.transform.GetComponent<MeshRenderer>().sharedMaterials[0] = targetMaterial1;
            //gobj.transform.GetComponent<MeshRenderer>().sharedMaterials[1] = targetMaterial2;
            gobj.transform.GetComponent<MeshRenderer>().sharedMaterials = new Material[] { temp,targetMaterial1 };
            gobj.transform.GetComponent<MeshRenderer>().sharedMaterials[0].SetTexture("_NormalTex",null);
        }
    }
}
