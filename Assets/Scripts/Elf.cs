using UnityEngine;

namespace Game
{
    public class Elf : Player
    {
        #region private fields
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private ParticleSystem _dodgeGlow;
        [SerializeField]
        private TrailRenderer _dodgeTrail;
        private bool _dodgeAvailable = true;
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
                _dodgeTrail.emitting = true;
                _dodgeGlow.Stop();
                _spriteRenderer.color = new Color(0, 1, 0, 0.5f);
                Invoke("CompleteDodge", 0.2f);
            }
        }

        private void CompleteDodge()
        {
            CanBeDamaged = true;
            _dodgeTrail.emitting = false;
            NavEntity.SetSpeed(GameController.Instance.PLAYER_DEFAULT_SPEED);
            _spriteRenderer.color = new Color(1, 1, 1, 1);
            Invoke("EnableDodge", 5f);
        }

        private void EnableDodge()
        {
            _dodgeAvailable = true;
            _dodgeGlow.Play();
        }
    }
}

