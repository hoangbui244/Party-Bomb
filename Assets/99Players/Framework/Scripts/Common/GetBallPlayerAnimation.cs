public class GetBallPlayerAnimation : Common_CharacterAnimation {
    public void SetPush(bool _isPush) {
        animator.SetBool("IsPush", _isPush);
    }
}
