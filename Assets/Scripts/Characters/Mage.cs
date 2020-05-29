using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Game
{
    public class Mage : Player
    {
        #region private fields
        [SerializeField]
        private ParticleSystem _fireBallGlow;
        [SerializeField]
        private GameObject _lightningFieldPrefab;
        [SerializeField]
        private GameObject _fireballPrefab;
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

            if (Input.GetKeyDown(KeyCode.Space) && SkillIsReady && !PowerUpIsActive)
            {
                var directions = System.Enum.GetValues(typeof(MovementDirections));
                foreach(MovementDirections direction in directions)
                {
                    if(direction != MovementDirections.None)
                    {
                        var fireball = Instantiate(_fireballPrefab).GetComponent<Fireball>();
                        fireball.Initialize(transform.position, direction, GameController.Instance.FIRE_BALL_SPEED);
                    }
                }
                _fireBallGlow.Stop();

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
            _animator.SetInteger("state", (int)AnimationStates.Idle);
            NavEntity.SetSpeed(GameController.Instance.PLAYER_DEFAULT_SPEED);
        }

        public override void EnableSkill()
        {
            base.EnableSkill();
            _fireBallGlow.Play();
        }

        public override void DisableSkill()
        {
            base.DisableSkill();
            _fireBallGlow.Stop();
        }

        public override void Die()
        {
            base.Die();
            _fireBallGlow.Stop();
        }
        #endregion public functions
    }
}
