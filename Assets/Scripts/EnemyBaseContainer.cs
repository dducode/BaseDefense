using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Zenject;

public class EnemyBaseContainer : MonoBehaviour
{
    [SerializeField] int maxEnemiesCount = 10;
    [SerializeField] int startEnemiesCount = 5;
    [SerializeField] float timeSpawn = 3f;
    [SerializeField] float radiusSpawn = 10f;
    [SerializeField] Transform[] targetPoints;

    float timeOfLastSpawn;
    List<EnemyCharacter> enemies;
    [Inject] EnemyCharacter.Factory enemyFactory;

    void Start()
    {
        enemies = new List<EnemyCharacter>();
        for (int i = 0; i < startEnemiesCount; i++)
            SpawnEnemy();
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
        if (Pools.EnemiesCount == 0)
        {
            enemy = enemyFactory.Create(targetPoints);
            SceneManager.MoveGameObjectToScene(enemy.gameObject, Game.EnemiesScene);
        }
        else
            enemy = Pools.PopEnemy();
        enemy.transform.localPosition = position;
        enemy.transform.localRotation = rotation;
        enemies.Add(enemy);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radiusSpawn);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (EnemyCharacter enemy in enemies)
                enemy.getCurrentState.Trigger(true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (EnemyCharacter enemy in enemies)
                enemy.getCurrentState.Trigger(false);
        }
    }
}
