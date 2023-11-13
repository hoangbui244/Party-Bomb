using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BeachSoccer {
    public class CharacterScript : MonoBehaviour {
        public enum ActionState {
            STANDARD,
            THROW_IN,
            CORNER_KICK,
            GOAL_KICK,
            KICK_OFF_STANDBY,
            SLIDING,
            DIVING_CATCH,
            HEADING,
            DIVING_HEAD,
            OVER_HEAD_KICK,
            JUMPING_VOLLEY,
            FREEZE,
            GOAL_PRODUCTION
        }
        public enum BodyPartsList {
            HEAD,
            BODY,
            HIP,
            SHOULDER_L,
            SHOULDER_R,
            ARM_L,
            ARM_R,
            LEG_L,
            LEG_R
        }
        [Serializable]
        public struct BodyParts {
            [SerializeField]
            [Header("体アンカ\u30fc")]
            public GameObject bodyAnchor;
            [SerializeField]
            [Header("スロ\u30fcインアンカ\u30fc")]
            public Transform throwInAnchor;
            public MeshRenderer[] rendererList;
            public Transform Parts(BodyPartsList _parts) {
                return rendererList[(int)_parts].transform;
            }
            public Transform Parts(int _parts) {
                return rendererList[_parts].transform;
            }
        }
        public delegate void AiActionMethod();
        public struct CharaParam {
            public int offence;
            public int defense;
            public int speed;
            public int stamina;
        }
        public class ThrowInData {
            public static readonly Vector3 ARM_ROT = new Vector3(30f, 180f, 180f);
            public static readonly float ARM_AFTER_ROT = -120f;
            public static readonly float ARM_HAND_ROT = 300f;
            public static readonly float UPPER_BODY_ROT = -17.24f;
            public static readonly float UPPER_BODY_AFTER_ROT = 10.24f;
        }
        public enum GoalProtect {
            WAIT,
            STEAL_BALL
        }
        [Serializable]
        public struct DebguShowData {
            public bool positionStopCheck;
            public bool deffenceArea;
            public bool coverArea;
            public bool ballNearDistance;
            public bool personalSpace;
            public bool moveTargetPos;
            public bool spacePos;
            public DebguShowData(bool _positionStopCheck, bool _deffenceArea, bool _coverArea, bool _ballNearDistance, bool _personalSpace, bool _moveTargetPos, bool _spacePos) {
                positionStopCheck = _positionStopCheck;
                deffenceArea = _deffenceArea;
                coverArea = _coverArea;
                ballNearDistance = _ballNearDistance;
                personalSpace = _personalSpace;
                moveTargetPos = _moveTargetPos;
                spacePos = _spacePos;
            }
        }
        public static readonly float PERSONAL_SPACE_IN = 3f;
        public static readonly float PERSONAL_SPACE_OUT = 4f;
        public static readonly float COVER_SPACE = 2.5f;
        private float PRESSURE_DISTANCE = 2f;
        private float SLIDING_DISTANCE = 200f;
        private float ADD_GRAVITY = 700f;
        private float WALK_SPEED = 0.6f;
        private float RUN_SPEED = 1f;
        public static readonly float NEAR_DISTANCE = 2f;
        private float SLIDING_INTERVAL = 1.25f;
        private float DIVING_CATCH_INTERVAL = 1f;
        private float HEADING_INTERVAL = 1.5f;
        private float DIVING_HEAD_INTERVAL = 1.5f;
        private float OVER_HEAD_KICK_INTERVAL = 1.5f;
        private float JUMPING_VOLLEY_INTERVAL = 1.5f;
        private float STEAL_RIGOR_TIME = 0.2f;
        private float GOAL_KICK_CHARGE_TIME = 1f;
        private float CORNER_KICK_CHARGE_TIME = 1f;
        private float[] STAMINA_RECOVRY_MAG = new float[3]
        {
            0.25f,
            0.5f,
            0.75f
        };
        private Vector3[] calcVec = new Vector3[2];
        private Vector3 rot;
        private ActionState actionState;
        [SerializeField]
        [Header("オブジェクト")]
        private Transform obj;
        private Rigidbody rigid;
        private Vector3 prevPos;
        private Vector3 nowPos;
        private Vector3 kickOffStandbyPos;
        private Vector3 returnBenchPos;
        private Vector3 movePos;
        private bool isMvePos;
        private float actionInterval;
        private float controlInterval;
        private bool isPassFromFriend;
        private bool isBallCatch;
        [SerializeField]
        [Header("走るエフェクト")]
        private ParticleSystem runEffect;
        private ParticleSystem slidingEffect;
        [SerializeField]
        [Header("汗エフェクト")]
        private ParticleSystem sweatEffect;
        private int[] SlidingEffectNo = new int[19]
        {
            0,
            0,
            0,
            1,
            2,
            3,
            0,
            2,
            2,
            0,
            2,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        };
        [SerializeField]
        [Header("吹き飛ばしエフェクト")]
        private ParticleSystem breakEffect;
        [SerializeField]
        [Header("スライディングエフェクト")]
        private ParticleSystem[] arraySlidingEffect;
        [SerializeField]
        [Header("スタイル")]
        private CharacterStyle style;
        [SerializeField]
        [Header("体のパ\u30fcツ")]
        private BodyParts bodyParts;
        private float runAnimationSpeed = 20f;
        private float runAnimationTime;
        private float kickAnimationTime;
        private int playSeRunCnt;
        private float runSeInterval;
        private bool isChangeAnimationNeutral;
        private float actionChangeInterval;
        private float movePosChangeInterval;
        private List<AiActionMethod> aiActionMethod = new List<AiActionMethod>();
        private float aiActionTime;
        private float actionLagTime;
        private bool isInitAiAction;
        private bool isCallAiAction;
        private RaycastHit raycastHit;
        private CharacterScript frontCharacter;
        private GameObject frontObj;
        private Vector3 movePlayPos;
        private float movePlayPosUpdateTime;
        private float passCheckTime;
        private float shootCheckTime;
        private float ignoreObstacleTime;
        private float joyJumpInterval;
        private int teamNo;
        private int opponentTeamNo;
        private int playerNo;
        private int charaNo;
        private new string name;
        private int uniformNumber;
        private float throwPower;
        private float kickPower;
        private float staminaValue;
        private float staminaValueMax;
        private float staminaRecoveryTime;
        private float moveSpeed = 233.75f;
        private float moveSpeedMax = 0.1375f;
        private Transform formationAnchor;
        private Vector3 formationPos;
        private GameDataParams.PositionType positionType;
        private CapsuleCollider charaCollider;
        private float charaBodySize;
        private float charaHeight;
        private StatusType charaParam;
        private float moveSpeedParamCorr = 1f;
        private float kickPowerCorr;
        private float throwInAnimationSpeed = 4f;
        private float throwInAnimationTime;
        private bool isBallThrow;
        private float throwInTime;
        private float cornerKickDelayTime;
        private float cornerKickTime;
        private float goalKickDelayTime;
        private float goalKickTime;
        private bool isDiving;
        private bool isBallKick;
        private float cursorRotSpeed = 0.5f;
        private float cursorRotTime;
        private Action setPlayMethod;
        private float STOP_CHECK_DISTANCE = 0.1f;
        private float STOP_CHECK_DISTANCE_ABOUT = 1f;
        private float AVOID_ROT = 15f;
        private float runTime;
        private Vector3 lookBallPos;
        private float ballSearchInterval;
        private string AiState;
        private float kickOffInterval;
        private List<Vector3> movePoslist = new List<Vector3>();
        private Vector3 shootVec;
        private Vector3 shootTarget;
        private float freezeTime;
        private float penaltyAreaStayTime;
        private float stealGiveUpTime;
        public GoalProtect goalProtect;
        public DebguShowData debguShowData = new DebguShowData(_positionStopCheck: false, _deffenceArea: false, _coverArea: false, _ballNearDistance: false, _personalSpace: false, _moveTargetPos: false, _spacePos: false);
        public bool IsPassFromFriend {
            get {
                return isPassFromFriend;
            }
            set {
                isPassFromFriend = value;
            }
        }
        public bool IsBallCatch {
            get {
                return isBallCatch;
            }
            set {
                isBallCatch = value;
            }
        }
        public float IgnoreObstacleTime {
            get {
                return ignoreObstacleTime;
            }
            set {
                ignoreObstacleTime = value;
            }
        }
        public int TeamNo {
            get {
                return teamNo;
            }
            set {
                teamNo = value;
            }
        }
        public int PlayerNo {
            get {
                return playerNo;
            }
            set {
                playerNo = value;
            }
        }
        public int CharaNo {
            get {
                return charaNo;
            }
            set {
                charaNo = value;
            }
        }
        private Transform OpponentGoal => FM.GetOpponentGoal(teamNo);
        public float DistanceBallFrom => MCM.GetDistanceFromBall(teamNo, charaNo);
        private MainCharacterManager MCM => SingletonCustom<MainCharacterManager>.Instance;
        private CpuArtificialIntelligence CpuAi => SingletonCustom<MainCharacterManager>.Instance.CpuAi;
        private FieldManager FM => SingletonCustom<FieldManager>.Instance;
        private BallManager BM => SingletonCustom<BallManager>.Instance;
        private BallScript Ball => SingletonCustom<BallManager>.Instance.GetBall();
        public void Init(int _teamNo, int _charaNo, Transform _formationAnchor, GameDataParams.PositionType _positionType) {
            if (GameSaveData.GetSelectMultiPlayerList()[_teamNo].Count > _charaNo) {
                style.SetGameStyle(GS_Define.GameType.ATTACK_BALL, GameSaveData.GetSelectMultiPlayerList()[_teamNo][_charaNo]);
            }
            rigid = GetComponent<Rigidbody>();
            charaCollider = obj.GetComponent<CapsuleCollider>();
            charaBodySize = charaCollider.radius * obj.localScale.x * 2f;
            charaHeight = charaCollider.height * obj.localScale.y;
            base.gameObject.name = base.gameObject.name + _teamNo.ToString() + "_" + _charaNo.ToString();
            teamNo = _teamNo;
            opponentTeamNo = ((teamNo == 0) ? 1 : 0);
            charaNo = _charaNo;
            playerNo = -1;
            runAnimationTime = 0f;
            playSeRunCnt = 0;
            formationAnchor = _formationAnchor;
            positionType = _positionType;
            base.transform.localPosition = formationAnchor.localPosition;
            nowPos = (prevPos = base.transform.position);
            base.transform.rotation = formationAnchor.rotation;
            if (positionType == GameDataParams.PositionType.GK) {
                obj.gameObject.layer = GameDataParams.ConvertLayerNo("Keeper");
            } else {
                obj.gameObject.layer = GameDataParams.ConvertLayerNo("Character");
            }
            charaParam = SchoolData.GetCommonCharaStatus(teamNo, charaNo);
            name = SchoolData.GetCommonCharacterName(teamNo, charaNo);
            uniformNumber = SchoolData.GetCommonCharacterUniformNumber(teamNo, charaNo);
            for (int i = 0; i < bodyParts.rendererList.Length; i++) {
                if (i != 0) {
                    bodyParts.rendererList[i].material = SingletonCustom<UniformListManager>.Instance.GetMaterial(teamNo, positionType == GameDataParams.PositionType.GK);
                }
            }
            staminaValue = (staminaValueMax = 4 + 2 * (charaParam.stamina / 10));
            moveSpeedParamCorr = 1f + (float)charaParam.speed * 0.05f;
            moveSpeed *= moveSpeedParamCorr;
            moveSpeedMax *= moveSpeedParamCorr;
            kickPowerCorr = 0.75f + (float)charaParam.offense * 0.025f;
        }
        public void ResetPosData() {
            nowPos = (prevPos = base.transform.position);
        }
        public void ResetControlInterval() {
            controlInterval = 0f;
        }
        public void UpdateMethod() {
            prevPos = nowPos;
            nowPos = base.transform.position;
            if (kickAnimationTime > 0f) {
                kickAnimationTime -= Time.deltaTime;
                if (kickAnimationTime <= 0f) {
                    ResetKickAnimation();
                }
                return;
            }
            switch (actionState) {
                case ActionState.THROW_IN:
                    ThrowInAction();
                    break;
                case ActionState.CORNER_KICK:
                    CornerKickAction();
                    break;
                case ActionState.GOAL_KICK:
                    GoalKickAction();
                    break;
                case ActionState.SLIDING:
                    SlidingAction();
                    break;
                case ActionState.DIVING_CATCH:
                    DivingCatchAction();
                    break;
                case ActionState.HEADING:
                    HeadingAction();
                    break;
                case ActionState.DIVING_HEAD:
                    DivingHeadAction();
                    break;
                case ActionState.OVER_HEAD_KICK:
                    OverHeadKickAction();
                    break;
                case ActionState.JUMPING_VOLLEY:
                    JumpingVolleyAction();
                    break;
                case ActionState.FREEZE:
                    if (GetPos(_isLocal: true).y > GetCharaHeight() * 0.6f) {
                        rigid.AddForce(0f, 0f - ADD_GRAVITY, 0f);
                    }
                    break;
                default:
                    MoveAnimation();
                    if (GetPos(_isLocal: true).y > GetCharaHeight() * 0.6f) {
                        rigid.AddForce(0f, 0f - ADD_GRAVITY, 0f);
                    }
                    break;
            }
            if (staminaRecoveryTime <= 0f) {
                staminaValue += Time.deltaTime * STAMINA_RECOVRY_MAG[2];
            } else {
                staminaRecoveryTime -= Time.deltaTime;
            }
            ParticleSystem.MainModule main = runEffect.main;
            main.loop = (SingletonCustom<MainCharacterManager>.Instance.CheckHaveBall(this) && GetMoveVec().magnitude >= 0.01f);
            if (main.loop && !runEffect.isPlaying) {
                runEffect.Play();
            }
            controlInterval += Time.deltaTime;
            runSeInterval -= Time.deltaTime;
        }
        public void Move(Vector3 _moveDir, float _moveSpeed, float _speedMag = 1f) {
            if (kickAnimationTime > 0f) {
                return;
            }
            Vector3 vector = _moveDir;
            if (_moveSpeed <= WALK_SPEED * 0.5f) {
                _moveSpeed = WALK_SPEED * 0.5f;
                if (staminaValue <= staminaValueMax) {
                    staminaValue += Time.deltaTime * STAMINA_RECOVRY_MAG[1];
                } else {
                    staminaValue = staminaValueMax;
                }
            } else if (_moveSpeed <= WALK_SPEED) {
                _moveSpeed = WALK_SPEED;
                if (staminaValue <= staminaValueMax) {
                    staminaValue += Time.deltaTime * STAMINA_RECOVRY_MAG[0];
                } else {
                    staminaValue = staminaValueMax;
                }
            } else if ((SingletonCustom<MainCharacterManager>.Instance.CheckControlChara(this) && MCM.CheckHaveBall(this)) || (!SingletonCustom<MainCharacterManager>.Instance.IsPlayer(playerNo) && MCM.CheckHaveBall(this))) {
                if (staminaValue > 0f) {
                    staminaValue -= Time.deltaTime;
                    if (staminaValue < 0f) {
                        staminaValue = 0f;
                    }
                }
            } else {
                staminaValue = staminaValueMax;
            }
            ParticleSystem.MainModule main = sweatEffect.main;
            if (staminaValue <= 0f && _moveSpeed >= WALK_SPEED) {
                main.loop = true;
                if (main.loop && !sweatEffect.isPlaying) {
                    sweatEffect.Play();
                }
                _moveSpeed = WALK_SPEED * 0.75f;
            } else {
                main.loop = false;
            }
            staminaRecoveryTime = 0.1f;
            if (CheckPositionType(GameDataParams.PositionType.GK) && MCM.CheckHaveBall(this)) {
                if (ConvertLocalVec(_moveDir).z < 0f && GetPos(_isLocal: true).z < GetCharaBodySize() * 2f) {
                    vector.z = 0f;
                }
                if (isBallCatch) {
                    if ((ConvertLocalVec(_moveDir).x < 0f && GetPos(_isLocal: true).x < (0f - FM.GetFieldData().penaltyAreaSize.x) * 0.5f + GetCharaBodySize()) || (ConvertLocalVec(_moveDir).x > 0f && GetPos(_isLocal: true).x > FM.GetFieldData().penaltyAreaSize.x * 0.5f - GetCharaBodySize())) {
                        vector.x = 0f;
                    }
                    if (ConvertLocalVec(_moveDir).z > 0f && GetPos(_isLocal: true).z > FM.GetFieldData().penaltyAreaSize.z - GetCharaBodySize()) {
                        vector.z = 0f;
                    }
                }
            }
            vector *= moveSpeed * _moveSpeed * _speedMag;
            rigid.AddForce(vector, ForceMode.Acceleration);
            if (rigid.velocity.magnitude > moveSpeedMax * _speedMag) {
                rigid.velocity = rigid.velocity.normalized * moveSpeedMax * _speedMag;
            }
            MoveRot(_moveDir);
            MoveAnimation();
        }
        private void MoveRot(Vector3 _moveDir, bool _immediate = false) {
            if (actionState != ActionState.SLIDING && actionState != ActionState.DIVING_CATCH && CalcManager.Length(nowPos, prevPos) > 0.01f) {
                calcVec[0] = _moveDir;
                rot.x = 0f;
                rot.y = CalcManager.Rot(calcVec[0], CalcManager.AXIS.Y);
                rot.z = 0f;
                if (_immediate) {
                    rigid.MoveRotation(Quaternion.Euler(rot));
                } else {
                    rigid.MoveRotation(Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(rot), 20f * Time.deltaTime));
                }
            }
        }
        public void KickAnimation() {
            bodyParts.Parts(BodyPartsList.HIP).SetLocalPosition(-0.2f, 0.549f, -0.25f);
            bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(330f, 0f, 25f);
            bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(25f, 0f, 345f);
            bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAnglesX(0f);
            bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAnglesX(285f);
            bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAnglesX(80f);
            bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAnglesX(280f);
            bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAnglesX(0f);
            bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAnglesX(0f);
            kickAnimationTime = 0.2f;
        }
        private void ResetKickAnimation() {
            bodyParts.Parts(BodyPartsList.HIP).SetLocalPosition(0f, 0.549f, 0f);
            bodyParts.Parts(BodyPartsList.HIP).SetLocalEulerAngles(0f, 0f, 0f);
            bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(0f, 0f, 0f);
            bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAnglesX(0f);
            bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAnglesX(0f);
            bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAnglesX(0f);
            bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAnglesX(0f);
            bodyParts.Parts(BodyPartsList.ARM_R).SetLocalEulerAnglesX(270f);
            bodyParts.Parts(BodyPartsList.ARM_L).SetLocalEulerAnglesX(270f);
        }
        private void MoveAnimation() {
            if (kickAnimationTime > 0f) {
                return;
            }
            if (actionState != ActionState.THROW_IN) {
                bodyParts.Parts(BodyPartsList.SHOULDER_L).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * 30f);
                bodyParts.Parts(BodyPartsList.SHOULDER_R).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * -30f);
            }
            bodyParts.Parts(BodyPartsList.LEG_L).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * 30f);
            bodyParts.Parts(BodyPartsList.LEG_R).SetLocalEulerAnglesX(Mathf.Sin(runAnimationTime * (float)Math.PI * 2f) * -30f);
            if (CalcManager.Length(nowPos, prevPos) > 0.01f) {
                runAnimationTime += CalcManager.Length(nowPos, prevPos) * runAnimationSpeed * Time.deltaTime;
                if (runAnimationTime >= (float)playSeRunCnt * 0.5f) {
                    playSeRunCnt++;
                    PlaySeRun();
                }
                if (runAnimationTime >= 1f) {
                    runAnimationTime = 0f;
                    playSeRunCnt = 1;
                }
                isChangeAnimationNeutral = false;
            } else {
                if (isChangeAnimationNeutral) {
                    return;
                }
                if (runAnimationTime <= 0.25f) {
                    runAnimationTime -= 1f * Time.deltaTime;
                    if (runAnimationTime <= 0f) {
                        runAnimationTime = 0f;
                        isChangeAnimationNeutral = true;
                    }
                } else if (runAnimationTime <= 0.5f) {
                    runAnimationTime += 1f * Time.deltaTime;
                    if (runAnimationTime >= 0.5f) {
                        runAnimationTime = 0.5f;
                        isChangeAnimationNeutral = true;
                    }
                } else if (runAnimationTime <= 0.75f) {
                    runAnimationTime -= 1f * Time.deltaTime;
                    if (runAnimationTime <= 0.5f) {
                        runAnimationTime = 0.5f;
                        isChangeAnimationNeutral = true;
                    }
                } else {
                    runAnimationTime += 1f * Time.deltaTime;
                    if (runAnimationTime >= 1f) {
                        runAnimationTime = 1f;
                        isChangeAnimationNeutral = true;
                    }
                }
            }
        }
        public void SlidingAction() {
            actionInterval -= Time.deltaTime;
            if (actionInterval <= 0f) {
                ResetObjPosition();
                actionState = ActionState.STANDARD;
            }
        }
        public void Sliding() {
            if (CheckActionState(ActionState.STANDARD)) {
                obj.SetLocalPosition(0f, 0.145f, 0.3f);
                obj.SetLocalEulerAngles(-40f, 70f, -60f);
                rigid.AddForce(base.transform.forward * SLIDING_DISTANCE, ForceMode.Impulse);
                actionInterval = SLIDING_INTERVAL;
                SingletonCustom<AudioManager>.Instance.SePlay("se_oni_sliding");
                if (slidingEffect == null) {
                    slidingEffect = UnityEngine.Object.Instantiate(arraySlidingEffect[0], base.transform.position, Quaternion.identity, base.transform);
                }
                slidingEffect.gameObject.SetActive(value: true);
                slidingEffect.Play();
                SetAction(AiSliding, _immediate: true, _forcibly: true);
                actionState = ActionState.SLIDING;
            }
        }
        public void DivingCatchAction() {
            actionInterval -= Time.deltaTime;
            if (actionInterval <= 0f) {
                ResetObjPosition();
                UnityEngine.Debug.Log("ダイビングキャッチ終了");
                actionState = ActionState.STANDARD;
            }
        }
        public void DivingCatch(Vector3 _ballPos) {
            StartCoroutine(_DivingCatch(_ballPos));
        }
        public IEnumerator _DivingCatch(Vector3 _ballPos) {
            Vector3 normalized = Ball.GetMoveVec().normalized;
            Vector3 shootPos = default(Vector3);
            Vector3 localShootPos;
            if (FM.ConvertLocalPos(BM.GetBallPos(), teamNo).z <= GetPos(_isLocal: true).z + GetCharaBodySize() * 0.5f + BM.GetBallSize() * 0.5f) {
                shootPos = _ballPos;
                UnityEngine.Debug.Log("チ\u30fcムロ\u30fcカル座標(ボ\u30fcル) = " + FM.ConvertLocalPos(shootPos, teamNo).ToString());
                localShootPos = FM.ConvertLocalPos(shootPos, teamNo) - GetPos(_isLocal: true);
                localShootPos.x *= 2f;
            } else {
                float num = Mathf.Abs(GetPos().z - _ballPos.z);
                shootPos.x = num * (normalized.x / Mathf.Abs(normalized.z));
                shootPos.y = num * (normalized.y / Mathf.Abs(normalized.z));
                shootPos.x += _ballPos.x;
                shootPos.y += _ballPos.y;
                shootPos.z = GetPos().z;
                UnityEngine.Debug.Log("チ\u30fcムロ\u30fcカル座標(ボ\u30fcル) = " + FM.ConvertLocalPos(shootPos, teamNo).ToString());
                localShootPos = FM.ConvertLocalPos(shootPos, teamNo) - GetPos(_isLocal: true);
            }
            string[] obj = new string[11]
            {
                "ballPos = ",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            };
            Vector3 vector = _ballPos;
            obj[1] = vector.ToString();
            obj[2] = Environment.NewLine;
            obj[3] = "shootVec = ";
            vector = normalized;
            obj[4] = vector.ToString();
            obj[5] = Environment.NewLine;
            obj[6] = "shootPos = ";
            vector = shootPos;
            obj[7] = vector.ToString();
            obj[8] = Environment.NewLine;
            obj[9] = "localShootPos = ";
            vector = localShootPos;
            obj[10] = vector.ToString();
            UnityEngine.Debug.Log(string.Concat(obj));
            if (BM.GetStateTime() <= 0.2f && BM.GetBallSpeed(_rigid: true) >= BallScript.FastBallBorder && UnityEngine.Random.Range(0, 100) < CpuAi.GetFreezePerWhenFastBall(teamNo) && Ball.CheckOpponentLastShootChara(teamNo) && Mathf.Abs(localShootPos.x) >= GetCharaBodySize() * 0.75f) {
                if (localShootPos.x > 0f) {
                    this.obj.SetLocalEulerAngles(0f, 0f, -90f);
                    this.obj.SetLocalPosition(-0.545f, 0.39f, 0f);
                } else {
                    this.obj.SetLocalEulerAngles(0f, 0f, 90f);
                    this.obj.SetLocalPosition(0.545f, 0.39f, 0f);
                }
                actionInterval = DIVING_CATCH_INTERVAL * 0.5f - CpuAi.GetKeeperFreezeTimeCorr(teamNo);
                isDiving = true;
                LookForward();
                actionState = ActionState.DIVING_CATCH;
                yield break;
            }
            actionState = ActionState.DIVING_CATCH;
            actionInterval = 0.5f;
            if ((!(Mathf.Abs(localShootPos.x) <= GetCharaBodySize() * 0.5f) || !(Mathf.Abs(localShootPos.y) <= GetCharaHeight())) && Ball.CheckOpponentLastShootChara(teamNo)) {
                if (BM.GetStateTime() <= 0.2f && BM.GetBallSpeed(_rigid: true) >= BallScript.FastBallBorder && UnityEngine.Random.Range(0, 100) < CpuAi.GetDivingDelayPerWhenFastBall(teamNo)) {
                    yield return new WaitForSeconds(0.2f);
                }
                if (UnityEngine.Random.Range(0, 100) < CpuAi.GetDivingDelayPer(teamNo)) {
                    yield return new WaitForSeconds(0.2f);
                }
            }
            if (Mathf.Abs(shootPos.x - FM.GetMyGoal(teamNo).position.x) <= FM.GetFieldData().goalSize.x) {
                Vector3 vector2 = ConvertWordVec(localShootPos);
                vector2.x *= 240f;
                vector2.y *= 150f;
                vector = vector2;
                UnityEngine.Debug.Log("枠内シュ\u30fcト : " + vector.ToString());
                if (UnityEngine.Random.Range(0, 100) < CpuAi.GetDivingMissPer(teamNo) && Mathf.Abs(localShootPos.x) >= GetCharaBodySize() * 0.75f) {
                    int num2 = UnityEngine.Random.Range(0, 100);
                    if (num2 <= 30) {
                        vector2.x *= UnityEngine.Random.Range(0.2f, 0.5f);
                    } else if (num2 <= 60) {
                        vector2.y *= UnityEngine.Random.Range(0f, 0.25f);
                    } else {
                        vector2.y *= UnityEngine.Random.Range(100f, 200f);
                    }
                }
                vector2.x = Mathf.Max(Mathf.Min(vector2.x, 275f), -275f);
                if (Mathf.Abs(localShootPos.x) >= GetCharaBodySize() * 0.75f) {
                    if (BM.GetStateTime() <= 0.5f) {
                        vector2.x = vector2.x * 1f + (1f - BM.GetStateTime() / 0.5f);
                    }
                    vector2.y = Mathf.Max(vector2.y, 50f);
                }
                vector2.y = Mathf.Min(vector2.y, 250f);
                if (ConvertLocalVec(vector2).z > 0f) {
                    vector2.z = 0f;
                }
                if (Mathf.Abs(vector2.x) >= 100f) {
                    if (localShootPos.x > 0f) {
                        this.obj.SetLocalEulerAngles(0f, 0f, -90f);
                        this.obj.SetLocalPosition(-0.545f, 0.39f, 0f);
                    } else {
                        this.obj.SetLocalEulerAngles(0f, 0f, 90f);
                        this.obj.SetLocalPosition(0.545f, 0.39f, 0f);
                    }
                    actionInterval = DIVING_CATCH_INTERVAL * Mathf.Max(vector2.magnitude * 0.01f, 1f) - CpuAi.GetKeeperFreezeTimeCorr(teamNo);
                    isDiving = true;
                } else {
                    float x = localShootPos.x;
                    isDiving = false;
                    actionInterval = DIVING_CATCH_INTERVAL * Mathf.Max(vector2.magnitude * 0.01f * 0.5f, 1f) - CpuAi.GetKeeperFreezeTimeCorr(teamNo);
                }
                rigid.velocity = CalcManager.mVector3Zero;
                rigid.Sleep();
                vector = vector2;
                UnityEngine.Debug.Log("divingVec = " + vector.ToString() + " : actionInterval = " + actionInterval.ToString());
                if (actionState == ActionState.DIVING_CATCH) {
                    rigid.AddForce(vector2, ForceMode.Impulse);
                    LookForward();
                }
            } else {
                UnityEngine.Debug.Log("枠外シュ\u30fcト");
            }
        }
        private void HeadingAction() {
            actionInterval -= Time.deltaTime;
            if (actionInterval <= 0f) {
                ResetObjPosition();
                actionState = ActionState.STANDARD;
            }
        }
        public void Heading() {
            if (CheckActionState(ActionState.STANDARD)) {
                Vector3 up = Vector3.up;
                up.x += BM.GetBallPos().x - GetPos().x;
                up.x = Mathf.Max(Mathf.Min(up.x, 0.25f), -0.25f);
                up.z += BM.GetBallPos().z - GetPos().z;
                up.z = Mathf.Max(Mathf.Min(up.z, 0.25f), -0.25f);
                rigid.AddForce(up * 300f, ForceMode.Impulse);
                actionInterval = HEADING_INTERVAL;
                SetAction(AiHeading, _immediate: true, _forcibly: true);
                actionState = ActionState.HEADING;
            }
        }
        private void DivingHeadAction() {
            actionInterval -= Time.deltaTime;
            if (actionInterval <= 0f) {
                ResetObjPosition();
                actionState = ActionState.STANDARD;
            }
        }
        public void DivingHead() {
            if (CheckActionState(ActionState.STANDARD)) {
                Vector3 up = Vector3.up;
                up.x += BM.GetBallPos().x - GetPos().x;
                up.x = Mathf.Max(Mathf.Min(up.x, 0.25f), -0.25f);
                up.z += BM.GetBallPos().z - GetPos().z;
                up.z = Mathf.Max(Mathf.Min(up.z, 0.25f), -0.25f);
                up.y = 1.3f;
                LeanTween.rotateX(obj.gameObject, 75f, 0.25f).setEaseOutQuad();
                rigid.AddForce(up * 300f, ForceMode.Impulse);
                actionInterval = DIVING_HEAD_INTERVAL;
                SetAction(AiDivingHead, _immediate: true, _forcibly: true);
                actionState = ActionState.DIVING_HEAD;
            }
        }
        private void OverHeadKickAction() {
            actionInterval -= Time.deltaTime;
            if (actionInterval <= 0f) {
                ResetObjPosition();
                actionState = ActionState.STANDARD;
            }
        }
        public void OverHeadKick() {
            if (CheckActionState(ActionState.STANDARD)) {
                Vector3 up = Vector3.up;
                up.x += BM.GetBallPos().x - GetPos().x;
                up.x = Mathf.Max(Mathf.Min(up.x, 0.25f), -0.25f);
                up.z += BM.GetBallPos().z - GetPos().z;
                up.z = Mathf.Max(Mathf.Min(up.z, 0.25f), -0.25f);
                up.y = 1.4f;
                obj.SetLocalEulerAngles(0f, 180f, 0f);
                LeanTween.rotateAroundLocal(obj.gameObject, new Vector3(1f, 0f, 0f), -180f, 0.2f).setEaseInCubic();
                LeanTween.rotateAroundLocal(obj.gameObject, new Vector3(1f, 0f, 0f), -80f, 0.2f).setEaseLinear().setDelay(0.7f);
                rigid.AddForce(up * 300f, ForceMode.Impulse);
                actionInterval = OVER_HEAD_KICK_INTERVAL;
                SetAction(AiDivingHead, _immediate: true, _forcibly: true);
                actionState = ActionState.OVER_HEAD_KICK;
            }
        }
        private void JumpingVolleyAction() {
            actionInterval -= Time.deltaTime;
            if (actionInterval <= 0f) {
                ResetObjPosition();
                actionState = ActionState.STANDARD;
            }
        }
        public void JumpingVolley() {
            if (CheckActionState(ActionState.STANDARD)) {
                Vector3 up = Vector3.up;
                up.x += BM.GetBallPos().x - GetPos().x;
                up.x = Mathf.Max(Mathf.Min(up.x, 0.25f), -0.25f);
                up.z += BM.GetBallPos().z - GetPos().z;
                up.z = Mathf.Max(Mathf.Min(up.z, 0.25f), -0.25f);
                up.y = 1.2f;
                LeanTween.rotateZ(obj.gameObject, 70f, 0.25f).setEaseOutQuad();
                rigid.AddForce(up * 300f, ForceMode.Impulse);
                actionInterval = JUMPING_VOLLEY_INTERVAL;
                SetAction(AiDivingHead, _immediate: true, _forcibly: true);
                actionState = ActionState.JUMPING_VOLLEY;
            }
        }
        private void CursorRotAnimation() {
            if (playerNo != -1 && !(SingletonCustom<ControllerManager>.Instance.GetTapTime(playerNo) > 0f)) {
                cursorRotTime += cursorRotSpeed * Time.deltaTime;
                switch (actionState) {
                    case ActionState.THROW_IN:
                        MCM.GetCursor(playerNo).transform.SetLocalEulerAnglesY(Mathf.Sin(cursorRotTime * (float)Math.PI * 2f) * -80f);
                        break;
                    case ActionState.CORNER_KICK:
                        MCM.GetCursor(playerNo).transform.SetLocalEulerAnglesY(Mathf.Sin(cursorRotTime * (float)Math.PI * 2f) * -30f);
                        break;
                    case ActionState.GOAL_KICK:
                        MCM.GetCursor(playerNo).transform.SetLocalEulerAnglesY(Mathf.Sin(cursorRotTime * (float)Math.PI * 2f) * -45f);
                        break;
                }
            }
        }
        public void SettingThrowIn(bool _player) {
            UnityEngine.Debug.Log("スロ\u30fcイン設定");
            ResetObjPosition();
            rigid.velocity = CalcManager.mVector3Zero;
            rigid.angularVelocity = CalcManager.mVector3Zero;
            rigid.isKinematic = true;
            base.transform.SetPosition(BM.GetBallPos().x, GetPos().y, BM.GetBallPos().z);
            for (int i = 0; i < 2; i++) {
                bodyParts.Parts(3 + i).SetLocalEulerAngles(ThrowInData.ARM_ROT.x, ThrowInData.ARM_ROT.y, ThrowInData.ARM_ROT.z);
            }
            for (int j = 0; j < 2; j++) {
                bodyParts.Parts(5 + j).SetLocalEulerAnglesX(ThrowInData.ARM_HAND_ROT);
            }
            bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAngles(ThrowInData.UPPER_BODY_ROT, 0f, 0f);
            Ball.GetRigid().isKinematic = true;
            Ball.GetCollider().isTrigger = true;
            Ball.transform.parent = bodyParts.throwInAnchor;
            Ball.transform.SetLocalPosition(0f, 0f, 0f);
            if (base.transform.position.x >= FM.GetAnchors().centerCircle.transform.position.x) {
                base.transform.SetEulerAngles(0f, -90f, 0f);
            } else {
                base.transform.SetEulerAngles(0f, 90f, 0f);
            }
            throwInAnimationTime = 0f;
            throwInTime = UnityEngine.Random.Range(1.25f, 2.25f);
            isBallThrow = false;
            setPlayMethod = CursorRotAnimation;
            if (!_player) {
                SetAction(AiThrowInStandby, _immediate: true, _forcibly: true);
            } else {
                SetAction(null, _immediate: true, _forcibly: true);
            }
            actionState = ActionState.THROW_IN;
        }
        private void ThrowInAction() {
            if (setPlayMethod != null) {
                setPlayMethod();
            }
        }
        private void ThrowInAnimation() {
            throwInAnimationTime += throwInAnimationSpeed * Time.deltaTime;
            if (throwInAnimationTime >= 0.7f) {
                ThrowIn();
            }
            if (throwInAnimationTime <= 1f) {
                for (int i = 0; i < 2; i++) {
                    bodyParts.Parts(3 + i).SetLocalEulerAngles(ThrowInData.ARM_ROT.x + (ThrowInData.ARM_AFTER_ROT - ThrowInData.ARM_ROT.x) * throwInAnimationTime, ThrowInData.ARM_ROT.y, ThrowInData.ARM_ROT.z);
                }
                for (int j = 0; j < 2; j++) {
                    bodyParts.Parts(5 + j).SetLocalEulerAnglesX(ThrowInData.ARM_HAND_ROT + (360f - ThrowInData.ARM_HAND_ROT) * throwInAnimationTime);
                }
                bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAnglesX(ThrowInData.UPPER_BODY_ROT + (ThrowInData.UPPER_BODY_AFTER_ROT - ThrowInData.UPPER_BODY_ROT) * throwInAnimationTime);
            } else {
                setPlayMethod = ThrowInFinishAnimation;
            }
        }
        private void ThrowInFinishAnimation() {
            throwInAnimationTime += throwInAnimationSpeed * Time.deltaTime;
            if (throwInAnimationTime >= 2f && MCM.GetHaveBallChara() != null) {
                for (int i = 0; i < 2; i++) {
                    bodyParts.Parts(3 + i).SetLocalEulerAngles(0f, 0f, 0f);
                }
                for (int j = 0; j < 2; j++) {
                    bodyParts.Parts(5 + j).SetLocalEulerAnglesX(-90f);
                }
                bodyParts.Parts(BodyPartsList.BODY).SetLocalEulerAnglesX(0f);
                setPlayMethod = null;
                actionState = ActionState.STANDARD;
            }
        }
        private void ThrowIn() {
            if (!isBallThrow) {
                isBallThrow = true;
                CharacterScript characterScript = MCM.SearchAllyPassed(this, teamNo);
                if (characterScript == null) {
                    characterScript = MCM.SearchMeNearest(this, teamNo);
                }
                MCM.ThrowIn(throwPower, this, characterScript);
                rigid.isKinematic = false;
                SingletonCustom<AudioManager>.Instance.SePlay("se_throw_ball");
            }
        }
        public void StartBallThrowIn(float _power) {
            if (!(setPlayMethod != new Action(CursorRotAnimation))) {
                throwPower = _power;
                setPlayMethod = ThrowInAnimation;
                LookCursorDir();
            }
        }
        public void SettingCornerKick(bool _player) {
            ResetObjPosition();
            rigid.isKinematic = true;
            rigid.velocity = CalcManager.mVector3Zero;
            rigid.angularVelocity = CalcManager.mVector3Zero;
            Vector3 vector = FM.CheckCornerPos(Ball.GetLineOutPos());
            base.transform.SetPosition(vector.x, GetPos().y, vector.z);
            Ball.GetRigid().isKinematic = true;
            Ball.GetCollider().isTrigger = true;
            base.transform.SetEulerAnglesY(CalcManager.Rot(BM.GetBallPos() - vector, CalcManager.AXIS.Y));
            cornerKickDelayTime = 0f;
            cornerKickTime = UnityEngine.Random.Range(1.25f, 2.25f);
            isBallKick = false;
            actionState = ActionState.CORNER_KICK;
            if (!_player) {
                SetAction(AiCornerKickStandby, _immediate: true, _forcibly: true);
            } else {
                SetAction(null, _immediate: true, _forcibly: true);
            }
        }
        private void CornerKickAction() {
            if (isBallKick) {
                cornerKickDelayTime += Time.deltaTime;
                if (BM.CheckBallState(BallManager.BallState.CORNER_KICK) && cornerKickDelayTime >= CORNER_KICK_CHARGE_TIME) {
                    CharacterScript characterScript = MCM.SearchAllyPassed(this, teamNo);
                    if (characterScript == null) {
                        characterScript = MCM.SearchMeNearest(this, teamNo);
                    }
                    MCM.CornerKick(kickPower, this, characterScript);
                } else if (cornerKickDelayTime >= 1.5f && MCM.GetHaveBallChara() != null) {
                    actionState = ActionState.STANDARD;
                }
            } else {
                CursorRotAnimation();
            }
        }
        public void StartCornerKick(float _power, bool _player = false) {
            isBallKick = true;
            rigid.isKinematic = false;
            kickPower = _power;
            cornerKickDelayTime = CORNER_KICK_CHARGE_TIME;
            LookCursorDir();
        }
        public void SettingGoalKick(bool _player) {
            ResetObjPosition();
            rigid.velocity = CalcManager.mVector3Zero;
            rigid.angularVelocity = CalcManager.mVector3Zero;
            rigid.isKinematic = true;
            Vector3 goalKickPos = FM.GetGoalKickPos(teamNo);
            base.transform.SetPosition(goalKickPos.x, goalKickPos.y, goalKickPos.z);
            UnityEngine.Debug.Log("キ\u30fcパ\u30fcの座標 = " + base.transform.position.ToString());
            Ball.GetRigid().isKinematic = true;
            Ball.GetCollider().isTrigger = true;
            LookForward();
            goalKickDelayTime = 0f;
            goalKickTime = UnityEngine.Random.Range(1.25f, 2.25f);
            actionState = ActionState.GOAL_KICK;
            isBallKick = false;
            if (!_player) {
                SetAction(AiGoalKickStandby, _immediate: true, _forcibly: true);
            } else {
                SetAction(null, _immediate: true, _forcibly: true);
            }
        }
        private void GoalKickAction() {
            if (isBallKick) {
                goalKickDelayTime += Time.deltaTime;
                if (BM.CheckBallState(BallManager.BallState.GOAL_KICK) && goalKickDelayTime >= GOAL_KICK_CHARGE_TIME) {
                    MCM.GoalKick(kickPower, this, null);
                } else if (goalKickDelayTime >= 1.5f && MCM.GetHaveBallChara() != null) {
                    actionState = ActionState.STANDARD;
                }
            } else {
                CursorRotAnimation();
            }
        }
        public void StartGoalKick(float _power, bool _player = false) {
            isBallKick = true;
            rigid.isKinematic = false;
            kickPower = _power;
            goalKickDelayTime = GOAL_KICK_CHARGE_TIME;
            LookCursorDir();
        }
        private void PlaySeRun() {
            if (MCM.CheckControlChara(this)) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_run", _loop: false, 0f, 0.4f);
            }
        }
        public bool CheckPositionType(GameDataParams.PositionType _type) {
            return positionType == _type;
        }
        public bool CheckCharaHide() {
            return !bodyParts.bodyAnchor.activeSelf;
        }
        public void SetActionState(ActionState _state) {
            if (actionState == ActionState.SLIDING || actionState == ActionState.DIVING_CATCH) {
                ResetObjPosition();
            }
            actionState = _state;
        }
        public void ResetObjPosition() {
            obj.SetLocalPosition(0f, 0f, 0f);
            obj.SetLocalEulerAngles(0f, 0f, 0f);
            base.transform.SetEulerAngles(0f, base.transform.rotation.eulerAngles.y, 0f);
            ResetKickAnimation();
            rigid.constraints = (RigidbodyConstraints)80;
        }
        private void OnCollisionEnter(Collision _col) {
            if (!SingletonCustom<MainGameManager>.Instance.CheckInPlay()) {
                return;
            }
            if (CheckActionState(ActionState.FREEZE)) {
                Ball.SetLastHitChara(this);
            } else {
                if (!CheckActionState(ActionState.STANDARD) && !CheckActionState(ActionState.SLIDING) && !CheckActionState(ActionState.DIVING_CATCH) && !CheckActionState(ActionState.HEADING) && !CheckActionState(ActionState.DIVING_HEAD) && !CheckActionState(ActionState.OVER_HEAD_KICK) && !CheckActionState(ActionState.JUMPING_VOLLEY)) {
                    return;
                }
                if (_col.transform.tag == "Ball" && BM.CheckBallState(BallManager.BallState.FREE) && CheckBreakChara(Ball.GetLastShootChara())) {
                    rigid.velocity = Ball.GetMoveVec().normalized * 5f;
                    Ball.SetLastHitChara(this);
                    obj.SetLocalPosition(0f, 0.11f, 0.374f);
                    obj.SetLocalEulerAngles(-45f, 0f, 0f);
                    rigid.constraints = RigidbodyConstraints.FreezeRotationZ;
                    breakEffect.Play();
                    UnityEngine.Debug.Log("吹き飛ばされる : BallSpeed = " + BM.GetBallSpeed().ToString());
                    if (PlayerNo != -1) {
                        SingletonCustom<HidVibration>.Instance.SetCommonVibration(PlayerNo);
                    }
                    SingletonCustom<AudioManager>.Instance.SePlay("se_chara_break");
                    freezeTime = 1.5f;
                    actionState = ActionState.FREEZE;
                    SetAction(AiFreeze, _immediate: true, _forcibly: true);
                } else if (CheckActionState(ActionState.HEADING)) {
                    if (_col.transform.tag == "Ball" && BM.CheckBallState(BallManager.BallState.FREE) && BM.GetBallPos(_offset: false).y >= GetPos().y - GetCharaHeight() * 0.5f) {
                        MCM.Heading(0.5f, this, null);
                        Ball.SetLastHitChara(this);
                        Ball.SetLastShootChara(this);
                        SingletonCustom<AudioManager>.Instance.SePlay("se_heading");
                        UnityEngine.Debug.Log("ヘディング成功");
                    }
                } else if (CheckActionState(ActionState.DIVING_HEAD)) {
                    if (_col.transform.tag == "Ball" && BM.CheckBallState(BallManager.BallState.FREE) && BM.GetBallPos(_offset: false).y >= GetPos().y - GetCharaHeight() * 0.5f) {
                        MCM.DivingHead(0.5f, this, null);
                        Ball.SetLastHitChara(this);
                        Ball.SetLastShootChara(this);
                        SingletonCustom<AudioManager>.Instance.SePlay("se_heading");
                        UnityEngine.Debug.Log("ダイビングヘッド成功");
                    }
                } else if (CheckActionState(ActionState.OVER_HEAD_KICK)) {
                    if (_col.transform.tag == "Ball" && BM.CheckBallState(BallManager.BallState.FREE) && BM.GetBallPos(_offset: false).y >= GetPos().y - GetCharaHeight()) {
                        MCM.OverHeadKick(0.5f, this, null);
                        Ball.SetLastHitChara(this);
                        Ball.SetLastShootChara(this);
                        SingletonCustom<AudioManager>.Instance.SePlay("se_heading");
                        UnityEngine.Debug.Log("オ\u30fcバ\u30fcヘッドキック成功");
                    }
                } else if (CheckActionState(ActionState.JUMPING_VOLLEY)) {
                    if (_col.transform.tag == "Ball" && BM.CheckBallState(BallManager.BallState.FREE) && BM.GetBallPos(_offset: false).y >= GetPos().y - GetCharaBodySize() * 0.5f) {
                        MCM.JumpingVolley(0.5f, this, null);
                        Ball.SetLastHitChara(this);
                        Ball.SetLastShootChara(this);
                        SingletonCustom<AudioManager>.Instance.SePlay("se_heading");
                        UnityEngine.Debug.Log("ジャンピングボレ\u30fc成功");
                    }
                } else if (_col.transform.tag == "Ball") {
                    if (BM.CheckBallState(BallManager.BallState.FREE)) {
                        if (CheckBallTrapSuccess(BM.GetBallSpeed(_rigid: true))) {
                            CatckBall();
                            if (CheckPositionType(GameDataParams.PositionType.GK)) {
                                SingletonCustom<AudioManager>.Instance.SePlay("se_keeper_catch");
                            }
                        } else {
                            UnityEngine.Debug.Log("トラップミス : チ\u30fcム" + teamNo.ToString() + " : No." + charaNo.ToString());
                            Ball.SetLastHitChara(this);
                            SingletonCustom<AudioManager>.Instance.SePlay("se_kick", _loop: false, 0f, 0.5f);
                        }
                    } else if (BM.CheckBallState(BallManager.BallState.KEEP) && BM.CheckStealInterval() && teamNo != MCM.GetHaveBallChara().teamNo && CheckBallSteal(MCM.GetHaveBallChara(), _ballHit: true)) {
                        BM.SettingStealInterval();
                        CatckBall();
                    }
                } else {
                    if (!(_col.transform.tag == "Character") || !BM.CheckBallState(BallManager.BallState.KEEP) || !MCM.CheckHaveBall(_col.gameObject) || !BM.CheckStealInterval()) {
                        return;
                    }
                    if (MCM.CheckOpponentHaveBall(teamNo)) {
                        if (CheckBallSteal(MCM.GetHaveBallChara(), _ballHit: false, MCM.GetHaveBallChara().ConvertLocalPos(base.transform.position).z >= -0.5f)) {
                            BM.SettingStealInterval();
                            CatckBall();
                        }
                    } else if (!MCM.IsPlayer(playerNo) && UnityEngine.Random.Range(0, 100) < 80) {
                        BM.SettingStealInterval();
                        MCM.KickBall(0.25f, MCM.GetHaveBallChara(), this);
                        CatckBall();
                    }
                }
            }
        }
        private void CatckBall() {
            if (!CheckActionState(ActionState.STANDARD)) {
                actionInterval = STEAL_RIGOR_TIME;
            }
            MCM.CatchBall(this);
            prevPos = (nowPos = base.transform.position);
        }
        private bool CheckBallSteal(CharacterScript _chara, bool _ballHit, bool _backPress = false) {
            if (_chara.CheckPositionType(GameDataParams.PositionType.GK) && isBallCatch) {
                return false;
            }
            if (CheckPositionType(GameDataParams.PositionType.GK) && FM.CheckInPenaltyArea(this)) {
                return true;
            }
            float num = 10f + (float)charaParam.defense + (float)Mathf.Max(charaParam.defense - _chara.GetCharaParam().offense, 0);
            if (_ballHit) {
                num *= 1.5f;
            }
            if (CheckActionState(ActionState.SLIDING)) {
                num *= 5f;
            }
            if (_backPress) {
                num /= 2f;
            }
            if (!MCM.IsPlayer(playerNo)) {
                num *= CpuAi.GetStealPerMag(teamNo);
            }
            if (!MCM.IsPlayer(_chara.playerNo)) {
                num *= CpuAi.GetStolenPerMag(teamNo);
            }
            return (float)UnityEngine.Random.Range(0, 100) < Mathf.Min(num, 90f);
        }
        private bool CheckBreakChara(CharacterScript _chara) {
            if (_chara == null || _chara.teamNo == teamNo || MCM.CheckPrevHaveBallTeam(teamNo)) {
                return false;
            }
            Vector3 velocity = Ball.GetRigid().velocity;
            velocity.y = 0f;
            if (velocity.magnitude * 0.075f < BallScript.FastBallBorder) {
                return false;
            }
            float num = 50f + (float)_chara.GetCharaParam().offense + (float)Mathf.Max(_chara.GetCharaParam().offense - charaParam.defense, 0) * 3f;
            if (CheckPositionType(GameDataParams.PositionType.GK)) {
                num *= 0.3f;
            }
            return (float)UnityEngine.Random.Range(0, 100) < Mathf.Min(num, 90f);
        }
        private bool CheckBallTrapSuccess(float _ballSpeed) {
            if (CheckPositionType(GameDataParams.PositionType.GK)) {
                int num = UnityEngine.Random.Range(0, 100);
                int num2 = Mathf.Max(80 - charaParam.offense * 2, 50);
                if (isDiving) {
                    if ((BM.GetBall().GetLastShootChara().teamNo != teamNo && _ballSpeed >= BallScript.SlowBallBorder && num <= num2) || _ballSpeed >= BallScript.FastBallBorder || !FM.CheckInPenaltyArea(this)) {
                        UnityEngine.Debug.Log("キャッチ失敗");
                        return false;
                    }
                } else if ((BM.GetBall().GetLastShootChara().teamNo != teamNo && _ballSpeed >= BallScript.SlowBallBorder && num <= num2) || _ballSpeed >= BallScript.FastBallBorder) {
                    UnityEngine.Debug.Log("キャッチ失敗");
                    return false;
                }
            }
            if (_ballSpeed >= BallScript.FastBallBorder) {
                UnityEngine.Debug.Log("ボ\u30fcルが早い");
                return false;
            }
            if (BM.GetStateTime() <= 0.01f) {
                UnityEngine.Debug.Log("ボ\u30fcルを蹴ってすぐ");
                return false;
            }
            return true;
        }
        public bool CheckKickoffPosition() {
            if (CalcManager.Length(kickOffStandbyPos, GetPos()) <= STOP_CHECK_DISTANCE) {
                return true;
            }
            return false;
        }
        public bool CheckObj(GameObject _obj) {
            if (!(_obj == obj.gameObject)) {
                return base.gameObject == _obj;
            }
            return true;
        }
        public bool CheckActionState(ActionState _state) {
            return actionState == _state;
        }
        public Vector3 CheckCanIntrusionArea(Vector3 _pos) {
            return _pos;
        }
        public Rigidbody GetRigid() {
            return rigid;
        }
        public Vector3 GetMoveVec() {
            return nowPos - prevPos;
        }
        public Vector3 GetPos(bool _isLocal = false) {
            if (_isLocal) {
                return base.transform.localPosition;
            }
            return base.transform.position;
        }
        public CapsuleCollider GetCollider() {
            return charaCollider;
        }
        public float GetCharaBodySize() {
            return charaBodySize;
        }
        public float GetCharaHeight() {
            return charaHeight;
        }
        public Vector3 GetFormationPos(bool _local = true, bool _half = false) {
            formationPos = formationAnchor.localPosition;
            if (!_half && positionType != 0) {
                formationPos.z *= 2f;
            }
            if (!_local) {
                formationPos = formationAnchor.parent.TransformPoint(formationPos);
            }
            return formationPos;
        }
        public ActionState GetActionState() {
            return actionState;
        }
        public bool IsBallThrow() {
            return isBallThrow;
        }
        public bool IsBallKick() {
            return isBallKick;
        }
        public bool IsSpecialAction() {
            if (actionState != ActionState.HEADING && actionState != ActionState.SLIDING && actionState != ActionState.DIVING_CATCH && actionState != ActionState.DIVING_HEAD && actionState != ActionState.OVER_HEAD_KICK) {
                return actionState == ActionState.JUMPING_VOLLEY;
            }
            return true;
        }
        public float GetControlInterval() {
            return controlInterval;
        }
        public void SetControlInterval(float _time) {
            controlInterval = _time;
        }
        public string GetName() {
            return name;
        }
        public int GetUniformNumber() {
            return uniformNumber;
        }
        public GameDataParams.PositionType GetPositionType() {
            return positionType;
        }
        public StatusType GetCharaParam() {
            return charaParam;
        }
        public float GetKickPowerCorr() {
            return kickPowerCorr;
        }
        public float GetStaminaPer() {
            return Mathf.Max(Mathf.Min(staminaValue / staminaValueMax, 1f), 0f);
        }
        public Vector3 ConvertLocalPos(Vector3 _pos) {
            return base.transform.InverseTransformPoint(_pos);
        }
        public Vector3 ConvertLocalVec(Vector3 _vec) {
            if (teamNo == 1) {
                _vec.x *= -1f;
                _vec.z *= -1f;
            }
            return _vec;
        }
        public Vector3 ConvertWordVec(Vector3 _vec) {
            if (teamNo == 1) {
                _vec.x *= -1f;
                _vec.z *= -1f;
            }
            return _vec;
        }
        public Vector3 CorrOffsideLine(Vector3 _pos) {
            Vector3 opponentOffsideLine = MCM.GetOpponentOffsideLine(teamNo);
            _pos = FM.ConvertLocalPos(_pos, teamNo);
            _pos.z = Mathf.Min(opponentOffsideLine.z - GetCharaBodySize() * 1.5f, _pos.z);
            return FM.ConvertWorldPos(_pos, teamNo);
        }
        public Vector3 CheckNoEntryArea(Vector3 _pos) {
            return _pos;
        }
        private bool FirstActionCall() {
            bool num = isInitAiAction;
            isInitAiAction = true;
            return !num;
        }
        public void SetAction(AiActionMethod _action, bool _immediate = true, bool _forcibly = false) {
            if ((aiActionMethod.Count <= 0 || !(aiActionMethod[0] == _action) || _forcibly) && ((actionChangeInterval <= 0f) | _immediate)) {
                aiActionMethod.Clear();
                aiActionMethod.Add(_action);
                aiActionTime = 0f;
                actionChangeInterval = 1f;
                isCallAiAction = false;
                isInitAiAction = false;
            }
        }
        public void SetNextAction(AiActionMethod _action, bool _immediate = true) {
            if ((aiActionMethod.Count <= 0 || !(aiActionMethod[aiActionMethod.Count - 1] == _action)) && ((actionChangeInterval <= 0f) | _immediate)) {
                aiActionMethod.Add(_action);
                aiActionTime = 0f;
                actionChangeInterval = 1f;
                isCallAiAction = false;
                isInitAiAction = false;
            }
        }
        public void AiMove() {
            if (ballSearchInterval <= 0f) {
                lookBallPos = BM.GetBallPos() + Ball.GetMoveVec();
                ballSearchInterval = CpuAi.GetBallSearchInterval(teamNo);
            } else {
                ballSearchInterval -= Time.deltaTime;
            }
            if (aiActionMethod != null && aiActionMethod.Count > 0 && aiActionMethod[aiActionMethod.Count - 1] != null) {
                AiState = aiActionMethod[aiActionMethod.Count - 1].Method.Name;
                aiActionMethod[aiActionMethod.Count - 1]();
                isCallAiAction = true;
                aiActionTime += Time.deltaTime;
                movePosChangeInterval -= Time.deltaTime;
                ignoreObstacleTime -= Time.deltaTime;
            }
            actionChangeInterval -= Time.deltaTime;
        }
        public void AiKickOffStandbyKicker() {
            if (FirstActionCall()) {
                bodyParts.bodyAnchor.SetActive(value: true);
                obj.gameObject.layer = GameDataParams.ConvertLayerNo("InvisibleChara");
                movePoslist.Clear();
                movePoslist.Add(kickOffStandbyPos);
            }
            if (!MoveTarget(kickOffStandbyPos, RUN_SPEED, STOP_CHECK_DISTANCE, Mathf.Max(Mathf.Min(Mathf.Abs((kickOffStandbyPos - GetPos()).magnitude), 2f), 1f))) {
                base.transform.LookAt(FM.GetAnchors().centerCircle.transform.position);
                base.transform.SetLocalEulerAnglesX(0f);
                base.transform.SetLocalEulerAnglesZ(0f);
                GetRigid().velocity = CalcManager.mVector3Zero;
                if (positionType == GameDataParams.PositionType.GK) {
                    obj.gameObject.layer = GameDataParams.ConvertLayerNo("Keeper");
                } else {
                    obj.gameObject.layer = GameDataParams.ConvertLayerNo("Character");
                }
                if (MCM.CheckHaveBall(this) && !SingletonCustom<MainGameManager>.Instance.IsKickOffStandby()) {
                    kickOffInterval = UnityEngine.Random.Range(1f, 1.5f);
                    SetAction(AiKickOff, _immediate: true, _forcibly: true);
                }
            }
        }
        public void AiKickOff() {
            base.transform.LookAt(FM.GetAnchors().centerCircle.transform.position);
            kickOffInterval -= Time.deltaTime;
            if (!(kickOffInterval > 0f)) {
                CharacterScript characterScript = MCM.SearchAllyPassed(this, TeamNo);
                base.transform.LookAt(characterScript.GetPos() + ConvertLocalVec(Vector3.forward) * GetCharaBodySize());
                MCM.KickBall(0.01f + 0.35f * Mathf.Min(Mathf.Max(CalcManager.Length(BM.GetBallPos(), GetPos()) - PERSONAL_SPACE_OUT, 0f), 1f), this, null);
                MCM.KickOff();
            }
        }
        public void AiKickOffStandby() {
            if (FirstActionCall()) {
                bodyParts.bodyAnchor.SetActive(value: true);
                movePoslist.Clear();
                movePoslist.Add(kickOffStandbyPos);
                obj.gameObject.layer = GameDataParams.ConvertLayerNo("InvisibleChara");
            }
            if (!MoveTarget(kickOffStandbyPos, RUN_SPEED, STOP_CHECK_DISTANCE, Mathf.Max(Mathf.Min(Mathf.Abs((kickOffStandbyPos - GetPos()).magnitude), 2f), 1f))) {
                LookForward();
                if (positionType == GameDataParams.PositionType.GK) {
                    obj.gameObject.layer = GameDataParams.ConvertLayerNo("Keeper");
                } else {
                    obj.gameObject.layer = GameDataParams.ConvertLayerNo("Character");
                }
            }
        }
        public void SettingReturnBench(Vector3 _benchPos) {
            ResetObjPosition();
            returnBenchPos = _benchPos;
            SetAction(AiReturnBench, _immediate: true, _forcibly: true);
        }
        public void AiReturnBench() {
            if (FirstActionCall()) {
                movePoslist.Clear();
                movePoslist.Add(returnBenchPos);
                obj.gameObject.layer = GameDataParams.ConvertLayerNo("InvisibleChara");
            }
            if (CalcManager.Length(returnBenchPos, GetPos()) <= GetCharaBodySize() * 1.5f) {
                bodyParts.bodyAnchor.SetActive(value: false);
            } else {
                Move((returnBenchPos - GetPos()).normalized, Mathf.Min(CalcManager.Length(returnBenchPos, GetPos()), RUN_SPEED), Mathf.Max(Mathf.Min(Mathf.Abs((returnBenchPos - GetPos()).magnitude), 2f), 1f));
            }
        }
        public void SetGoalProduction(int _goalTeam) {
            actionState = ActionState.GOAL_PRODUCTION;
            ResetObjPosition();
            if (_goalTeam == teamNo) {
                joyJumpInterval = UnityEngine.Random.Range(0.5f, 0.75f);
                SetAction(AiJoy, _immediate: true, _forcibly: true);
            } else {
                SetAction(AiDespair, _immediate: true, _forcibly: true);
            }
        }
        public void AiDespair() {
        }
        public void AiJoy() {
            joyJumpInterval -= Time.deltaTime;
            if (joyJumpInterval <= 0f) {
                joyJumpInterval = UnityEngine.Random.Range(1f, 2f);
                rigid.AddForce(Vector3.up * 300f, ForceMode.Impulse);
            }
        }
        public void SetMovePos(Vector3 _pos, float _moveTime) {
            ResetObjPosition();
            movePos = _pos;
            movePos.y = BM.GetBallPos().y;
            movePoslist.Clear();
            movePoslist.Add(movePos);
            isMvePos = true;
            SetAction(AiMovePos, _immediate: true, _forcibly: true);
        }
        public void AiMovePos() {
            if (isMvePos && !MoveTarget(movePos, RUN_SPEED, STOP_CHECK_DISTANCE, Mathf.Max(Mathf.Min(Mathf.Abs((movePos - GetPos()).magnitude), 2f), 1f))) {
                isMvePos = false;
            }
        }
        private bool MoveTarget(Vector3 _moveTarget, float _maxSpeed, float _stopDistance, float _speedMag = 1f) {
            if (CalcManager.Length(movePoslist[movePoslist.Count - 1], GetPos()) > _stopDistance) {
                if (movePosChangeInterval <= 0f && ignoreObstacleTime <= 0f) {
                    for (int num = movePoslist.Count - 1; num > 0; num--) {
                        if (CalcManager.Length(movePoslist[num], GetPos()) <= GetCharaBodySize()) {
                            movePoslist.RemoveAt(num);
                        }
                    }
                    Vector3 _movePos = movePoslist[movePoslist.Count - 1];
                    if (CheckFrontGoal(_movePos - GetPos(), Mathf.Min((_movePos - GetPos()).magnitude, GetCharaBodySize() * 1.5f))) {
                        if (movePoslist.Count > 1) {
                            movePoslist[movePoslist.Count - 1] = FM.GetMyGoal(teamNo).position;
                        } else {
                            movePoslist.Add(FM.GetMyGoal(teamNo).position);
                        }
                        movePosChangeInterval = 0.2f;
                    } else if (CheckChangeTargetPos(ref _movePos, _movePos)) {
                        movePoslist.Add(_movePos);
                        movePosChangeInterval = 0.2f;
                    } else if (movePoslist.Count >= 2) {
                        int num2 = movePoslist.Count;
                        for (int i = 0; i < movePoslist.Count; i++) {
                            Vector3 dir = movePoslist[i] - GetPos();
                            if (!CheckCharacterFront(dir, dir.magnitude)) {
                                num2 = i;
                                break;
                            }
                        }
                        for (int num3 = movePoslist.Count - 1; num3 > num2; num3--) {
                            movePoslist.RemoveAt(num3);
                        }
                    }
                }
                Move((movePoslist[movePoslist.Count - 1] - GetPos()).normalized, Mathf.Min(CalcManager.Length(movePoslist[movePoslist.Count - 1], GetPos()), _maxSpeed), _speedMag);
            } else {
                if (movePoslist.Count <= 1) {
                    return false;
                }
                movePoslist.RemoveAt(movePoslist.Count - 1);
            }
            return true;
        }
        public void SettingKickOffStandbyPos(Vector3 _pos, int _kickerNo = -1, bool _kickOffTeam = true) {
            ResetObjPosition();
            kickOffStandbyPos = _pos;
            if (_kickerNo != -1) {
                kickOffStandbyPos = FM.ConvertWorldPos(FM.ConvertLocalPos(kickOffStandbyPos, teamNo) + Vector3.forward * 0.65f, teamNo);
            }
            if (!_kickOffTeam && CalcManager.Length(kickOffStandbyPos, FM.GetAnchors().centerCircle.transform.position) <= FM.GetAnchors().centerCircle.radius) {
                kickOffStandbyPos = FM.GetAnchors().centerCircle.transform.position + (kickOffStandbyPos - FM.GetAnchors().centerCircle.transform.position).normalized * FM.GetAnchors().centerCircle.radius * 1.1f;
            }
        }
        public void AiStandby() {
            LookForward();
        }
        public void AiReturnPosition() {
            if (FirstActionCall()) {
                movePoslist.Clear();
                movePoslist.Add(CalcManager.mVector3Zero);
                movePlayPos = GetFormationPos(_local: false);
            }
            if (movePlayPosUpdateTime <= 0f) {
                movePlayPosUpdateTime = UnityEngine.Random.Range(2f, 3f);
                movePlayPos = MCM.SearchSpacePos(GetFormationPos(_local: false), -1, 1, -1, 1, teamNo);
            } else {
                movePlayPosUpdateTime -= Time.deltaTime;
            }
            movePoslist[0] = MCM.ConvertRestrictedArea(this, CorrOffsideLine(MCM.ConvertOptimalPositioning(movePlayPos, teamNo, positionType)));
            MoveTarget(movePoslist[0], RUN_SPEED, STOP_CHECK_DISTANCE);
        }
        public void AiMoveSetPlayPosition() {
        }
        public void AiRunTowardBall() {
            RunTowardBall();
        }
        public void AiFreeze() {
            freezeTime -= Time.deltaTime;
            if (freezeTime <= 0f) {
                actionState = ActionState.STANDARD;
                ResetObjPosition();
                SetAction(AiStandby, _immediate: true, _forcibly: true);
            }
        }
        public void AiUpPosition() {
            if (FirstActionCall()) {
                movePoslist.Clear();
                movePoslist.Add(CalcManager.mVector3Zero);
                movePlayPos = GetFormationPos(_local: false);
            }
            if (movePlayPosUpdateTime <= 0f) {
                movePlayPosUpdateTime = UnityEngine.Random.Range(1f, 3f);
                movePlayPos = MCM.SearchSpacePos(GetFormationPos(_local: false), -4, 4, -4, 5, teamNo);
            } else {
                movePlayPosUpdateTime -= Time.deltaTime;
            }
            movePoslist[0] = CorrOffsideLine(MCM.ConvertRestrictedArea(this, MCM.ConvertOptimalPositioning(movePlayPos, teamNo, positionType)));
            MoveTarget(movePoslist[0], RUN_SPEED, STOP_CHECK_DISTANCE);
        }
        public void AiFrontDribble() {
            if (FirstActionCall()) {
                movePoslist.Clear();
                movePoslist.Add(OpponentGoal.position);
                movePoslist[0] = GetPos();
                actionLagTime = UnityEngine.Random.Range(0.1f, 0.5f);
            }
            if (movePlayPosUpdateTime <= 0f) {
                movePlayPosUpdateTime = UnityEngine.Random.Range(0.1f, 0.5f);
                if ((!CheckCharacterFront(OpponentGoal.position - GetPos(), NEAR_DISTANCE) && UnityEngine.Random.Range(0f, 100f) <= 80f) || (CheckInCenteringArea() && UnityEngine.Random.Range(0f, 100f) <= 80f)) {
                    movePoslist[0] = GetPos() + (OpponentGoal.position - GetPos()).normalized * NEAR_DISTANCE;
                } else if (teamNo == 0) {
                    movePoslist[0] = MCM.SearchSpacePos(GetPos(), -4, 4, -1, 5, teamNo);
                } else {
                    movePoslist[0] = MCM.SearchSpacePos(GetPos(), -4, 4, -5, 1, teamNo);
                }
            } else {
                movePlayPosUpdateTime -= Time.deltaTime;
            }
            if (CalcManager.Length(movePoslist[movePoslist.Count - 1], GetPos()) > STOP_CHECK_DISTANCE) {
                Move((movePoslist[movePoslist.Count - 1] - GetPos()).normalized, Mathf.Min(CalcManager.Length(movePoslist[movePoslist.Count - 1], GetPos()), WALK_SPEED + CpuAi.GetRunSpeed(teamNo)));
            } else if ((!CheckCharacterFront(OpponentGoal.position - GetPos(), NEAR_DISTANCE) && UnityEngine.Random.Range(0f, 100f) <= 80f) || (CheckInCenteringArea() && UnityEngine.Random.Range(0f, 100f) <= 80f)) {
                movePoslist[0] = GetPos() + (OpponentGoal.position - GetPos()).normalized * NEAR_DISTANCE;
            } else if (teamNo == 0) {
                movePoslist[0] = MCM.SearchSpacePos(GetPos(), -4, 4, -1, 5, teamNo);
            } else {
                movePoslist[0] = MCM.SearchSpacePos(GetPos(), -4, 4, -5, 1, teamNo);
            }
            if (!(aiActionTime >= actionLagTime)) {
                return;
            }
            shootCheckTime -= Time.deltaTime;
            if (shootCheckTime <= 0f) {
                if (FM.CheckInPenaltyArea(GetPos(), opponentTeamNo, _half: true)) {
                    shootCheckTime = UnityEngine.Random.Range(0f, 0.2f);
                    if (CheckGoalDefenseOnlyOne()) {
                        UnityEngine.Debug.Log("１人しか守っていない");
                        SetNextAction(AiAimGoal);
                        return;
                    }
                    if (CheckOpenShootCourse()) {
                        UnityEngine.Debug.Log("ゴ\u30fcルがら空き");
                        SetNextAction(AiAimGoal);
                        return;
                    }
                    if (!CheckCharacterFront(OpponentGoal.position - GetPos(), 2f) || UnityEngine.Random.Range(0, 100) < CpuAi.GetShootPerWhenFrontGoal(teamNo)) {
                        UnityEngine.Debug.Log("ゴ\u30fcルを狙う");
                        SetNextAction(AiAimGoal);
                        return;
                    }
                }
                if (FM.CheckInPenaltyArea(this, _my: false)) {
                    shootCheckTime = UnityEngine.Random.Range(0f, 0.2f);
                    UnityEngine.Debug.Log("ペナルティ\u30fcエリア内");
                    if (CheckGoalDefenseOnlyOne()) {
                        UnityEngine.Debug.Log("１人しか守っていない");
                        SetNextAction(AiAimGoal);
                        return;
                    }
                    if (CheckOpenShootCourse()) {
                        UnityEngine.Debug.Log("ゴ\u30fcルがら空き");
                        SetNextAction(AiAimGoal);
                        return;
                    }
                    if (!CheckCharacterFront(OpponentGoal.position - GetPos(), 2f) || UnityEngine.Random.Range(0, 100) < CpuAi.GetShootPerWhenInPenalty(teamNo)) {
                        UnityEngine.Debug.Log("ゴ\u30fcルを狙う");
                        SetNextAction(AiAimGoal);
                        return;
                    }
                }
                if (MCM.GetDistanceFromOpponentGoal(teamNo, charaNo) <= PERSONAL_SPACE_IN) {
                    shootCheckTime = UnityEngine.Random.Range(0f, 0.2f);
                    if (CheckGoalDefenseOnlyOne()) {
                        UnityEngine.Debug.Log("１人しか守っていない");
                        SetNextAction(AiAimGoal);
                        return;
                    }
                    if (CheckOpenShootCourse()) {
                        UnityEngine.Debug.Log("ゴ\u30fcルがら空き");
                        SetNextAction(AiAimGoal);
                        return;
                    }
                    if (!CheckCharacterFront(OpponentGoal.position - GetPos(), 2f) || UnityEngine.Random.Range(0, 100) < CpuAi.GetShootPerWhenInPenalty(teamNo)) {
                        UnityEngine.Debug.Log("ゴ\u30fcルを狙う");
                        SetNextAction(AiAimGoal);
                        return;
                    }
                }
            }
            passCheckTime -= Time.deltaTime;
            if (!(passCheckTime <= 0f)) {
                return;
            }
            passCheckTime = UnityEngine.Random.Range(0.2f, 0.3f);
            if (FM.CheckInPenaltyArea(this, _my: false)) {
                if (CheckCharacterFront(GetMoveVec(), GetCharaBodySize()) && (float)UnityEngine.Random.Range(0, 100) < (float)CpuAi.GetPassPerWhenFrontOpponent(teamNo) * 0.5f) {
                    UnityEngine.Debug.Log("パス");
                    SetNextAction(AiPass);
                }
            } else if (!CheckGoalDefenseOnlyOne() && UnityEngine.Random.Range(0, 100) < CpuAi.GetUnconditionalPassPer(teamNo)) {
                UnityEngine.Debug.Log("パス");
                SetNextAction(AiPass);
            } else if (!CheckGoalDefenseOnlyOne() && staminaValue <= 0f && UnityEngine.Random.Range(0, 100) < CpuAi.GetPassPerWhenNoStamina(teamNo)) {
                UnityEngine.Debug.Log("パス");
                SetNextAction(AiPass);
            } else if (CheckCharacterFront(GetMoveVec(), 2f) && UnityEngine.Random.Range(0, 100) < CpuAi.GetPassPerWhenFrontOpponent(teamNo)) {
                UnityEngine.Debug.Log("パス");
                SetNextAction(AiPass);
            }
        }
        public void AiPass() {
            if (!RunTargetCertainTime(MCM.SearchNearestWhoCanPass(this).GetPos(), 0.5f, RUN_SPEED)) {
                UnityEngine.Debug.Log(MCM.SearchNearestWhoCanPass(this).name + "にパス");
                MCM.KickBall(0f, this, null);
                SetAction(AiStandby, _immediate: true, _forcibly: true);
            }
        }
        public void AiAimGoal() {
            if (FirstActionCall()) {
                if (UnityEngine.Random.Range(0, 100) < CpuAi.GetShootPerFrontKeeper(teamNo)) {
                    shootVec = MCM.GetOpponentKeeper(teamNo).GetPos() - GetPos();
                    shootVec = CalcManager.PosRotation2D(shootVec, CalcManager.mVector3Zero, UnityEngine.Random.Range(-5f, 5f), CalcManager.AXIS.Y);
                } else if (UnityEngine.Random.Range(0, 100) < CpuAi.GetShootMissPer(teamNo)) {
                    shootVec = OpponentGoal.position - GetPos();
                    shootVec = CalcManager.PosRotation2D(shootVec, CalcManager.mVector3Zero, UnityEngine.Random.Range(20f, 30f) * CalcManager.RandomPlusOrMinus(), CalcManager.AXIS.Y);
                } else {
                    shootVec = OpponentGoal.position - GetPos();
                    shootVec = CalcManager.PosRotation2D(shootVec, CalcManager.mVector3Zero, UnityEngine.Random.Range(5f, 10f) * CalcManager.RandomPlusOrMinus(), CalcManager.AXIS.Y);
                }
                shootTarget = GetPos() + shootVec;
                base.transform.LookAt(shootTarget);
                if (UnityEngine.Random.Range(0, 100) < CpuAi.GetOverShootPer(teamNo)) {
                    MCM.KickBall(UnityEngine.Random.Range(0.5f, 0.6f), this, null);
                } else {
                    MCM.KickBall(UnityEngine.Random.Range(0.2f, 0.4f), this, null);
                }
                UnityEngine.Debug.Log("シュ\u30fcト");
                SetAction(AiStandby, _immediate: true, _forcibly: true);
            }
        }
        public void AiShoot() {
            UnityEngine.Debug.Log("シュ\u30fcトする");
        }
        public void AiOffenseCover() {
        }
        public void AiSpacePositioning() {
            UnityEngine.Debug.Log("スペ\u30fcスに走る");
        }
        public void AiBallSteal() {
            Move((GetLookBallPos() - GetPos()).normalized, Mathf.Min(CalcManager.Length(GetLookBallPos(), GetPos()), WALK_SPEED + CpuAi.GetRunSpeed(teamNo)));
            if (CheckBallFront(GetLookBallPos() - GetPos(), 1f)) {
                SetNextAction(AiSlidingStandby);
                return;
            }
            stealGiveUpTime -= Time.deltaTime;
            if (stealGiveUpTime <= 0f && CalcManager.CheckRange(CalcManager.Rot(CalcManager.mVector3Zero, base.transform.forward, MCM.GetHaveBallChara().transform.forward, Vector3.up), -20f, 20f)) {
                SetNextAction(AiPutPressure);
            }
        }
        public void AiPutPressure() {
            PutPressure();
            if (aiActionTime >= 1.5f || CheckBallFront(GetLookBallPos() - GetPos(), 0.5f)) {
                stealGiveUpTime = UnityEngine.Random.Range(1f, 3f);
                SetNextAction(AiBallSteal);
            }
        }
        private void PutPressure() {
            Vector3 normalized = (GetLookBallPos() - FM.GetMyGoal(teamNo).position).normalized;
            Vector3 a = GetLookBallPos() - normalized * PRESSURE_DISTANCE;
            Move((a - GetPos()).normalized, Mathf.Min(CalcManager.Length(a, GetPos()), WALK_SPEED));
        }
        public void AiBackPress() {
        }
        public void AiSlidingStandby() {
            if (!RunTargetCertainTime(GetLookBallPos(), 0.5f, RUN_SPEED)) {
                Sliding();
            }
        }
        public void AiSliding() {
        }
        public void AiHeading() {
        }
        public void AiDivingHead() {
        }
        public void AiOverHeadKick() {
        }
        public void AiJumpingVolley() {
        }
        public void AiDefenseCover() {
            Vector3 normalized = (GetLookBallPos() - FM.GetMyGoal(teamNo).position).normalized;
            Vector3 a = GetLookBallPos() - normalized * PRESSURE_DISTANCE * 2f;
            Move((a - GetPos()).normalized, Mathf.Min(CalcManager.Length(a, GetPos()), WALK_SPEED));
        }
        public void AiDownPosition() {
            if (FirstActionCall()) {
                movePoslist.Clear();
                movePoslist.Add(CalcManager.mVector3Zero);
                movePlayPos = GetFormationPos(_local: false);
            }
            if (movePlayPosUpdateTime <= 0f) {
                movePlayPosUpdateTime = UnityEngine.Random.Range(1f, 3f);
                movePlayPos = MCM.SearchSpacePos(GetFormationPos(_local: false), -4, 4, -4, 5, teamNo);
            } else {
                movePlayPosUpdateTime -= Time.deltaTime;
            }
            movePoslist[0] = CorrOffsideLine(MCM.ConvertOptimalPositioning(movePlayPos, teamNo, positionType));
            MoveTarget(movePoslist[0], RUN_SPEED, STOP_CHECK_DISTANCE);
        }
        public void AiKeeperPass() {
            if (FirstActionCall()) {
                movePoslist.Clear();
                movePoslist.Add(FM.GetAnchors().centerCircle.transform.position);
                CalcManager.mCalcVector3 = movePoslist[0];
                CalcManager.mCalcVector3.x += UnityEngine.Random.Range((0f - FM.GetAnchors().centerCircle.radius) * 0.5f, FM.GetAnchors().centerCircle.radius * 0.5f);
                movePoslist[0] = CalcManager.mCalcVector3;
            }
            if (aiActionTime >= 2f || (isPassFromFriend && aiActionTime >= 0.5f)) {
                base.transform.LookAt(movePoslist[0]);
                UnityEngine.Debug.Log(MCM.SearchNearestWhoCanPass(this).name + "にパス");
                MCM.KickBall(UnityEngine.Random.Range(0.7f, 1f), this, null);
                SetAction(AiStandby, _immediate: true, _forcibly: true);
            } else {
                LookForward();
            }
        }
        public void AiGoalProtect() {
            if (FirstActionCall()) {
                ResetObjPosition();
            }
            if (CheckActionState(ActionState.DIVING_CATCH)) {
                if (MCM.CheckHaveBall(this) && !FM.CheckInPenaltyArea(this)) {
                    SingletonCustom<BallManager>.Instance.Release(this);
                }
                LookForward();
            } else {
                if (!CheckActionState(ActionState.STANDARD)) {
                    return;
                }
                if (MCM.CheckHaveBallTeam(teamNo)) {
                    goalProtect = GoalProtect.WAIT;
                }
                switch (goalProtect) {
                    case GoalProtect.WAIT:
                        if (FM.CheckInPenaltyArea(BM.GetBallPos(), teamNo)) {
                            if (BM.CheckBallState(BallManager.BallState.KEEP) && MCM.CheckOpponentHaveBall(teamNo)) {
                                if (penaltyAreaStayTime >= 0.2f) {
                                    if (UnityEngine.Random.Range(0, 100) < 30 + charaParam.defense) {
                                        goalProtect = GoalProtect.STEAL_BALL;
                                    }
                                    penaltyAreaStayTime = -0.5f;
                                } else {
                                    penaltyAreaStayTime += Time.deltaTime;
                                }
                            } else if (BM.CheckBallState(BallManager.BallState.FREE)) {
                                if (penaltyAreaStayTime >= 0.1f) {
                                    goalProtect = GoalProtect.STEAL_BALL;
                                } else {
                                    penaltyAreaStayTime += Time.deltaTime;
                                }
                            } else {
                                penaltyAreaStayTime = 0f;
                            }
                        } else {
                            penaltyAreaStayTime = 0f;
                        }
                        ProtectShootCourse();
                        break;
                    case GoalProtect.STEAL_BALL: {
                            if (!FM.CheckInPenaltyArea(BM.GetBallPos(), teamNo)) {
                                goalProtect = GoalProtect.WAIT;
                            }
                            Vector3 ballPos = BM.GetBallPos();
                            if (BM.GetBallPos(_offset: false).y >= GetPos().y + GetCharaHeight()) {
                                ballPos.z = FM.GetMyGoal(teamNo).position.z + ConvertLocalVec(Vector3.forward).z * GetCharaBodySize();
                            }
                            Move((ballPos - GetPos()).normalized, Mathf.Min(CalcManager.Length(ballPos, GetPos()), WALK_SPEED + CpuAi.GetRunSpeed(teamNo)));
                            break;
                        }
                }
                if (!(kickAnimationTime <= 0f) || !FM.CheckInPenaltyArea(BM.GetBallPos(), teamNo)) {
                    return;
                }
                if (FM.ConvertLocalPos(BM.GetBallPos(), teamNo).z <= GetPos(_isLocal: true).z + GetCharaBodySize() + BM.GetBallSize() && BM.CheckBallState(BallManager.BallState.FREE)) {
                    DivingCatch(BM.GetBallPos(_offset: false));
                } else {
                    if ((!(BM.GetBallPos(_offset: false, _local: true).y <= FM.GetFieldData().goalSize.y) && !FM.CheckInPenaltyArea(BM.GetBallPos(), teamNo, _half: true)) || !BM.CheckBallState(BallManager.BallState.FREE) || (!(Ball.GetLastShootChara() != this) && !(BM.GetStateTime() >= 0.2f))) {
                        return;
                    }
                    Vector3 moveVec = Ball.GetMoveVec();
                    moveVec.y = 0f;
                    if (moveVec.magnitude >= BallScript.SlowBallBorder || (Mathf.Abs(GetPos().x - BM.GetBallPos().x) >= GetCharaBodySize() * 2f && Mathf.Abs(GetPos().z - BM.GetBallPos().z) <= GetCharaBodySize() * 0.5f)) {
                        if (Mathf.Abs(CalcManager.Rot(ConvertLocalVec(Ball.GetMoveVec()), CalcManager.AXIS.Y) - 180f) < 60f) {
                            Vector3 ballPos2 = BM.GetBallPos();
                            ballPos2.y = base.transform.position.y;
                            base.transform.LookAt(ballPos2);
                            base.transform.SetLocalEulerAnglesX(0f);
                            base.transform.SetLocalEulerAnglesZ(0f);
                            DivingCatch(BM.GetBallPos(_offset: false));
                        }
                    } else if (MCM.GetDistanceFromBall(teamNo, charaNo) <= GetCharaBodySize() + GetCharaHeight() && BM.GetBallPos(_offset: false).y >= GetPos().y + GetCharaHeight()) {
                        DivingCatch(BM.GetBallPos(_offset: false));
                    }
                }
            }
        }
        private void ProtectShootCourse() {
            Vector3 vector;
            if (FM.CheckInPenaltyArea(BM.GetBallPos(), teamNo)) {
                vector = FM.GetMyGoal(teamNo).position;
                vector.z += ConvertLocalVec(Vector3.forward * GetCharaBodySize()).z;
                vector.x = BM.GetBallPos().x;
                vector -= FM.GetMyGoal(teamNo).position;
                vector.x = Mathf.Min(Mathf.Max(vector.x, (0f - FM.GetFieldData().goalSize.x) * 0.4f), FM.GetFieldData().goalSize.x * 0.4f);
                vector += FM.GetMyGoal(teamNo).position;
            } else {
                vector = FM.GetMyGoal(teamNo).position;
                Vector3 normalized = (GetLookBallPos() - vector).normalized;
                normalized.x *= 0.5f;
                vector += normalized * GetCharaBodySize();
                vector -= FM.GetMyGoal(teamNo).position;
                vector.x = Mathf.Min(Mathf.Max(vector.x, (0f - FM.GetFieldData().goalSize.x) * 0.4f), FM.GetFieldData().goalSize.x * 0.4f);
                vector += FM.GetMyGoal(teamNo).position;
            }
            if (CalcManager.Length(vector, GetPos()) > STOP_CHECK_DISTANCE) {
                Move((vector - GetPos()).normalized, Mathf.Min(CalcManager.Length(vector, GetPos()), WALK_SPEED + CpuAi.GetRunSpeed(teamNo)));
            }
            Vector3 worldPosition = GetLookBallPos();
            worldPosition.y = base.transform.position.y;
            base.transform.LookAt(worldPosition);
            base.transform.SetLocalEulerAnglesX(0f);
            base.transform.SetLocalEulerAnglesZ(0f);
        }
        public void AiThrowInStandby() {
            throwInTime -= Time.deltaTime;
            if (throwInTime <= 0f) {
                SingletonCustom<GameUiManager>.Instance.TimeLimitFinish();
                StartBallThrowIn(1f);
                base.transform.LookAt(MCM.SearchNearestWhoCanPass(this).GetPos());
                SetNextAction(AiThrowIn);
                UnityEngine.Debug.Log("AI:スロ\u30fcイン");
            }
        }
        public void AiThrowIn() {
            if (actionState == ActionState.STANDARD) {
                SetAction(AiStandby);
            }
        }
        public void AiCornerKickStandby() {
            cornerKickTime -= Time.deltaTime;
            if (cornerKickTime <= 0f) {
                SingletonCustom<GameUiManager>.Instance.TimeLimitFinish();
                StartCornerKick(UnityEngine.Random.Range(0.75f, 1f));
                Vector3 worldPosition = FM.GetOpponentGoal(teamNo).transform.position + ConvertWordVec(Vector3.back) * UnityEngine.Random.Range(FM.GetFieldData().penaltyAreaSize.z * 0.5f, FM.GetFieldData().penaltyAreaSize.z);
                base.transform.LookAt(worldPosition);
                SetNextAction(AiCornerKick);
                UnityEngine.Debug.Log("AI:コ\u30fcナ\u30fcキック");
            }
        }
        public void AiCornerKick() {
            if (actionState == ActionState.STANDARD) {
                SetAction(AiStandby);
            }
        }
        public void AiGoalKickStandby() {
            goalKickTime -= Time.deltaTime;
            if (goalKickTime <= 0f) {
                SingletonCustom<GameUiManager>.Instance.TimeLimitFinish();
                StartGoalKick(UnityEngine.Random.Range(0.5f, 0.75f));
                Vector3 position = FM.GetAnchors().centerCircle.transform.position;
                position.x += UnityEngine.Random.Range((0f - FM.GetAnchors().centerCircle.radius) * 0.5f, FM.GetAnchors().centerCircle.radius * 0.5f);
                base.transform.LookAt(position);
                SetNextAction(AiGoalKick);
                UnityEngine.Debug.Log("AI:ゴ\u30fcルキック");
            }
        }
        public void AiGoalKick() {
            if (actionState == ActionState.STANDARD) {
                SetAction(AiStandby);
            }
        }
        private void RunTowardBall() {
            Move((GetLookBallPos() - GetPos()).normalized, Mathf.Min(CalcManager.Length(GetLookBallPos(), GetPos()), RUN_SPEED));
        }
        private bool RunTowardTarget(Vector3 _pos, float _maxSpeed) {
            if (CalcManager.Length(_pos, GetPos()) > STOP_CHECK_DISTANCE) {
                Move((_pos - GetPos()).normalized, Mathf.Min(CalcManager.Length(_pos, GetPos()), _maxSpeed));
                return false;
            }
            return true;
        }
        private bool RunTargetCertainTime(Vector3 _pos, float _time, float _moveSpeed) {
            runTime += Time.deltaTime;
            if (runTime <= _time) {
                Move((_pos - GetPos()).normalized, _moveSpeed);
                return true;
            }
            runTime = 0f;
            return false;
        }
        private bool CheckBallFront(Vector3 _dir, float _checkDistance) {
            int layer = obj.gameObject.layer;
            obj.gameObject.layer = GameDataParams.ConvertLayerNo("NoHit");
            if (Physics.SphereCast(base.transform.position, GetCharaBodySize() * 0.5f, _dir, out raycastHit, _checkDistance, LayerMask.GetMask("Character", "Keeper", "InvisibleChara", "Ball"))) {
                obj.gameObject.layer = layer;
                if (raycastHit.collider.tag == "Ball") {
                    return true;
                }
                return false;
            }
            obj.gameObject.layer = layer;
            return false;
        }
        private bool CheckCharacterFront(Vector3 _dir, float _checkDistance, bool _avoidBall = false) {
            int layer = obj.gameObject.layer;
            obj.gameObject.layer = GameDataParams.ConvertLayerNo("NoHit");
            if (Physics.SphereCast(base.transform.position, GetCharaBodySize() * 0.75f, _dir, out raycastHit, _checkDistance, LayerMask.GetMask("Character", "Keeper", "InvisibleChara"))) {
                obj.gameObject.layer = layer;
                if (raycastHit.collider.tag == "Character") {
                    frontCharacter = MCM.GetChara(raycastHit.collider.gameObject);
                    return true;
                }
                return false;
            }
            obj.gameObject.layer = layer;
            return false;
        }
        private bool CheckFrontGoal(Vector3 _dir, float _checkDistance) {
            int layer = obj.gameObject.layer;
            obj.gameObject.layer = GameDataParams.ConvertLayerNo("NoHit");
            if (Physics.Raycast(base.transform.position, _dir, out raycastHit, _checkDistance, LayerMask.GetMask("Goal"))) {
                obj.gameObject.layer = layer;
                if (raycastHit.collider.tag == "Goalpost" || raycastHit.collider.tag == "GoalNet" || raycastHit.collider.tag == "Goal") {
                    frontObj = raycastHit.collider.gameObject;
                    return true;
                }
                return false;
            }
            obj.gameObject.layer = layer;
            return false;
        }
        private bool CheckOpenShootCourse() {
            Vector3 position = OpponentGoal.position;
            if (!CheckCharacterFront(position - GetPos(), (position - GetPos()).magnitude)) {
                return true;
            }
            Vector3 vector = OpponentGoal.InverseTransformPoint(GetPos());
            if (CalcManager.CheckRange(vector.x, 0f - FM.GetAnchors().goalSize.size.x * 0.5f, FM.GetAnchors().goalSize.size.x * 0.5f)) {
                vector.z = 0f;
                Vector3 a = OpponentGoal.TransformPoint(vector);
                if (CheckCharacterFront(a - GetPos(), (a - GetPos()).magnitude)) {
                    if (frontCharacter.CheckPositionType(GameDataParams.PositionType.GK)) {
                        return true;
                    }
                    return false;
                }
            }
            return true;
        }
        private bool CheckGoalDefenseOnlyOne() {
            int num = 0;
            for (int i = 0; i < MCM.CharaList[opponentTeamNo].charas.Length && !(MCM.GetDistanceFromMyGoal(opponentTeamNo, i) >= MCM.GetDistanceFromOpponentGoal(teamNo, charaNo)); i++) {
                num++;
            }
            return num <= 2;
        }
        private bool CheckInCenteringArea() {
            if (Mathf.Abs(FM.GetOpponentGoal(teamNo).position.z - GetPos().z) <= FM.GetFieldData().penaltyAreaSize.z && FM.CheckInPenaltyArea(this, _my: false)) {
                return true;
            }
            return false;
        }
        private bool CheckChangeTargetPos(ref Vector3 _movePos, Vector3 _endPos, bool _avoidBall = false) {
            Vector3 vector = _movePos - GetPos();
            if (debguShowData.moveTargetPos) {
                UnityEngine.Debug.DrawRay(GetPos(), vector, Color.black, vector.magnitude);
            }
            if (CheckCharacterFront(vector, Mathf.Max(vector.magnitude, GetCharaBodySize() * 3f), _avoidBall)) {
                if (debguShowData.moveTargetPos) {
                    UnityEngine.Debug.Log("前方に選手検知 : チ\u30fcム" + frontCharacter.teamNo.ToString() + "No." + frontCharacter.charaNo.ToString());
                }
                float num = CalcManager.Length(frontCharacter.GetPos(), GetPos());
                Vector3 vector2 = CalcManager.PosRotation2D(frontCharacter.GetMoveVec(), Vector3.zero, CalcManager.Rot(GetMoveVec(), CalcManager.AXIS.Y), CalcManager.AXIS.Y);
                if (frontCharacter.GetMoveVec().magnitude >= 0.1f) {
                    if (vector2.x > 0f) {
                        if (num <= GetCharaBodySize() * 3f) {
                            vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, -90f, CalcManager.AXIS.Y);
                            if (debguShowData.moveTargetPos) {
                                UnityEngine.Debug.Log("AI:左後ろに避ける");
                            }
                        } else {
                            vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, 0f - AVOID_ROT, CalcManager.AXIS.Y);
                            if (debguShowData.moveTargetPos) {
                                UnityEngine.Debug.Log("AI:左前に避ける");
                            }
                        }
                    } else if (num <= GetCharaBodySize() * 3f) {
                        vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, 90f, CalcManager.AXIS.Y);
                        if (debguShowData.moveTargetPos) {
                            UnityEngine.Debug.Log("AI;右後ろに避ける");
                        }
                    } else {
                        vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, AVOID_ROT, CalcManager.AXIS.Y);
                        if (debguShowData.moveTargetPos) {
                            UnityEngine.Debug.Log("AI:右前に避ける");
                        }
                    }
                } else if (ConvertLocalPos(frontCharacter.GetPos()).x > 0f) {
                    if (num <= GetCharaBodySize() * 1.5f) {
                        vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, -90f, CalcManager.AXIS.Y);
                        if (debguShowData.moveTargetPos) {
                            UnityEngine.Debug.Log("AI:左後ろに避ける");
                        }
                    } else {
                        vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, 0f - AVOID_ROT, CalcManager.AXIS.Y);
                        if (debguShowData.moveTargetPos) {
                            UnityEngine.Debug.Log("AI:左前に避ける");
                        }
                    }
                } else if (num <= GetCharaBodySize() * 1.5f) {
                    vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, 90f, CalcManager.AXIS.Y);
                    if (debguShowData.moveTargetPos) {
                        UnityEngine.Debug.Log("AI:右後ろに避ける");
                    }
                } else {
                    vector = CalcManager.PosRotation2D(vector, CalcManager.mVector3Zero, AVOID_ROT, CalcManager.AXIS.Y);
                    if (debguShowData.moveTargetPos) {
                        UnityEngine.Debug.Log("AI:右前に避ける");
                    }
                }
                _movePos = GetPos() + vector.normalized * vector.magnitude * 0.5f;
                return true;
            }
            return false;
        }
        private void LookForward() {
            if (teamNo == 0) {
                rigid.MoveRotation(Quaternion.Euler(0f, 0f, 0f));
            } else {
                rigid.MoveRotation(Quaternion.Euler(0f, 180f, 0f));
            }
        }
        private void LookCursorDir() {
            if (playerNo != -1) {
                UnityEngine.Debug.Log("カ\u30fcソルの方を向く");
                MCM.GetCursor(playerNo).transform.parent = base.transform.parent;
                base.transform.parent = MCM.GetCursor(playerNo).transform;
                base.transform.SetLocalEulerAnglesY(0f);
                base.transform.parent = MCM.GetCursor(playerNo).transform.parent;
                base.transform.SetLocalScale(1f, 1f, 1f);
                MCM.GetCursor(playerNo).transform.parent = base.transform;
                MCM.GetCursor(playerNo).transform.SetLocalEulerAnglesY(0f);
                MCM.GetCursor(playerNo).transform.SetLocalScale(1f, 1f, 1f);
            }
        }
        public Vector3 GetLookBallPos() {
            return lookBallPos;
        }
        public bool CheckAiAction(AiActionMethod _action, int _index = 0) {
            if (aiActionMethod.Count > 0) {
                return _action == aiActionMethod[_index];
            }
            return false;
        }
        private void OnDrawGizmos() {
            if (rigid == null) {
                return;
            }
            if ((debguShowData.spacePos || MCM.CheckHaveBall(base.gameObject)) && movePoslist.Count > 0) {
                Gizmos.color = ColorPalet.lightblue;
                Gizmos.DrawWireSphere(CorrOffsideLine(MCM.ConvertOptimalPositioning(movePoslist[0], teamNo, positionType)), PERSONAL_SPACE_IN);
            }
            if (debguShowData.moveTargetPos && movePoslist.Count > 0) {
                for (int i = 0; i < movePoslist.Count; i++) {
                    Gizmos.color = new Color(1f / (float)movePoslist.Count * (float)i, 1f / (float)movePoslist.Count * (float)i, 1f / (float)movePoslist.Count * (float)i);
                    Gizmos.DrawWireSphere(movePoslist[i], 0.5f);
                }
            }
            if (debguShowData.personalSpace) {
                Gizmos.color = ColorPalet.green;
                Gizmos.DrawWireSphere(shootTarget, PERSONAL_SPACE_IN);
            }
            if (debguShowData.personalSpace) {
                Gizmos.color = ColorPalet.yellow;
                Gizmos.DrawWireSphere(base.transform.position, PERSONAL_SPACE_IN);
            }
        }
    }
}
