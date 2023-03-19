using BaseDefense.Characters;
using UnityEngine;
using Zenject;

namespace BaseDefense.UI {

    public class ShopWindow : MonoBehaviour {

        ///<summary>Рамка для выбранного игроком оружия</summary>
        [SerializeField, Tooltip("Рамка для выбранного игроком оружия")]
        private RectTransform frame;

        [Inject]
        private PlayerCharacter m_player;


        public void SelectGun (GunSlot slot) {
            frame.localPosition = slot.transform.localPosition;
            m_player.SelectGun(slot.GunName);
        }

    }

}