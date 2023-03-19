using BaseDefense.UI;
using UnityEngine;
using Zenject;

namespace BaseDefense.Installers {

    public class UserInterfaceInstaller : MonoInstaller {

        [SerializeField]
        private JoystickController joystick;


        [SerializeField]
        private DisplayingUI ui;


        public override void InstallBindings () {
            Container.Bind<JoystickController>().FromInstance(joystick).AsSingle();
            Container.Bind<DisplayingUI>().FromInstance(ui).AsSingle();
        }

    }

}