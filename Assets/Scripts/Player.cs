using System;
using UnityEngine;

namespace Game
{
    public class Player : Navigator
    {
        #region properties
        public bool Initialized { get; private set; }
        #endregion properties

        #region private fields
        private MovementDirection _movementDirection;
        #endregion private fields
        
        #region unity event functions
        // Update is called once per frame
        void Update()
        {
            if (!Initialized)
            {
                Debug.Log("Initialize player first");
                return;
            }   

            //read player's input
            var inputDirection = _movementDirection;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                inputDirection = MovementDirection.Up;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                inputDirection = MovementDirection.Down;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                inputDirection = MovementDirection.Left;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                inputDirection = MovementDirection.Right;
            }

            //it means the entity has stopped
            if (NavEntity.CanMove && NavEntity.ReachedDestination)
            {
                bool changedDirection = false;

                if (inputDirection != _movementDirection)
                {
                    int xIndexShiftNewDirection = 0;
                    int yIndexShiftNewDirection = 0;

                    //calc the direction on grid coordinates for new input
                    switch (inputDirection)
                    {
                        case MovementDirection.Up:
                            xIndexShiftNewDirection = 0;
                            yIndexShiftNewDirection = 1;
                            break;
                        case MovementDirection.Down:
                            xIndexShiftNewDirection = 0;
                            yIndexShiftNewDirection = -1;
                            break;
                        case MovementDirection.Left:
                            xIndexShiftNewDirection = -1;
                            yIndexShiftNewDirection = 0;
                            break;
                        case MovementDirection.Right:
                            xIndexShiftNewDirection = 1;
                            yIndexShiftNewDirection = 0;
                            break;
                    }

                    //try finding a walkable node in the new direction
                    Pacman.NavNode nodeInDirection =
                            GameManager.Instance.LevelManager.NavGraph.GetNode(
                                (int)Math.Round(NavEntity.Position.Item1) + xIndexShiftNewDirection,
                                (int)Math.Round(NavEntity.Position.Item2) + yIndexShiftNewDirection);

                    //found a node, change player's path and direction
                    if (nodeInDirection != null)
                    {
                        changedDirection = true;
                        _movementDirection = inputDirection;
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
                    switch (_movementDirection)
                    {
                        case MovementDirection.Up:
                            xIndexShiftOldDirection = 0;
                            yIndexShiftOldDirection = 1;
                            break;
                        case MovementDirection.Down:
                            xIndexShiftOldDirection = 0;
                            yIndexShiftOldDirection = -1;
                            break;
                        case MovementDirection.Left:
                            xIndexShiftOldDirection = -1;
                            yIndexShiftOldDirection = 0;
                            break;
                        case MovementDirection.Right:
                            xIndexShiftOldDirection = 1;
                            yIndexShiftOldDirection = 0;
                            break;
                    }

                    //try finding a walkable node in the old direction
                    Pacman.NavNode nodeInDirection =
                            GameManager.Instance.LevelManager.NavGraph.GetNode(
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
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //test if it was a pellet
            var pellet = collision.gameObject.GetComponent<Pellet>();
            if(pellet != null)
            {
                pellet.Collect();
                return;
            }
        }
        #endregion unity event functions  

        #region public functions
        public void Initialize(int x, int y)
        {
            //initialize player
            NavEntity = new Pacman.NavEntity();
            _movementDirection = MovementDirection.Left;
            NavEntity.SetCurrentPosition(x, y);
            NavEntity.SetSpeed(5);
            NavEntity.EnableMoving();
            Initialized = true;
        }
        #endregion public functions

    }
}
