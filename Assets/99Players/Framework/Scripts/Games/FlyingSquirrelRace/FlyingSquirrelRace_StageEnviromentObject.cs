using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_StageEnviromentObject : FlyingSquirrelRace_StageObject
{
	[SerializeField]
	private bool enabledScroll;
	[SerializeField]
	private float scrollSpeed = 1f;
	[SerializeField]
	[Header("動作可能判定にするクラス")]
	private FlyingSquirrelRace_ObstacleObject_CallAction callAcition;
	private bool isAction;
	protected override void OnInitialize()
	{
		callAcition?.Initialize(SetIsAction);
	}
	private void Update()
	{
		if (isAction)
		{
			UpdateScroll();
		}
	}
	private void UpdateScroll()
	{
		if (enabledScroll)
		{
			base.transform.LocalPosition((Vector3 v) => v.X((float x) => x + (0f - scrollSpeed) * Time.deltaTime));
		}
	}
	public void SetIsAction()
	{
		isAction = true;
		callAcition.gameObject.SetActive(value: false);
	}
}
