using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ResultBalloonManager : MonoBehaviour {
    private List<GameObject> arrayballonList = new List<GameObject>();
    [SerializeField]
    private GameObject[] arrayBallon;
    [SerializeField]
    [Header("初回に生成する風船の数")]
    private int INIT_CREATE_COUNT = 30;
    [SerializeField]
    private Transform ballonParentTransform;
    [SerializeField]
    private Transform leftTransform;
    [SerializeField]
    private Transform rightTransform;
    [SerializeField]
    [Header("生成する間隔（最小時間）")]
    private float createRandomTime_MIN;
    [SerializeField]
    [Header("生成する間隔（最大時間）")]
    private float createRandomTime_MAX;
    [SerializeField]
    [Header("再生成するまでの生成間隔に乗算する倍率")]
    private float reCreateInterval = 10f;
    private void Start() {
        BalloonCreate();
    }
    private void OnDisable() {
        for (int i = 0; i < arrayballonList.Count; i++) {
            StopCoroutine(BalloonCreateActive(arrayballonList[i]));
        }
    }
    public void BalloonCreate(GameObject _obj = null) {
        if (_obj == null) {
            StartCoroutine(BalloonCreateActive());
        } else {
            StartCoroutine(BalloonCreateActive(_obj));
        }
    }
    private IEnumerator BalloonCreateActive() {
        for (int i = 0; i < INIT_CREATE_COUNT; i++) {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(position: new Vector3(UnityEngine.Random.Range(leftTransform.localPosition.x, rightTransform.localPosition.x), leftTransform.localPosition.y, 0f), original: arrayBallon[i % arrayBallon.Length], rotation: arrayBallon[i % arrayBallon.Length].transform.rotation);
            gameObject.transform.parent = ballonParentTransform;
            gameObject.transform.SetLocalPositionZ(arrayBallon[i % arrayBallon.Length].transform.localPosition.z);
            arrayballonList.Add(gameObject);
            gameObject.SetActive(value: true);
            yield return new WaitForSeconds(UnityEngine.Random.Range(createRandomTime_MIN, createRandomTime_MAX));
        }
    }
    private IEnumerator BalloonCreateActive(GameObject _obj) {
        yield return new WaitForSeconds(UnityEngine.Random.Range(createRandomTime_MIN, createRandomTime_MAX) * reCreateInterval);
        Vector3 localPosition = new Vector3(UnityEngine.Random.Range(leftTransform.localPosition.x, rightTransform.localPosition.x), leftTransform.localPosition.y, _obj.transform.localPosition.z);
        _obj.transform.localPosition = localPosition;
        _obj.SetActive(value: true);
    }
    public void Init() {
        if (arrayballonList.Count != 0) {
            for (int i = 0; i < arrayballonList.Count; i++) {
                UnityEngine.Object.Destroy(arrayballonList[i]);
            }
        }
        arrayballonList.Clear();
    }
}
