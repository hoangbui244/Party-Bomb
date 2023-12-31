﻿using UnityEngine;
using FIMSpace.FOptimizing;

/// <summary>
/// FC: Scriptable container for IFLOD
/// </summary>
//[CreateAssetMenu(menuName = "Custom Optimizers/FLOD_Terrain Reference")]
public sealed class ScrLOD_Terrain : ScrLOD_Base
{
    [SerializeField]
    private LODI_Terrain settings;
    public override ILODInstance GetLODInstance() { return settings; }
    public ScrLOD_Terrain() { settings = new LODI_Terrain(); }


    public override ScrLOD_Base GetScrLODInstance()
    { return CreateInstance<ScrLOD_Terrain>(); }


    public override ScrLOD_Base CreateNewScrCopy()
    {
        ScrLOD_Terrain lodA = CreateInstance<ScrLOD_Terrain>();
        lodA.settings = settings.GetCopy() as LODI_Terrain;
        return lodA;
    }


    public override ScriptableLODsController GenerateLODController(Component target, ScriptableOptimizer optimizer)
    {
        Terrain t = target as Terrain;
        if (!t) t = target.GetComponentInChildren<Terrain>();
        if (t) if (!optimizer.ContainsComponent(t))
            {
                return new ScriptableLODsController(optimizer, t, -1, "Terrain", this);
            }

        return null;
    }
}
