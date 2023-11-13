using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_StageObject : DecoratedMonoBehaviour
{
	private const string PlayerTag = "Player";
	private static readonly List<Transform> Pool = new List<Transform>();
	protected bool IsActive
	{
		get;
		private set;
	}
	protected FlyingSquirrelRace_Stage Owner
	{
		get;
		private set;
	}
	protected FlyingSquirrelRace_StagePattern Parent
	{
		get;
		private set;
	}
	public void Initialize(FlyingSquirrelRace_Stage owner, FlyingSquirrelRace_StagePattern parent, int layer)
	{
		Owner = owner;
		Parent = parent;
		IsActive = false;
		SetLayer(layer);
		OnInitialize();
	}
	public void Activate()
	{
		IsActive = true;
	}
	public void Deactivate()
	{
		IsActive = false;
	}
	public void FixedUpdateMethod(float speed)
	{
		OnFixedUpdateMethod(speed);
	}
	protected virtual void OnInitialize()
	{
	}
	protected virtual void OnFixedUpdateMethod(float speed)
	{
	}
	private void SetLayer(int layer)
	{
		Pool.Clear();
		base.transform.GetAllChildren(Pool, recursive: true);
		base.gameObject.layer = layer;
		foreach (Transform item in Pool)
		{
			item.gameObject.layer = layer;
		}
		Pool.Clear();
	}
	protected bool TryGetPlayer(Collider other, out FlyingSquirrelRace_Player player)
	{
		if (!other.CompareTag("Player"))
		{
			player = null;
			return false;
		}
		player = other.GetComponent<FlyingSquirrelRace_Player>();
		return player != null;
	}
}
