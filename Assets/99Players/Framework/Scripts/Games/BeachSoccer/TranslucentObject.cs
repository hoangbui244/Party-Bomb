using System.Collections.Generic;
using UnityEngine;
namespace BeachSoccer
{
	public class TranslucentObject : MonoBehaviour
	{
		[SerializeField]
		[Header("子を非表示にするかどうか")]
		private bool isHideChild;
		private Material material;
		private MeshRenderer renderer;
		private BallScript ball;
		private Camera stageCamera;
		private List<GameObject> childsObj = new List<GameObject>();
		[SerializeField]
		[Header("動作有効")]
		private bool isEnable = true;
		[SerializeField]
		[Header("半透明動作")]
		private bool isAlpha;
		public void SetEnable(bool _isEnable)
		{
			isEnable = _isEnable;
			UnityEngine.Debug.Log("set:" + base.gameObject.name + " :" + isEnable.ToString());
		}
		private void Start()
		{
			material = GetComponent<MeshRenderer>().material;
			renderer = GetComponent<MeshRenderer>();
			if (isHideChild)
			{
				foreach (Transform item in base.transform)
				{
					childsObj.Add(item.gameObject);
				}
			}
		}
		public void Translucent()
		{
			if (!isEnable)
			{
				return;
			}
			if (isAlpha)
			{
				material.color = new Color(material.color.r, material.color.g, material.color.b, 0.5f);
			}
			else
			{
				renderer.enabled = false;
			}
			if (isHideChild)
			{
				for (int i = 0; i < childsObj.Count; i++)
				{
					childsObj[i].SetActive(value: false);
				}
			}
		}
		public void Reset()
		{
			if (isAlpha)
			{
				material.color = new Color(material.color.r, material.color.g, material.color.b, 1f);
			}
			else
			{
				renderer.enabled = true;
			}
			if (isHideChild)
			{
				for (int i = 0; i < childsObj.Count; i++)
				{
					childsObj[i].SetActive(value: true);
				}
			}
		}
	}
}
