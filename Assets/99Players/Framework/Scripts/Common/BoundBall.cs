using UnityEngine;
public class BoundBall : MonoBehaviour {
    private Rigidbody rigid;
    private Vector3 pluseRigid = new Vector3(0f, 1.5f, 0f);
    private void Start() {
        rigid = GetComponent<Rigidbody>();
    }
    private void OnCollisionExit(Collision _collision) {
        if (_collision.gameObject.tag == "Character") {
            Vector3 normalized = rigid.velocity.normalized;
            float magnitude = _collision.rigidbody.velocity.magnitude;
            rigid.velocity += normalized * magnitude;
            rigid.velocity += pluseRigid;
        }
    }
}
