using System.Collections;
using UnityEngine;

namespace Game
{
    public class Elf : Player
    {
        #region private fields
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private GameObject _arrowPrefab;
        [SerializeField]
        private ParticleSystem _dodgeGlow;
        private bool _dodgeAvailable = true;
        private Coroutine CastArrowsCoroutine;
        #endregion private fields

        protected override void Update()
        {
            base.Update();

            if(Input.GetKeyDown(KeyCode.Space) && _dodgeAvailable)
            {
                //perform dodge
                _dodgeAvailable = false;
                CanBeDamaged = false;
                NavEntity.SetSpeed(
                    GameController.Instance.PLAYER_DEFAULT_SPEED *
                    GameController.Instance.ELF_DODGE_SPEED);
                _dodgeGlow.Stop();
                _animator.SetInteger("state", (int)PlayerAnimationState.Action1);
                Invoke("CompleteDodge", GameController.Instance.ELF_DODGE_DURATION);
            }
        }

        public override void EnablePowerUp()
        {
            base.EnablePowerUp();
            CancelInvoke();
            _animator.SetInteger("state", (int)PlayerAnimationState.Action2);
            NavEntity.SetSpeed(
                GameController.Instance.PLAYER_DEFAULT_SPEED *
                GameController.Instance.KNIGHT_POWERUP_SPEED_MUL);
            if (CastArrowsCoroutine != null)
                StopCoroutine(CastArrows());
            CastArrowsCoroutine = StartCoroutine(CastArrows());
        }

        public override void DisablePowerUp()
        {
            base.DisablePowerUp();
            CancelInvoke();
            EnableDodge();
            _animator.SetInteger("state", (int)PlayerAnimationState.Idle);
            NavEntity.SetSpeed(GameController.Instance.PLAYER_DEFAULT_SPEED);
        }

        private void CompleteDodge()
        {
            CanBeDamaged = true;
            NavEntity.SetSpeed(GameController.Instance.PLAYER_DEFAULT_SPEED);
            _animator.SetInteger("state", (int)PlayerAnimationState.Idle);
            Invoke("EnableDodge", GameController.Instance.SKILL_INTERVAL);
        }

        private void EnableDodge()
        {
            _dodgeAvailable = true;
            _dodgeGlow.Play();
        }

        IEnumerator CastArrows()
        {
            var timer = 0f;
            while (PowerUpIsActive)
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
                
                while(timer < GameController.Instance.ARROW_CAST_INTERVAL)
                {
                    timer += Time.deltaTime;
                    yield return null;
                }

                timer = 0;
            }
        }
    }
}

