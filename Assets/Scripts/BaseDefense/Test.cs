using BaseDefense.Characters;
using UnityEngine;

namespace BaseDefense
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private KeyCode keyForDestroyEnemies;

        private void Update()
        {
            if (Input.GetKeyDown(keyForDestroyEnemies))
            {
                var enemies = FindObjectsOfType<EnemyCharacter>();
                foreach (var enemy in enemies)
                {
                    Destroy(enemy.gameObject);
                }
            }
        }
    }
}