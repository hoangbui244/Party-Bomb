using SaveDataDefine;
using System.Collections.Generic;
using UnityEngine;
public class SettingResultDataManager {
    public enum ResultDataType {
        SCORE,
        TIME,
        TIME_RACE
    }
    public enum CollectionDataType {
        WIN,
        TIME,
        TIME_RACE,
        SCORE
    }
    public struct RankingData {
        public int playerIdx;
        public float time;
        public int score;
    }
    private ResultDataType resultType = ResultDataType.TIME;
    private CollectionDataType collectionType = CollectionDataType.SCORE;
    private List<RankingData> listRankData = new List<RankingData>();
    private RankingResultManager result;
    private GS_Define.GameType gameType;
    private int[] collectionBorderInt;
    private float[] collectionBorderFloat;
    public const float TIME_MAX = 599f;
    public void SettingData(List<RankingData> _data, RankingResultManager _result, ResultDataType _resultType, CollectionDataType _collectionType, GS_Define.GameType _gameType, int[] _collectionBorderInt = null, float[] _collectionBorderFloat = null) {
        listRankData = _data;
        result = _result;
        resultType = _resultType;
        collectionType = _collectionType;
        gameType = _gameType;
        collectionBorderInt = _collectionBorderInt;
        collectionBorderFloat = _collectionBorderFloat;
        SettingResultData();
        SettingCollection();
    }
    private void SettingResultData() {
        switch (resultType) {
            case ResultDataType.SCORE:
                listRankData.Sort((RankingData a, RankingData b) => b.score - a.score);
                break;
            case ResultDataType.TIME:
                listRankData.Sort((RankingData a, RankingData b) => (int)(b.time * 100f) - (int)(a.time * 100f));
                if (listRankData[listRankData.Count - 1].time < 0f) {
                    listRankData.Insert(0, listRankData[listRankData.Count - 1]);
                    listRankData.RemoveAt(listRankData.Count - 1);
                }
                break;
            case ResultDataType.TIME_RACE:
                listRankData.Sort((RankingData a, RankingData b) => (int)(a.time * 100f) - (int)(b.time * 100f));
                break;
        }
        int[] array = new int[listRankData.Count];
        int[] array2 = new int[listRankData.Count];
        float[] array3 = new float[listRankData.Count];
        for (int i = 0; i < listRankData.Count; i++) {
            array2[i] = listRankData[i].score;
            array3[i] = Mathf.Min(listRankData[i].time, 599f);
            array[i] = listRankData[i].playerIdx;
        }
        switch (resultType) {
            case ResultDataType.SCORE:
                ResultGameDataParams.SetRecord_Int(array2, array);
                result.ShowResult_Score();
                break;
            case ResultDataType.TIME:
            case ResultDataType.TIME_RACE:
                for (int j = 0; j < listRankData.Count; j++) {
                    CalcManager.ConvertTimeToRecordString(array3[j], array[j]);
                }
                ResultGameDataParams.SetRecord_Float(array3, array);
                result.ShowResult_Time();
                break;
        }
    }
    private void SettingCollection() {
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && listRankData[0].playerIdx == 0) {
            switch (collectionType) {
                case CollectionDataType.WIN:
                    CheckCollectionWin();
                    break;
                case CollectionDataType.SCORE:
                    CheckCollectionScore();
                    break;
                case CollectionDataType.TIME:
                    CheckCollectionTime();
                    break;
                case CollectionDataType.TIME_RACE:
                    CheckCollectionTimeRace();
                    break;
            }
        }
    }
    private void CheckCollectionWin() {
        switch (SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetAiStrengthSetting()) {
            case SystemData.AiStrength.WEAK:
                SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, gameType);
                break;
            case SystemData.AiStrength.NORAML:
                SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, gameType);
                break;
            case SystemData.AiStrength.STRONG:
                SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, gameType);
                break;
        }
    }
    private void CheckCollectionScore() {
        int score = listRankData[0].score;
        if (score >= collectionBorderInt[0]) {
            SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, gameType);
        }
        if (score >= collectionBorderInt[1]) {
            SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, gameType);
        }
        if (score >= collectionBorderInt[2]) {
            SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, gameType);
        }
    }
    private void CheckCollectionTime() {
        float time = listRankData[0].time;
        if (time >= collectionBorderFloat[0]) {
            SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, gameType);
        }
        if (time >= collectionBorderFloat[1]) {
            SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, gameType);
        }
        if (time >= collectionBorderFloat[2]) {
            SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, gameType);
        }
    }
    private void CheckCollectionTimeRace() {
        float time = listRankData[0].time;
        if (time <= collectionBorderFloat[0]) {
            SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.BRONZE, gameType);
        }
        if (time <= collectionBorderFloat[1]) {
            SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.SILVER, gameType);
        }
        if (time <= collectionBorderFloat[2]) {
            SingletonCustom<SaveDataManager>.Instance.SaveData.trophyData.SetOpen(TrophyData.Type.GOLD, gameType);
        }
    }
}
