using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GhostA : Ghost
    {
        #region unity event functions
        void Update()
        {
            if (NavEntity.CanMove && NavEntity.ReachedDestination && !IsDead)
            {
                //follow player
                var currentNode = 
                    GameController.Instance.LevelManager.NavGraph.GetNode(
                        NavEntity.LastIndexes.Item1, NavEntity.LastIndexes.Item2);

                var path =
                    GameController.Instance.LevelManager.NavGraph.ShortestPath(
                            currentNode, GameController.Instance.GetPlayerCurrentNode());

                //moves a max of 3 nodes per time
                NavEntity.SetPath(path.GetRange(0, Mathf.Min(3, path.Count)));
            }
        }
        #endregion unity event functions
    }
}