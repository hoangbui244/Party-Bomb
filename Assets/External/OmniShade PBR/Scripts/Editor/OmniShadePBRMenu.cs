//------------------------------------
//           OmniShade PBR
//     Copyright© 2023 OmniShade     
//------------------------------------
using UnityEngine;
using UnityEditor;

public static class OmniShadePBRMenu {

	[MenuItem("Tools/" + OmniShadePBR.NAME + "/Refresh Materials")]
	static void RefreshMaterials() {
		var matGUIDs = AssetDatabase.FindAssets("t:material");
		foreach (var guid in matGUIDs) {
			var path = AssetDatabase.GUIDToAssetPath(guid);
			var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
			if (mat != null) {
				var shader = mat.shader;
				if (shader != null && shader.name.StartsWith(OmniShadePBR.SHADER_NAME))
					OmniShadePBRGUI.AutoEnableShaderKeywords(mat);
			}
		}

		AssetDatabase.SaveAssets();
		EditorUtility.DisplayDialog(OmniShadePBR.NAME, "Materials refreshed.", "Close");
	}
}
