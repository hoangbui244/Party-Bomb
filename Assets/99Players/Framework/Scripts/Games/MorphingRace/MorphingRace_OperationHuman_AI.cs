using UnityEngine;
public class MorphingRace_OperationHuman_AI : MorphingRace_OperationCharacter_AI
{
	private MorphingRace_OperationHuman human;
	public void Init(MorphingRace_OperationHuman _human)
	{
		human = _human;
		Init(human.GetPlayer());
	}
	public override void UpdateMethod()
	{
		SetInput();
		SetMove();
		human.Move();
	}
	protected override void SetMove()
	{
		player.SetMoveDir(Vector3.forward);
	}
	protected override void MoveInput()
	{
		human.MoveInput();
	}
}
