using UnityEngine;

namespace Game
{
    public class Mage : Player
    {
        #region private fields
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private ParticleSystem _fireBallGlow;
        [SerializeField]
        private GameObject _lightningFieldPrefab;
        [SerializeField]
        private GameObject _fireballPrefab;
        private bool _fireBallIsAvailable = true;
        private Vector2 _lastNodeIndexes;
        #endregion private fields

        protected override void Update()
        {
            if (PowerUpIsActive && NavEntity.CanMove && NavEntity.ReachedDestination)
            {
                if(_lastNodeIndexes.x != NavEntity.LastIndexes.Item1 ||
                    _lastNodeIndexes.y != NavEntity.LastIndexes.Item2)
                {
                    //spawn a lightning field
                    var lightningField = Instantiate(_lightningFieldPrefab);
                    lightningField.transform.position = transform.position;

                    _lastNodeIndexes = new Vector2(
                            NavEntity.LastIndexes.Item1,
                            NavEntity.LastIndexes.Item2);
                }
                
            }

            if (Input.GetKeyDown(KeyCode.Space) && _fireBallIsAvailable && !PowerUpIsActive)
            {
                _fireBallIsAvailable = false;

                var directions = System.Enum.GetValues(typeof(MovementDirections));
                foreach(MovementDirections direction in directions)
                {
                    if(direction != MovementDirections.None)
                    {
                        var fireball = Instantiate(_fireballPrefab).GetComponent<Fireball>();
                        fireball.Initialize(transform.position, direction);
                    }
                }
                _fireBallGlow.Stop();
                Invoke("EnableFireball", 5f);
            }

            base.Update();
        }

        public override void EnablePowerUp()
        {
            base.EnablePowerUp();
            CancelInvoke();
            _animator.SetInteger("state", (int)PlayerAnimationState.Action2);
            NavEntity.SetSpeed(
                GameController.Instance.PLAYER_DEFAULT_SPEED *
                 GameController.Instance.MAGE_POWERUP_SPEED_MUL);
            _lastNodeIndexes = new Vector2(
                            NavEntity.LastIndexes.Item1,
                            NavEntity.LastIndexes.Item2);
        }

        public override void DisablePowerUp()
        {
            base.DisablePowerUp();
            CancelInvoke();
            EnableFireball();
            _animator.SetInteger("state", (int)PlayerAnimationState.Idle);
            NavEntity.SetSpeed(GameController.Instance.PLAYER_DEFAULT_SPEED);
        }

        private void EnableFireball()
        {
            _fireBallGlow.Play();
            _fireBallIsAvailable = true;
        }
    }
}
