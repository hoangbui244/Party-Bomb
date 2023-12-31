﻿using System.IO;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FOptimizing
{
    public static class Optimizers_MenuItems
    {
        //[MenuItem("Assets/Create/FImpossible Creations/Optimizers/Create Custom Component's LODs Parameters Script (rename after)", false, 1)]
        //static void CreateCustomLODParamsScript()
        //{
        //    string fullPath = Application.dataPath + "/FImpossible Creations/Optimizers/Code/Templates/FLODType_Template.txt";
        //    CreateScriptAsset("Assets/FImpossible Creations/Optimizers/Code/Templates/FLODType_Template.txt", "YourComponentNameHere.cs", fullPath);
        //}

        [MenuItem("Assets/Create/FImpossible Creations/Optimizers/Create Custom Scriptable LOD Instance Script (rename after)", false, 1)]
        static void CreateCustomIFLODScript()
        {
            string fullPath = Application.dataPath + "/FImpossible Creations/Plugins - Other/Optimizers 2/Code/Templates/IFLOD_Template.txt";
            CreateScriptAsset("Assets/FImpossible Creations/Plugins - Other/Optimizers 2/Code/Templates/IFLOD_Template.txt", "YourComponentNameHere.cs", fullPath);
        }

#if UNITY_2019_4_OR_NEWER
        [MenuItem("Assets/Create/FImpossible Creations/Optimizers/Create Custom (2019.4+) LOD Instance Script (rename after)", false, 2)]
        static void CreateCustom2020IFLODScript()
        {
            string fullPath = Application.dataPath + "/FImpossible Creations/Plugins - Other/Optimizers 2/Code/Templates/IFLOD2020_Template.txt";
            CreateScriptAsset("Assets/FImpossible Creations/Plugins - Other/Optimizers 2/Code/Templates/IFLOD2020_Template.txt", "YourComponentNameHere.cs", fullPath);
        }
#endif

        static void CreateScriptAsset(string templatePath, string targetName, string fullPath)
        {
            if (File.Exists(fullPath))
            {
#if UNITY_2019_1_OR_NEWER
                ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, targetName);
#else
                typeof(ProjectWindowUtil).GetMethod("CreateScriptAsset", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, new object[] { templatePath, targetName });
#endif
            }
            else
                Debug.LogError("[OPTIMIZERS] File under path '" + fullPath + "' doesn't exist, directory probably was moved");
        }
    }
}