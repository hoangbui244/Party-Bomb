using System;
using UnityEngine;
namespace BeachSoccer {
    [Serializable]
    public struct FormationList {
        [SerializeField]
        public FormationData formationData;
        [SerializeField]
        public string name;
        [SerializeField]
        public string info;
        [SerializeField]
        public GameDataParams.Rarity rarity;
    }
}
