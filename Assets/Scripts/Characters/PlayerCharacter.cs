using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCharacter : BaseCharacter
{
    private static PlayerCharacter instance;

    public static PlayerCharacter getInstance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PlayerCharacter>();
            return instance;
        }
    }

    [SerializeField] GameObject gun;
    [SerializeField] Transform recoveryPoint;
    [SerializeField] TextMeshProUGUI HP;
    [SerializeField] Slider HPBar;
    EnemyBaseContainer enemyBase;
    GunShooting gunShooting;
    Vector3 lookToEnemy;
    Vector3 lookToMovement;
    IEnumerator attackEnemy;
    bool inBase;

    void Start()
    {
        currentHP = maxHealthPoint;
        HPBar.maxValue = maxHealthPoint;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        gunShooting = gun.GetComponent<GunShooting>();
        gun.SetActive(false);
        attackEnemy = Attack();
    }

    void Update()
    {
        HP.text = currentHP.ToString();
        HPBar.value = currentHP;
        if (lookToEnemy != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
            transform.rotation, Quaternion.LookRotation(lookToEnemy), Time.smoothDeltaTime * 15f
            );
        else if (lookToMovement != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
            transform.rotation, Quaternion.LookRotation(lookToMovement), Time.smoothDeltaTime * 15f
            );
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
            HP.text = "0";
            HPBar.value = 0;
            animator.SetBool("alive", false);
            gun.SetActive(false);
            this.enabled = false;
            controller.enabled = false;
            BroadcastMessages.SendMessage(MessageType.DEATH_PLAYER);
            StopCoroutine(attackEnemy);
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
    }
    IEnumerator RecoveryHP()
    {
        while (currentHP < maxHealthPoint && inBase)
        {
            currentHP++;
            yield return new WaitForSeconds(.2f);
        }
    }

    public void Movement(float speed, Vector3 move)
    {
        move = move * speed * maxSpeed;
        lookToMovement = move;
        animator.SetFloat("speed", speed * maxSpeed);
        controller.Move(move * Time.smoothDeltaTime);
    }

    IEnumerator Attack()
    {
        while (true)
        {
            GameObject enemy = GetNearestEnemy();
            if (enemy is not null)
            {
                lookToEnemy = enemy.transform.position - transform.position;
                gunShooting.Shot(enemy.transform.position + Vector3.up);
            }
            else
                lookToEnemy = Vector3.zero;
            yield return null;
        }
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBase"))
        {
            enemyBase = other.gameObject.GetComponent<EnemyBaseContainer>();
            bool inEnemyBase = true;
            inBase = false;
            SetParams(inEnemyBase);
            StartCoroutine(attackEnemy);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemyBase"))
        {
            bool inEnemyBase = false;
            inBase = true;
            SetParams(inEnemyBase);
            StopCoroutine(attackEnemy);
            StartCoroutine(RecoveryHP());
            lookToEnemy = Vector3.zero;
        }
    }
    void SetParams(bool inBase)
    {
        gun.SetActive(inBase);
        animator.SetBool("inEnemyBase", inBase);
    }
}
