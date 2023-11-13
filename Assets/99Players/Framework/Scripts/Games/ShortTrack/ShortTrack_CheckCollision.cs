using System.Collections.Generic;
using UnityEngine;
public class ShortTrack_CheckCollision : MonoBehaviour
{
	private ShortTrack_Character targetChara;
	private ShortTrack_Character charaTmp;
	[NonReorderable]
	private List<ShortTrack_Character> collsionList = new List<ShortTrack_Character>();
	public void Init(ShortTrack_Character _targetChara)
	{
		targetChara = _targetChara;
	}
	public bool GetIsCollision()
	{
		return collsionList.Count == 0;
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer(SHORTTRACK.LAYER_CHARACTER))
		{
			charaTmp = other.gameObject.GetComponent<ShortTrack_Character>();
			if (charaTmp != null && charaTmp != targetChara && !collsionList.Contains(charaTmp))
			{
				collsionList.Add(charaTmp);
			}
		}
	}
	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer(SHORTTRACK.LAYER_CHARACTER))
		{
			charaTmp = other.gameObject.GetComponent<ShortTrack_Character>();
			if (charaTmp != null && charaTmp != targetChara && !collsionList.Contains(charaTmp))
			{
				collsionList.Add(charaTmp);
			}
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer(SHORTTRACK.LAYER_CHARACTER))
		{
			charaTmp = other.gameObject.GetComponent<ShortTrack_Character>();
			if (charaTmp != null && charaTmp != targetChara && collsionList.Contains(charaTmp))
			{
				collsionList.Remove(charaTmp);
			}
		}
	}
}
