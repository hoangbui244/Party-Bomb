using UnityEngine;
/// <summary>
/// 
/// </summary>
public class Common_CharacterAnimation : MonoBehaviour {
    /// <summary>
    /// 
    /// </summary>
    protected Animator animator;
    /// <summary>
    /// 
    /// </summary>
    public void Init() {
        animator = GetComponent<Animator>();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_isBool"></param>
    public void SetIdleAnimation(bool _isBool) {
        if (animator.GetBool("IsIdle") != _isBool) {
            animator.SetBool("IsIdle", _isBool);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_moveSpeed"></param>
    public void SetMoveAnimation(float _moveSpeed) {
        animator.SetFloat("MoveSpeed", _moveSpeed);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_isBool"></param>
    public void SetJumpAnimation(bool _isBool) {
        if (animator.GetBool("IsJump") != _isBool) {
            animator.SetBool("IsJump", _isBool);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public virtual void SetGameEndAnimation() {
        SetIdleAnimation(_isBool: true);
        SetMoveAnimation(0f);
        SetJumpAnimation(_isBool: false);
    }
}
