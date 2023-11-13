using UnityEngine;
public class Cloud : MonoBehaviour {
    private enum Dir {
        Left,
        Right
    }
    [SerializeField]
    [Header("動かす方向")]
    private Dir direction;
    [SerializeField]
    [Header("速度")]
    private float speed = 1f;
    private void Update() {
        Vector3 eulerAngles = base.transform.eulerAngles;
        eulerAngles.y += Time.deltaTime * (float)((direction != 0) ? 1 : (-1)) * speed;
        base.transform.eulerAngles = eulerAngles;
    }
}
