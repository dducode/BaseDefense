using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Zenject;

public class EnemyBaseContainer : MonoBehaviour
{
    ///<summary>Максимально возможное количество врагов на базе</summary>
    [SerializeField, Tooltip("Максимально возможное количество врагов на базе")] 
    int maxEnemiesCount = 10;

    ///<summary>Начальное количество врагов на базе</summary>
    [SerializeField, Tooltip("Начальное количество врагов на базе")] 
    int startEnemiesCount = 5;

    ///<summary>Временной интервал между порождением новых врагов</summary>
    [SerializeField, Tooltip("Временной интервал между порождением новых врагов")] 
    float timeSpawn = 3f;

    ///<summary>Максимальное расстояние от центра базы для порождения новых врагов</summary>
    [SerializeField, Tooltip("Максимальное расстояние от центра базы для порождения новых врагов")] 
    float radiusSpawn = 10f;

    ///<summary>Целевые точки для патруля врагами</summary>
    [SerializeField, Tooltip("Целевые точки для патруля врагами")] 
    Transform[] targetPoints;

    ///<summary>Время, прошедшее с момента последнего порождения врага</summary>
    float timeOfLastSpawn;

    ///<summary>Содержит всех врагов, находящихся на базе</summary>
    ///<remarks>Мёртвые враги удаляются из списка</remarks>
    List<EnemyCharacter> enemies;
    [Inject] EnemyCharacter.Factory enemyFactory;

    void Start()
    {
        enemies = new List<EnemyCharacter>();
        for (int i = 0; i < startEnemiesCount; i++)
            SpawnEnemy();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radiusSpawn);
    }

    void Update()
    {
        if (timeOfLastSpawn + timeSpawn <= Time.time && enemies.Count < maxEnemiesCount)
        {
            SpawnEnemy();
            timeOfLastSpawn = Time.time;
        }

        for (int i = 0; i < enemies.Count; i++)
            if (!enemies[i].EnemyUpdate())
                enemies.RemoveAt(i);
    }

    void SpawnEnemy()
    {
        Vector3 position = transform.position + Random.insideUnitSphere * radiusSpawn;
        position.y = 0;
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
        EnemyCharacter enemy;
        if (ObjectsPool<EnemyCharacter>.IsEmpty())
        {
            enemy = enemyFactory.Create(targetPoints);
            SceneManager.MoveGameObjectToScene(enemy.gameObject, Game.EnemiesScene);
        }
        else
            enemy = ObjectsPool<EnemyCharacter>.Pop();
        enemy.Spawn(position, rotation);
        enemies.Add(enemy);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (EnemyCharacter enemy in enemies)
                enemy.SetTrigger(true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (EnemyCharacter enemy in enemies)
                enemy.SetTrigger(false);
        }
    }
}
