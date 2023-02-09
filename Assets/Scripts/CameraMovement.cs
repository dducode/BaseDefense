using UnityEngine;
using Zenject;
using BaseDefense.Characters;

namespace BaseDefense
{
    public class CameraMovement : MonoBehaviour
    {
        Vector3 startPos;
        Vector3 movement;
        [Inject] PlayerCharacter player;

        void Start()
        {
            startPos = transform.position;
            movement = startPos;
        }

        void LateUpdate()
        {
            movement = player.transform.position + startPos;
            transform.position = movement;
        }
    }
}


