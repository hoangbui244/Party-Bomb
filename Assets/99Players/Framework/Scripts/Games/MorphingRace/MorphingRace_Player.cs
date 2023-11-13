using GamepadInput;
using System;
using UnityEngine;
public class MorphingRace_Player : MonoBehaviour {
    private MorphingRace_OperationCharacter currentOperationCharacter;
    [SerializeField]
    [Header("操作するキャラ配列")]
    private MorphingRace_OperationCharacter[] arrayOperationCharacter;
    [SerializeField]
    [Header("キャラクラス")]
    private MorphingRace_Character character;
    protected Rigidbody rigid;
    private int playerNo;
    private MorphingRace_Define.UserType userType;
    private int npadId;
    private bool isGoal;
    private float goalTime;
    private float inputInterval;
    private float beforeInputTime;
    private bool isOnceInput;
    private float inputContinueTime;
    private Vector3 moveDir;
    [SerializeField]
    [Header("変身エフェクト")]
    private ParticleSystem morphingEffect;
    [SerializeField]
    [Header("プレイヤ\u30fcの色にする変身エフェクト")]
    private ParticleSystem morphingEffect_ColorChange;
    [SerializeField]
    [Header("水しぶきエフェクト")]
    private ParticleSystem waterSplashEffect;
    private bool isCantOperation;
    private bool isMorphing;
    private MorphingRace_MorphingTarget_Mouse morphingTargetMouse;
    private MorphingRace_MorphingTarget_Eagle morphingTargetEagle;
    private MorphingRace_MorphingTarget_Fish morphingTargetFish;
    private MorphingRace_MorphingTarget_Dog morphingTargetDog;
    public void Init(int _playerNo, int _userType) {
        playerNo = _playerNo;
        userType = (MorphingRace_Define.UserType)_userType;
        character.Init(this);
        character.SetStyle(_userType);
        character.SetLayer();
        rigid = GetComponent<Rigidbody>();
        for (int i = 0; i < arrayOperationCharacter.Length; i++) {
            arrayOperationCharacter[i].Init(this);
            arrayOperationCharacter[i].gameObject.SetActive(value: false);
        }
        SetMorphingCharacter(MorphingRace_FieldManager.TargetPrefType.Human);
        //??morphingEffect_ColorChange.main.startColor = SingletonCustom<MorphingRace_PlayerManager>.Instance.GetMorphingEffectColor(SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[(int)userType]);
    }
    public void UpdateMethod() {
        if (!GetIsCpu()) {
            npadId = ((!SingletonCustom<JoyConManager>.Instance.IsSingleMode()) ? playerNo : 0);
        }
        if (!isCantOperation && !isMorphing) {
            currentOperationCharacter.UpdateMethod_Base();
        }
    }
    public void SetMorphingCharacter(MorphingRace_FieldManager.TargetPrefType _characterType) {
        if (currentOperationCharacter != null) {
            morphingEffect.Play();
            if (!GetIsCpu()) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_morphingrace_morphing");
                SingletonCustom<HidVibration>.Instance.SetCommonVibration(playerNo);
            }
            currentOperationCharacter.gameObject.SetActive(value: false);
            isMorphing = true;
            LeanTween.delayedCall(base.gameObject, 0.25f, (Action)delegate {
                isMorphing = false;
                if (!isGoal && !GetIsCpu()) {
                    if (_characterType == MorphingRace_FieldManager.TargetPrefType.Dog) {
                        SingletonCustom<MorphingRace_UIManager>.Instance.ChangeControllerUIType(playerNo, MorphingRace_UIManager.ControllerUIType.Dog);
                    } else {
                        SingletonCustom<MorphingRace_UIManager>.Instance.ChangeControllerUIType(playerNo, MorphingRace_UIManager.ControllerUIType.Common);
                    }
                }
            });
        }
        SetInputContinueTime(0f);
        currentOperationCharacter = arrayOperationCharacter[(int)_characterType];
        currentOperationCharacter.SetMorphingCharacter();
        currentOperationCharacter.StopMove();
        currentOperationCharacter.gameObject.SetActive(value: true);
        if (SingletonCustom<MorphingRace_GameManager>.Instance.GetIsViewCamera(playerNo)) {
            SingletonCustom<MorphingRace_CameraManager>.Instance.SetFollowOffset(playerNo, (int)currentOperationCharacter.GetCharacterType());
        }
    }
    public bool GetButtonDown_A() {
        return SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.A);
    }
    public bool GetButtonDown_B() {
        return SingletonCustom<JoyConManager>.Instance.GetButtonDown(npadId, SatGamePad.Button.B);
    }
    public bool GetButton_A() {
        return SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.A);
    }
    public void GameStartAnimation() {
        character.GameStartAnimation();
    }
    public Vector3 GetMoveDir() {
        return moveDir;
    }
    public void SetMoveDir(Vector3 _moveDir) {
        moveDir = _moveDir;
    }
    public void SetMove() {
        Vector3 stickDir = GetStickDir();
        if (currentOperationCharacter.GetCharacterType() == MorphingRace_FieldManager.TargetPrefType.Dog) {
            float num = Mathf.Atan2(stickDir.z, stickDir.x) * 57.29578f;
            float num2 = Mathf.Abs(num);
            UnityEngine.Debug.Log("angle " + num.ToString());
            UnityEngine.Debug.Log("angleAbs " + num2.ToString());
            if (40f < num2 && num2 < 140f && num <= 0f) {
                stickDir.z = -1f;
            } else {
                stickDir.z = 1f;
            }
        }
        SetMoveDir(stickDir);
    }
    public void SetInputInterval() {
        inputInterval = Time.time - beforeInputTime;
    }
    public float GetInputInterval() {
        return inputInterval;
    }
    public void SetBeforeInputTime() {
        beforeInputTime = Time.time;
    }
    public float GetInputContinueTime() {
        return inputContinueTime;
    }
    public void SetInputContinueTime(bool _isAdd) {
        if (_isAdd) {
            inputContinueTime += Time.deltaTime;
            if (inputContinueTime > SingletonCustom<MorphingRace_PlayerManager>.Instance.GetMaxInputContinueTime()) {
                inputContinueTime = SingletonCustom<MorphingRace_PlayerManager>.Instance.GetMaxInputContinueTime();
            }
        } else {
            inputContinueTime -= Time.deltaTime;
            if (inputContinueTime < 0f) {
                inputContinueTime = 0f;
            }
        }
    }
    public void SetInputContinueTime(float _inputContinueTime) {
        inputContinueTime = _inputContinueTime;
    }
    private Vector3 GetStickDir() {
        float num = 0f;
        float num2 = 0f;
        Vector3 vector = CalcManager.mVector3Zero;
        JoyConManager.AXIS_INPUT axisInput = SingletonCustom<JoyConManager>.Instance.GetAxisInput(npadId);
        num = axisInput.Stick_L.x;
        num2 = axisInput.Stick_L.y;
        if (true && Mathf.Abs(num) < 0.2f && Mathf.Abs(num2) < 0.2f) {
            num = 0f;
            num2 = 0f;
            if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Dpad_Right)) {
                num = 1f;
            } else if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Dpad_Left)) {
                num = -1f;
            }
            if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Dpad_Up)) {
                num2 = 1f;
            } else if (SingletonCustom<JoyConManager>.Instance.GetButton(npadId, SatGamePad.Button.Dpad_Down)) {
                num2 = -1f;
            }
        }
        switch (currentOperationCharacter.GetCharacterType()) {
            case MorphingRace_FieldManager.TargetPrefType.Human:
            case MorphingRace_FieldManager.TargetPrefType.Mouse:
                vector = new Vector3(num, 0f, 1f);
                break;
            case MorphingRace_FieldManager.TargetPrefType.Dog:
                vector = new Vector3(num, 0f, num2);
                break;
            case MorphingRace_FieldManager.TargetPrefType.Eagle:
            case MorphingRace_FieldManager.TargetPrefType.Fish:
                vector = new Vector3(num, num2, 1f);
                break;
        }
        if (vector.sqrMagnitude < 0.0400000028f) {
            return Vector3.zero;
        }
        return vector.normalized;
    }
    public MorphingRace_OperationCharacter GetCurrentOperationCharacter() {
        return currentOperationCharacter;
    }
    public MorphingRace_Character GetCharacter() {
        return character;
    }
    public int GetPlayerNo() {
        return playerNo;
    }
    public MorphingRace_Define.UserType GetUserType() {
        return userType;
    }
    public bool GetIsCpu() {
        return userType >= MorphingRace_Define.UserType.CPU_1;
    }
    public Rigidbody GetRigidbody() {
        return rigid;
    }
    public bool GetIsGoal() {
        return isGoal;
    }
    public void SetGoal() {
        SetGoal(SingletonCustom<MorphingRace_GameManager>.Instance.GetGameTime());
        int rank = SingletonCustom<MorphingRace_GameManager>.Instance.GetRank(playerNo);
        SingletonCustom<MorphingRace_GameManager>.Instance.SetIsGoal(playerNo, (int)userType, rank);
        character.GoalRankAnimation(SingletonCustom<MorphingRace_CharacterManager>.Instance.GetGoalAnimationTime(), rank);
    }
    public float GetGoalTime() {
        return goalTime;
    }
    public void SetGoal(float _time) {
        isGoal = true;
        goalTime = _time;
    }
    public void SetAutoGoalTime() {
        UnityEngine.Debug.Log("<color=red>CPUの自動タイムを設定</color>");
        SingletonCustom<MorphingRace_FieldManager>.Instance.SetFowardDistance(playerNo, base.transform.position);
        float time = SingletonCustom<MorphingRace_FieldManager>.Instance.GetDistanceToGoal(playerNo) / SingletonCustom<MorphingRace_FieldManager>.Instance.GetFowardDistance(playerNo) * SingletonCustom<MorphingRace_GameManager>.Instance.GetGameTime();
        time = CalcManager.ConvertDecimalSecond(time);
        UnityEngine.Debug.Log("（下２桁切り捨て）_playerNo : " + playerNo.ToString() + " ゴ\u30fcルタイム " + time.ToString());
        SetGoal(time);
    }
    public bool GetIsOnceInput() {
        return isOnceInput;
    }
    public void SetIsOnceInput() {
        isOnceInput = true;
        LeanTween.delayedCall(base.gameObject, 2f, (Action)delegate {
            SingletonCustom<MorphingRace_UIManager>.Instance.SetControllerBalloonActive(playerNo, MorphingRace_UIManager.ControllerUIType.Common, _isFade: true, _isActive: false);
        });
    }
    public MorphingRace_MorphingTarget_Mouse GetMorphingTarget_Mouse() {
        return morphingTargetMouse;
    }
    public MorphingRace_MorphingTarget_Eagle GetMorphingTargetEagle() {
        return morphingTargetEagle;
    }
    public MorphingRace_MorphingTarget_Fish GetMorphingTargetFish() {
        return morphingTargetFish;
    }
    public MorphingRace_MorphingTarget_Dog GetMorphingTargetDog() {
        return morphingTargetDog;
    }
    private void OnTriggerEnter(Collider other) {
        if (SingletonCustom<MorphingRace_GameManager>.Instance.GetIsGameEnd()) {
            return;
        }
        MorphingRace_MorphingTarget_Collider component = other.GetComponent<MorphingRace_MorphingTarget_Collider>();
        if (component != null) {
            component.HideMorphingPoint();
            currentOperationCharacter.StopMove();
            MorphingRace_MorphingTarget morphingTarget = component.GetMorphingTarget();
            if (other.tag == "Goal") {
                SetGoal();
                MorphingHuman(morphingTarget);
                Vector3 position = ((MorphingRace_MorphingTarget_Goal)morphingTarget).GetAfterGoalMoveAnchor(playerNo).position;
                position.x = base.transform.position.x;
                Vector3 vec = (position - base.transform.position).normalized;
                LeanTween.move(base.gameObject, position, 1f).setOnUpdate(delegate (float _value) {
                    currentOperationCharacter.MoveAfterGoal(vec, _value);
                }).setEaseOutCubic();
                if (SingletonCustom<MorphingRace_GameManager>.Instance.GetIsViewCamera(playerNo)) {
                    LeanTween.delayedCall(base.gameObject, 1.5f, (Action)delegate {
                        SingletonCustom<MorphingRace_CameraManager>.Instance.AfterGoalAnimation(playerNo);
                    });
                }
                return;
            }
            if (other.tag == "CheckPoint") {
                UnityEngine.Debug.Log("変身前");
                switch (component.GetMorphingPoint().GetMorphingTargetType()) {
                    case MorphingRace_FieldManager.TargetPrefType.Mouse:
                        MorphingMouse(morphingTarget);
                        break;
                    case MorphingRace_FieldManager.TargetPrefType.Eagle:
                        MorphingEagle(morphingTarget);
                        break;
                    case MorphingRace_FieldManager.TargetPrefType.Fish:
                        MorphingFish(morphingTarget);
                        break;
                    case MorphingRace_FieldManager.TargetPrefType.Dog:
                        MorphingDog(morphingTarget);
                        break;
                }
            }
        }
        if (!isMorphing && other.tag == "Water") {
            InOutWater();
        }
    }
    private void MorphingHuman(MorphingRace_MorphingTarget _morphingTarget) {
        if (currentOperationCharacter.GetCharacterType() == MorphingRace_FieldManager.TargetPrefType.Dog) {
            morphingTargetDog = null;
            SetMorphingCharacter(MorphingRace_FieldManager.TargetPrefType.Human);
            if (GetIsCpu()) {
                currentOperationCharacter.MorphingInit();
            }
        }
    }
    private void MorphingMouse(MorphingRace_MorphingTarget _morphingTarget) {
        if (currentOperationCharacter.GetCharacterType() == MorphingRace_FieldManager.TargetPrefType.Human) {
            character.ResetAnimation(0f);
            morphingTargetMouse = (MorphingRace_MorphingTarget_Mouse)_morphingTarget;
            SetMorphingCharacter(MorphingRace_FieldManager.TargetPrefType.Mouse);
            if (GetIsCpu()) {
                currentOperationCharacter.MorphingInit();
            }
        }
    }
    private void MorphingEagle(MorphingRace_MorphingTarget _morphingTarget) {
        if (currentOperationCharacter.GetCharacterType() == MorphingRace_FieldManager.TargetPrefType.Mouse) {
            morphingTargetMouse = null;
            morphingTargetEagle = (MorphingRace_MorphingTarget_Eagle)_morphingTarget;
            SetMorphingCharacter(MorphingRace_FieldManager.TargetPrefType.Eagle);
            currentOperationCharacter.MorphingInit();
        }
    }
    private void MorphingFish(MorphingRace_MorphingTarget _morphingTarget) {
        isCantOperation = true;
        rigid.isKinematic = true;
        if (currentOperationCharacter.GetCharacterType() == MorphingRace_FieldManager.TargetPrefType.Eagle) {
            morphingTargetFish = (MorphingRace_MorphingTarget_Fish)_morphingTarget;
            morphingTargetFish.GetCantBackColliderActive(playerNo, _isActive: false);
            Vector3 position = morphingTargetFish.GetInWaterJumpAnchor(playerNo).position;
            position.x = base.transform.position.x;
            Vector3 position2 = morphingTargetFish.GetInWaterStartAnchor(playerNo).position;
            position2.x = base.transform.position.x;
            MorphingFishMove(position, position2, SingletonCustom<MorphingRace_CharacterManager>.Instance.GetMorphingFishJumpAnimationTime(), delegate {
                isCantOperation = false;
                rigid.isKinematic = false;
                rigid.useGravity = false;
                morphingTargetFish.SetLaneWaterUpColliderActive(playerNo, _isActive: true);
            });
        }
    }
    private void MorphingDog(MorphingRace_MorphingTarget _morphingTarget) {
        switch (currentOperationCharacter.GetCharacterType()) {
            case MorphingRace_FieldManager.TargetPrefType.Fish: {
                    isCantOperation = true;
                    rigid.isKinematic = true;
                    morphingTargetFish.SetLaneWaterUpColliderActive(playerNo, _isActive: false);
                    Vector3 position = morphingTargetFish.GetOutWaterJumpAnchor(playerNo).position;
                    position.x = base.transform.position.x;
                    Vector3 position2 = morphingTargetFish.GetOutWaterLandingAnchor(playerNo).position;
                    position2.x = base.transform.position.x;
                    MorphingFishMove(position, position2, SingletonCustom<MorphingRace_CharacterManager>.Instance.GetMorphingFishJumpAnimationTime(), delegate {
                        isCantOperation = false;
                        rigid.isKinematic = false;
                        rigid.useGravity = true;
                        morphingTargetFish.GetCantBackColliderActive(playerNo, _isActive: true);
                        morphingTargetFish = null;
                    });
                    break;
                }
            case MorphingRace_FieldManager.TargetPrefType.Dog:
                morphingTargetDog = (MorphingRace_MorphingTarget_Dog)_morphingTarget;
                currentOperationCharacter.MorphingInit();
                break;
        }
    }
    private void MorphingFishMove(Vector3 _movePos, Vector3 _endPos, float _animTime, Action _callBack = null) {
        Vector3 vec = (_movePos - base.transform.position).normalized;
        LeanTween.move(base.gameObject, _movePos, _animTime).setOnUpdate((Action<float>)delegate {
            currentOperationCharacter.InOutWaterRot(vec);
        });
        LeanTween.delayedCall(base.gameObject, _animTime, (Action)delegate {
            vec = (_endPos - base.transform.position).normalized;
            LeanTween.move(base.gameObject, _endPos, _animTime).setOnUpdate((Action<float>)delegate {
                currentOperationCharacter.InOutWaterRot(vec);
            });
            LeanTween.delayedCall(base.gameObject, _animTime, (Action)delegate {
                currentOperationCharacter.InOutWaterRot(Vector3.forward, _immediate: true);
                if (_callBack != null) {
                    _callBack();
                }
            });
        });
    }
    private void InOutWater() {
        waterSplashEffect.Play();
        switch (currentOperationCharacter.GetCharacterType()) {
            case MorphingRace_FieldManager.TargetPrefType.Eagle:
                morphingTargetEagle = null;
                if (!GetIsCpu()) {
                    SingletonCustom<AudioManager>.Instance.SePlay("se_morphingrace_in_water");
                }
                SetMorphingCharacter(MorphingRace_FieldManager.TargetPrefType.Fish);
                if (GetIsCpu()) {
                    currentOperationCharacter.MorphingInit();
                }
                break;
            case MorphingRace_FieldManager.TargetPrefType.Fish:
                if (!GetIsCpu()) {
                    SingletonCustom<AudioManager>.Instance.SePlay("se_morphingrace_out_water");
                }
                SetMorphingCharacter(MorphingRace_FieldManager.TargetPrefType.Dog);
                ((MorphingRace_OperationDog)currentOperationCharacter).SetIsJumpping(_isJumpping: true);
                break;
        }
    }
}
