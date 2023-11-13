using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CharaScore {
    private Dictionary<BeachVolley_Character, int> histories = new Dictionary<BeachVolley_Character, int>();
    public void Init() {
        histories.Clear();
    }
    public void AddPoint(BeachVolley_Character chara) {
        int value = 0;
        histories.TryGetValue(chara, out value);
        value++;
        histories[chara] = value;
        UnityEngine.Debug.Log(chara.GetName() + value.ToString());
    }
    public List<BeachVolley_Character> GetPointRanking() {
        IOrderedEnumerable<KeyValuePair<BeachVolley_Character, int>> orderedEnumerable = from x in histories
                                                                                         orderby x.Value descending
                                                                                         select x;
        List<BeachVolley_Character> list = new List<BeachVolley_Character>();
        foreach (KeyValuePair<BeachVolley_Character, int> item in orderedEnumerable) {
            list.Add(item.Key);
        }
        return list;
    }
    public int CharaGetPoint(BeachVolley_Character chara) {
        return histories[chara];
    }
}
