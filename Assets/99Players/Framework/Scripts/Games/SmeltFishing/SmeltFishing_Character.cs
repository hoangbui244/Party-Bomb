using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Extension;
public class SmeltFishing_Character : MonoBehaviour
{
	public enum State
	{
		None,
		Moving,
		Fishing
	}
	[SerializeField]
	[DisplayName("キャラクタ\u30fcの見た目")]
	private CharacterStyle characterStyle;
	[SerializeField]
	[DisplayName("移動")]
	private SmeltFishing_CharacterMovement movement;
	[SerializeField]
	[DisplayName("移動中のアクション")]
	private SmeltFishing_CharacterMovingAction movingAction;
	[SerializeField]
	[DisplayName("釣り中のアクション")]
	private SmeltFishing_CharacterFishingAction fishingAction;
	[SerializeField]
	[DisplayName("プレイヤ\u30fcのSE")]
	private SmeltFishing_CharacterSfx sfx;
	[SerializeField]
	private NavMeshAgent agent;
	[SerializeField]
	[Header("バケツを置くアンカ\u30fc")]
	private Transform bucketAnchor;
	private SmeltFishing_CharacterRenderer renderer;
	private SmeltFishing_CharacterCamera camera;
	private SmeltFishing_FishingSpace fishingSpace;
	private State state;
	public int PlayerNo
	{
		get;
		private set;
	}
	public SmeltFishing_Definition.ControlUser ControlUser
	{
		get;
		private set;
	}
	public int Score
	{
		get;
		private set;
	}
	public bool IsMoving => state == State.Moving;
	public bool IsPlayer => ControlUser < SmeltFishing_Definition.ControlUser.Cpu1;
	public SmeltFishing_IcePlate UseIcePlate
	{
		get;
		private set;
	}
	public SmeltFishing_CharacterMovement Movement => movement;
	public SmeltFishing_CharacterCamera Camera => camera;
	public void Init(int no, int user)
	{
		PlayerNo = no;
		ControlUser = (SmeltFishing_Definition.ControlUser)user;
		Score = 0;
		state = State.None;
		characterStyle.SetGameStyle(GS_Define.GameType.BOMB_ROULETTE, user);
		InitComponents();
		agent.updatePosition = false;
		agent.updateRotation = false;
		SingletonCustom<SmeltFishing_UI>.Instance.SetControlModeMoving(PlayerNo);
		SingletonCustom<SmeltFishing_UI>.Instance.SetPlayerIcon(PlayerNo, user);
		SingletonCustom<SmeltFishing_UI>.Instance.SetCharaIcon(PlayerNo, user);
	}
	public void GameStart()
	{
		state = State.Moving;
		if (IsPlayer)
		{
			sfx.GameStart();
		}
	}
	public void GameEnd()
	{
		state = State.None;
		sfx.GameEnd();
		fishingAction.GameEnd();
		SingletonCustom<SmeltFishing_UI>.Instance.ForceHideAssistComment(PlayerNo);
		SingletonCustom<SmeltFishing_UI>.Instance.HideCurrentIfNeeded(PlayerNo);
		SingletonCustom<SmeltFishing_UI>.Instance.HideOperationInformation(PlayerNo);
	}
	public void UpdateMethod()
	{
		switch (state)
		{
		case State.Moving:
			UpdateMovement();
			UpdateMovingAction();
			camera.FollowCharacter();
			break;
		case State.Fishing:
			UpdateFishingAction();
			UpdateFishingSpace();
			break;
		}
		UpdateSfx();
	}
	public void FixedUpdateMethod()
	{
		State state = this.state;
		if (state == State.Moving)
		{
			FixedUpdateMovement();
		}
	}
	public void SitDown(SmeltFishing_IcePlate icePlate)
	{
		if (state == State.Moving && !icePlate.IsUsing && !(UseIcePlate != null) && !camera.InTransition)
		{
			UseIcePlate = icePlate;
			TransitionToSitDown();
		}
	}
	public void StandUp()
	{
		if (state == State.Fishing && !(UseIcePlate == null) && UseIcePlate.IsUsing && !camera.InTransition)
		{
			TransitionToStandUp();
		}
	}
	public void AddScore(int addScore)
	{
		Score += addScore * 100;
		SingletonCustom<SmeltFishing_UI>.Instance.UpdateScore(PlayerNo, Score);
	}
	public State GetState()
	{
		return state;
	}
	public Transform GetBucketAnchor()
	{
		return bucketAnchor;
	}
	private void InitComponents()
	{
		renderer = GetComponent<SmeltFishing_CharacterRenderer>();
		camera = SingletonCustom<SmeltFishing_CameraRegistry>.Instance.GetCamera(PlayerNo);
		fishingSpace = SingletonCustom<SmeltFishing_FishingSpaceRegistry>.Instance.GetFishingSpace(PlayerNo);
		sfx.Init();
		renderer.Init(this);
		camera.Init(this);
		fishingSpace.Init(this);
		movement.Init(this);
		movingAction.Init(this);
		fishingAction.Init(this);
	}
	private void UpdateMovement()
	{
		movement.UpdateMethod();
		agent.nextPosition = base.transform.position;
	}
	private void UpdateMovingAction()
	{
		movingAction.UpdateMethod();
	}
	private void UpdateFishingAction()
	{
		fishingAction.UpdateMethod();
	}
	private void UpdateFishingSpace()
	{
		fishingSpace.UpdateMethod();
	}
	private void UpdateSfx()
	{
		sfx.UpdateMethod();
	}
	private void FixedUpdateMovement()
	{
		movement.FixedUpdateMethod();
	}
	private void TransitionToSitDown()
	{
		state = State.Fishing;
		UseIcePlate.Use(this);
		fishingSpace.SitDown(UseIcePlate);
		movement.SitDown(fishingSpace);
		camera.SetAsFixedCamera();
		LeanTween.delayedCall(base.gameObject, camera.ConfigTransitionDuration() * 0.9f, (Action)delegate
		{
			fishingSpace.ChangeChairLayer(SmeltFishing_Definition.LayerDefinition.Layers[PlayerNo]);
			renderer.ChangeBodyPartsLayer(SmeltFishing_Definition.LayerDefinition.Layers[PlayerNo]);
		});
		fishingAction.SetupFishingRod(UseIcePlate);
		if (IsPlayer)
		{
			SingletonCustom<SmeltFishing_UI>.Instance.SetControlModeCastLine(PlayerNo);
		}
	}
	private void TransitionToStandUp()
	{
		state = State.Moving;
		UseIcePlate.UnUse(this);
		UseIcePlate = null;
		fishingSpace.StandUp();
		movement.StandUp();
		camera.SetAsFollowCamera();
		LeanTween.delayedCall(base.gameObject, camera.ConfigTransitionDuration() * 0.1f, (Action)delegate
		{
			fishingSpace.ChangeChairLayer(SmeltFishing_Definition.LayerDefinition.DefaultLayer);
			renderer.ChangeBodyPartsLayer(SmeltFishing_Definition.LayerDefinition.DefaultLayer);
		});
		fishingAction.TakeInFishingRod();
		if (IsPlayer)
		{
			SingletonCustom<SmeltFishing_UI>.Instance.HideAssistComment(PlayerNo);
			SingletonCustom<SmeltFishing_UI>.Instance.SetControlModeMoving(PlayerNo);
			SingletonCustom<SmeltFishing_UI>.Instance.HideSitDownUI(PlayerNo);
		}
	}
}
