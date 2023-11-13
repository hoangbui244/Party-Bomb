using System.Collections.Generic;
using UnityEngine;
namespace SatBox.OurWinterSports.Audience {
    [CreateAssetMenu(menuName = "Our Winter Sports/Audience Settings", fileName = "AudienceSettings")]
    public class AudienceSettings : ScriptableObject {
        [SerializeField]
        private List<GameObject> standingAudiences = new List<GameObject>();
        [SerializeField]
        private List<GameObject> sitAudiences = new List<GameObject>();
        [SerializeField]
        private List<Material> boyBodyMaterials = new List<Material>();
        [SerializeField]
        private List<Material> boyFaceMaterials = new List<Material>();
        [SerializeField]
        private List<Material> girlBodyMaterials = new List<Material>();
        [SerializeField]
        private List<Material> girlFaceMaterials = new List<Material>();
        public List<GameObject> StandingAudiences => standingAudiences;
        public List<GameObject> SitAudiences => sitAudiences;
        public List<Material> BoyBodyMaterials => boyBodyMaterials;
        public List<Material> BoyFaceMaterials => boyFaceMaterials;
        public List<Material> GirlBodyMaterials => girlBodyMaterials;
        public List<Material> GirlFaceMaterials => girlFaceMaterials;
    }
}
