using UnityEngine;
public class InformCollider : MonoBehaviour {
    [SerializeField]
    [Header("伝播対象")]
    private InformCollider target;
    public virtual void _OnCollisionEnter(Collision collision) {
    }
    private void OnCollisionEnter(Collision collision) {
        if (target != null) {
            target._OnCollisionEnter(collision);
        }
        _OnCollisionEnter(collision);
    }
}
