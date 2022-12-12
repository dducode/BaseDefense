using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCharacter : BaseCharacter
{
    public float MaxHealthPoint => maxHealthPoint;
    public float CurrentHP => currentHP;

    [SerializeField] Transform gunSlot;
    [SerializeField] Transform recoveryPoint;
    Gun gun;
    Vector3 lookToEnemy;
    Vector3 lookToMovement;
    bool inBase;
    bool inEnemyBase;

    void Awake() => BroadcastMessages.AddListener(MessageType.RESTART, Resurrection);
    void OnDestroy() => BroadcastMessages.RemoveListener(MessageType.RESTART, Resurrection);
    void Start()
    {
        currentHP = maxHealthPoint;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        gun = gunSlot.GetChild(0).GetComponent<Gun>();
        gun.gameObject.SetActive(false);
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
            inEnemyBase = true;
            SetParams(inEnemyBase);
        }
        if (other.CompareTag("PlayerBase"))
            inBase = true;
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
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, attackDistance);
    }
    public void SelectGun(GunSlot slot)
    {
        gun = Game.Shop.Select(slot, gun);
        gun.transform.parent = gunSlot;
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.identity;
        gun.transform.localScale = Vector3.one;
    }

    public override void GetDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            animator.SetBool("alive", false);
            gun.gameObject.SetActive(false);
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
            gun.Shot(enemy.transform.position + Vector3.up);
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
        gun.gameObject.SetActive(inBase);
        animator.SetBool("inEnemyBase", inBase);
    }
}
