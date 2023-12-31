﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace FIMSpace.FOptimizing
{
    /// <summary>
    /// FM: Helper class for single LOD level settings on ParticleSystem
    /// </summary>
    //[CreateAssetMenu(menuName = "Custom Optimizers/FLOD_ParticleSystem Reference")]
    [System.Serializable]
    public sealed class LODI_ParticleSystem : ILODInstance
    {
        #region Main Settings : Interface Properties

        public int Index { get { return index; } set { index = value; } }
        internal int index = -1;
        public string Name { get { return LODName; } set { LODName = value; } }
        internal string LODName = "";
        public bool CustomEditor { get { return false; } }
        public bool Disable { get { return SetDisabled; } set { SetDisabled = value; } }
        [HideInInspector] public bool SetDisabled = false;
        public bool DrawDisableOption { get { return true; } }
        public bool SupportingTransitions { get { return true; } }
        public bool DrawLowererSlider { get { return true; } }
        public float QualityLowerer { get { return QLowerer; } set { QLowerer = value; } }
        [SerializeField, HideInInspector] private float QLowerer = 1f;
        public string HeaderText { get { return "Particle System LOD Settings"; } }
        public float ToCullDelay { get { return CullingDelay; } }
        internal float CullingDelay = 0f;
        public bool SupportVersions { get { return false; } }
        public int DrawingVersion { get { return 1; } set { new System.NotImplementedException(); } }
        public bool LockSettings { get { return _Locked; } set { _Locked = value; } }
        [HideInInspector][SerializeField] private bool _Locked = false;
        public Texture Icon { get { return null; } }

        public Component TargetComponent { get { return cmp; } }
        [SerializeField][HideInInspector] private ParticleSystem cmp;

        #endregion


        [Space(4)]
        [FPD_Suffix(0f, 1f)]
        [Tooltip("Percentage value of emmision rate for LOD level (percentage of initial emmission rate)")]
        public float EmmissionAmount = 1f;
        [FPD_Suffix(0f, 1f)]
        [Tooltip("Percentage value of burst rates for LOD level (percentage of initial burst rates)")]
        public float BurstsAmount = 1f;

        [FPD_Suffix(0f, 5f, FPD_SuffixAttribute.SuffixMode.PercentageUnclamped)]
        [Tooltip("Multiplier for particles size, if you make emmission smaller, particle size should become bigger to mask lower quality in distance")]
        public float ParticleSizeMul = 1f;


        /// <summary> List of bursts </summary>
        [SerializeField]
        [HideInInspector]
        private ParticleSystem.Burst[] Bursts; // Just initial settings

        [FPD_Suffix(0f, 1f)]
        [Tooltip("Percentage value of 'Max Particles' count for LOD level (percentage of initial 'Max Particles' count)")]
        public float MaxParticlAmount = 1f;

        [Tooltip("Percentage value of emmision rate over distance for LOD level (percentage of initial emmission rate)")]
        [FPD_Suffix(0f, 1f)]
        public float OverDistanceMul = 1f;

        [FPD_Suffix(0f, 1f)]
        [Tooltip("Percentage Alpha values of 'ColorOverLifetimeAlpha' for LOD level (percentage of initial 'ColorOverLifetimeAlpha' alpha keys on gradient)")]
        public float LifetimeAlpha = 1f;


        // Base settings parameters
        [HideInInspector]
        [Tooltip("Changing particle emission bursts can produce unwanted Garbage Collector allocation")]
        public bool ChangeBursts = true;
        [HideInInspector]
        [Tooltip("Changing color gradients can produce unwanted Garbage Collector allocation")]
        public bool ChangeGradients = true;


        [SerializeField]
        [HideInInspector]
        private ParticleSystem.MinMaxGradient ColorOverLifetime; // Just initial settings

        ParticleSystemRenderer allocatedRenderer = null; // Just initial settings
        GradientColorKey[] allocatedColorKeys = null; // Just initial settings

        ParticleSystem.Burst[] allocatedBursts = null;
        GradientAlphaKey[] allocatedGradientKeys = null;
        GradientAlphaKey[] allocatedGradientMinKeys = null;
        GradientAlphaKey[] allocatedGradientMaxKeys = null;
        bool usingGradients = false;


        public void SetSameValuesAsComponent(Component component)
        {
            if (component == null) Debug.LogError("[OPTIMIZERS] Given component is null instead of ParticleSystem!");

            ParticleSystem comp = component as ParticleSystem;

            if (comp != null)
            {
                cmp = comp;

                EmmissionAmount = comp.emission.rateOverTimeMultiplier;
                OverDistanceMul = comp.emission.rateOverDistanceMultiplier;

                allocatedRenderer = comp.GetComponent<ParticleSystemRenderer>();

                #region Bursts

                BurstsAmount = 1f;
                ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[comp.emission.burstCount];
                comp.emission.GetBursts(bursts);
                Bursts = bursts;

                #endregion


                MaxParticlAmount = comp.main.maxParticles;

                LifetimeAlpha = 1f;

                ColorOverLifetime = comp.colorOverLifetime.color;


                #region Color Gradients

                ColorOverLifetime = comp.colorOverLifetime.color;
                usingGradients = false;

                if (ColorOverLifetime.gradient != null)
                { allocatedGradientKeys = ColorOverLifetime.gradient.alphaKeys; if (!usingGradients) usingGradients = allocatedGradientKeys.Length > 0; }

                if (ColorOverLifetime.gradientMin != null)
                { allocatedGradientMinKeys = ColorOverLifetime.gradientMin.alphaKeys; if (!usingGradients) usingGradients = allocatedGradientMinKeys.Length > 0; }

                if (ColorOverLifetime.gradientMax != null)
                { allocatedGradientMaxKeys = ColorOverLifetime.gradientMax.alphaKeys; if (!usingGradients) usingGradients = allocatedGradientMaxKeys.Length > 0; }

                allocatedColorKeys = ColorOverLifetime.gradient.colorKeys;

                #endregion


                ParticleSizeMul = comp.main.startSizeMultiplier;
            }
        }


        public void ApplySettingsToTheComponent(Component component, ILODInstance initialSettingsRef)
        {

            // Casting LOD to correct type
            LODI_ParticleSystem initialSettings = initialSettingsRef as LODI_ParticleSystem;
            ParticleSystem comp = component as ParticleSystem;

            #region Security

            // Checking if casting is right
            if (initialSettings == null || comp == null) { Debug.Log("[OPTIMIZERS] Target LOD is not ParticleSystem LOD or is null"); return; }

            #endregion


            ParticleSystemRenderer pr = initialSettings.allocatedRenderer;
            if (pr == null)
            {
                pr = comp.GetComponent<ParticleSystemRenderer>();
                initialSettings.allocatedRenderer = pr;
            }

            if (Disable)
            {
                if (pr.enabled)
                {
                    pr.enabled = false;
                    comp.Pause(false);
                }
            }
            else
            {
                if (pr.enabled == false)
                {
                    pr.enabled = true;
                    comp.Play(false);
                }
            }

            var emmission = comp.emission;
            var main = comp.main;

            emmission.rateOverTimeMultiplier = initialSettings.EmmissionAmount * EmmissionAmount;
            emmission.rateOverDistanceMultiplier = initialSettings.OverDistanceMul * OverDistanceMul;

            if (ChangeBursts)
                if (initialSettings.Bursts != null)
                {
                    if (allocatedBursts == null) allocatedBursts = new ParticleSystem.Burst[initialSettings.Bursts.Length];
                    ParticleSystem.Burst[] bursts = allocatedBursts;

                    for (int i = 0; i < bursts.Length; i++)
                    {
                        bursts[i] = initialSettings.Bursts[i];
                        bursts[i].minCount = (short)(initialSettings.Bursts[i].minCount * BurstsAmount);
                        bursts[i].maxCount = (short)(initialSettings.Bursts[i].maxCount * BurstsAmount);
                    }

                    emmission.SetBursts(bursts);
                }

            main.maxParticles = (int)(initialSettings.MaxParticlAmount * MaxParticlAmount);

            #region Lifetime color settings

            if (initialSettings.usingGradients)
            {
                if (ChangeGradients)
                {
                    ParticleSystem.MinMaxGradient newColor = comp.colorOverLifetime.color;

                    if (initialSettings.ColorOverLifetime.mode == ParticleSystemGradientMode.Gradient)
                    {
                        if (initialSettings.ColorOverLifetime.gradient != null)
                        {
                            if (allocatedGradientKeys == null) allocatedGradientKeys = new GradientAlphaKey[initialSettings.allocatedGradientKeys.Length];
                            GradientAlphaKey[] keys = allocatedGradientKeys;

                            for (int i = 0; i < keys.Length; i++)
                            {
                                keys[i].alpha = initialSettings.allocatedGradientKeys[i].alpha * LifetimeAlpha;
                                keys[i].time = initialSettings.allocatedGradientKeys[i].time;
                            }

                            newColor.gradient.SetKeys(initialSettings.allocatedColorKeys, keys);
                            //newColor.gradient.SetKeys(comp.colorOverLifetime.color.gradient.colorKeys, keys);
                        }
                    }
                    else
                    {
                        if (initialSettings.ColorOverLifetime.gradientMin != null)
                        {
                            if (allocatedGradientKeys == null) allocatedGradientKeys = new GradientAlphaKey[initialSettings.allocatedGradientMinKeys.Length];
                            GradientAlphaKey[] keys = allocatedGradientKeys;

                            for (int i = 0; i < keys.Length; i++)
                            {
                                newColor.gradientMin.alphaKeys[i].alpha = initialSettings.allocatedGradientMinKeys[i].alpha * LifetimeAlpha;
                                newColor.gradientMin.alphaKeys[i].time = initialSettings.allocatedGradientMinKeys[i].time;
                            }

                            newColor.gradientMin.SetKeys(initialSettings.allocatedColorKeys, keys);

                            keys = new GradientAlphaKey[initialSettings.ColorOverLifetime.gradientMax.alphaKeys.Length];
                            for (int i = 0; i < keys.Length; i++)
                            {
                                newColor.gradientMax.alphaKeys[i].alpha = initialSettings.allocatedGradientMaxKeys[i].alpha * LifetimeAlpha;
                                newColor.gradientMax.alphaKeys[i].time = initialSettings.allocatedGradientMaxKeys[i].time;
                            }

                            newColor.gradientMax.SetKeys(initialSettings.allocatedColorKeys, keys);
                        }
                    }

                    var col = comp.colorOverLifetime;
                    col.color = newColor;
                }
            }

            #endregion

            main.startSizeMultiplier = initialSettings.ParticleSizeMul * ParticleSizeMul;

            CullingDelay = comp.main.startLifetime.constantMax;
        }

        public void AssignAutoSettingsAsForLODLevel(int lodIndex, int lodCount, Component component)
        {
            ParticleSystem comp = component as ParticleSystem;
            if (comp == null) Debug.LogError("[OPTIMIZERS] Given component for reference values is null or is not ParticleSystem Component!");

            cmp = comp;

            // REMEMBER: LOD = 0 is not nearest but one after nearest
            // Trying to auto configure universal LOD settings

            // Making multiplier even smaller for particle systems to change quality even lower automatically
            float mulNoLow = FLOD.GetValueForLODLevel(1f, 0f, lodIndex, lodCount);
            float mul = mulNoLow * QualityLowerer;
            EmmissionAmount = mul;
            OverDistanceMul = mul;
            BurstsAmount = mul;
            MaxParticlAmount = Mathf.Min(1f, mulNoLow * 1.5f);
            ParticleSizeMul = 1.75f - mul * 0.75f;

            Name = "LOD" + (lodIndex + 2); // + 2 to view it in more responsive way for user inside inspector window
        }

        public void AssignSettingsAsForCulled(Component component)
        {
            FLOD.AssignDefaultCulledParams(this);
            EmmissionAmount = 0f;
            OverDistanceMul = 0f;
            BurstsAmount = 0f;
            MaxParticlAmount = 0f;
            ParticleSizeMul = 1.5f;
            LifetimeAlpha = 0f;
        }

        public void AssignSettingsAsForNearest(Component component)
        {
            FLOD.AssignDefaultNearestParams(this);
        }

        public void AssignSettingsAsForHidden(Component component)
        {
            FLOD.AssignDefaultHiddenParams(this);
            MaxParticlAmount = 0.1f;
        }

        public ILODInstance GetCopy() { return MemberwiseClone() as ILODInstance; }


        public void InterpolateBetween(ILODInstance lodA, ILODInstance lodB, float transitionToB)
        {
            FLOD.DoBaseInterpolation(this, lodA, lodB, transitionToB);

            LODI_ParticleSystem a = lodA as LODI_ParticleSystem;
            LODI_ParticleSystem b = lodB as LODI_ParticleSystem;

            EmmissionAmount = Mathf.Lerp(a.EmmissionAmount, b.EmmissionAmount, transitionToB);
            OverDistanceMul = Mathf.Lerp(a.OverDistanceMul, b.OverDistanceMul, transitionToB);
            BurstsAmount = Mathf.Lerp(a.BurstsAmount, b.BurstsAmount, transitionToB);
            MaxParticlAmount = Mathf.Lerp(a.MaxParticlAmount, b.MaxParticlAmount, transitionToB);
            LifetimeAlpha = Mathf.Lerp(a.LifetimeAlpha, b.LifetimeAlpha, transitionToB);
            ParticleSizeMul = Mathf.Lerp(a.ParticleSizeMul, b.ParticleSizeMul, transitionToB);
        }


#if UNITY_EDITOR

        public void AssignToggler(ILODInstance reference)
        {
            LODI_ParticleSystem p = reference as LODI_ParticleSystem;
            if (p != null)
            {
                ChangeBursts = p.ChangeBursts;
                ChangeGradients = p.ChangeGradients;
            }
        }

        public void DrawTogglers(SerializedProperty prop)
        {

            UnityEditor.EditorGUILayout.BeginVertical(FEditor.FGUI_Resources.BGInBoxLightStyle);

            EditorGUILayout.PropertyField(prop.FindPropertyRelative("ChangeBursts"));
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("ChangeGradients"));

            UnityEditor.EditorGUILayout.EndVertical();

        }

        public void CustomEditorWindow(SerializedProperty iflodProp, LODsControllerBase lODsControllerBase)
        { }

        public void DrawVersionSwitch(SerializedProperty iflodProp, LODsControllerBase lODsControllerBase) { }
#endif

    }
}
