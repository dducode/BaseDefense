using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class EnemyBase : MonoBehaviour
{
    ///<summary>Максимально возможное количество врагов на базе</summary>
    ///<value>[1, 100]</value>
    [Tooltip("Максимально возможное количество врагов на базе. [1, 100]")]
    [SerializeField, Range(1, 100)] int maxEnemiesCount = 10;

    ///<summary>Начальное количество врагов на базе</summary>
    ///<value>[0, 100]</value>
    [Tooltip("Начальное количество врагов на базе. [0, 100]")]
    [SerializeField, Range(0, 100)] int startEnemiesCount = 5;

    ///<summary>Временной интервал между порождением новых врагов</summary>
    ///<value>[0, infinity]</value>
    [Tooltip("Временной интервал между порождением новых врагов. [0, infinity]")]
    [SerializeField, Min(0)] float timeSpawn = 3f;

    [SerializeField] EnemyFactory enemyFactory;

    ///<summary>Содержит всех врагов, находящихся на базе</summary>
    ///<remarks>Мёртвые враги удаляются из списка</remarks>
    List<EnemyCharacter> enemies;

    ///<summary>Время, прошедшее с момента последнего порождения врага</summary>
    float timeOfLastSpawn;

    void Start()
    {
        enemies = new List<EnemyCharacter>();
        for (int i = 0; i < startEnemiesCount; i++)
            enemies.Add(enemyFactory.SpawnEnemy());
    }

    ///<returns>Возвращает true, если прошло достаточно времени с момента последнего порождения</returns>
    public bool HasTimePassed()
    {
        return timeOfLastSpawn + timeSpawn < Time.time;
    }

    void Update()
    {
        // необходима проверка на null, т.к. enemyFactory может быть уничтожена игроком
        if (enemyFactory != null && enemies.Count < maxEnemiesCount && HasTimePassed())
        {
            enemies.Add(enemyFactory.SpawnEnemy());
            timeOfLastSpawn = Time.time;
        }

        for (int i = 0; i < enemies.Count; i++)
            if (!enemies[i].EnemyUpdate())
                enemies.RemoveAt(i);
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
