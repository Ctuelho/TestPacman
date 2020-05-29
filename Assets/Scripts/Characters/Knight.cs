using UnityEngine;

namespace Game
{
    public class Knight : Player
    {
        #region private fields
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private ParticleSystem _slashGlow;
        #endregion private fields

        #region unity event functions
        protected override void Update()
        {
            if (IsDead)
            {
                return;
            }

            base.Update();
            
            if (Input.GetKeyDown(KeyCode.Space) && SkillIsReady && !PowerUpIsActive)
            {
                _animator.SetInteger("state", (int)AnimationStates.Action1);
                Invoke("EndSlash", GameController.Instance.KNIGHT_SLASH_DURATION);

                GameEvents.Instance.OnPlayerUsedSkill(new GameEvents.OnPlayerUsedSkillEventArgs());
            }
        }
        #endregion unity event functions

        #region public functions
        public override void EnablePowerUp()
        {
            base.EnablePowerUp();
            CancelInvoke();
            _animator.SetInteger("state", (int)AnimationStates.Action2);
            NavEntity.SetSpeed(
                GameController.Instance.PLAYER_DEFAULT_SPEED *
                GameController.Instance.KNIGHT_POWERUP_SPEED_MUL);
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
            _slashGlow.Play();
        }

        public override void DisableSkill()
        {
            base.DisableSkill();
            _slashGlow.Stop();
        }

        public override void Die()
        {
            base.Die();
            _slashGlow.Stop();
        }
        #endregion public functions

        #region private functions
        private void EndSlash()
        {
            _animator.SetInteger("state", (int)AnimationStates.Idle);
        }
        #endregion private functions
    }
}
