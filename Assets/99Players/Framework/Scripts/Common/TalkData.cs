public class TalkData {
    public enum Character {
        BOY_GREEN,
        TEACHER_ODA
    }
    public enum FaceDiff {
        Normal_0
    }
    public Character character;
    public FaceDiff faceDiff;
    public string text;
    public TalkData(Character _character, FaceDiff _faceDiff, string _text) {
        character = _character;
        faceDiff = _faceDiff;
        text = _text;
    }
}
