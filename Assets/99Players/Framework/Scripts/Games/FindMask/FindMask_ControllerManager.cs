using UnityEngine;
public class FindMask_ControllerManager : SingletonCustom<FindMask_ControllerManager>
{
	[SerializeField]
	[Header("プレイヤ\u30fcが操作するコントロ\u30fcラ\u30fc")]
	private FindMask_CharacterController playerCharacterController;
	[SerializeField]
	[Header("CPUが操作するコントロ\u30fcラ\u30fc")]
	private FindMask_CharacterControllerAI cpuCharacterController;
	public void Init()
	{
		cpuCharacterController.Init();
	}
	public void UpdateMethod()
	{
		if (SingletonCustom<FindMask_GameManager>.Instance.TurnPlayerNo < 4)
		{
			playerCharacterController.UpdateMethod();
		}
		else
		{
			cpuCharacterController.UpdateMethod();
		}
	}
	public void AddFindPairCount()
	{
		cpuCharacterController.AddFindPairCount();
	}
	public void ResetFindPairCount()
	{
		cpuCharacterController.ResetFindPairCount();
	}
	public void RecordLastSelectMaskNo(int _firstMaskNo, int _secondMsakNo)
	{
		cpuCharacterController.RecordLastSelectMaskNo(_firstMaskNo, _secondMsakNo);
	}
}
