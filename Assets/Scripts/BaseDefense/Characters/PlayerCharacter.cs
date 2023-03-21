using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;
using BaseDefense.AttackImplemention.Guns;
using BaseDefense.BroadcastMessages;
using BaseDefense.BroadcastMessages.Messages;
using BaseDefense.Items;
using BaseDefense.SaveSystem;
using BaseDefense.UI;
using UnityEngine.Profiling;

namespace BaseDefense.Characters {

    [RequireComponent(typeof(ItemCollecting), typeof(DisplayHealthPoints))]
    public sealed class PlayerCharacter : BaseCharacter {

        ///<summary>Transform руки, в которой игрок держит оружие</summary>
        [Header("Связанные объекты")]
        [Tooltip("Transform руки, в которой игрок держит оружие")]
        [SerializeField]
        private Transform gunSlot;

        ///<inheritdoc cref="DisplayHealthPoints"/>
        private DisplayHealthPoints m_displayHealthPoints;

        ///<inheritdoc cref="ItemCollecting"/>
        private ItemCollecting m_itemCollecting;

        private bool m_inEnemyBase;


        [Inject]
        public void Constructor (JoystickController joystick, Shop shop, DisplayingUI displayingUI) {
            m_joystick = joystick;
            m_shop = shop;
            m_displayingUI = displayingUI;
        }


        public override void Save (GameDataWriter writer) {
            base.Save(writer);
            writer.Write(m_gun.Id);
            writer.Write(maxHealthPoints);
            writer.Write(maxSpeed);
            m_itemCollecting.Save(writer);
        }


        public override void Load (GameDataReader reader) {
            base.Load(reader);
            SelectGun(reader.ReadInteger());
            maxHealthPoints = reader.ReadFloat();
            maxSpeed = reader.ReadFloat();
            m_itemCollecting.Load(reader);
        }


        public override void Hit (float damage) {
            CurrentHealthPoints -= damage;
            m_displayHealthPoints.UpdateView((int) CurrentHealthPoints);
            var emission = HitEffect.emission;
            emission.SetBurst(0,
                new ParticleSystem.Burst(0, (int) damage * 100 / maxHealthPoints));
            HitEffect.Play();
        }


        private Shop m_shop;
        private Gun m_gun;


        ///<summary>Вызывается при выборе оружия из магазина</summary>
        ///<param name="gunId">Выбранное оружие</param>
        public void SelectGun (int gunId) {
            m_gun = m_shop.TakeGun(gunId, m_gun);
            m_gun.transform.parent = gunSlot;
            m_gun.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }


        public void Upgrade (UpgradableProperty property) {
            switch (property.upgradablePropertyType) {
                case UpgradablePropertyType.SPEED:
                    UpgradeSpeed(property.CurrentStep);
                    break;
                case UpgradablePropertyType.CAPACITY:
                    UpgradeMaxCapacity(property.CurrentStep);
                    break;
                case UpgradablePropertyType.MAX_HEALTH:
                    UpgradeMaxHealth(property.CurrentStep);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }



        #region PlayerUpgrades

        private void UpgradeSpeed (UpgradablePropertyStep propertyStep) {
            maxSpeed = propertyStep.value;
        }


        private void UpgradeMaxCapacity (UpgradablePropertyStep propertyStep) {
            m_itemCollecting.UpgradeCapacity(propertyStep);
        }


        private void UpgradeMaxHealth (UpgradablePropertyStep propertyStep) {
            maxHealthPoints = propertyStep.value;
            CurrentHealthPoints = maxHealthPoints;
            m_displayHealthPoints.SetMaxValue((int) maxHealthPoints);
            m_displayHealthPoints.UpdateView((int) CurrentHealthPoints);
        }

        #endregion



        #region PlayerInitialization

        protected override void Awake () {
            base.Awake();
            m_itemCollecting = GetComponent<ItemCollecting>();
            m_displayHealthPoints = GetComponent<DisplayHealthPoints>();
            m_gun = gunSlot.GetChild(0).GetComponent<Gun>();
            Messenger.SubscribeTo<RestartMessage>(Resurrection);
        }


        private void OnDestroy () {
            Messenger.UnsubscribeFrom<RestartMessage>(Resurrection);
        }


        private void Start () {
            m_displayHealthPoints.SetMaxValue((int) maxHealthPoints);
            m_displayHealthPoints.UpdateView((int) CurrentHealthPoints);
            m_gun.gameObject.SetActive(false);
            m_gazeDirection = transform.forward;
        }

        #endregion



        #region PlayerUpdate

        private static readonly int SpeedId = Animator.StringToHash("speed");


        ///<summary>Направление взгляда в сторону атакуемой сущности</summary>
        private Vector3 m_gazeDirection;

        private Vector3 m_move;

        private JoystickController m_joystick;


        private void Update () {
            // Реализует плавный поворот к цели с определённой скоростью
            LookToTarget(m_gazeDirection, 15);

            Movement();

            if (m_inEnemyBase) {
                // Ожидание перехода анимации для предотвращения преждевременной атаки
                if (!Animator.IsInTransition(0))
                    Attack();
                // Если анимация ещё проигрывается - ничего не делаем
                else { }
            }
            else HealthRecovery();

            void LookToTarget (Vector3 target, float rotationSpeed) {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, Quaternion.LookRotation(target), Time.smoothDeltaTime * rotationSpeed
                );
            }
        }


        private void Movement () {
            m_move = m_joystick.GetInput();
            Animator.SetFloat(SpeedId, m_move.magnitude * maxSpeed);
            m_move *= maxSpeed;
            Controller.Move(m_move * Time.smoothDeltaTime);
            m_gazeDirection = m_move == Vector3.zero ? transform.forward : m_move;
        }


        private void Attack () {
            Profiler.BeginSample("Attack");

            var attackable = GetNearestAttackable();

            if (attackable is null) return;

            var attackablePosition = attackable.transform.position;
            var playerPosition = transform.position;
            attackablePosition.y = playerPosition.y = 0;
            m_gazeDirection = attackablePosition - playerPosition;

            // Прицеливаемся
            const float aimingAccuracy = 0.95f;

            if (Vector3.Dot(m_gazeDirection.normalized, transform.forward) > aimingAccuracy) {
                // Если стреляем из гранатомёта - определяем безопасное расстояние
                if (m_gun is GrenadeLauncher grenade) {
                    var playerTransform = transform;
                    var ray = new Ray(playerTransform.position + Vector3.up, playerTransform.forward);

                    // Если расстояние до цели безопасно - стреляем
                    if (Physics.Raycast(ray, out var hit) && hit.distance > grenade.DamageRadius)
                        grenade.Shot();
                    // Иначе не стреляем
                    else { }
                }
                // Иначе стреляем сразу
                else m_gun.Shot();
            }
            // Если ещё не прицелились - ничего не делаем
            else { }

            Profiler.EndSample();
        }


        ///<returns>Возвращает ближайшую к игроку атакуемую сущность. Если рядом таких нет - возвращает null</returns>
        private GameObject GetNearestAttackable () {
            // ReSharper disable once Unity.PreferNonAllocApi
            var attackables = Physics.OverlapSphere(
                transform.position, attackDistance, 1 << 3
            ); // 1 << 3 - маска слоя атакуемой сущности (3: AttackableLayer)
            var enemies = attackables.Where((e) => e.GetComponent<EnemyCharacter>()).ToArray();

            // Приоритет отдаётся врагам. Если они не найдены - атакуем другие сущности
            return FindTarget(enemies.Any() ? enemies : attackables);

            GameObject FindTarget (IEnumerable targets) {
                GameObject target = null;

                float dist = Mathf.Infinity;

                foreach (Collider coll in targets) {
                    var distance = coll.transform.position - transform.position;

                    if (distance.magnitude < dist) {
                        target = coll.gameObject;
                        dist = distance.magnitude;
                    }
                }

                return target;
            }
        }


        private void HealthRecovery () {
            const float recoveryTime = 30;
            CurrentHealthPoints += maxHealthPoints * Time.smoothDeltaTime / recoveryTime;
            m_displayHealthPoints.UpdateView((int) CurrentHealthPoints);
        }

        #endregion



        #region TriggerEvents

        private static readonly int InEnemyBase = Animator.StringToHash("inEnemyBase");

        private DisplayingUI m_displayingUI;


        private void OnTriggerEnter (Collider other) {
            if (other.GetComponent<Item>() is { } item)
                switch (item) {
                    case Gem gem:
                        m_itemCollecting.PutGem(gem);
                        break;
                    case Money money:
                        m_itemCollecting.StackMoney(money);
                        break;
                    default:
                        throw new NotImplementedException($"Обработка объекта {item} не реализована");
                }
            else if (other.CompareTag("gun shop"))
                m_displayingUI.OpenShop();
            else if (other.CompareTag("player upgrades"))
                m_displayingUI.OpenUpgrades();
            else if (other.CompareTag("enemy field"))
                SetParams(true);
        }


        private void OnTriggerExit (Collider other) {
            if (other.CompareTag("gun shop"))
                m_displayingUI.CloseShop();
            else if (other.CompareTag("player upgrades"))
                m_displayingUI.CloseUpgrades();
            else if (other.CompareTag("enemy field")) {
                SetParams(false);
                if (!m_itemCollecting.DropIsInProcess)
                    StartCoroutine(m_itemCollecting.DropMoney());
            }
        }


        ///<summary>Устанавливает параметры, связанные с нахождением на вражеской базе</summary>
        ///<param name="value">Значение, устанавливаемое необходимым параметрам</param>
        private void SetParams (bool value) {
            m_inEnemyBase = value;
            m_gun.gameObject.SetActive(value);
            Animator.SetBool(InEnemyBase, value);
        }

        #endregion



        #region PlayerLifecycle

        private static readonly int Alive = Animator.StringToHash("alive");


        protected override void OnDeath () {
            Animator.SetBool(Alive, false);
            m_gun.gameObject.SetActive(false);
            Enabled = false;
            MeshRenderer.material.color = deathColor;
            Messenger.SendMessage<DeathPlayerMessage>();
        }


        private void Resurrection () {
            var respawn = GameObject.FindGameObjectWithTag("Respawn").transform;
            Animator.SetBool(Alive, true);
            Animator.SetBool(InEnemyBase, false);
            transform.SetPositionAndRotation(respawn.position, Quaternion.identity);
            Enabled = true;
            CurrentHealthPoints = maxHealthPoints;
            m_inEnemyBase = false;
            MeshRenderer.material.color = DefaultColor;
        }

        #endregion

    }

}