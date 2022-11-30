using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCharacter : BaseCharacter
{
    private static PlayerCharacter instance;

    public static PlayerCharacter Instance => instance;
    public float MaxHealthPoint => maxHealthPoint;
    public float CurrentHP => currentHP;

    [SerializeField] Transform slotForGun;
    [SerializeField] Transform recoveryPoint;
    GameObject gun;
    EnemyBaseContainer enemyBase;
    GunShooting gunShooting;
    Vector3 lookToEnemy;
    Vector3 lookToMovement;
    bool inBase;
    bool inEnemyBase;

    void Awake() => instance = this;
    void Start()
    {
        currentHP = maxHealthPoint;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        gun = slotForGun.GetChild(0).gameObject;
        gunShooting = gun.GetComponent<GunShooting>();
        gun.SetActive(false);
    }

    void Update()
    {
        if (lookToEnemy != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
            transform.rotation, Quaternion.LookRotation(lookToEnemy), Time.smoothDeltaTime * 15f
            );
        else if (lookToMovement != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
            transform.rotation, Quaternion.LookRotation(lookToMovement), Time.smoothDeltaTime * 15f
            );

        if (inEnemyBase) Attack();
        else if (inBase) RecoveryHP();
        Movement();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBase"))
        {
            enemyBase = other.gameObject.GetComponent<EnemyBaseContainer>();
            inEnemyBase = true;
            SetParams(inEnemyBase);
        }
        if (other.CompareTag("PlayerBase"))
            inBase = true;
        if (other.CompareTag("Shop"))
            Game.UI.OpenShop();
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemyBase"))
        {
            inEnemyBase = false;
            SetParams(inEnemyBase);
            lookToEnemy = Vector3.zero;
        }
        if (other.CompareTag("PlayerBase")) 
            inBase = false;
        if (other.CompareTag("Shop"))
            Game.UI.CloseShop();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, attackDistance);
    }

    public override void GetDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            animator.SetBool("alive", false);
            gun.SetActive(false);
            this.enabled = false;
            controller.enabled = false;
            BroadcastMessages.SendMessage(MessageType.DEATH_PLAYER);
        }
    }

    public void Resurrection()
    {
        animator.SetBool("alive", true);
        animator.SetBool("inEnemyBase", false);
        lookToEnemy = Vector3.zero;
        transform.position = recoveryPoint.position;
        transform.rotation = Quaternion.identity;
        this.enabled = true;
        controller.enabled = true;
        currentHP = maxHealthPoint;
        inEnemyBase = false;
        inBase = true;
    }
    void RecoveryHP()
    {
        if (currentHP < maxHealthPoint)
            currentHP += Time.deltaTime * 5f;
    }

    void Movement()
    {
        Vector3 move = Game.GetInputFromJoystick();
        lookToMovement = move;
        animator.SetFloat("speed", move.magnitude * maxSpeed);
        move = move * maxSpeed;
        controller.Move(move * Time.smoothDeltaTime);
    }

    void Attack()
    {
        GameObject enemy = GetNearestEnemy();
        if (enemy is not null)
        {
            lookToEnemy = enemy.transform.position - transform.position;
            gunShooting.Shot(enemy.transform.position + Vector3.up);
        }
        else
            lookToEnemy = Vector3.zero;
    }

    GameObject GetNearestEnemy()
    {
        GameObject target = null;
        Collider[] enemies = Physics.OverlapSphere(transform.position, attackDistance, 1 << 3);
        float dist = Mathf.Infinity;
        foreach (Collider enemy in enemies)
        {
            Vector3 distance = enemy.transform.position - transform.position;
            if (distance.magnitude < dist)
            {
                target = enemy.gameObject;
                dist = distance.magnitude;
            }
        }

        return target;
    }

    void SetParams(bool inBase)
    {
        gun.SetActive(inBase);
        animator.SetBool("inEnemyBase", inBase);
    }
}
