public class TalkDataTable {
    public enum TalkType {
        TALK_FIRST_CONTACT,
        TALK_GAME_RELEASE,
        COLLECTION_INFO
    }
    private TalkData[] talkFirstContact;
    private TalkData[] talkGameRelease;
    private TalkData[] talkCollectionInfo;
    public TalkDataTable() {
        talkFirstContact = new TalkData[3]
        {
            new TalkData(TalkData.Character.BOY_GREEN, TalkData.FaceDiff.Normal_0, SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 126)),
            new TalkData(TalkData.Character.BOY_GREEN, TalkData.FaceDiff.Normal_0, SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 127)),
            new TalkData(TalkData.Character.BOY_GREEN, TalkData.FaceDiff.Normal_0, SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 128))
        };
        talkGameRelease = new TalkData[1]
        {
            new TalkData(TalkData.Character.BOY_GREEN, TalkData.FaceDiff.Normal_0, SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 136))
        };
        talkCollectionInfo = new TalkData[3]
        {
            new TalkData(TalkData.Character.BOY_GREEN, TalkData.FaceDiff.Normal_0, SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 131)),
            new TalkData(TalkData.Character.BOY_GREEN, TalkData.FaceDiff.Normal_0, SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 132)),
            new TalkData(TalkData.Character.BOY_GREEN, TalkData.FaceDiff.Normal_0, SingletonCustom<TextDataBaseManager>.Instance.Get(TextDataBaseItemEntity.DATABASE_NAME.COMMON, 133))
        };
    }
    public TalkData[] GetTalkData(TalkType _type) {
        switch (_type) {
            case TalkType.TALK_FIRST_CONTACT:
                return talkFirstContact;
            case TalkType.TALK_GAME_RELEASE:
                return talkGameRelease;
            case TalkType.COLLECTION_INFO:
                return talkCollectionInfo;
            default:
                return null;
        }
    }
}
