using UnityEditor;

namespace BaseDefense
{
    [CustomEditor(typeof(Projectile), true)]
    public class InspectorForProjectile : Editor
    {
        public override void OnInspectorGUI()
        {
            if (target is Bullet)
                EditorGUILayout.HelpBox(
                    "Урон от пули зависит от её массы и скорости", 
                    MessageType.Info
                );
            if (target is Grenade)
                EditorGUILayout.HelpBox(
                    "Урон от гранаты и радиус поражения устанавливаются в инспекторе гранатомёта", 
                    MessageType.Info
                );
            if (target is Arrow)
                EditorGUILayout.HelpBox(
                    "Урон от яда стрелы и его длительность устанавливаются в инспекторе арбалета", 
                    MessageType.Info
                );
            DrawDefaultInspector();
        }
    }
}


