namespace Game
{
    public class Ghost : Navigator
    {
        #region public fields
        public bool CanDamage = true;
        public bool CanBeDamaged = false;
        public bool IsDead = false;
        #endregion public fields   

        #region public functions
        public override void Initialize(int x, int y)
        {
            base.Initialize(x, y);

            NavEntity.SetSpeed(GameController.Instance.GHOST_DEFAULT_SPEED);
        }
        #endregion public functions
    }
}