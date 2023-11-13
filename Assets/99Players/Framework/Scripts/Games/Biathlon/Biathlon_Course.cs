using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Extension;
using UnityStandardAssets.Utility;
public class Biathlon_Course : MonoBehaviour
{
	[SerializeField]
	[DisplayName("スタ\u30fcト位置")]
	private Transform[] characterAnchors;
	[SerializeField]
	private WaypointCircuit placeCircuit;
	[SerializeField]
	private WaypointCircuit[] aiCircuit;
	[SerializeField]
	private Biathlon_Target[] targets;
	[SerializeField]
	private Biathlon_CourseGuide[] shootingGuideArrows;
	[SerializeField]
	private Biathlon_CourseGuide[] reverseGuideCrosses;
	[SerializeField]
	private ParticleSystem[] snowy;
	[SerializeField]
	private float snowyProbability = 0.3f;
	[SerializeField]
	private Vector3 minVelocity = -new Vector3(0.5f, 0f, 0.5f);
	[SerializeField]
	private Vector3 maxVelocity = new Vector3(0.5f, 0f, 0.5f);
	[SerializeField]
	private float positionOffset = 3f;
	[SerializeField]
	private int raceLap = 2;
	[SerializeField]
	private Transform lookAt;
	[SerializeField]
	private Transform leftBottomAnchor;
	[SerializeField]
	private Transform rightTopAnchor;
	private int[] randomizedIndexes;
	public int RaceLap => raceLap;
	public float Length => placeCircuit.Length;
	public Vector3 LookAtPosition => lookAt.position;
	public Transform LeftBottomAnchor => leftBottomAnchor;
	public Transform RightTopAnchor => rightTopAnchor;
	public void Init()
	{
		randomizedIndexes = (from i in Enumerable.Range(0, targets.Length)
			orderby Guid.NewGuid()
			select i).ToArray();
		Biathlon_CourseGuide[] array = reverseGuideCrosses;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].Deactivate();
		}
		array = shootingGuideArrows;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].Activate();
		}
		if (SingletonCustom<Biathlon_GameMain>.Instance.IsSinglePlay && !(UnityEngine.Random.value > snowyProbability))
		{
			float num = Mathf.Lerp(minVelocity.x, maxVelocity.x, UnityEngine.Random.value);
			float num2 = Mathf.Lerp(minVelocity.x, maxVelocity.x, UnityEngine.Random.value);
			ParticleSystem[] array2 = snowy;
			for (int j = 0; j < array2.Length; j++)
			{
				ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = array2[j].velocityOverLifetime;
				velocityOverLifetime.xMultiplier = num;
				velocityOverLifetime.zMultiplier = num2;
			}
			Transform transform = snowy[0].transform;
			Vector3 localPosition = transform.localPosition;
			localPosition.x = (0f - num) * positionOffset;
			localPosition.z = (0f - num2) * positionOffset;
			transform.localPosition = localPosition;
			snowy[0].Play(withChildren: true);
		}
	}
	public Biathlon_Target GetTargetForPrepare()
	{
		for (int i = 0; i < randomizedIndexes.Length; i++)
		{
			int num = randomizedIndexes[i];
			Biathlon_Target biathlon_Target = targets[num];
			if (!biathlon_Target.IsUsing && !biathlon_Target.IsPrepare)
			{
				return biathlon_Target;
			}
		}
		return null;
	}
	public Vector3 GetCharacterStartPosition(int no)
	{
		return characterAnchors[no].position;
	}
	public WaypointCircuit GetPlaceCircuit()
	{
		return placeCircuit;
	}
	public WaypointCircuit GetAiCircuit()
	{
		return aiCircuit[UnityEngine.Random.Range(0, aiCircuit.Length)];
	}
	public WaypointCircuit GetAiCircuit(int _no)
	{
		return aiCircuit[_no % aiCircuit.Length];
	}
	public void ActivateShootingGuide(int no)
	{
		shootingGuideArrows[no].Activate();
	}
	public void DeactivateShootingGuide(int no)
	{
		shootingGuideArrows[no].Deactivate();
	}
	public void ActivateReverseGuide(int no)
	{
		reverseGuideCrosses[no].Activate();
	}
	public void DeactivateReverseGuide(int no)
	{
		reverseGuideCrosses[no].Deactivate();
	}
	public void IgnoreCollision(Biathlon_Character character, Collider col)
	{
		for (int i = 0; i < shootingGuideArrows.Length; i++)
		{
			if (i != character.PlayerNo)
			{
				shootingGuideArrows[i].IgnoreCollision(col);
			}
		}
		for (int j = 0; j < reverseGuideCrosses.Length; j++)
		{
			if (j != character.PlayerNo)
			{
				reverseGuideCrosses[j].IgnoreCollision(col);
			}
		}
	}
}
