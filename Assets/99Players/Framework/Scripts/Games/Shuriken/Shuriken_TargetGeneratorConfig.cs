using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_TargetGeneratorConfig : DecoratedScriptableObject
{
	[SerializeField]
	[DisplayName("最低限表示される的の数")]
	private int minimumDisplayTargets = 8;
	[SerializeField]
	[DisplayName("再表示までの最低間隔")]
	private int minimumDisplayInterval = 8;
	public int MinimumDisplayTargets => minimumDisplayTargets;
	public int MinimumDisplayInterval => minimumDisplayInterval;
}
