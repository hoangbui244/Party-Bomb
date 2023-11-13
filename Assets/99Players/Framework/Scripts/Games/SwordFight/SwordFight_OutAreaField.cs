using UnityEngine;
public class SwordFight_OutAreaField : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (SingletonCustom<SwordFight_MainGameManager>.Instance.GetGameState() == SwordFight_MainGameManager.GameState.DURING_GAME && other.gameObject.GetComponentInParent<SwordFight_CharacterScript>() != null && other.gameObject.GetComponentInParent<SwordFight_CharacterScript>().GetActionState() != SwordFight_CharacterScript.ActionState.DEATH)
		{
			other.gameObject.GetComponentInParent<SwordFight_CharacterScript>().DeathAnimation();
		}
	}
}
