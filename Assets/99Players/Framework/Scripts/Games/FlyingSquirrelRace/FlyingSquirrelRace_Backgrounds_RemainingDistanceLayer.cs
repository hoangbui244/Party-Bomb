using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_Backgrounds_RemainingDistanceLayer : FlyingSquirrelRace_Backgrounds_ObjectLayer
{
	public override void Initialize()
	{
		base.Initialize();
		foreach (Transform item in base.transform)
		{
			item.LocalPosition((Vector3 v) => v.X((float x) => x * base.Speed));
		}
	}
}
