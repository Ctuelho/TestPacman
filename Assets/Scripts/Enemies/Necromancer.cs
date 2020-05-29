using UnityEngine;

namespace Game
{
    public class Necromancer : Enemy
    {
        #region private fields
        [SerializeField]
        private GameObject _magicPrefab;
        private float _magicTimer;
        #endregion private fields

        #region unity event functions
        void Update()
        {
            if (Spawned)
            {
                switch (ActionState)
                {
                    case EnemyActionState.Following:

                        //test condictions for firing the magi ball
                        if (CanDamage)
                        {
                            if (_magicTimer > GameController.Instance.NECRO_MAGIC_COOLDOWN)
                            {
                                _magicTimer = 0;
                                //fires the magic
                                var directions = System.Enum.GetValues(typeof(MovementDirections));
                                foreach (MovementDirections direction in directions)
                                {
                                    if (direction != MovementDirections.None)
                                    {
                                        var fireball = Instantiate(_magicPrefab).GetComponent<Fireball>();
                                        fireball.Initialize(transform.position, direction, GameController.Instance.NECRO_MAGIC_SPEED);
                                    }
                                }
                            }
                            else
                            {
                                _magicTimer += Time.deltaTime;
                            }
                        }


                        if (NavEntity.CanMove && NavEntity.ReachedDestination && !IsDead)
                        {
                            //follow player
                            var currentNode =
                                GameController.Instance.LevelManager.NavGraph.GetNode(
                                    NavEntity.LastIndexes.Item1, NavEntity.LastIndexes.Item2);

                            var path =
                                GameController.Instance.LevelManager.NavGraph.ShortestPath(
                                    currentNode, GameController.Instance.GetPlayerCurrentNode(),
                                        new System.Collections.Generic.List<Pacman.NodeType>()
                                        { Pacman.NodeType.NonWalkable });

                            //moves a max of 3 nodes per time
                            NavEntity.SetPath(path.GetRange(0, Mathf.Min(3, path.Count)));
                        }
                        break;
                    case EnemyActionState.Fleeing:
                        if (NavEntity.CanMove && NavEntity.ReachedDestination && !IsDead)
                        {
                            //avoid player
                            var currentNode =
                                GameController.Instance.LevelManager.NavGraph.GetNode(
                                    NavEntity.LastIndexes.Item1, NavEntity.LastIndexes.Item2);

                            var path =
                                GameController.Instance.LevelManager.NavGraph.FarthestPath(
                                    currentNode, GameController.Instance.GetPlayerCurrentNode(),
                                        new System.Collections.Generic.List<Pacman.NodeType>()
                                        { Pacman.NodeType.NonWalkable });

                            NavEntity.SetPath(path);
                        }
                        break;
                }
            }
        }
        #endregion unity event functions

        #region public functions
        public override void Initialize(int x, int y)
        {
            base.Initialize(x, y);

            ActionState = EnemyActionState.Waiting;
            Life = GameController.Instance.MEDIUM_LIFE;
            IsDead = false;
            CanBeDamaged = false;
            CanDamage = false;
            _animator.SetInteger("state", (int)AnimationStates.Idle);
            NavEntity.SetSpeed(GameController.Instance.ENEMY_MEDIUM_SPEED);
            NavEntity.DisableMoving();
            CancelInvoke();
        }
        #endregion public functions
    }
}
