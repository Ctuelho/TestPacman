using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Enemy : Navigator
    {
        #region properties 
        public EnemyTypes EnemyType { get; protected set; } = EnemyTypes.None;
        public bool CanDamage { get; protected set; } = true;
        public bool CanBeDamaged { get; protected set; } = true;
        public bool IsDead { get; protected set; } = false;
        public bool Spawned { get; protected set; } = false;
        public int Life { get; protected set; } = 1;
        public EnemyActionState ActionState { get; protected set; } = EnemyActionState.None;
        #endregion properties

        #region private fields
        [SerializeField]
        private Rigidbody2D _rigidbody;
        [SerializeField]
        protected Animator _animator;
        #endregion private fields

        #region unity event functions
        protected virtual void OnEnable()
        {
            GameEvents.Instance.PowerUpActive += OnPowerActiveUpListener;
        }

        protected virtual void OnDisable()
        {
            GameEvents.Instance.PowerUpActive -= OnPowerActiveUpListener;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //check if it was a spell
            if(collision.gameObject.layer == GameController.Instance.SPELLS_LAYER)
            {
                Debug.Log("OnTriggerEnter2D spell " + gameObject.name);
                ReceiveDamage();
            }
            else if (collision.gameObject.layer == GameController.Instance.PLAYER_LAYER)
            {
                if(CanDamage)
                    GameEvents.Instance.OnPlayerDamaged(new GameEvents.PlayerDamagedEventArgs());
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.layer == GameController.Instance.SPELLS_LAYER)
            {
                Debug.Log("OnTriggerStay2D spell " + gameObject.name);
                ReceiveDamage();
            }
        }
        #endregion unity event functions

        #region private functions
        protected virtual void ReceiveDamage()
        {
            if (!CanBeDamaged)
                return;

            Life--;
            if(Life == 0)
            {
                Die();
            }
            else
            {
                NavEntity.DisableMoving();
                CanBeDamaged = false;
                CanDamage = false;
                _animator.SetInteger("state", (int)AnimationStates.Hurt);
                Invoke("RecoverFromDamage", GameController.Instance.RECOVER_INVERVAL);
            }  
        }

        protected virtual void RecoverFromDamage()
        {
            if(Life == 0)
            {
                Die();
            }
            else
            {
                _animator.SetInteger("state", (int)AnimationStates.Idle);
                NavEntity.EnableMoving();
                CanBeDamaged = true;
                CanDamage = true;
            }
        }

        protected virtual void Die()
        {
            NavEntity.DisableMoving();
            ActionState = EnemyActionState.Dead;
            CanBeDamaged = false;
            CanDamage = false;
            IsDead = true;
            _animator.SetInteger("state", (int)AnimationStates.Dead);
            Invoke("RecoverFromDeathAnimation", GameController.Instance.DEATH_ANIMATION_INTERVAL);
            GameEvents.Instance.OnEnemyDead(new GameEvents.OnEnemyDeadEventArgs { EnemyType = EnemyType });
        }

        private void RecoverFromDeathAnimation()
        {
            gameObject.SetActive(false);
        }
        #endregion private functions

        #region public functions
        public void SetEnemyType(EnemyTypes enemyType)
        {
            EnemyType = enemyType;
        }

        public override void Initialize(int x, int y)
        {
            base.Initialize(x, y);

            ActionState = EnemyActionState.Waiting;
            Life = GameController.Instance.SMALL_LIFE;
            IsDead = false;
            CanBeDamaged = false;
            CanDamage = false;
            _animator.SetInteger("state", (int)AnimationStates.Idle);
            NavEntity.SetSpeed(GameController.Instance.ENEMY_SLOW_SPEED);
            NavEntity.DisableMoving();
            CancelInvoke();
        }

        public void WalkOutsideSpawnZone(List<Pacman.NavNode> path)
        {
            NavEntity.SetPath(path);
            NavEntity.EnableMoving();
            StartCoroutine(WaitWalkingOutsideSpawnZone());
        }
        #endregion public functions

        #region event listeners
        protected virtual void OnPowerActiveUpListener(object sender, GameEvents.PowerUpActiveEventArgs args)
        {
            if (IsDead)
                return;

            if (args.PowerUpStatus)
            {
                CanDamage = false;
                ActionState = EnemyActionState.Fleeing;
            }
            else
            {
                CanDamage = true;
                ActionState = EnemyActionState.Following;
            }
        }
        #endregion event listeners

        #region coroutines
        IEnumerator WaitWalkingOutsideSpawnZone()
        {
            while(!NavEntity.ReachedDestination)
            {
                yield return null;
            }
            ActionState = EnemyActionState.Following;
            Spawned = true;
            CanDamage = true;
            CanBeDamaged = true;
        }
        #endregion coroutines
    }
}