using System;
using UnityEngine;
public class SmeltFishing_VerticalLayout : MonoBehaviour
{
	[SerializeField]
	private Transform[] children = Array.Empty<Transform>();
	[SerializeField]
	private Vector2 positionOffset;
	[SerializeField]
	private float height;
	[SerializeField]
	private float space;
	public void ForceLayoutUpdate()
	{
		Layout();
	}
	private void Update()
	{
	}
	private void Layout()
	{
		Vector2 v = positionOffset;
		v.y -= height / 2f;
		for (int i = 0; i < children.Length; i++)
		{
			Transform transform = children[i];
			if (transform.gameObject.activeSelf)
			{
				transform.localPosition = v;
				v.y -= height;
				v.y -= space;
			}
		}
	}
}
