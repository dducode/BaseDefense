using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Zenject;

[RequireComponent(typeof(DisplayHealthPoints))]
public class EnemyStation : MonoBehaviour, IAttackable
{
    ///<summary>Максимальное количество здоровья станции</summary>
    ///<value>[1, infinity]</value>
    [Tooltip("Максимальное количество здоровья станции. [1, infinity]")]
    [SerializeField, Min(1)] float maxHealthPoints = 300;

    ///<summary>Анимация, воспроизводимая при уничтожении станции</summary>
    [Tooltip("Анимация, воспроизводимая при уничтожении станции")]
    [SerializeField] ParticleSystem destroyEffect;

    ///<summary>Максимальное расстояние от центра спавна для порождения новых врагов</summary>
    ///<value>[0, infinity]</value>
    [Tooltip("Максимальное расстояние от центра спавна для порождения новых врагов. [0, infinity]")]
    [SerializeField, Min(0)] float radiusSpawn = 10f;

    [SerializeField] Transform centerOfSpawn;
    [SerializeField] EnemyCharacter enemyPrefab;

    ///<summary>Текущее количество здоровья базы</summary>
    ///<value>[0, maxHealthPoints]</value>
    float currentHealthPoints;
    ///<inheritdoc cref="currentHealthPoints"/>
    public float CurrentHealthPoints
    {
        get => currentHealthPoints;
        set
        {
            currentHealthPoints = value;
            currentHealthPoints = Mathf.Clamp(currentHealthPoints, 0, maxHealthPoints);
            if (currentHealthPoints == 0)
                Destroy();
        }
    }

    DisplayHealthPoints displayHealthPoints;
    [Inject] EnemyCharacter.Factory enemyFactory;

    void Awake()
    {
        CurrentHealthPoints = maxHealthPoints;
        displayHealthPoints = GetComponent<DisplayHealthPoints>();
    }

    void Start()
    {
        displayHealthPoints.SetMaxValue((int)maxHealthPoints);
        displayHealthPoints.UpdateView((int)CurrentHealthPoints);
    }

    void OnDrawGizmosSelected()
    {
        if (centerOfSpawn == null)
            return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(centerOfSpawn.position, radiusSpawn);
    }

    public EnemyCharacter SpawnEnemy(Transform[] targetPoints)
    {
        Vector3 position = centerOfSpawn.position + Random.insideUnitSphere * radiusSpawn;
        position.y = 0;
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
        EnemyCharacter enemy = ObjectsPool<EnemyCharacter>.GetEqual(enemyFactory, enemyPrefab);
        enemy.Spawn(targetPoints, position, rotation);
        
        return enemy;
    }

    public void Hit(float damage)
    {
        CurrentHealthPoints -= damage;
        displayHealthPoints.UpdateView((int)CurrentHealthPoints);
    }

    void Destroy()
    {
        Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
