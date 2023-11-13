using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public enum AlignerPosition {
    Center,
    TopLeft
}
public class AlignerWindow : EditorWindow {
    public List<GameObject> GameObjectList;
    private int m_GameObjectPerRow;
    private float m_RowSpacing;
    private float m_ColumnSpacing;
    private AlignerPosition m_Position;
    private bool m_IgnoreInactive;
    [MenuItem("99Players/AlignerWindow")]
    public static void ShowWindow() {
        GetWindow<AlignerWindow>("AlignerWindow");
    }
    private void OnGUI() {
        SerializedObject serializedObject = new SerializedObject(this);
        m_GameObjectPerRow = EditorGUILayout.IntField("GameObjectPerRow", m_GameObjectPerRow);
        m_RowSpacing = EditorGUILayout.FloatField("Row Spacing", m_RowSpacing);
        m_ColumnSpacing = EditorGUILayout.FloatField("Column Spacing", m_ColumnSpacing);
        m_Position = (AlignerPosition)EditorGUILayout.EnumPopup("Position", m_Position);
        m_IgnoreInactive = EditorGUILayout.Toggle("Ignore Inactive", m_IgnoreInactive);
        // m_GameObjects
        SerializedProperty gameObjectsProperty = serializedObject.FindProperty(nameof(GameObjectList));
        EditorGUILayout.PropertyField(gameObjectsProperty, true);
        serializedObject.ApplyModifiedProperties();
        // Button
        if (GUILayout.Button("Align", GUILayout.Width(100), GUILayout.Height(30))) {
            GameObject[] alignedObjects;
            if (m_IgnoreInactive) {
                alignedObjects = GameObjectList.Where(gObj => gObj.activeSelf).ToArray();
            }
            else {
                alignedObjects = GameObjectList.ToArray();
            }
            for (int i = 0; i < alignedObjects.Length; i++) {
                int rowCount, currRow, currRowPos;
                float x, y;
                switch (m_Position) {
                    case AlignerPosition.Center:
                        // Calculate x position
                        currRowPos = i % m_GameObjectPerRow;
                        if (m_GameObjectPerRow % 2 == 0) {
                            x = (currRowPos - m_GameObjectPerRow / 2) * m_RowSpacing;
                        }
                        else {
                            x = (currRowPos - m_GameObjectPerRow / 2 - 1) * m_RowSpacing;
                        }
                        // Calculate y position
                        currRow = i / m_GameObjectPerRow;
                        rowCount = alignedObjects.Length / m_GameObjectPerRow;
                        if (rowCount % 2 == 0) {
                            y = -((currRow - rowCount / 2) * m_ColumnSpacing);
                        } else {
                            y = -((currRow - rowCount / 2 - 1) * m_ColumnSpacing);
                        }
                        break;
                    case AlignerPosition.TopLeft:
                        // Calculate x position
                        currRowPos = i % m_GameObjectPerRow;
                        x = currRowPos * m_RowSpacing;
                        // Calculate y position
                        currRow = i / m_GameObjectPerRow;
                        y = -(currRow * m_ColumnSpacing);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                // Set new position
                Transform currTransform = alignedObjects[i].transform;
                currTransform.localPosition = new Vector3(x, y, currTransform.localPosition.z);
            }
        }
    }
}