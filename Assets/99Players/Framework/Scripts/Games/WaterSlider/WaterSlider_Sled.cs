using UnityEngine;
public class WaterSlider_Sled : MonoBehaviour {
    public enum State {
        STANDBY,
        DRIVE,
        GOAL
    }
    public enum AnimetionMode {
        LOOKBACK,
        LOOKFRONT,
        RANK
    }
    private State currentState;
    [SerializeField]
    [Header("車のTransform")]
    public Transform car;
    [SerializeField]
    [Header("搭乗キャラ")]
    private WaterSlider_Character rideCharacter;
    [SerializeField]
    [Header("スピ\u30fcドの限界値")]
    private float maxSpeed;
    [SerializeField]
    [Header("加速度")]
    public float acceleration = 0.15f;
    [SerializeField]
    [Header("方向の変えやすさ")]
    public float directionChangeSpeed;
    [SerializeField]
    [Header("曲がる角度の大きさ")]
    private float angleSpeed;
    [SerializeField]
    [Header("浮き輪モデル")]
    private MeshRenderer FloatRenderer;
    [SerializeField]
    [Header("浮き輪モデルマテリアル_1")]
    private Material[] arrayFloatMat_1;
    [SerializeField]
    [Header("浮き輪モデルマテリアル_2")]
    private Material[] arrayFloatMat_2;
    [SerializeField]
    [Header("浮き輪モデルマテリアル影_0")]
    private Material arrayFloatMat_Shadow_0;
    [SerializeField]
    [Header("浮き輪モデルマテリアル影_1")]
    private Material arrayFloatMat_Shadow_1;
    [SerializeField]
    [Header("浮き輪モデルアウトライン")]
    private Material arrayFloatMat_OutLine;
    [SerializeField]
    [Header("加速時の集中線エフェクト")]
    private ParticleSystem psAddSpeedLine;
    [SerializeField]
    [Header("着水時のエフェクト")]
    private ParticleSystem landingEffect;
    [SerializeField]
    [Header("着水時のエフェクトのカバ\u30fcオブジェクト")]
    private GameObject landingEffectCover;
    [SerializeField]
    [Header("コ\u30fcナ\u30fcにぶつかった時のエフェクトのカバ\u30fcオブジェクト")]
    private GameObject cornerEffectCover;
    [SerializeField]
    [Header("コ\u30fcナ\u30fcにぶつかった時のエフェクト")]
    private ParticleSystem cornerEffect;
    [SerializeField]
    [Header("エフェクトを出す角度の大きさ")]
    private float effectAngle;
    [SerializeField]
    [Header("走行時のエフェクト")]
    private ParticleSystem moveEffect;
    [SerializeField]
    [Header("テレポ\u30fcト")]
    private GameObject teleport;
    private Vector3 moveVector;
    private SphereCollider sphereCollider;
    private Rigidbody rb;
    private bool isInit;
    private Vector3 initPos;
    private float addOnSpeed;
    private float addTerrainSpeed;
    private float whirlEffectTime;
    private float addSpeedLineTime;
    private float slipstreamTime;
    private bool isSePlay;
    private bool isDriftSePlay;
    private bool isDriftInput;
    [SerializeField]
    [Header("受けた衝撃の大きさの最大値")]
    private float maxImpact;
    private float impact;
    private bool shock;
    private Vector3 shockVec;
    private float time;
    [SerializeField]
    [Header("ク\u30fcルタイム")]
    private float shockCoolTime;
    [SerializeField]
    [Header("衝撃を与える時間")]
    private float shockTime;
    private RaycastHit hit;
    private float hitDisntace;
    [SerializeField]
    [Header("キャラクタ\u30fcオブジェクト")]
    private GameObject characterRoot;
    private float characterRootRotSpeed;
    private Quaternion targerCharacterRot;
    [SerializeField]
    [Header("接地判定")]
    private bool isGround;
    private RaycastHit groundHit;
    private float groundDisntace = 1f;
    private bool tunnel;
    [SerializeField]
    [Header("搭乗キャラ(回転用)")]
    private GameObject rideCharacterRot;
    [SerializeField]
    [Header("カメラ(見る方向)")]
    private GameObject lookPos;
    [SerializeField]
    [Header("アニメ\u30fcタ\u30fc")]
    private Animator animator;
    [SerializeField]
    [Header("アニメ\u30fcタ\u30fc再生速度")]
    private float animeTime;
    private float handle;
    private AnimetionMode _animetionMode;
    [SerializeField]
    [Header("振り向きアニメ\u30fcションの実行間隔")]
    private float animeTimePace;
    [SerializeField]
    [Header("振り向きアニメ\u30fcションの戻る時間")]
    private float animeTimePaceReturn;
    private float anime_TimeKeep;
    private float animeTimePace_Range;
    private bool bankAssist;
    private bool bank;
    private bool cornerEffect_;
    private Vector3 cornerPos;
    private bool goalCool;
    private bool speeUp;
    [SerializeField]
    [Header("加速時間")]
    private float speeUpTime;
    private float speeUpTimeKeep;
    [SerializeField]
    [Header("バンクの頂点")]
    private GameObject bankTop;
    private float timeStart = 6f;
    private float timeKeep;
    private Quaternion lookRot;
    private Vector3 flyPos;
    public State CurrentState => currentState;
    public bool IsSePlay => isSePlay;
    public void Init(int _playerNo) {
        if (!isInit) {
            rb = GetComponent<Rigidbody>();
            sphereCollider = GetComponent<SphereCollider>();
            initPos = base.transform.position;
            isInit = true;
        }
        base.transform.position = initPos;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        car.position = base.transform.position + Vector3.down * sphereCollider.radius;
        addOnSpeed = 0f;
        addTerrainSpeed = 0f;
        whirlEffectTime = 0f;
        addSpeedLineTime = 0f;
        slipstreamTime = 0f;
        SetCurrentState(State.STANDBY);
        Material[] materials = FloatRenderer.materials;
        materials[0] = arrayFloatMat_1[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[_playerNo]];
        materials[1] = arrayFloatMat_2[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[_playerNo]];
        materials[2] = arrayFloatMat_Shadow_0;
        materials[3] = arrayFloatMat_Shadow_1;
        materials[4] = arrayFloatMat_OutLine;
        FloatRenderer.materials = materials;
        isSePlay = false;
        isDriftInput = false;
        psAddSpeedLine.Clear();
        hitDisntace = WaterSlider_Define.HIT_DISTANCE;
        characterRootRotSpeed = WaterSlider_Define.CHARACTER_ROOT_ROTSPEED;
        handle = 0f;
        bank = false;
        switch (UnityEngine.Random.Range(0, 3)) {
            case 2:
                animeTimePace_Range = 1f;
                break;
            case 1:
                animeTimePace_Range = -1f;
                break;
            default:
                animeTimePace_Range = 0f;
                break;
        }
        _animetionMode = AnimetionMode.RANK;
        goalCool = false;
        speeUp = false;
        timeKeep = 0f;
        bankAssist = false;
    }
    public void SetCurrentState(State _state) {
        currentState = _state;
        if (currentState == State.GOAL) {
            SingletonCustom<WaterSlider_GameManager>.Instance.IsSledMoveSe(rideCharacter.PlayerNo);
        }
    }
    private void FixedUpdate() {
        if (!rideCharacter.IsCpu) {
            UnityEngine.Debug.DrawRay(base.transform.position, rb.velocity, Color.black);
            UnityEngine.Debug.DrawRay(base.transform.position, rideCharacter.transform.forward, Color.white);
        }
        switch (currentState) {
            case State.STANDBY:
                if (rb.velocity != Vector3.zero) {
                    rb.velocity = Vector3.zero;
                }
                break;
            case State.DRIVE:
                timeKeep += Time.deltaTime;
                rb.useGravity = true;
                if (rb.velocity != Vector3.zero) {
                    if (Physics.Raycast(characterRoot.transform.position, Vector3.down, out hit, hitDisntace)) {
                        Quaternion.FromToRotation(Vector3.up, hit.normal);
                        targerCharacterRot = Quaternion.Euler(hit.normal);
                    }
                    if (hit.collider != null) {
                        Vector3 velocity = rb.velocity;
                        velocity.Normalize();
                        characterRoot.transform.rotation = Quaternion.Slerp(characterRoot.transform.rotation, Quaternion.LookRotation(targerCharacterRot * velocity, hit.normal), characterRootRotSpeed * Time.deltaTime);
                    }
                }
                if (timeKeep > timeStart) {
                    car.forward = Quaternion.Euler(0f, rideCharacter.GetInputHorizontal() * directionChangeSpeed, 0f) * car.forward;
                } else {
                    car.forward = Quaternion.Euler(0f, rideCharacter.GetInputHorizontal(), 0f) * car.forward;
                }
                moveVector = rb.velocity.normalized;
                CharaAnimation();
                if (anime_TimeKeep > animeTimePace + animeTimePace_Range) {
                    AnimeCont();
                } else {
                    anime_TimeKeep += Time.deltaTime;
                }
                car.position = base.transform.position + Vector3.down * sphereCollider.radius;
                if (!isGround) {
                    moveEffect.Stop();
                    if (Physics.Raycast(characterRoot.transform.position, Vector3.down, out groundHit, groundDisntace)) {
                        if (!groundHit.collider.name.Contains("MoveObj")) {
                            rb.AddForce(0f, -1f, 0f);
                        } else {
                            groundHit.collider.GetComponent<WaterSlider_Sled>().rb.AddForce(-base.transform.forward * 2f);
                        }
                    }
                    if (base.transform.position.y > flyPos.y + 0.2f) {
                        UnityEngine.Debug.Log("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                        rb.AddForce(0f, -100f, 0f);
                    }
                } else {
                    moveEffect.Play();
                    if (!bank) {
                        if (rb.velocity.y < 0f) {
                            Accele(bankAssist);
                        }
                    } else {
                        Accele(bankAssist);
                    }
                    if (!rideCharacter.IsCpu) {
                        if (rb.velocity.sqrMagnitude >= 0.5f) {
                            if (!isSePlay) {
                                if (SingletonCustom<WaterSlider_GameManager>.Instance.CurrentState != WaterSlider_GameManager.State.GOAL_WAIT) {
                                    WaterSlider_GameManager.State currentState2 = SingletonCustom<WaterSlider_GameManager>.Instance.CurrentState;
                                }
                                isSePlay = true;
                            }
                        } else if (isSePlay) {
                            SingletonCustom<WaterSlider_GameManager>.Instance.IsSledMoveSe(rideCharacter.PlayerNo);
                            isSePlay = false;
                        }
                    }
                    CheckSlipstream();
                    if (whirlEffectTime > 0f) {
                        whirlEffectTime -= Time.deltaTime;
                        int num = Time.frameCount % 5;
                    }
                    if (rb.velocity.sqrMagnitude >= 40f) {
                        addSpeedLineTime += Time.deltaTime;
                    }
                    if (addSpeedLineTime > 0f) {
                        addSpeedLineTime -= Time.deltaTime;
                        psAddSpeedLine.Emit(1);
                    }
                    Vector3 a = Quaternion.Euler(0f, rideCharacter.GetInputHorizontal() * angleSpeed, 0f) * rideCharacter.gameObject.transform.forward;
                    if (rb.velocity.sqrMagnitude < 15f) {
                        rb.AddForce(a - rideCharacter.gameObject.transform.forward);
                    } else if (rideCharacter.IsCpu) {
                        rb.AddForce((a - rideCharacter.gameObject.transform.forward) * rideCharacter.GetCorvePower());
                    } else {
                        rb.AddForce((a - rideCharacter.gameObject.transform.forward) * 10f);
                    }
                    if (speeUp) {
                        //??moveEffect.emission.rateOverTime = 200f;
                        speeUpTimeKeep += Time.deltaTime;
                        if (speeUpTime > speeUpTimeKeep) {
                            if (rb.velocity.sqrMagnitude >= maxSpeed + 30f) {
                                while (rb.velocity.sqrMagnitude >= maxSpeed + 30f) {
                                    rb.velocity *= 0.98f;
                                }
                            } else {
                                Vector3 forward = rideCharacter.gameObject.transform.forward;
                                forward.y = 0f;
                                rb.AddForce(forward * acceleration * 10f);
                            }
                        } else {
                            psAddSpeedLine.Stop();
                            speeUpTimeKeep = 0f;
                            speeUp = false;
                        }
                    } else {
                        if (rb.velocity.sqrMagnitude >= maxSpeed) {
                            while (rb.velocity.sqrMagnitude >= maxSpeed) {
                                rb.velocity *= 0.98f;
                            }
                        }
                        //??moveEffect.emission.rateOverTime = rb.velocity.sqrMagnitude / maxSpeed * 50f;
                    }
                    if (addOnSpeed > 0f) {
                        addOnSpeed -= Time.deltaTime * 0.75f;
                    }
                    addOnSpeed = Mathf.Clamp(addOnSpeed, 0f, 3.5f);
                    if (addTerrainSpeed > 0f) {
                        addTerrainSpeed -= Time.deltaTime * 0.5f;
                    }
                    if (rb.velocity.sqrMagnitude >= 1.25f) {
                        int num2 = Time.frameCount % 10;
                    }
                }
                if (!GetShock()) {
                    break;
                }
                time += Time.deltaTime;
                if (time < shockCoolTime) {
                    if (time < shockTime) {
                        rb.AddForce(GetShockVec().normalized * GetPower());
                    }
                } else {
                    time = 0f;
                    SetShock(_shock: false);
                }
                break;
            case State.GOAL:
                if (!goalCool) {
                    handle = 0.5f;
                    CharaAnimation();
                    CharaAnimationFront();
                    moveEffect.Stop();
                    car.forward = new Vector3(-1f, 0f, 0f);
                    characterRoot.transform.rotation = new Quaternion(0f, 0f, 0f, 1f);
                    lookRot = new Quaternion(0f, 90f, 0f, 1f);
                    goalCool = true;
                }
                car.transform.rotation = Quaternion.Slerp(car.transform.rotation, lookRot, 0.01f * Time.deltaTime);
                car.position = base.transform.position + Vector3.down * sphereCollider.radius;
                if (rb.velocity.sqrMagnitude > 0f) {
                    rb.velocity -= rb.velocity * Time.deltaTime * 2f;
                }
                break;
        }
    }
    public void CheckSlipstream() {
        if (currentState == State.GOAL) {
            return;
        }
        if (SingletonCustom<WaterSlider_GameManager>.Instance.IsSlipstreamZone(rideCharacter.PlayerNo)) {
            slipstreamTime += Time.deltaTime;
            int num = Time.frameCount % 2;
            if (slipstreamTime >= 1.75f) {
                Speedup();
                slipstreamTime = 0f;
            }
        } else {
            slipstreamTime = 0f;
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.name.Contains("BankTop")) {
            bankAssist = false;
        }
        if (other.name.Contains("GoalChecker") && currentState == State.DRIVE) {
            if (rideCharacter.CurrentRank == 0 && SingletonCustom<WaterSlider_GameManager>.Instance.CurrentState != WaterSlider_GameManager.State.RESULT) {
                SingletonCustom<AudioManager>.Instance.VoicePlay("voice_common_goal_girl");
            }
            SetCurrentState(State.GOAL);
            rideCharacter.OnGoal();
        }
        if (other.name.Contains("Bank_In")) {
            if (!bank) {
                bank = true;
                bankAssist = true;
            } else {
                bank = false;
            }
        }
        if (other.name.Contains("SpeedItem") && currentState == State.DRIVE) {
            if (!rideCharacter.IsCpu) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_eraserrace_dash");
            }
            psAddSpeedLine.Play();
            speeUp = true;
            speeUpTimeKeep = 0f;
        }
        if (other.name.Contains("Bush") && SingletonCustom<WaterSlider_GameManager>.Instance.CurrentState != WaterSlider_GameManager.State.GOAL_WAIT) {
            WaterSlider_GameManager.State currentState2 = SingletonCustom<WaterSlider_GameManager>.Instance.CurrentState;
        }
        if (other.name.Contains("Tunnel_In")) {
            tunnel = true;
        }
        if (other.name.Contains("Tunnel_Out")) {
            tunnel = false;
        }
    }
    private void OnTriggerStay(Collider other) {
        if (other.name.Contains("AccZone")) {
            addTerrainSpeed = 2f;
        }
        other.name.Contains("WaterSlider_Course_1_Field");
    }
    private void OnTriggerExit(Collider other) {
        if (currentState == State.DRIVE) {
            if (!rideCharacter.IsCpu && other.name.Contains("WaterFall")) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_WaterSlider_WaterOut");
            }
            if (other.name.Contains("Bank_Out")) {
                bank = false;
            }
        }
    }
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name.Contains("MoveObj") && currentState == State.DRIVE && timeKeep > timeStart && !GetShock() && !collision.collider.GetComponent<WaterSlider_Sled>().GetShock()) {
            SetShock(_shock: true);
            if (rb.velocity.sqrMagnitude < collision.collider.GetComponent<WaterSlider_Sled>().rb.velocity.sqrMagnitude) {
                float sqrMagnitude = collision.collider.GetComponent<WaterSlider_Sled>().rb.velocity.sqrMagnitude;
                float sqrMagnitude2 = rb.velocity.sqrMagnitude;
                SetShockVec(collision.collider.GetComponent<WaterSlider_Sled>().rb.velocity + rb.velocity);
                SetPower(collision.collider.GetComponent<WaterSlider_Sled>().rb.velocity.sqrMagnitude);
                if (GetPower() * 1.5f > maxImpact) {
                    SetPower(maxImpact);
                } else {
                    SetPower(GetPower() * 1.5f);
                }
            }
        }
        if (collision.gameObject.name.Contains("WaterSlider_Course_1_Field") && currentState == State.DRIVE && !isGround) {
            landingEffectCover.transform.rotation = characterRoot.transform.rotation;
            landingEffect.Play();
            if (!rideCharacter.IsCpu) {
                SingletonCustom<AudioManager>.Instance.SePlay("se_WaterSlider_Waterlanding");
            }
        }
    }
    private void OnCollisionStay(Collision collision) {
        if (!isGround && collision.gameObject.layer == LayerMask.NameToLayer("Field")) {
            isGround = true;
            rb.drag = 0f;
            rb.angularDrag = 0f;
            rb.mass = 1f;
            flyPos = Vector3.zero;
        }
        if (!collision.gameObject.name.Contains("WaterSlider_Course_1_Field")) {
            return;
        }
        Vector3 position = rideCharacter.transform.position;
        Vector3 dir = cornerPos - position;
        UnityEngine.Debug.DrawRay(base.transform.position, dir, Color.red);
        if (currentState != State.DRIVE) {
            return;
        }
        Vector3 forward = rideCharacter.transform.forward;
        Vector3 velocity = rb.velocity;
        forward.y = 0f;
        velocity.y = 0f;
        if (!(rb.velocity.sqrMagnitude >= 17f) || !(Vector3.Angle(forward, velocity) > effectAngle)) {
            return;
        }
        cornerEffect_ = true;
        if (cornerEffect_) {
            ContactPoint[] contacts = collision.contacts;
            for (int i = 0; i < contacts.Length; i++) {
                ContactPoint contactPoint = contacts[i];
                cornerPos = contactPoint.point;
                cornerEffect_ = false;
            }
        }
        Vector3 position2 = rideCharacter.transform.position;
        Vector3 euler = cornerPos - position2;
        cornerEffectCover.transform.rotation *= Quaternion.Euler(euler);
        cornerEffect.Play();
        if (!rideCharacter.IsCpu) {
            SingletonCustom<AudioManager>.Instance.SePlay("se_WaterSlider_Waterlanding");
        }
    }
    private void OnCollisionExit(Collision collision) {
        if (isGround && collision.gameObject.layer == LayerMask.NameToLayer("Field")) {
            isGround = false;
            flyPos = base.transform.position;
        }
    }
    private void Accele(bool x) {
        if (x) {
            if (rb.velocity.sqrMagnitude < 25f) {
                rb.AddForce((bankTop.transform.position - rideCharacter.gameObject.transform.position) * acceleration * 3f);
            }
        } else {
            rb.AddForce(rideCharacter.gameObject.transform.forward * acceleration * (1f + addOnSpeed));
        }
    }
    private void Speedup(bool _isSlipStream = true) {
        if (SingletonCustom<WaterSlider_GameManager>.Instance.CurrentState != WaterSlider_GameManager.State.GOAL_WAIT) {
            WaterSlider_GameManager.State currentState2 = SingletonCustom<WaterSlider_GameManager>.Instance.CurrentState;
        }
        if (!rideCharacter.IsCpu) {
            SingletonCustom<HidVibration>.Instance.SetCustomVibration(rideCharacter.PlayerNo, HidVibration.VibrationType.Normal, 0.15f);
        }
        addOnSpeed += 10f;
        rb.velocity *= 5f;
        whirlEffectTime = (_isSlipStream ? 0f : 0.75f);
        if (!_isSlipStream) {
            addSpeedLineTime = 0.75f;
        }
        rideCharacter.CameraMover.AddSpeed();
    }
    private void CharaAnimation() {
        handle = Mathf.Lerp(handle, 1f - (1f - rideCharacter.GetInputHorizontal()) * 0.5f, animeTime * Time.deltaTime);
        animator.SetFloat("Handle", handle);
    }
    private void CharaAnimationRight() {
        animator.SetTrigger("ToRightBack");
    }
    private void CharaAnimationLeft() {
        animator.SetTrigger("ToLeftBack");
    }
    private void CharaAnimationFront() {
        animator.SetTrigger("ToRun");
        switch (UnityEngine.Random.Range(0, 3)) {
            case 2:
                animeTimePace_Range = 1f;
                break;
            case 1:
                animeTimePace_Range = -1f;
                break;
            default:
                animeTimePace_Range = 0f;
                break;
        }
    }
    public float GetPower() {
        return impact;
    }
    public void SetPower(float _impact) {
        impact = _impact;
    }
    public void SetShock(bool _shock) {
        shock = _shock;
    }
    public bool GetShock() {
        return shock;
    }
    public Vector3 GetShockVec() {
        return shockVec;
    }
    public void SetShockVec(Vector3 _shockVec) {
        shockVec = _shockVec;
    }
    public bool GetTunnel() {
        return tunnel;
    }
    public WaterSlider_Character GetRideChara() {
        return rideCharacter;
    }
    private void AnimeCont() {
        switch (_animetionMode) {
            case AnimetionMode.RANK:
                if (rideCharacter.CurrentRank != 3) {
                    if (UnityEngine.Random.Range(0, 2) == 1) {
                        CharaAnimationRight();
                        _animetionMode = AnimetionMode.LOOKBACK;
                    } else {
                        CharaAnimationLeft();
                        _animetionMode = AnimetionMode.LOOKBACK;
                    }
                }
                break;
            case AnimetionMode.LOOKBACK:
                anime_TimeKeep += Time.deltaTime;
                if (anime_TimeKeep > animeTimePaceReturn + animeTimePace + animeTimePace_Range) {
                    _animetionMode = AnimetionMode.LOOKFRONT;
                }
                break;
            case AnimetionMode.LOOKFRONT:
                _animetionMode = AnimetionMode.RANK;
                CharaAnimationFront();
                anime_TimeKeep = 0f;
                break;
        }
    }
    public bool GetBank() {
        return bank;
    }
}
