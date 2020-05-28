using System;
using UnityEngine;

namespace Game
{
    public class Player : Navigator
    {
        #region properties
        public bool CanBeDamaged { get; protected set; } = true;
        public bool PowerUpIsActive { get; protected set; } = false;
        public MovementDirections MovementDirection { get; private set; } = MovementDirections.Left;
        #endregion properties

        #region unity event functions
        // Update is called once per frame
        protected virtual void Update()
        {
            //player moves on tile per time

            if (!Initialized)
            {
                Debug.Log("Initialize player first");
                return;
            }   

            //read player's input
            var inputDirection = MovementDirection;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                inputDirection = MovementDirections.Up;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                inputDirection = MovementDirections.Down;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                inputDirection = MovementDirections.Left;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                inputDirection = MovementDirections.Right;
            }

            //it means the entity has stopped
            if (NavEntity.CanMove && NavEntity.ReachedDestination)
            {
                GameEvents.Instance.OnPlayerWalkedTile(
                    new GameEvents.OnPlayerWalkedTileEventArgs()
                    {
                        indexX = NavEntity.LastIndexes.Item1, 
                        indexY = NavEntity.LastIndexes.Item2
                    });             

                bool changedDirection = false;

                if (inputDirection != MovementDirection)
                {
                    int xIndexShiftNewDirection = 0;
                    int yIndexShiftNewDirection = 0;

                    //calc the direction on grid coordinates for new input
                    switch (inputDirection)
                    {
                        case MovementDirections.Up:
                            xIndexShiftNewDirection = 0;
                            yIndexShiftNewDirection = 1;
                            break;
                        case MovementDirections.Down:
                            xIndexShiftNewDirection = 0;
                            yIndexShiftNewDirection = -1;
                            break;
                        case MovementDirections.Left:
                            xIndexShiftNewDirection = -1;
                            yIndexShiftNewDirection = 0;
                            break;
                        case MovementDirections.Right:
                            xIndexShiftNewDirection = 1;
                            yIndexShiftNewDirection = 0;
                            break;
                    }

                    //try finding a walkable node in the new direction
                    Pacman.NavNode nodeInDirection =
                            GameController.Instance.LevelManager.NavGraph.GetNode(
                                (int)Math.Round(NavEntity.Position.Item1) + xIndexShiftNewDirection,
                                (int)Math.Round(NavEntity.Position.Item2) + yIndexShiftNewDirection);

                    //found a node, change player's path and direction
                    if (nodeInDirection != null)
                    {
                        changedDirection = true;
                        MovementDirection = inputDirection;
                        NavEntity.SetPath(
                            new System.Collections.Generic.List<Pacman.NavNode>() { nodeInDirection });
                    }
                }

                //try to move in same direction
                if (!changedDirection)
                {
                    int xIndexShiftOldDirection = 0;
                    int yIndexShiftOldDirection = 0;

                    //calc the direction on grid coordinates for new input
                    switch (MovementDirection)
                    {
                        case MovementDirections.Up:
                            xIndexShiftOldDirection = 0;
                            yIndexShiftOldDirection = 1;
                            break;
                        case MovementDirections.Down:
                            xIndexShiftOldDirection = 0;
                            yIndexShiftOldDirection = -1;
                            break;
                        case MovementDirections.Left:
                            xIndexShiftOldDirection = -1;
                            yIndexShiftOldDirection = 0;
                            break;
                        case MovementDirections.Right:
                            xIndexShiftOldDirection = 1;
                            yIndexShiftOldDirection = 0;
                            break;
                    }

                    //try finding a walkable node in the old direction
                    Pacman.NavNode nodeInDirection =
                            GameController.Instance.LevelManager.NavGraph.GetNode(
                                (int)Math.Round(NavEntity.Position.Item1) + xIndexShiftOldDirection,
                                (int)Math.Round(NavEntity.Position.Item2) + yIndexShiftOldDirection);

                    //found a node, change player's target
                    if (nodeInDirection != null)
                    {
                        NavEntity.SetPath(
                            new System.Collections.Generic.List<Pacman.NavNode>() { nodeInDirection });
                    }
                }
            }

            if (MovementDirection == MovementDirections.Left)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (MovementDirection == MovementDirections.Right)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            
        }
        #endregion unity event functions  

        #region public functions
        public override void Initialize(int x, int y)
        {
            base.Initialize(x, y);

            MovementDirection = MovementDirections.Left;
            NavEntity.SetSpeed(GameController.Instance.PLAYER_DEFAULT_SPEED);
        }

        public virtual void EnablePowerUp()
        {
            PowerUpIsActive = true;
            CanBeDamaged = false;
        }

        public virtual void DisablePowerUp()
        {
            PowerUpIsActive = false;
            CanBeDamaged = true;
        }
        #endregion public functions
    }
}
