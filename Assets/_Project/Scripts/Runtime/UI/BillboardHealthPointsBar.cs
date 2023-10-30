using UnityEngine;
using UnityEngine.Assertions;

namespace BaseDefense.UI {

    public class BillboardHealthPointsBar : MonoBehaviour {

        private Transform m_mainCamera;


        private void Start () {
            const string message = "MainCamera не установлена";
            Assert.IsNotNull(Camera.main, message);
            m_mainCamera = Camera.main.gameObject.transform;
        }


        private void LateUpdate () => transform.rotation = m_mainCamera.rotation;

    }

}