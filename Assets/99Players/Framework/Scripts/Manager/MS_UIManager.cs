using System;
using System.Collections;
using UnityEngine;
public class MS_UIManager : SingletonCustom<MS_UIManager> {
    [Serializable]
    public struct TournamentUIData {
        [Header("組のフェ\u30fcド下敷き")]
        public Sprite teamFadeUnderlay;
    }
    [SerializeField]
    [Header("リ\u30fcグ戦UIオブジェクト")]
    private GameObject object_Battle_League_UI;
    [SerializeField]
    [Header("ト\u30fcナメント戦UIオブジェクト")]
    private GameObject object_Battle_Tournament_UI;
    [SerializeField]
    [Header("協力(2vs2)UIオブジェクト")]
    private GameObject object_Battle_Doubles_UI;
    [SerializeField]
    [Header("ト\u30fcナメント戦UI管理処理")]
    private MS_TournamentUIManager tournamentUIManager;
    [SerializeField]
    [Header("各団のカラ\u30fc")]
    private Color[] teamColor;
    [SerializeField]
    [Header("1人用_キッカ\u30fcUI")]
    private GameObject objOperationSingleKicker;
    [SerializeField]
    [Header("1人用_キ\u30fcパ\u30fcUI")]
    private GameObject objOperationSingleKeeper;
    [SerializeField]
    [Header("多人数用_UI")]
    private GameObject objOperationMulti;
    [SerializeField]
    [Header("1人用_CPUスキップ")]
    private GameObject objOperationSingleCpuSkip;
    [SerializeField]
    [Header("多人数用_CPUスキップ")]
    private GameObject objOperationMultiCpuSkip;
    [SerializeField]
    [Header("開始時説明表示キッカ\u30fc")]
    private GameObject objInfoKicker;
    [SerializeField]
    [Header("開始時説明表示キ\u30fcパ\u30fc")]
    private GameObject objInfoKeeper;
    [SerializeField]
    [Header("左テキストレイアウト")]
    private GameObject objLeftTextLayout;
    [SerializeField]
    [Header("左アイコンレイアウト")]
    private GameObject objLeftIconLayout;
    [SerializeField]
    [Header("右テキストレイアウト")]
    private GameObject objRightTextLayout;
    [SerializeField]
    [Header("右アイコンレイアウト")]
    private GameObject objRightIconLayout;
    [SerializeField]
    [Header("キャラ画像配列")]
    private Sprite[] arrayCharacter;
    private bool isBattleLeagueMode;
    private bool isBattleTournamentMode;
    private bool isBattleDoublesMode;
    private Vector3 screenPos;
    private Vector3 markPos;
    public void Init() {
        isBattleLeagueMode = (isBattleTournamentMode = (isBattleDoublesMode = false));
        object_Battle_League_UI.SetActive(value: false);
        object_Battle_Tournament_UI.SetActive(value: false);
        object_Battle_Doubles_UI.SetActive(value: false);
        if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat != GS_Define.GameFormat.COOP) {
            if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE) {
                object_Battle_Tournament_UI.SetActive(value: true);
                isBattleTournamentMode = true;
                tournamentUIManager.Init();
            } else if (SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.BATTLE_AND_COOP) {
                isBattleDoublesMode = true;
            }
        }
        SetPlayerIcon();
    }
    public void SetPlayerIcon(float _hideTime = 3.75f) {
        switch (SingletonCustom<GameSettingManager>.Instance.TeamNum) {
            case 2:
                switch (SingletonCustom<GameSettingManager>.Instance.PlayerNum) {
                    case 2:
                        objOperationSingleKicker.SetActive(value: false);
                        objOperationSingleKeeper.SetActive(value: false);
                        objOperationMulti.SetActive(value: true);
                        break;
                    case 3:
                        objOperationSingleKicker.SetActive(value: false);
                        objOperationSingleKeeper.SetActive(value: false);
                        objOperationMulti.SetActive(value: true);
                        break;
                }
                break;
            case 3:
                objOperationSingleKicker.SetActive(value: false);
                objOperationSingleKeeper.SetActive(value: false);
                objOperationMulti.SetActive(value: true);
                break;
            case 4:
                if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 1) {
                    objOperationSingleKicker.SetActive(value: true);
                    objOperationSingleKeeper.SetActive(value: false);
                    objOperationMulti.SetActive(value: false);
                } else {
                    objOperationSingleKicker.SetActive(value: false);
                    objOperationSingleKeeper.SetActive(value: false);
                    objOperationMulti.SetActive(value: true);
                }
                break;
        }
        WaitAfterExec(_hideTime, delegate {
            HideOperationPlayerIcon();
        });
    }
    public void HideOperationPlayerIcon() {
    }
    public void UpdatePlayerIcon() {
    }
    public void DisableSkip() {
        objOperationSingleCpuSkip.SetActive(value: false);
        objOperationMultiCpuSkip.SetActive(value: false);
    }
    public void UpdateGameTime(float _time) {
    }
    public void StartLeagueUIAnimation(int _roundCount, bool _winnerTeamLeft, bool _isGameEnd, Action _endCallBack) {
    }
    public void SetLeagueTeamData() {
    }
    public void SetLeagueBattleResultData(bool _isWinnerLeftTeam) {
    }
    public void SetTournamentTeamData() {
        tournamentUIManager.SetTeamData(SingletonCustom<MS_GameManager>.Instance.GetTournamentVSTeamData()[0], MS_TournamentUIManager.RoundType.Round_1st);
        tournamentUIManager.SetTeamData(SingletonCustom<MS_GameManager>.Instance.GetTournamentVSTeamData()[1], MS_TournamentUIManager.RoundType.Round_2nd);
    }
    public void StartTournamentUIAnimation(bool _winnerTeamLeft, Action _endCallBack) {
        tournamentUIManager.StartLineAnimation(_winnerTeamLeft, _endCallBack);
    }
    private IEnumerator SetAlphaColor(SpriteRenderer _tk2dSprite, float _fadeTime, float _alpha = 1f, float _delayTime = 0f, bool _isFadeOut = false) {
        float time = 0f;
        Color color = Color.white;
        float startAlpha = 0f;
        float endAlpha = _alpha;
        if (_isFadeOut) {
            startAlpha = _alpha;
            endAlpha = 0f;
        }
        yield return new WaitForSeconds(_delayTime);
        while (time < _fadeTime) {
            color = _tk2dSprite.color;
            color.a = Mathf.Lerp(startAlpha, endAlpha, time / _fadeTime);
            _tk2dSprite.color = color;
            time += Time.deltaTime;
            yield return null;
        }
    }
    public void NextSettingTornament() {
        tournamentUIManager.NextSetting();
    }
    public int[] GetTournamentFinalRoundTeamNoArray() {
        return tournamentUIManager.GetFinalRoundTeamNo();
    }
    public int[] GetTournamentLoserBattleTeamNoArray() {
        return tournamentUIManager.GetLoserBattleTeamNo();
    }
    public MS_TournamentUIManager.RoundType GetNowTournamentType() {
        return tournamentUIManager.CurrentRoundType;
    }
    public int[] GetTournamentWinnerTeamNoList() {
        return tournamentUIManager.GetWinnerTeamNo();
    }
    public string[] GetTournamentMatchTeamNoList() {
        return tournamentUIManager.GetMatchTeamNo();
    }
    public void Debug_SetTournamentWinnerTeamNoList() {
        tournamentUIManager.Debug_TournamentWinnerList();
    }
    public void StartTeamNameAnimation() {
        StartCoroutine(UpdateTeamDataAnimation());
    }
    private IEnumerator UpdateTeamDataAnimation() {
        yield return new WaitForSeconds(3f);
        yield return new WaitForSeconds(0.6f);
    }
    private void StartTeamNameLeanTween(Transform _teamAnchor, float _posY, float _time = 0.5f) {
        LeanTween.moveLocalY(_teamAnchor.gameObject, _posY, _time).setEaseOutQuart();
    }
    public void StartSetAnimation(bool _isLeftTeam) {
        if (!isBattleLeagueMode && !isBattleTournamentMode) {
            bool isBattleDoublesMode2 = isBattleDoublesMode;
        }
    }
    private IEnumerator SetAnimation(SpriteRenderer _sprite) {
        _sprite.SetAlpha(0f);
        _sprite.transform.SetLocalEulerAnglesZ(180f);
        _sprite.transform.SetLocalScale(2f, 2f, 2f);
        yield return new WaitForSeconds(1f);
        LeanTween.value(_sprite.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate (float alpha) {
            _sprite.SetAlpha(alpha);
        });
        LeanTween.rotateZ(_sprite.gameObject, 0f, 0.5f);
        LeanTween.scale(_sprite.gameObject, new Vector3(1f, 1f, 1f), 0.5f);
    }
    private IEnumerator LeagueMarkAnimation(SpriteRenderer _sprite) {
        _sprite.SetAlpha(0f);
        _sprite.transform.SetLocalEulerAnglesZ(180f);
        _sprite.transform.SetLocalScale(2f, 2f, 2f);
        yield return new WaitForSeconds(1f);
        LeanTween.value(_sprite.gameObject, 0f, 1f, 0.5f).setOnUpdate(delegate (float alpha) {
            _sprite.SetAlpha(alpha);
        });
        LeanTween.scale(_sprite.gameObject, new Vector3(1f, 1f, 1f), 0.5f).setEaseOutBack();
    }
    private void SetAlpha_SetIcon(SpriteRenderer[] _icon, int _winCount) {
        if (_winCount == 1) {
            _icon[0].SetAlpha(1f);
            _icon[1].SetAlpha(0f);
        } else {
            _icon[0].SetAlpha(0f);
            _icon[1].SetAlpha(0f);
        }
    }
    private void Update() {
    }
}
