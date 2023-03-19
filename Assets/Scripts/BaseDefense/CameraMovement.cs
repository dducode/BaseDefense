using UnityEngine;
using Zenject;
using BaseDefense.Characters;

namespace BaseDefense {

    public class CameraMovement : MonoBehaviour {

        private Vector3 m_startPos;
        private Vector3 m_movement;

        [Inject]
        private PlayerCharacter m_player;


        private void Start () {
            m_startPos = transform.position;
            m_movement = m_startPos;
        }


        private void LateUpdate () {
            m_movement = m_player.transform.position + m_startPos;
            transform.position = m_movement;
        }

    }

}