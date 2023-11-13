using UnityEngine;
public class MorphingRace_MorphingTarget_Mouse : MorphingRace_MorphingTarget
{
	private struct Gate
	{
		public Transform startAnchor;
		public MeshRenderer[] arrayStartGateMesh;
		public Transform goalAnchor;
		public MeshRenderer[] arrayGoalGateMesh;
	}
	private Gate[] arrayGate;
	public override void Init()
	{
		base.Init();
		arrayGate = new Gate[SingletonCustom<GameSettingManager>.Instance.PlayerGroupList.Length];
	}
	public void BuildNavMesh(MorphingRace_GenerateObstacle _obstacleObj)
	{
		int aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
		for (int i = 0; i < _obstacleObj.GetArrayGenerateAnchor().Length; i++)
		{
			float num = Random.Range(MorphingRace_Define.CPU_MOUSE_NAV_MESH_OBSTACLE_ACTIVE_PROBABILITY[aiStrength] - 0.2f, MorphingRace_Define.CPU_MOUSE_NAV_MESH_OBSTACLE_ACTIVE_PROBABILITY[aiStrength] + 0.2f);
			if (Random.Range(0f, 1f) < num)
			{
				MorphingRace_MorphingTarget_Mouse_NavObstacle componentInChildren = _obstacleObj.GetArrayGenerateAnchor()[i].GetComponentInChildren<MorphingRace_MorphingTarget_Mouse_NavObstacle>();
				componentInChildren.SetNavMeshObstacleShuffle();
				componentInChildren.SetArrayNavMeshObjectActive(_isActive: true);
			}
		}
		MorphingRace_MorphingTarget_Mouse_Nav component = _obstacleObj.GetComponent<MorphingRace_MorphingTarget_Mouse_Nav>();
		component.SetArrayHideObjectActive(_isActive: true);
		component.BuildNavMesh();
		component.SetArrayHideObjectActive(_isActive: false);
	}
	public void SetGateInfo(int _playerNo, Transform _startAnchor, MeshRenderer[] _arrayStartGateMesh, Transform _goalAnchor, MeshRenderer[] _arrayGoalGateMesh)
	{
		arrayGate[_playerNo].startAnchor = _startAnchor;
		arrayGate[_playerNo].arrayStartGateMesh = _arrayStartGateMesh;
		arrayGate[_playerNo].goalAnchor = _goalAnchor;
		arrayGate[_playerNo].arrayGoalGateMesh = _arrayGoalGateMesh;
	}
	public bool CheckPassThroughStartAnchor(int _playerNo, Vector3 _pos)
	{
		if (arrayGate[_playerNo].startAnchor.position.z <= _pos.z)
		{
			return true;
		}
		return false;
	}
	public bool CheckPassThroughGoalAnchor(int _playerNo, Vector3 _pos)
	{
		if (arrayGate[_playerNo].goalAnchor.position.z <= _pos.z)
		{
			return true;
		}
		return false;
	}
	public void AlphaStartGate(int _playerNo)
	{
		AlphaGate(arrayGate[_playerNo].arrayStartGateMesh);
	}
	public void AlphaGoalGate(int _playerNo)
	{
		AlphaGate(arrayGate[_playerNo].arrayGoalGateMesh);
	}
	private void AlphaGate(MeshRenderer[] _arrayMeesh)
	{
		LeanTween.value(base.gameObject, 1f, 0.4f, 0.5f).setOnUpdate(delegate(float _value)
		{
			for (int i = 0; i < _arrayMeesh.Length; i++)
			{
				Color color = _arrayMeesh[i].material.color;
				color.a = _value;
				_arrayMeesh[i].material.color = color;
			}
		});
	}
}
