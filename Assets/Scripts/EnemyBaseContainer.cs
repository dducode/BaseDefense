using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyBaseContainer : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] int maxEnemiesCount = 10;
    [SerializeField] float timeSpawn = 3f;
    [SerializeField] float radiusSpawn = 10f;
    [SerializeField] Transform[] targetPoints;
    float _timeSpawn;
    List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        EnemyCharacter[] _enemies = FindObjectsOfType<EnemyCharacter>();
        foreach (EnemyCharacter enemy in _enemies)
        {
            enemies.Add(enemy.gameObject);
            enemy.GetData(this, targetPoints);
        }
    }

    void Update()
    {
        if (_timeSpawn >= timeSpawn)
        {
            Vector3 enemyPos = transform.position + Random.insideUnitSphere * radiusSpawn;
            enemyPos.y = 0;
            Quaternion enemyRot = Random.rotation;
            enemyRot.x = 0;
            enemyRot.z = 0;
            GameObject _enemy = Instantiate(enemy, enemyPos, enemyRot);
            enemies.Add(_enemy);
            _enemy.GetComponent<EnemyCharacter>().GetData(this, targetPoints);;
            _timeSpawn = 0;
        }
        else if (enemies.Count < maxEnemiesCount)
            _timeSpawn += Time.deltaTime;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radiusSpawn);
    }

    public void RemoveEnemyFromList(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject enemy in enemies)
                enemy.GetComponent<EnemyCharacter>().getCurrentState.Trigger(true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject enemy in enemies)
                enemy.GetComponent<EnemyCharacter>().getCurrentState.Trigger(false);
        }
    }
}
