using System.Collections;
using UnityEngine;

namespace Game
{
    public class Elf : Player
    {
        #region private fields
        [SerializeField]
        private GameObject _arrowPrefab;
        [SerializeField]
        private ParticleSystem _dodgeGlow;
        private Vector2 _lastNodeIndexes;
        #endregion private fields

        #region unity event functions
        protected override void Update()
        {
            if (IsDead)
            {
                return;
            }

            if (PowerUpIsActive && NavEntity.CanMove && NavEntity.ReachedDestination)
            {
                if (_lastNodeIndexes.x != NavEntity.LastIndexes.Item1 ||
                    _lastNodeIndexes.y != NavEntity.LastIndexes.Item2)
                {
                    var directions = System.Enum.GetValues(typeof(MovementDirections));
                    foreach (MovementDirections direction in directions)
                    {
                        if (direction != MovementDirections.None)
                        {
                            var arrow = Instantiate(_arrowPrefab).GetComponent<Arrow>();
                            arrow.Initialize(transform.position, direction);
                        }
                    }
                }
            }

            if(Input.GetKeyDown(KeyCode.Space) && SkillIsReady)
            {
                //perform dodge
                CanBeDamaged = false;
                NavEntity.SetSpeed(
                    GameController.Instance.PLAYER_DEFAULT_SPEED *
                    GameController.Instance.ELF_DODGE_SPEED);
                _animator.SetInteger("state", (int)AnimationStates.Action1);
                Invoke("CompleteDodge", GameController.Instance.ELF_DODGE_DURATION);

                GameEvents.Instance.OnPlayerUsedSkill(new GameEvents.OnPlayerUsedSkillEventArgs());
            }

            base.Update();
        }
        #endregion unity event functions

        #region public functions
        public override void EnablePowerUp()
        {
            base.EnablePowerUp();
            _animator.SetInteger("state", (int)AnimationStates.Action2);  
        }

        public override void DisablePowerUp()
        {
            base.DisablePowerUp();
            _animator.SetInteger("state", (int)AnimationStates.Idle);
            NavEntity.SetSpeed(GameController.Instance.PLAYER_DEFAULT_SPEED);
        }

        public override void EnableSkill()
        {
            base.EnableSkill(); 
            _dodgeGlow.Play();
        }

        public override void DisableSkill()
        {
            base.DisableSkill();
            _dodgeGlow.Stop();
        }

        public override void Die()
        {
            base.Die();
            _dodgeGlow.Stop();
        }
        #endregion public functions

        #region private functions
        private void CompleteDodge()
        {
            NavEntity.SetSpeed(GameController.Instance.PLAYER_DEFAULT_SPEED);
            if (PowerUpIsActive)
            {
                _animator.SetInteger("state", (int)AnimationStates.Action2);
            }
            else
            {
                _animator.SetInteger("state", (int)AnimationStates.Idle);
                CanBeDamaged = true;
            }
        }
    }
    #endregion private functions
}

