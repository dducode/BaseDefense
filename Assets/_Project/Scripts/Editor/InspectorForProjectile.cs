using BaseDefense.AttackImplemention.Projectiles;
using UnityEditor;

namespace BaseDefense.Editor {

    [CustomEditor(typeof(Projectile), true)]
    public class InspectorForProjectile : UnityEditor.Editor {

        public override void OnInspectorGUI () {
            var message = string.Empty;

            switch (target) {
                case Bullet:
                    message = "Урон от пули зависит от её массы и скорости";
                    break;
                case Grenade:
                    message = "Урон от гранаты и радиус поражения устанавливаются в инспекторе гранатомёта";
                    break;
                case Arrow:
                    message = "Урон от яда стрелы и его длительность устанавливаются в инспекторе арбалета";
                    break;
            }

            EditorGUILayout.HelpBox(message, MessageType.Info);

            DrawDefaultInspector();
        }

    }

}