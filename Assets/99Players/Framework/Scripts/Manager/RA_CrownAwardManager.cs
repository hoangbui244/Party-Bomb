using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public class RA_CrownAwardManager : SingletonCustom<RA_CrownAwardManager> {
    [Header("デバッグ:1位（1人）確定演出")]
    public bool isWinnerOneEnable;
    [Header("デバッグ:1位（2人）確定演出")]
    public bool isWinnerTwoEnable;
    [Header("デバッグ:1位（3人）確定演出")]
    public bool isWinnerThreeEnable;
    [Header("デバッグ:1位（4人）確定演出")]
    public bool isWinnerFourEnable;
    [SerializeField]
    [Header("キャラクタ\u30fcクラス")]
    private RA_Character[] arrayCharacter;
    [SerializeField]
    [Header("カメラ管理クラス")]
    private RA_CameraMover camera;
    [SerializeField]
    [Header("アンカ\u30fc：開始位置（左）")]
    private Transform anchorBranchPointLeftStart;
    [SerializeField]
    [Header("アンカ\u30fc：分岐点（左）")]
    private Transform anchorBranchPointLeft;
    [SerializeField]
    [Header("アンカ\u30fc：分岐点2（左）")]
    private Transform anchorBranchPointLeft2;
    [SerializeField]
    [Header("アンカ\u30fc：開始位置（右）")]
    private Transform anchorBranchPointRightStart;
    [SerializeField]
    [Header("アンカ\u30fc：分岐点（右）")]
    private Transform anchorBranchPointRight;
    [SerializeField]
    [Header("アンカ\u30fc：分岐点2（右）")]
    private Transform anchorBranchPointRight2;
    [SerializeField]
    [Header("アンカ\u30fc:分岐点")]
    private Transform anchorBranchPoint;
    [SerializeField]
    [Header("アンカ\u30fc:優勝地点")]
    private Transform[] anchorWinner;
    [SerializeField]
    [Header("ファンタジ\u30fcクエスト：アンカ\u30fc配列")]
    private Transform[] anchorFantasy;
    [SerializeField]
    [Header("アンカ\u30fcOne:4位")]
    private Transform anchorBranchPoint3;
    [SerializeField]
    [Header("アンカ\u30fcOne:3位")]
    private Transform anchorBranchPoint2;
    [SerializeField]
    [Header("アンカ\u30fcOne:2位")]
    private Transform anchorBranchPoint1;
    [SerializeField]
    [Header("アンカ\u30fcOne:1位")]
    private Transform anchorCrownPoint;
    [SerializeField]
    [Header("クラウンOne:0")]
    private GameObject crownOne;
    [SerializeField]
    [Header("アンカ\u30fcTwo:4位")]
    private Transform anchorBranchPointTwo3;
    [SerializeField]
    [Header("アンカ\u30fcTwo:3位")]
    private Transform anchorBranchPointTwo2;
    [SerializeField]
    [Header("アンカ\u30fcTwo:2位")]
    private Transform anchorBranchPointTwo1;
    [SerializeField]
    [Header("アンカ\u30fcTwo:1位")]
    private Transform anchorCrownPointTwo;
    [SerializeField]
    [Header("クラウンTwo:0")]
    private GameObject crownTwo0;
    [SerializeField]
    [Header("クラウンTwo:1")]
    private GameObject crownTwo1;
    [SerializeField]
    [Header("アンカ\u30fcThree:4位")]
    private Transform anchorBranchPointThree3;
    [SerializeField]
    [Header("アンカ\u30fcThree:3位")]
    private Transform anchorBranchPointThree2;
    [SerializeField]
    [Header("アンカ\u30fcThree:2位")]
    private Transform anchorBranchPointThree1;
    [SerializeField]
    [Header("アンカ\u30fcThree:1位")]
    private Transform anchorBranchPointThree0;
    [SerializeField]
    [Header("クラウンThree:0")]
    private GameObject crownThree0;
    [SerializeField]
    [Header("クラウンThree:1")]
    private GameObject crownThree1;
    [SerializeField]
    [Header("クラウンThree:2")]
    private GameObject crownThree2;
    [SerializeField]
    [Header("アンカ\u30fcFour:4位")]
    private Transform anchorBranchPointFour3;
    [SerializeField]
    [Header("アンカ\u30fcFour:3位")]
    private Transform anchorBranchPointFour2;
    [SerializeField]
    [Header("アンカ\u30fcFour:2位")]
    private Transform anchorBranchPointFour1;
    [SerializeField]
    [Header("アンカ\u30fcFour:1位")]
    private Transform anchorBranchPointFour0;
    [SerializeField]
    [Header("クラウンFour:0")]
    private GameObject crownFour0;
    [SerializeField]
    [Header("クラウンFour:1")]
    private GameObject crownFour1;
    [SerializeField]
    [Header("クラウンFour:2")]
    private GameObject crownFour2;
    [SerializeField]
    [Header("クラウンFour:3")]
    private GameObject crownFour3;
    [SerializeField]
    [Header("台座パタ\u30fcンオブジェクト")]
    private GameObject[] arrayWinnerObj;
    [SerializeField]
    [Header("紙吹雪")]
    private GameObject objConfetti;
    [SerializeField]
    [Header("キャプション")]
    private SpriteRenderer spRenderCaption;
    [SerializeField]
    [Header("キャプション裏")]
    private SpriteRenderer spRenderCaptionBack;
    [SerializeField]
    [Header("表彰台メッシュ")]
    private MeshFilter[] arrayPlatformMesh;
    [SerializeField]
    [Header("メッシュコライダ\u30fc")]
    private MeshCollider[] arrayPlatformMeshCol;
    [SerializeField]
    [Header("表彰台メッシュ")]
    private Mesh[] platformMesh;
    [SerializeField]
    [Header("ライト")]
    private Light mapLight;
    [SerializeField]
    [Header("演出ライト")]
    private GameObject[] arrayTopLight;
    [SerializeField]
    [Header("演出ライト値")]
    private Light[] arrayTopLightParam;
    [SerializeField]
    [Header("ライトアニメ\u30fcタ\u30fc1")]
    private Animator[] arrayTopLightAnim;
    private int[] finalRanking;
    private int[] checkRanking;
    public void Init() {
        switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
            case 1:
                arrayCharacter[0].Init(0);
                arrayCharacter[1].Init(4);
                arrayCharacter[2].Init(5);
                arrayCharacter[3].Init(6);
                break;
            case 2:
                arrayCharacter[0].Init(0);
                arrayCharacter[1].Init(1);
                arrayCharacter[2].Init(4);
                arrayCharacter[3].Init(5);
                break;
            case 3:
                arrayCharacter[0].Init(0);
                arrayCharacter[1].Init(1);
                arrayCharacter[2].Init(2);
                arrayCharacter[3].Init(4);
                break;
            case 4:
                arrayCharacter[0].Init(0);
                arrayCharacter[1].Init(1);
                arrayCharacter[2].Init(2);
                arrayCharacter[3].Init(3);
                break;
        }
        finalRanking = ResultGameDataParams.GetPlayKingFinalRanking();
        int[] array = new int[4];
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < finalRanking.Length; j++) {
                if (finalRanking[j] == i) {
                    array[i] = j;
                }
            }
        }
        checkRanking = array;
        for (int k = 0; k < arrayPlatformMesh.Length; k++) {
            UnityEngine.Debug.Log("[temp]> i:" + k.ToString() + "check:" + checkRanking[k].ToString() + " final:" + finalRanking[k].ToString());
        }
        int num = 0;
        int num2 = -1;
        int num3 = 1;
        array = ResultGameDataParams.GetPlayKingFinalRanking();
        for (int l = 0; l < 4; l++) {
            if (checkRanking[l] == 0) {
                num2 = SingletonCustom<GameSettingManager>.Instance.GetSportsDayTotalTeamScore(array[l]);
            }
        }
        for (int m = 0; m < 4; m++) {
            UnityEngine.Debug.Log("currentScore:" + num2.ToString() + " teamScore:" + SingletonCustom<GameSettingManager>.Instance.GetSportsDayTotalTeamScore(array[m]).ToString());
            if (num2 == SingletonCustom<GameSettingManager>.Instance.GetSportsDayTotalTeamScore(array[m])) {
                checkRanking[m] = num;
            }
        }
        int num4 = 0;
        for (int n = 0; n < finalRanking.Length; n++) {
            if (finalRanking[n] == 0) {
                num4++;
            }
        }
        UnityEngine.Debug.Log("王冠数:" + num4.ToString());
        num4 = 1;
        if (num4 == 2) {
            for (int num5 = 0; num5 < finalRanking.Length; num5++) {
                if (finalRanking[num5] > 0) {
                    finalRanking[num5]++;
                }
            }
        }
        List<int> list = new List<int>();
        for (int num6 = 0; num6 < checkRanking.Length; num6++) {
            list.Add(checkRanking[num6]);
        }
        list.Sort();
        int num7 = list[0];
        num3 = 0;
        for (int num8 = 0; num8 < arrayPlatformMesh.Length; num8++) {
            UnityEngine.Debug.Log("i:" + num8.ToString() + "check:" + checkRanking[num8].ToString() + " final:" + finalRanking[num8].ToString());
            if (num7 != list[num8]) {
                num7 += num3;
                num3 = 1;
            } else {
                num3++;
            }
            arrayPlatformMesh[num8].mesh = platformMesh[num7];
            arrayPlatformMeshCol[num8].sharedMesh = platformMesh[num7];
            checkRanking[num8] = num7;
        }
        StartCoroutine(_CrownFantasy());
        WaitAfterExec(0.7f, delegate {
            camera.SetCameraState(RA_CameraMover.State.START_ZOOM);
        });
    }
    private IEnumerator _CrownFantasy() {
        yield return new WaitForSeconds(1.25f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy", _loop: false, 0f, 1f, 1f, 1.05f);
        int num;
        for (int i = 0; i < arrayCharacter.Length; i = num) {
            arrayCharacter[i].transform.position = anchorFantasy[0].position;
            arrayCharacter[i].AddMovePos(anchorFantasy[1].position);
            arrayCharacter[i].AddMovePos(anchorFantasy[2].position);
            arrayCharacter[i].AddMovePos(anchorFantasy[3].position);
            arrayCharacter[i].AddMovePos(anchorFantasy[4].position);
            arrayCharacter[i].AddMovePos(anchorFantasy[5].position);
            arrayCharacter[i].AddMovePos(anchorFantasy[6 + i].position);
            arrayCharacter[i].SetState(RA_Character.State.MOVE_POS);
            arrayCharacter[i].gameObject.SetActive(value: true);
            arrayCharacter[i].SetRanking(checkRanking[i]);
            yield return new WaitForSeconds(0.5f);
            num = i + 1;
        }
        yield return new WaitForSeconds(2f);
        camera.SetCameraState(RA_CameraMover.State.CROWN_ZOOM);
        yield return new WaitForSeconds(4f);
        LeanTween.value(base.gameObject, 1f, 0.125f, 1f).setOnUpdate(delegate (float _value) {
            mapLight.color = new Color(_value, _value, _value);
        });
        yield return new WaitForSeconds(0.5f);
        for (int j = 0; j < arrayTopLight.Length; j++) {
            arrayTopLight[finalRanking[j]].SetActive(value: true);
            arrayTopLight[finalRanking[j]].transform.SetLocalScaleY(0f);
            arrayTopLightAnim[finalRanking[j]].enabled = true;
            arrayTopLightParam[finalRanking[j]].range = 0f;
        }
        LeanTween.value(base.gameObject, 0f, 1f, 0.7f).setOnUpdate(delegate (float _value) {
            for (int num3 = 0; num3 < arrayTopLight.Length; num3++) {
                arrayTopLightParam[num3].range = _value * 1.63f;
                arrayTopLight[num3].transform.SetLocalScaleY(15f * _value);
            }
        });
        SingletonCustom<AudioManager>.Instance.SePlay("se_sugoroku_drum_roll", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
        yield return new WaitForSeconds(2f);
        SingletonCustom<AudioManager>.Instance.SePlay("SE_seien");
        yield return new WaitForSeconds(1.25f);
        LeanTween.cancel(base.gameObject);
        LeanTween.value(base.gameObject, 1f, 0f, 0.7f).setOnUpdate(delegate (float _value) {
            for (int num2 = 0; num2 < arrayTopLight.Length; num2++) {
                arrayTopLightParam[num2].range = _value * 1.63f;
                arrayTopLight[num2].transform.SetLocalScaleY(15f * _value);
            }
        });
        yield return new WaitForSeconds(0.75f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
        SingletonCustom<AudioManager>.Instance.SePlay("se_heal");
        int highScore = 0;
        int lowScore = 10000;
        for (int k = 0; k < 4; k++) {
            if (highScore < SingletonCustom<GameSettingManager>.Instance.GetSportsDayTotalTeamScore(k)) {
                highScore = SingletonCustom<GameSettingManager>.Instance.GetSportsDayTotalTeamScore(k);
            }
            if (lowScore > SingletonCustom<GameSettingManager>.Instance.GetSportsDayTotalTeamScore(k)) {
                lowScore = SingletonCustom<GameSettingManager>.Instance.GetSportsDayTotalTeamScore(k);
            }
        }
        for (int l = 0; l < arrayTopLight.Length; l++) {
            if (highScore == SingletonCustom<GameSettingManager>.Instance.GetSportsDayTotalTeamScore(l)) {
                arrayTopLight[l].SetActive(value: true);
                arrayTopLightParam[l].range = 1.63f;
                arrayTopLight[l].transform.SetLocalScaleY(0f);
                arrayTopLightAnim[l].enabled = false;
                arrayTopLight[l].transform.SetLocalEulerAngles(0f, 0f, 0f);
            } else {
                arrayTopLight[l].SetActive(value: false);
            }
        }
        LeanTween.value(base.gameObject, 0f, 15f, 0.25f).setOnUpdate(delegate (float _value) {
            for (int n = 0; n < arrayTopLight.Length; n++) {
                if (arrayTopLight[n].activeSelf) {
                    arrayTopLight[n].transform.SetLocalScaleY(_value);
                }
            }
        });
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
        objConfetti.SetActive(value: true);
        camera.SetCameraState(RA_CameraMover.State.WINNER_ZOOM);
        yield return new WaitForSeconds(0.15f);
        for (int m = 0; m < arrayCharacter.Length; m++) {
            if (lowScore == SingletonCustom<GameSettingManager>.Instance.GetSportsDayTotalTeamScore(m)) {
                arrayCharacter[m].SetLoser();
            }
            if (highScore == SingletonCustom<GameSettingManager>.Instance.GetSportsDayTotalTeamScore(m)) {
                arrayCharacter[m].SetWinner();
            }
        }
        yield return new WaitForSeconds(0.25f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_lastresult", _loop: false, 0f, 1f, 1f, 0.25f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy", _loop: false, 0f, 1f, 1f, 0.25f);
        spRenderCaption.gameObject.SetActive(value: true);
        spRenderCaption.transform.SetLocalScale(3f, 3f, 1f);
        LeanTween.scale(spRenderCaption.gameObject, Vector3.one, 1f).setEaseOutBack().setOnComplete((Action)delegate {
            LeanTween.scale(spRenderCaptionBack.gameObject, Vector3.one * 1.5f, 1.5f);
            LeanTween.value(0.75f, 0f, 1.05f).setOnUpdate(delegate (float value) {
                spRenderCaptionBack.color = new Color(1f, 1f, 1f, value);
            });
        });
    }
    private IEnumerator _CrownOne() {
        yield return new WaitForSeconds(3f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy", _loop: false, 0f, 1f, 1f, 1.05f);
        for (int i = 0; i < arrayCharacter.Length; i++) {
            if (finalRanking[3] == i) {
                arrayCharacter[i].transform.position = anchorBranchPointLeftStart.position;
                arrayCharacter[i].AddMovePos(anchorBranchPointLeft.position);
                arrayCharacter[i].AddMovePos(anchorBranchPointLeft2.position);
                arrayCharacter[i].AddMovePos(anchorBranchPoint3.position);
                arrayCharacter[i].SetState(RA_Character.State.MOVE_POS);
                arrayCharacter[i].gameObject.SetActive(value: true);
                arrayCharacter[i].SetRanking(checkRanking[3]);
            }
        }
        yield return new WaitForSeconds(3f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy", _loop: false, 0f, 1f, 1f, 1.05f);
        for (int j = 0; j < arrayCharacter.Length; j++) {
            if (finalRanking[2] == j) {
                arrayCharacter[j].transform.position = anchorBranchPointRightStart.position;
                arrayCharacter[j].AddMovePos(anchorBranchPointRight.position);
                arrayCharacter[j].AddMovePos(anchorBranchPointRight2.position);
                arrayCharacter[j].AddMovePos(anchorBranchPoint2.position);
                arrayCharacter[j].SetState(RA_Character.State.MOVE_POS);
                arrayCharacter[j].gameObject.SetActive(value: true);
                arrayCharacter[j].SetRanking(checkRanking[2]);
            }
        }
        yield return new WaitForSeconds(3f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy", _loop: false, 0f, 1f, 1f, 1.05f);
        for (int k = 0; k < arrayCharacter.Length; k++) {
            if (finalRanking[1] == k) {
                arrayCharacter[k].transform.position = anchorBranchPointLeftStart.position;
                arrayCharacter[k].AddMovePos(anchorBranchPointLeft.position);
                arrayCharacter[k].AddMovePos(anchorBranchPointLeft2.position);
                arrayCharacter[k].AddMovePos(anchorBranchPoint1.position);
                arrayCharacter[k].SetState(RA_Character.State.MOVE_POS);
                arrayCharacter[k].gameObject.SetActive(value: true);
                arrayCharacter[k].SetRanking(checkRanking[1]);
            }
        }
        yield return new WaitForSeconds(3f);
        for (int l = 0; l < arrayCharacter.Length; l++) {
            if (finalRanking[0] == l) {
                arrayCharacter[l].transform.position = anchorBranchPointRightStart.position;
                arrayCharacter[l].AddMovePos(anchorBranchPointRight.position);
                arrayCharacter[l].AddMovePos(anchorBranchPointRight2.position);
                arrayCharacter[l].AddMovePos(anchorCrownPoint.position);
                arrayCharacter[l].SetState(RA_Character.State.MOVE_POS);
                arrayCharacter[l].gameObject.SetActive(value: true);
                arrayCharacter[l].SetRanking(checkRanking[0]);
            }
        }
        yield return new WaitForSeconds(0.5f);
        SingletonCustom<AudioManager>.Instance.SePlay("SE_seien");
        yield return new WaitForSeconds(1f);
        camera.SetCameraState(RA_CameraMover.State.CROWN_ZOOM);
        yield return new WaitForSeconds(1.5f);
        yield return new WaitForSeconds(1.5f);
        for (int m = 0; m < arrayCharacter.Length; m++) {
            if (checkRanking[m] == 0) {
                arrayCharacter[finalRanking[m]].SetWinner();
            }
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
        objConfetti.SetActive(value: true);
        camera.SetCameraState(RA_CameraMover.State.WINNER_ZOOM);
        yield return new WaitForSeconds(1.05f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
        SingletonCustom<AudioManager>.Instance.SePlay("se_heal");
        yield return new WaitForSeconds(0.25f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_lastresult", _loop: false, 0f, 1f, 1f, 0.25f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy", _loop: false, 0f, 1f, 1f, 0.25f);
        spRenderCaption.gameObject.SetActive(value: true);
        spRenderCaption.transform.SetLocalScale(3f, 3f, 1f);
        LeanTween.scale(spRenderCaption.gameObject, Vector3.one, 1f).setEaseOutBack().setOnComplete((Action)delegate {
            LeanTween.scale(spRenderCaptionBack.gameObject, Vector3.one * 1.5f, 1.5f);
            LeanTween.value(0.75f, 0f, 1.05f).setOnUpdate(delegate (float value) {
                spRenderCaptionBack.color = new Color(1f, 1f, 1f, value);
            });
        });
    }
    private IEnumerator _CrownTwo() {
        yield return new WaitForSeconds(3f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy", _loop: false, 0f, 1f, 1f, 1.25f);
        for (int k = 0; k < arrayCharacter.Length; k++) {
            if (finalRanking[k] == 3) {
                arrayCharacter[k].AddMovePos(anchorBranchPoint.position);
                arrayCharacter[k].AddMovePos(anchorBranchPointTwo3.position);
                arrayCharacter[k].SetState(RA_Character.State.MOVE_POS);
                arrayCharacter[k].gameObject.SetActive(value: true);
            }
        }
        yield return new WaitForSeconds(3f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy", _loop: false, 0f, 1f, 1f, 1.25f);
        for (int l = 0; l < arrayCharacter.Length; l++) {
            if (finalRanking[l] == 2) {
                arrayCharacter[l].AddMovePos(anchorBranchPoint.position);
                arrayCharacter[l].AddMovePos(anchorBranchPointTwo2.position);
                arrayCharacter[l].SetState(RA_Character.State.MOVE_POS);
                arrayCharacter[l].gameObject.SetActive(value: true);
            }
        }
        yield return new WaitForSeconds(3f);
        int winnerCnt2 = 0;
        float[] winnerPos = new float[2]
        {
            -0.28f,
            0.28f
        };
        int num;
        for (int j = 0; j < arrayCharacter.Length; j = num) {
            if (finalRanking[j] == 0) {
                switch (winnerCnt2) {
                    case 0:
                        arrayCharacter[j].AddMovePos(anchorBranchPointTwo1.position);
                        arrayCharacter[j].SetState(RA_Character.State.MOVE_CROWN);
                        arrayCharacter[j].SetCrown(crownTwo0);
                        break;
                    case 1:
                        arrayCharacter[j].AddMovePos(anchorCrownPointTwo.position);
                        arrayCharacter[j].SetState(RA_Character.State.MOVE_CROWN);
                        arrayCharacter[j].SetCrown(crownTwo1);
                        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
                        break;
                }
                arrayCharacter[j].transform.SetLocalPositionX(winnerPos[winnerCnt2]);
                arrayCharacter[j].gameObject.SetActive(value: true);
                num = winnerCnt2 + 1;
                winnerCnt2 = num;
                yield return new WaitForSeconds(1.5f);
            }
            num = j + 1;
        }
        yield return new WaitForSeconds(0.5f);
        camera.SetCameraState(RA_CameraMover.State.CROWN_ZOOM);
        SingletonCustom<AudioManager>.Instance.SePlay("SE_seien");
        winnerCnt2 = 0;
        for (int j = 0; j < arrayCharacter.Length; j = num) {
            if (finalRanking[j] == 0) {
                Vector3 position = anchorWinner[winnerCnt2].position;
                position.x = arrayCharacter[j].transform.position.x;
                position.z = arrayCharacter[j].transform.position.z;
                arrayCharacter[j].AddMovePos(position);
                if (winnerCnt2 == 0) {
                    arrayCharacter[j].SetWinnerTime(2.55f);
                }
                if (winnerCnt2 == 1) {
                    position.z = arrayCharacter[j].transform.position.z + 0.5f;
                    arrayCharacter[j].AddMovePos(position);
                    position.x = anchorWinner[winnerCnt2].position.x + 0.1f;
                    arrayCharacter[j].AddMovePos(position);
                }
                arrayCharacter[j].AddMovePos(anchorWinner[winnerCnt2].position);
                arrayCharacter[j].SetState(RA_Character.State.MOVE_LADDER);
                num = winnerCnt2 + 1;
                winnerCnt2 = num;
                yield return new WaitForSeconds(1.5f);
            }
            num = j + 1;
        }
        objConfetti.SetActive(value: true);
        camera.SetCameraState(RA_CameraMover.State.WINNER_ZOOM);
        yield return new WaitForSeconds(1.5f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
        SingletonCustom<AudioManager>.Instance.SePlay("se_heal");
        yield return new WaitForSeconds(1.5f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_lastresult", _loop: false, 0f, 1f, 1f, 0.25f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy", _loop: false, 0f, 1f, 1f, 0.25f);
        spRenderCaption.gameObject.SetActive(value: true);
        spRenderCaption.transform.SetLocalScale(3f, 3f, 1f);
        LeanTween.scale(spRenderCaption.gameObject, Vector3.one, 1f).setEaseOutBack().setOnComplete((Action)delegate {
            LeanTween.scale(spRenderCaptionBack.gameObject, Vector3.one * 1.5f, 1.5f);
            LeanTween.value(1f, 0f, 0.05f).setOnUpdate(delegate (float value) {
                spRenderCaptionBack.color = new Color(1f, 1f, 1f, value);
            });
        });
    }
    private IEnumerator _CrownFour() {
        yield return new WaitForSeconds(2f);
        //??RuntimeHelpers.InitializeArray(new float[4], (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy", _loop: false, 0f, 1f, 1f, 1.25f);
        arrayCharacter[0].AddMovePos(anchorBranchPoint.position);
        arrayCharacter[0].AddMovePos(anchorBranchPointFour3.position);
        arrayCharacter[0].SetState(RA_Character.State.MOVE_POS);
        arrayCharacter[0].gameObject.SetActive(value: true);
        yield return new WaitForSeconds(3f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy", _loop: false, 0f, 1f, 1f, 1.25f);
        arrayCharacter[1].AddMovePos(anchorBranchPoint.position);
        arrayCharacter[1].AddMovePos(anchorBranchPointFour2.position);
        arrayCharacter[1].SetState(RA_Character.State.MOVE_POS);
        arrayCharacter[1].gameObject.SetActive(value: true);
        yield return new WaitForSeconds(3f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy", _loop: false, 0f, 1f, 1f, 1.25f);
        arrayCharacter[2].AddMovePos(anchorBranchPoint.position);
        arrayCharacter[2].AddMovePos(anchorBranchPointFour1.position);
        arrayCharacter[2].SetState(RA_Character.State.MOVE_POS);
        arrayCharacter[2].gameObject.SetActive(value: true);
        yield return new WaitForSeconds(3f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy", _loop: false, 0f, 1f, 1f, 1.25f);
        arrayCharacter[3].AddMovePos(anchorBranchPoint.position);
        arrayCharacter[3].AddMovePos(anchorBranchPointFour0.position);
        arrayCharacter[3].SetState(RA_Character.State.MOVE_POS);
        arrayCharacter[3].gameObject.SetActive(value: true);
        yield return new WaitForSeconds(0.5f);
        camera.SetCameraState(RA_CameraMover.State.THREE_OR_MORE_ZOOM);
        SingletonCustom<AudioManager>.Instance.SePlay("SE_seien");
        yield return new WaitForSeconds(3.25f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
        objConfetti.SetActive(value: true);
        camera.SetCameraState(RA_CameraMover.State.THREE_OR_MORE_ZOOM);
        yield return new WaitForSeconds(1f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy");
        SingletonCustom<AudioManager>.Instance.SePlay("se_heal");
        yield return new WaitForSeconds(0.15f);
        for (int i = 0; i < arrayCharacter.Length; i++) {
            if (finalRanking[i] == 0) {
                arrayCharacter[i].SetThreeWinnerOrMore();
                arrayCharacter[i].SetState(RA_Character.State.WINNER_THREE_OR_MORE);
            } else {
                arrayCharacter[i].SetThreeOrMoreLose();
                arrayCharacter[i].SetState(RA_Character.State.WINNER_THREE_OR_MORE);
            }
        }
        SingletonCustom<AudioManager>.Instance.SePlay("se_result_lastresult", _loop: false, 0f, 1f, 1f, 0.25f);
        SingletonCustom<AudioManager>.Instance.SePlay("se_cheer_joy", _loop: false, 0f, 1f, 1f, 0.25f);
        spRenderCaption.gameObject.SetActive(value: true);
        spRenderCaption.transform.SetLocalScale(3f, 3f, 1f);
        LeanTween.scale(spRenderCaption.gameObject, Vector3.one, 1f).setEaseOutBack().setOnComplete((Action)delegate {
            LeanTween.scale(spRenderCaptionBack.gameObject, Vector3.one * 1.5f, 1.5f);
            LeanTween.value(1f, 0f, 0.05f).setOnUpdate(delegate (float value) {
                spRenderCaptionBack.color = new Color(1f, 1f, 1f, value);
            });
        });
    }
    private new void OnDestroy() {
        LeanTween.cancel(spRenderCaption.gameObject);
        LeanTween.cancel(spRenderCaptionBack.gameObject);
        LeanTween.cancel(base.gameObject);
    }
}
