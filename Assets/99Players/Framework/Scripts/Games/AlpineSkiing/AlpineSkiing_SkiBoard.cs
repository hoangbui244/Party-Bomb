using System.Collections;
using TMPro;
using UnityEngine;
public class AlpineSkiing_SkiBoard : MonoBehaviour {
    public enum MoveCursorDirection {
        UP,
        RIGHT,
        LEFT,
        DOWN
    }
    public enum SkiBoardProcessType {
        STANDBY,
        SLIDING,
        ACCEL,
        CURVE,
        GOAL
    }
    public enum CameraPosType {
        NEAR,
        NORMAL,
        DISTANT
    }
    [SerializeField]
    [Header("対象のAlpineSkiing_Player")]
    public AlpineSkiing_Player player;
    [SerializeField]
    [Header("搭乗キャラオブジェクトの位置移動用")]
    public GameObject characterObj;
    [SerializeField]
    [Header("搭乗キャラの管理スクリプト")]
    private AlpineSkiing_Character character;
    [SerializeField]
    [Header("Characterの親アンカ\u30fc")]
    public GameObject characterAnchor;
    [SerializeField]
    [Header("移動時エフェクト")]
    private ParticleSystem psMoveSmoke;
    [SerializeField]
    [Header("ドリフトエフェクト（左）")]
    private ParticleSystem psDriftLeft;
    [SerializeField]
    [Header("ドリフトエフェクト（右）")]
    private ParticleSystem psDriftRight;
    [SerializeField]
    [Header("加速エフェクト渦")]
    private ParticleSystem psSpeedEffectWhirl;
    [SerializeField]
    [Header("減速エフェクト")]
    private ParticleSystem psPenaltyEffect;
    [SerializeField]
    [Header("加速度")]
    public float acceleration = 2f;
    [SerializeField]
    [Header("方向の変えやすさ")]
    public float directionChangeSpeed = 3f;
    [SerializeField]
    [Header("ドリフトになる最小の角度")]
    public float driftAngle = 7f;
    [SerializeField]
    [Header("速度表示")]
    private TextMeshPro speedText;
    [SerializeField]
    [Header("AlpineSkiing_Camera")]
    private AlpineSkiing_Camera cameraEffect;
    [SerializeField]
    [Header("AlpineSkiing_Animation")]
    private AlpineSkiing_Animation anime;
    [SerializeField]
    [Header("AlpineSkiing_RadioControl")]
    public AlpineSkiing_RadioControl radioControl;
    [SerializeField]
    [Header("プレイヤ\u30fc番号")]
    private int playerNo;
    private Rigidbody rb;
    private Vector3 moveVector;
    private Vector3 localMoveVector;
    private bool isDriftInput;
    private float driftTime;
    private Vector3 driftDir;
    private float curveTime;
    private float addAccelSpeed;
    private float addPoleSpeed;
    private float whirlEffectTime;
    private bool isAddPoleSpeed;
    private bool isPenaltyTime;
    private AlpineSkiing_Define.UserType userType;
    public SkiBoardProcessType processType;
    private float speed;
    private bool direction;
    private bool isStockPedal;
    private bool isStockPedalPlay;
    private bool isCurveSEPlay;
    private CameraPosType cameraPos;
    public int PlayerNo => playerNo;
    public CameraPosType CameraPos => cameraPos;
    public void PlayerInit(AlpineSkiing_Player _player) {
        userType = _player.UserType;
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        ProcessTypeChange(SkiBoardProcessType.STANDBY);
        cameraEffect.Init();
        cameraPos = CameraPosType.NORMAL;
    }
    public void StartMethod() {
        ProcessTypeChange(SkiBoardProcessType.SLIDING);
        anime.SetAnim(AlpineSkiing_Animation.AnimType.STANDBY);
    }
    public void UpdateMethod() {
        radioControl.UpdateMethod();
    }
    private void FixedUpdate() {
        characterObj.transform.position = base.transform.position;
        if (processType == SkiBoardProcessType.SLIDING || processType == SkiBoardProcessType.ACCEL || processType == SkiBoardProcessType.CURVE) {
            rb.AddForce(character.angleAcc * acceleration);
            if (processType == SkiBoardProcessType.ACCEL) {
                rb.AddForce(character.angleAcc * acceleration);
            }
            moveVector = rb.velocity.normalized;
            if (AlpineSkiing_Define.GM.IsDuringGame() && player.UserType <= AlpineSkiing_Define.UserType.PLAYER_1 && !SingletonCustom<AudioManager>.Instance.IsSePlaying("se_alpineskiing_slide")) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_alpineskiing_slide", _loop: true);
            }
            if (Vector3.Angle(characterAnchor.transform.forward, moveVector) >= driftAngle) {
                if (Vector3.Cross(moveVector, characterAnchor.transform.forward).y < 0f) {
                    direction = true;
                } else {
                    direction = false;
                }
                if (!isDriftInput) {
                    driftDir = moveVector;
                    isDriftInput = true;
                }
                if (rb.velocity.sqrMagnitude >= Mathf.Pow(5f + addAccelSpeed + addPoleSpeed, 2f)) {
                    while (rb.velocity.sqrMagnitude >= Mathf.Pow(5f + addAccelSpeed + addPoleSpeed, 2f)) {
                        rb.velocity *= 0.98f;
                    }
                }
                if (driftTime <= 2.5f && processType != SkiBoardProcessType.CURVE) {
                    rb.AddForce(driftDir * rb.velocity.sqrMagnitude);
                }
                if (character.isForward) {
                    rb.AddForce(characterAnchor.transform.forward * (rb.velocity.sqrMagnitude * Mathf.Clamp(driftTime * driftTime, 0f, 2f)) * 0.7f);
                }
                driftTime = Mathf.Clamp(driftTime + Time.fixedDeltaTime, 0f, 3f);
                if (processType == SkiBoardProcessType.CURVE) {
                    curveTime = Mathf.Clamp(driftTime + Time.fixedDeltaTime, 0f, 3f);
                    if (rb.velocity.sqrMagnitude >= 2.05f) {
                        if (direction) {
                            psDriftRight.Emit(3);
                        } else {
                            psDriftLeft.Emit(3);
                        }
                    }
                    if (direction) {
                        anime.SetAnim(AlpineSkiing_Animation.AnimType.CURVE_RIGHT);
                    } else {
                        anime.SetAnim(AlpineSkiing_Animation.AnimType.CURVE_LEFT);
                    }
                    if (AlpineSkiing_Define.GM.IsDuringGame() && !isCurveSEPlay && player.UserType <= AlpineSkiing_Define.UserType.PLAYER_4) {
                        isCurveSEPlay = true;
                        SingletonCustom<AudioManager>.Instance.SePlay("se_alpineskiing_curve");
                    }
                    if (character.isForward) {
                        rb.AddForce(characterAnchor.transform.forward * rb.velocity.sqrMagnitude * 0.5f);
                    }
                } else if (processType == SkiBoardProcessType.SLIDING) {
                    anime.SetAnim(AlpineSkiing_Animation.AnimType.STANDBY);
                    if (isStockPedal) {
                        isStockPedal = false;
                    }
                    if (curveTime > 0.5f && character.isForward) {
                        rb.velocity = character.angleAcc * rb.velocity.magnitude;
                    }
                    curveTime = 0f;
                }
            } else {
                if (isCurveSEPlay && driftTime > 0.1f) {
                    isCurveSEPlay = false;
                }
                driftTime = 0f;
                isDriftInput = false;
                if (curveTime > 0.5f && character.isForward) {
                    rb.velocity = character.angleAcc * rb.velocity.magnitude;
                }
                curveTime = 0f;
                if (processType == SkiBoardProcessType.SLIDING) {
                    if (rb.velocity.sqrMagnitude >= 30f) {
                        anime.SetAnim(AlpineSkiing_Animation.AnimType.ACCEL);
                    } else if (!isStockPedalPlay) {
                        if (isStockPedal) {
                            isStockPedalPlay = true;
                            isStockPedal = false;
                            anime.SetAnim(AlpineSkiing_Animation.AnimType.SLIDING_STOCK_END);
                            if (AlpineSkiing_Define.GM.IsDuringGame() && player.UserType <= AlpineSkiing_Define.UserType.PLAYER_4) {
                                SingletonCustom<AudioManager>.Instance.SePlay("se_alpineskiing_stock");
                            }
                            StartCoroutine(IsStockPedalPlayWait());
                        } else {
                            isStockPedalPlay = true;
                            isStockPedal = true;
                            anime.SetAnim(AlpineSkiing_Animation.AnimType.SLIDING_STOCK_START);
                            StartCoroutine(IsStockPedalPlayWait());
                        }
                    }
                } else if (processType == SkiBoardProcessType.ACCEL) {
                    anime.SetAnim(AlpineSkiing_Animation.AnimType.ACCEL);
                    if (isStockPedal) {
                        isStockPedal = false;
                    }
                } else {
                    SkiBoardProcessType processType2 = processType;
                }
            }
            //??psMoveSmoke.emission.rateOverTime = Mathf.Clamp(rb.velocity.sqrMagnitude * 2f, 0f, 180f);
            if (psMoveSmoke.isStopped) {
                psMoveSmoke.Play();
            }
            if (addPoleSpeed > 0f) {
                addPoleSpeed -= Time.deltaTime * 0.02f;
                if (!character.isForward) {
                    addPoleSpeed -= Time.deltaTime * 0.1f;
                }
                if (addPoleSpeed < 0f) {
                    addPoleSpeed = 0f;
                }
            }
            if (processType == SkiBoardProcessType.SLIDING) {
                if (addAccelSpeed > 0f) {
                    addAccelSpeed -= Time.deltaTime * 1f;
                } else {
                    addAccelSpeed = 0f;
                }
            } else if (processType == SkiBoardProcessType.ACCEL) {
                addAccelSpeed = Mathf.Clamp(addAccelSpeed + Time.deltaTime * 0.5f, 0f, 2f);
            }
            if (rb.velocity.sqrMagnitude >= Mathf.Pow(5f + addAccelSpeed + addPoleSpeed, 2f)) {
                while (rb.velocity.sqrMagnitude >= Mathf.Pow(5f + addAccelSpeed + addPoleSpeed, 2f)) {
                    rb.velocity *= 0.98f;
                }
            }
        } else if (processType == SkiBoardProcessType.GOAL) {
            if (!psMoveSmoke.isStopped) {
                psMoveSmoke.Stop();
            }
            if (rb.velocity.sqrMagnitude > 0.01f) {
                rb.velocity *= 0.98f;
            } else {
                rb.velocity *= 0f;
            }
        } else if (processType == SkiBoardProcessType.STANDBY) {
            rb.velocity *= 0f;
        }
    }
    public void MoveCursor(MoveCursorDirection _dir, float _input) {
        switch (_dir) {
            case MoveCursorDirection.UP:
            case MoveCursorDirection.DOWN:
                break;
            case MoveCursorDirection.RIGHT:
                CurveMove(_input);
                break;
            case MoveCursorDirection.LEFT:
                CurveMove(_input);
                break;
        }
    }
    public void ProcessTypeChange(SkiBoardProcessType _type) {
        switch (_type) {
            case SkiBoardProcessType.STANDBY:
                processType = SkiBoardProcessType.STANDBY;
                break;
            case SkiBoardProcessType.SLIDING:
                if (processType == SkiBoardProcessType.STANDBY || processType == SkiBoardProcessType.CURVE) {
                    processType = SkiBoardProcessType.SLIDING;
                }
                if (processType == SkiBoardProcessType.ACCEL) {
                    cameraEffect.MotionBlur(active: false);
                    processType = SkiBoardProcessType.SLIDING;
                }
                if (player.UserType <= AlpineSkiing_Define.UserType.PLAYER_4) {
                    AlpineSkiing_Define.GM.AccelSEManager();
                }
                break;
            case SkiBoardProcessType.ACCEL:
                if (processType == SkiBoardProcessType.SLIDING) {
                    cameraEffect.MotionBlur(active: true);
                    processType = SkiBoardProcessType.ACCEL;
                    if (player.UserType <= AlpineSkiing_Define.UserType.PLAYER_4) {
                        AlpineSkiing_Define.GM.AccelSEManager(_set: true);
                    }
                }
                break;
            case SkiBoardProcessType.CURVE:
                if (processType == SkiBoardProcessType.SLIDING) {
                    processType = SkiBoardProcessType.CURVE;
                }
                break;
            case SkiBoardProcessType.GOAL:
                cameraEffect.RadialBlur(active: false);
                cameraEffect.MotionBlur(active: false);
                radioControl.speedLineObjs.SetActive(value: false);
                processType = SkiBoardProcessType.GOAL;
                break;
        }
    }
    public void CameraPosTypeChange(bool _set) {
        if (_set) {
            switch (cameraPos) {
                case CameraPosType.NEAR:
                    cameraPos = CameraPosType.NORMAL;
                    break;
                case CameraPosType.NORMAL:
                    cameraPos = CameraPosType.DISTANT;
                    break;
                case CameraPosType.DISTANT:
                    cameraPos = CameraPosType.NEAR;
                    break;
            }
        } else {
            switch (cameraPos) {
                case CameraPosType.NEAR:
                    cameraPos = CameraPosType.DISTANT;
                    break;
                case CameraPosType.NORMAL:
                    cameraPos = CameraPosType.NEAR;
                    break;
                case CameraPosType.DISTANT:
                    cameraPos = CameraPosType.NORMAL;
                    break;
            }
        }
    }
    private void CurveMove(float _input) {
        if (processType == SkiBoardProcessType.SLIDING) {
            characterObj.transform.forward = Vector3.Lerp(characterObj.transform.forward, Quaternion.Euler(0f, _input * directionChangeSpeed * Time.deltaTime, 0f) * characterObj.transform.forward, 0.1f);
        } else if (processType == SkiBoardProcessType.ACCEL) {
            characterObj.transform.forward = Vector3.Lerp(characterObj.transform.forward, Quaternion.Euler(0f, _input * directionChangeSpeed * 0.7f * Time.deltaTime, 0f) * characterObj.transform.forward, 0.1f);
        } else if (processType == SkiBoardProcessType.CURVE) {
            characterObj.transform.forward = Vector3.Lerp(characterObj.transform.forward, Quaternion.Euler(0f, _input * directionChangeSpeed * 1.3f * Time.deltaTime, 0f) * characterObj.transform.forward, 0.1f);
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (processType == SkiBoardProcessType.GOAL) {
            return;
        }
        if (other.name.Contains("GoalChecker") && processType != SkiBoardProcessType.GOAL) {
            ProcessTypeChange(SkiBoardProcessType.GOAL);
            int num = 0;
            for (int i = 0; i < AlpineSkiing_Define.PM.UserData_Group1.Length; i++) {
                if (AlpineSkiing_Define.PM.UserData_Group1[playerNo].goalTime > AlpineSkiing_Define.PM.UserData_Group1[i].goalTime) {
                    num++;
                }
            }
            switch (num) {
                case 0:
                    player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.HAPPY);
                    anime.SetAnim(AlpineSkiing_Animation.AnimType.WINNER);
                    break;
                case 1:
                    player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.SMILE);
                    anime.SetAnim(AlpineSkiing_Animation.AnimType.JOY);
                    break;
                case 2:
                    player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.NORMAL);
                    anime.SetAnim(AlpineSkiing_Animation.AnimType.FIGHT);
                    break;
                case 3:
                    player.characterStyle.SetMainCharacterFaceDiff((int)userType, StyleTextureManager.MainCharacterFaceType.SAD);
                    anime.SetAnim(AlpineSkiing_Animation.AnimType.SAD);
                    break;
            }
            if (!AlpineSkiing_Define.GM.IsGameEnd()) {
                if (player.UserType <= AlpineSkiing_Define.UserType.PLAYER_4) {
                    SingletonCustom<AudioManager>.Instance.SePlay("se_cracker", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
                    if (AlpineSkiing_Define.PM.PlayerOnlyOneCheck()) {
                        AlpineSkiing_Define.GM.StartForcedGoal();
                    }
                }
                AlpineSkiing_Define.PM.SetGoalTime(userType, AlpineSkiing_Define.GM.GetGameTime());
                if (AlpineSkiing_Define.PM.PlayerGoalCheck()) {
                    AlpineSkiing_Define.GM.GameEnd();
                }
            }
        }
        if (other.name.Contains("SkiPole") && !isAddPoleSpeed && !isPenaltyTime) {
            isAddPoleSpeed = true;
            if (addPoleSpeed <= 8f) {
                addPoleSpeed += 0.5f;
            }
            StartCoroutine(IsAddPoleSpeedWait());
            if (psSpeedEffectWhirl != null) {
                psSpeedEffectWhirl.Play();
            }
            cameraEffect.RadialBlur(active: true);
            if (AlpineSkiing_Define.GM.IsDuringGame() && player.UserType <= AlpineSkiing_Define.UserType.PLAYER_4) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_alpineskiing_accel", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
                SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Strong, 1f);
            }
        }
        if (other.name.Contains("Penalty") && !isPenaltyTime && !isAddPoleSpeed) {
            isPenaltyTime = true;
            addPoleSpeed -= 2f;
            if (addPoleSpeed < 0f) {
                addPoleSpeed = 0f;
            }
            StartCoroutine(IsPenaltyTimeWait());
            if (psPenaltyEffect != null) {
                psPenaltyEffect.Emit(1);
            }
            if (AlpineSkiing_Define.GM.IsDuringGame() && player.UserType <= AlpineSkiing_Define.UserType.PLAYER_4) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_alpineskiing_penalty", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
                SingletonCustom<HidVibration>.Instance.SetCustomVibration((int)userType, HidVibration.VibrationType.Common, 0.5f);
            }
        }
    }
    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Wall")) {
            return;
        }
        if (addPoleSpeed > 3f) {
            addPoleSpeed = 3f;
        }
        if (addPoleSpeed > 0f) {
            addPoleSpeed -= Time.deltaTime * 0.1f;
            if (addPoleSpeed < 0f) {
                addPoleSpeed = 0f;
            }
        }
    }
    private IEnumerator IsAddPoleSpeedWait() {
        yield return new WaitForSeconds(0.5f);
        isAddPoleSpeed = false;
    }
    private IEnumerator IsPenaltyTimeWait() {
        yield return new WaitForSeconds(0.5f);
        isPenaltyTime = false;
    }
    private IEnumerator IsStockPedalPlayWait() {
        yield return new WaitForSeconds(1f);
        isStockPedalPlay = false;
    }
}
