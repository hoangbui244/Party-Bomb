using System;
using System.Collections.Generic;
using UnityEngine;
public class RA_Character : MonoBehaviour {
    public enum State {
        NONE,
        MOVE_POS,
        MOVE_CROWN,
        MOVE_LADDER,
        WINNER_WAIT,
        CROWN_WAIT,
        GET_CROWN,
        WINNER,
        WINNER_THREE_OR_MORE,
        LOSE,
        FADE_WAIT,
        STANDBY,
        JUMP
    }
    [SerializeField]
    [Header("スタイル")]
    private CharacterStyle style;
    [SerializeField]
    [Header("アニメ\u30fcション")]
    private RA_CharacterAnim anim;
    [SerializeField]
    [Header("ヘッドアンカ\u30fc")]
    private Transform anchorHead;
    [SerializeField]
    [Header("シ\u30fcンクラス")]
    private Scene_ResultAnnouncement rootScene;
    private List<Vector3> listTarget = new List<Vector3>();
    private float POS_Y_FIX = 0.015f;
    private State currentState;
    private float rotCompSpeed = 0.45f;
    private Vector3 relativePos;
    private Quaternion look;
    private GameObject objCrown;
    private float crownTime;
    private float waitTime;
    private int currentPlayerNo;
    private float winnerWait;
    private int ranking;
    public void Init(int _playerNo) {
        style.SetMainCharacterStyle(_playerNo);
        currentPlayerNo = _playerNo;
    }
    public void SetWinnerTime(float _time) {
        winnerWait = _time;
    }
    public void AddMovePos(Vector3 _targetPos) {
        listTarget.Add(_targetPos);
    }
    public void SetState(State _state) {
        currentState = _state;
        State state = currentState;
        if (state != 0 && (uint)(state - 1) <= 1u) {
            anim.SetAnim(RA_CharacterAnim.AnimType.WALK);
        }
    }
    public void SetThreeWinnerOrMore() {
        style.SetMainCharacterFaceDiff(currentPlayerNo, StyleTextureManager.MainCharacterFaceType.HAPPY);
        anim.SetAnim(RA_CharacterAnim.AnimType.WINNER);
    }
    public void SetThreeOrMoreLose() {
        style.SetMainCharacterFaceDiff(currentPlayerNo, StyleTextureManager.MainCharacterFaceType.SAD);
        anim.SetAnim(RA_CharacterAnim.AnimType.LOSE);
    }
    public void SetCrown(GameObject _obj) {
        objCrown = _obj;
    }
    public void SetWinner() {
        style.SetMainCharacterFaceDiff(currentPlayerNo, StyleTextureManager.MainCharacterFaceType.HAPPY);
        anim.SetAnim(RA_CharacterAnim.AnimType.WINNER);
        currentState = State.WINNER;
    }
    public void SetLoser() {
        style.SetMainCharacterFaceDiff(currentPlayerNo, StyleTextureManager.MainCharacterFaceType.SAD);
    }
    public void SetRanking(int _ranking) {
        ranking = _ranking;
    }
    private void Update() {
        switch (currentState) {
            case State.NONE:
            case State.FADE_WAIT:
                break;
            case State.MOVE_POS:
                if (listTarget.Count <= 0) {
                    break;
                }
                base.transform.position = Vector3.MoveTowards(base.transform.position, listTarget[0], Time.deltaTime * 0.75f);
                if (Physics.Raycast(base.transform.position + Vector3.up * 50f, Vector3.down, out RaycastHit hitInfo2, 1000f)) {
                    base.transform.SetPositionY(hitInfo2.point.y + POS_Y_FIX);
                }
                CalcManager.mCalcVector3 = listTarget[0];
                CalcManager.mCalcVector3.y = base.transform.position.y;
                relativePos = CalcManager.mCalcVector3 - base.transform.position;
                look = Quaternion.LookRotation(relativePos);
                base.transform.rotation = Quaternion.Slerp(base.transform.rotation, look, rotCompSpeed);
                if (CalcManager.Length(base.transform.position, listTarget[0]) < 0.075f) {
                    listTarget.RemoveAt(0);
                    if (listTarget.Count <= 0) {
                        currentState = State.STANDBY;
                        currentState = State.JUMP;
                        anim.SetAnim(RA_CharacterAnim.AnimType.STANDBY);
                    }
                }
                break;
            case State.MOVE_CROWN:
                if (listTarget.Count <= 0) {
                    break;
                }
                base.transform.position = Vector3.MoveTowards(base.transform.position, listTarget[0], Time.deltaTime * 0.75f);
                if (Physics.Raycast(base.transform.position + Vector3.up * 50f, Vector3.down, out RaycastHit hitInfo3, 1000f)) {
                    base.transform.SetPositionY(hitInfo3.point.y + POS_Y_FIX);
                }
                CalcManager.mCalcVector3 = listTarget[0];
                CalcManager.mCalcVector3.y = base.transform.position.y;
                relativePos = CalcManager.mCalcVector3 - base.transform.position;
                look = Quaternion.LookRotation(relativePos);
                base.transform.rotation = Quaternion.Slerp(base.transform.rotation, look, rotCompSpeed);
                if (CalcManager.Length(base.transform.position, listTarget[0]) < 0.075f) {
                    listTarget.RemoveAt(0);
                    if (listTarget.Count <= 0) {
                        currentState = State.MOVE_LADDER;
                    }
                }
                break;
            case State.MOVE_LADDER:
                if (listTarget.Count <= 0) {
                    break;
                }
                base.transform.position = Vector3.MoveTowards(base.transform.position, listTarget[0], Time.deltaTime * 0.75f);
                CalcManager.mCalcVector3 = listTarget[0];
                CalcManager.mCalcVector3.y = base.transform.position.y;
                relativePos = CalcManager.mCalcVector3 - new Vector3(0f, 0f, -1f) - base.transform.position;
                look = Quaternion.LookRotation(relativePos);
                base.transform.rotation = Quaternion.Slerp(base.transform.rotation, look, rotCompSpeed);
                if (CalcManager.Length(base.transform.position, listTarget[0]) < 0.075f) {
                    listTarget.RemoveAt(0);
                    if (listTarget.Count <= 0) {
                        currentState = State.STANDBY;
                        anim.SetAnim(RA_CharacterAnim.AnimType.STANDBY);
                        LeanTween.delayedCall((winnerWait > 0f) ? winnerWait : 0f, (Action)delegate {
                            currentState = State.WINNER_WAIT;
                            anim.SetAnim(RA_CharacterAnim.AnimType.WINNER);
                            style.SetMainCharacterFaceDiff(currentPlayerNo, StyleTextureManager.MainCharacterFaceType.HAPPY);
                            LeanTween.delayedCall(1.75f, (Action)delegate {
                                currentState = State.GET_CROWN;
                                LeanTween.delayedCall(0.075f, (Action)delegate {
                                    anim.SetAnim(RA_CharacterAnim.AnimType.STICK);
                                });
                            });
                        });
                    }
                }
                break;
            case State.STANDBY:
                relativePos = base.transform.position - new Vector3(0f, 0f, 1f) - base.transform.position;
                look = Quaternion.LookRotation(relativePos);
                base.transform.rotation = Quaternion.Slerp(base.transform.rotation, look, rotCompSpeed);
                break;
            case State.JUMP:
                if (Physics.Raycast(base.transform.position + Vector3.up * 50f, Vector3.down, out RaycastHit hitInfo, 1000f) && hitInfo.point.y + POS_Y_FIX > base.transform.position.y) {
                    base.transform.SetPositionY(hitInfo.point.y + POS_Y_FIX);
                }
                relativePos = base.transform.position - new Vector3(0f, 0f, 1f) - base.transform.position;
                look = Quaternion.LookRotation(relativePos);
                base.transform.rotation = Quaternion.Slerp(base.transform.rotation, look, rotCompSpeed);
                break;
            case State.WINNER_WAIT:
                relativePos = base.transform.position - new Vector3(0f, 0f, 1f) - base.transform.position;
                look = Quaternion.LookRotation(relativePos);
                base.transform.rotation = Quaternion.Slerp(base.transform.rotation, look, rotCompSpeed);
                break;
            case State.CROWN_WAIT:
                relativePos = new Vector3(2000f, base.transform.position.y, base.transform.position.z) - base.transform.position;
                look = Quaternion.LookRotation(relativePos);
                base.transform.rotation = Quaternion.Slerp(base.transform.rotation, look, rotCompSpeed * 0.8f);
                break;
            case State.GET_CROWN:
                relativePos = new Vector3(2000f, base.transform.position.y, base.transform.position.z) - base.transform.position;
                look = Quaternion.LookRotation(relativePos);
                base.transform.rotation = Quaternion.Slerp(base.transform.rotation, look, rotCompSpeed * 0.8f);
                waitTime += Time.deltaTime;
                if (waitTime >= 1.75f) {
                    currentState = State.WINNER;
                    waitTime = 0f;
                }
                break;
            case State.WINNER:
                relativePos = new Vector3(2000f, base.transform.position.y, base.transform.position.z) - base.transform.position;
                look = Quaternion.LookRotation(relativePos);
                waitTime += Time.deltaTime;
                if (waitTime >= 5.25f) {
                    if (winnerWait <= 0f) {
                        SingletonCustom<SceneManager>.Instance.FadeExec(delegate {
                            rootScene.CallResult();
                        });
                    }
                    currentState = State.FADE_WAIT;
                }
                break;
            case State.WINNER_THREE_OR_MORE:
                relativePos = base.transform.position - new Vector3(0f, 0f, 1f) - base.transform.position;
                look = Quaternion.LookRotation(relativePos);
                base.transform.rotation = Quaternion.Slerp(base.transform.rotation, look, rotCompSpeed);
                waitTime += Time.deltaTime;
                if (waitTime >= 3.25f) {
                    if (winnerWait <= 0f) {
                        SingletonCustom<SceneManager>.Instance.FadeExec(delegate {
                            rootScene.CallResult();
                        });
                    }
                    currentState = State.FADE_WAIT;
                }
                break;
            case State.LOSE:
                relativePos = base.transform.position - new Vector3(0f, 0f, 1f) - base.transform.position;
                look = Quaternion.LookRotation(relativePos);
                base.transform.rotation = Quaternion.Slerp(base.transform.rotation, look, rotCompSpeed);
                waitTime += Time.deltaTime;
                if (waitTime >= 3.25f) {
                    currentState = State.FADE_WAIT;
                }
                break;
        }
    }
    private void OnDestroy() {
        LeanTween.cancel(base.gameObject);
    }
}
