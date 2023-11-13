using UnityEngine;
namespace SatBox.OurWinterSports.Audience {
    public class AudienceCharacter : MonoBehaviour {
        [SerializeField]
        private AudienceCharacterType type;
        public AudienceCharacterType Type => type;
    }
}
