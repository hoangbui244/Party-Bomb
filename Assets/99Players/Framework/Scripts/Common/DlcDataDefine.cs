using UnityEngine;
public class DlcDataDefine : MonoBehaviour {
    public class DlcBaseData : MonoBehaviour {
        public int id;
        public virtual void Init() {
        }
    }
    public static int DLC_1_IDX = 0;
    public static int DLC_2_IDX = 1;
    public static int DLC_3_IDX = 2;
}
