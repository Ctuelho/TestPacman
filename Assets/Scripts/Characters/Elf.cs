using System.Collections;
using System.Collections.Generic;
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
        private Coroutine _fireKnivesCoroutine;
        #endregion private fields

        #region unity event functions
        protected override void Update()
        {
            if (IsDead)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space) && SkillIsReady && !PowerUpIsActive)
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
            NavEntity.SetSpeed(GameController.Instance.PLAYER_DEFAULT_SPEED *
                                    GameController.Instance.ELF_POWERUP_SPEED_MUL);
            _animator.SetInteger("state", (int)AnimationStates.Action2);
            if (_fireKnivesCoroutine != null)
                StopCoroutine(_fireKnivesCoroutine);
            _fireKnivesCoroutine = StartCoroutine(FireKnives());
        }

        public override void DisablePowerUp()
        {
            base.DisablePowerUp();
            _animator.SetInteger("state", (int)AnimationStates.Idle);
            NavEntity.SetSpeed(GameController.Instance.PLAYER_DEFAULT_SPEED);
            if (_fireKnivesCoroutine != null)
                StopCoroutine(_fireKnivesCoroutine);
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
            if (PowerUpIsActive)
            {
                NavEntity.SetSpeed(GameController.Instance.PLAYER_DEFAULT_SPEED *
                                    GameController.Instance.ELF_POWERUP_SPEED_MUL);
            }
            else
            {
                NavEntity.SetSpeed(GameController.Instance.PLAYER_DEFAULT_SPEED);
                _animator.SetInteger("state", (int)AnimationStates.Idle);
                CanBeDamaged = true;
            }
        }
        #endregion private functions

        #region coroutines
        IEnumerator FireKnives()
        {
            var timer = 0f;
            while (PowerUpIsActive)
            {
                timer += Time.deltaTime;
                if (timer >= GameController.Instance.ARROW_CAST_INTERVAL)
                {
                    timer -= GameController.Instance.ARROW_CAST_INTERVAL;
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
                yield return null;
            }
        }
        #endregion coroutines
    }
}

