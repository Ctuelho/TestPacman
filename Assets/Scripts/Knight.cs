using UnityEngine;

namespace Game
{
    public class Knight : Player
    {
        #region private fields
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private ParticleSystem _slashGlow;

        private bool _slashAvailable = true;
        #endregion private fields

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Space) && _slashAvailable && !PowerUpIsActive)
            {
                _slashAvailable = false;
                _animator.SetInteger("state", (int)PlayerAnimationState.Action1);
                _slashGlow.Stop();
                Invoke("EndSlash", GameController.Instance.KNIGHT_SLASH_DURATION);
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
        }

        public override void DisablePowerUp()
        {
            base.DisablePowerUp();
            CancelInvoke();
            EnableSlash();
            _animator.SetInteger("state", (int)PlayerAnimationState.Idle);
            NavEntity.SetSpeed(GameController.Instance.PLAYER_DEFAULT_SPEED);
        }

        private void EndSlash()
        {
            _animator.SetInteger("state", (int)PlayerAnimationState.Idle);
            Invoke("EnableSlash", GameController.Instance.SKILL_INTERVAL);
        }

        private void EnableSlash()
        {
            _slashGlow.Play();
            _slashAvailable = true;
        }
    }
}
