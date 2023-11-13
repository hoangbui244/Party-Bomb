using UnityEngine;
public class AlpineSkiing_Props_Airship_Anime : MonoBehaviour {
    private void Update() {
        base.gameObject.transform.AddEulerAnglesY(Time.deltaTime * -1f);
    }
}
