using UnityEngine;

namespace Game
{
    public class Goblim : Enemy
    {
        #region private fields
        private bool _isAfraid;
        #endregion private fields

        #region unity event functions
        private void Update()
        {
            Debug.Log(transform.position);
            if (Spawned)
            {
                switch (ActionState)
                {
                    case EnemyActionState.Following:
                        if (_isAfraid)
                        {
                            Flee();
                        }
                        else if (NavEntity.CanMove && NavEntity.ReachedDestination && !IsDead)
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

                            NavEntity.SetPath(path.GetRange(0, Mathf.Min(4, path.Count)));
                        }
                        break;
                    case EnemyActionState.Fleeing:
                        Flee();
                        break;
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            GameEvents.Instance.SkillActive += OnPlayerSkillEnabledListener;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameEvents.Instance.SkillActive -= OnPlayerSkillEnabledListener;
        }
        #endregion unity event functions

        #region public functions
        public override void Initialize(int x, int y)
        {
            base.Initialize(x, y);

            ActionState = EnemyActionState.Waiting;
            Life = GameController.Instance.SMALL_LIFE;
            IsDead = false;
            CanBeDamaged = false;
            CanDamage = false;
            _animator.SetInteger("state", (int)AnimationStates.Idle);
            NavEntity.SetSpeed(GameController.Instance.ENEMY_HIGH_SPEED);
            NavEntity.DisableMoving();
            CancelInvoke();
        }
        #endregion public functions

        #region private functions
        private void Flee()
        {
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

                //moves a max of 2 nodes per time
                NavEntity.SetPath(path);
            }
        }
        #endregion private functions

        #region event listeners
        private void OnPlayerSkillEnabledListener(object sender, GameEvents.SkillActiveEventArgs args)
        {
            _isAfraid = args.SkillStatus;
        }
        #endregion event listeners
    }
}
