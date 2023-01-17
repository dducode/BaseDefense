using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Zenject;

[RequireComponent(typeof(DisplayHealthPoints))]
public class EnemyFactory : MonoBehaviour, IAttackable
{
    ///<summary>Максимальное количество здоровья базы</summary>
    ///<value>[1, infinity]</value>
    [Tooltip("Максимальное количество здоровья базы. [1, infinity]")]
    [SerializeField, Min(1)] float maxHealthPoints = 300;

    ///<summary>Анимация, воспроизводимая при уничтожении базы</summary>
    [Tooltip("Анимация, воспроизводимая при уничтожении базы")]
    [SerializeField] ParticleSystem destroyEffect;

    ///<summary>Максимальное расстояние от центра базы для порождения новых врагов</summary>
    ///<value>[0, infinity]</value>
    [Tooltip("Максимальное расстояние от центра базы для порождения новых врагов. [0, infinity]")]
    [SerializeField, Min(0)] float radiusSpawn = 10f;

    ///<summary>Целевые точки для патруля врагами</summary>
    [Tooltip("Целевые точки для патруля врагами")]
    [SerializeField] Transform[] targetPoints;

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
                DestroyBase();
        }
    }

    [Inject] EnemyCharacter.Factory enemyFactory;
    DisplayHealthPoints displayHealthPoints;

    void Awake()
    {
        CurrentHealthPoints = maxHealthPoints;
        displayHealthPoints = GetComponent<DisplayHealthPoints>();
        displayHealthPoints.SetMaxValue((int)maxHealthPoints);
        displayHealthPoints.UpdateView((int)CurrentHealthPoints);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (transform.parent != null)
            Gizmos.DrawWireSphere(transform.parent.position, radiusSpawn);
    }

    public EnemyCharacter SpawnEnemy()
    {
        Vector3 position = transform.parent.position + Random.insideUnitSphere * radiusSpawn;
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
        
        return enemy;
    }

    public void Hit(float damage)
    {
        CurrentHealthPoints -= damage;
        displayHealthPoints.UpdateView((int)CurrentHealthPoints);
    }

    void DestroyBase()
    {
        Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
