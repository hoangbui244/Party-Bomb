using UnityEngine;
public class HandSeal_QualityManager : MonoBehaviour
{
	private SkinWeights skinWeights;
	private void Start()
	{
		skinWeights = QualitySettings.skinWeights;
		QualitySettings.skinWeights = SkinWeights.TwoBones;
	}
	private void OnDestroy()
	{
		QualitySettings.skinWeights = skinWeights;
	}
}
