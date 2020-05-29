using System;
using System.Collections.Generic;
using System.Linq;

namespace Pacman 
{
    #region navigation graph
    public enum NodeType 
    { 
        None, 
        Normal, 
        PlayerSpawn,
        PowerUp,
        BonusPoint,
        EnemyPosition1,
        EnemyPosition2,
        EnemyPosition3,
        EnemyPosition4,
        WarperUp,
        WarperDown,
        WarperLeft,
        WarperRight,
        NonWalkable
    }

    public class NavNode
    {
        #region NavNode public fields
        public Tuple<int, int> Indexes;
        public NavNode Up;
        public NavNode Down;
        public NavNode Left;
        public NavNode Right;
        public NodeType NodeType;

        //for graph algorithms
        public List<NavNode> Neighbors;
        public bool Visited;
        public int Distance;
        public NavNode PreviousNodeInPath;
        #endregion NavNode public fields

        public NavNode() { }
    }

    public class NavGraph
    {
        #region NavGraph public fields
        public List<NavNode> Nodes { get; private set; }
        public NavNode PlayerSpawnNode;
        public NavNode EnemyPosition1Node;
        public NavNode EnemyPosition2Node;
        public NavNode EnemyPosition3Node;
        public NavNode EnemyPosition4Node;
        public NavNode BonusPointNode;
        public NavNode WarperUp;
        public NavNode WarperDown;
        public NavNode WarperLeft;
        public NavNode WarperRight;
        public int maxX;
        public int maxY;
        #endregion NavGraph public fields

        #region NavGraph public functions
        public NavGraph()
        {
            Nodes = new List<NavNode>();
        }

        /// <summary>
        /// Adds a node to the graph
        /// </summary>
        /// <param name="node">The node to be added</param>
        public void AddNavNode(NavNode node)
        {
            Nodes.Add(node);

            //set the bounds of the grid
            if (node.Indexes.Item1 > maxX)
                maxX = node.Indexes.Item1;

            if (node.Indexes.Item2 > maxY)
                maxY = node.Indexes.Item2;
        }

        /// <summary>
        /// Search in the graph for the node with the given coordinates on the grid
        /// </summary>
        /// <param name="x">X position on the grid</param>
        /// <param name="y">Y position on the grid</param>
        /// <returns>The node, if found; null otherwise</returns>
        public NavNode GetNode(int x, int y)
        {
            NavNode result = Nodes
                .Where(n => n.Indexes.Item1 == x && n.Indexes.Item2 == y)
                .FirstOrDefault();

            if(result != null)
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Search in the graph for the nodes with the given node type
        /// </summary>
        /// <param name="nodeType">The type required</param>
        /// <returns>A list containing the nodes, may be empty if none is found</returns>
        public List<NavNode> GetNodesOfType(NodeType nodeType)
        {
            var nodes =
                Nodes.Where(n => n.NodeType == nodeType);

            List<NavNode> result = new List<NavNode>();

            foreach(var node in nodes)
            {
                result.Add(node);
            }

            return result;
        }

        /// <summary>
        /// Tries to link each node with it's up, down, left and right neighbors
        /// in the grid
        /// </summary>
        public void LinkNodes()
        {
            foreach(NavNode node in Nodes)
            {
                node.Up =       GetNode(node.Indexes.Item1 + 0, node.Indexes.Item2 + 1);
                node.Down =     GetNode(node.Indexes.Item1 + 0, node.Indexes.Item2 - 1);
                node.Left =     GetNode(node.Indexes.Item1 - 1, node.Indexes.Item2 + 0);
                node.Right =    GetNode(node.Indexes.Item1 + 1, node.Indexes.Item2 + 0);

                node.Neighbors =
                    new List<NavNode>()
                    {
                        node.Up,
                        node.Down,
                        node.Left,
                        node.Right
                    };
            }
        }

        /// <summary>
        /// Uses Dijkstra Shortest Path Algorithm to find the shortest path
        /// between the given nodes, using adjacency matrix representation
        /// and fixing the distance between neighbor nodes as 1
        /// </summary>
        /// <param name="from">The starting node</param>
        /// <param name="to">The target node</param>
        /// /// <param name="exclude">Nodes of the types in the list won't be considered</param>
        /// <returns>A list containing the shortest path between the given nodes</returns>
        public List<NavNode> ShortestPath(NavNode from, NavNode to, List<NodeType> exclude = null)
        {
            List<NavNode> result = new List<NavNode>();

            //mark all nodes as unvisited
            //set the distance to "infinity"
            //and clear the path
            foreach(var node in Nodes)
            {
                node.Visited = false;
                node.Distance = int.MaxValue;
                node.PreviousNodeInPath = null; 
            }

            //set the from node distance to 0
            from.Distance = 0;

            //the current node, set it to "from"
            NavNode currentNode = from;

            //iterate trhough the nodes until the
            //"to" node is visited
            while (!to.Visited)
            {
                //mark current node as visited
                //and remove a node to be visited
                currentNode.Visited = true;

                //calc the distance to the neighbors
                foreach(var neighbor in currentNode.Neighbors)
                {
                    //if the neighbor is already visited, ignore it
                    if (neighbor == null || neighbor.Visited)
                        continue;

                    //ignore neighbor if it matches any exluded type
                    if (exclude != null && exclude.Contains(neighbor.NodeType))
                        continue;

                    //cumulative distance from the starting node
                    int distance = currentNode.Distance + 1;
                    if(distance < neighbor.Distance)
                    {
                        neighbor.Distance = distance;
                        neighbor.PreviousNodeInPath = currentNode;
                    }
                }

                //change the current node to the non-visited
                //node with the shortest distance
                currentNode = Nodes.Where(n => !n.Visited).OrderBy(n => n.Distance).FirstOrDefault();
            }

            //fill the result
            result.Add(to);
            currentNode = to;
            while(currentNode != from)
            {
                result.Add(currentNode.PreviousNodeInPath);
                currentNode = currentNode.PreviousNodeInPath;
            }
            
            result.Reverse();
            return result.ToList();
        }

        /// <summary>
        /// Uses a modification of Dijkstra Shortest Path Algorithm to find 
        /// the farthest path away from the target node, relative to the
        /// "from" node
        /// </summary>
        /// <param name="from">The starting node</param>
        /// <param name="target">The target node to run way from</param>
        /// /// <param name="exclude">Nodes of the types in the list won't be considered</param>
        /// <returns>A list containing the shortest path between the given nodes</returns>
        public List<NavNode> FarthestPath(NavNode from, NavNode target, List<NodeType> exclude = null)
        {
            List<NavNode> result = new List<NavNode>();

            //limits the path size for performance
            const int maxSize = 10;
            int greatestDistance = 0;

            //mark all nodes as unvisited
            //set the distance to "- infinity"
            //and clear the path
            foreach (var node in Nodes)
            {
                node.Visited = false;
                node.Distance = int.MinValue;
                node.PreviousNodeInPath = null;
            }

            //set the from node distance to 0
            from.Distance = 0;

            //the current node, set it to "from"
            NavNode currentNode = from;

            //number of nodes yet to be visited - target
            int nodesToBeVisited = Nodes.Count - 1;

            //iterate trhough the nodes until the
            //"to" node is visited
            while (nodesToBeVisited > 0 || greatestDistance < maxSize)
            {
                //mark current node as visited
                //and remove a node to be visited
                currentNode.Visited = true;

                nodesToBeVisited--;

                //calc the distance to the neighbors
                foreach (var neighbor in currentNode.Neighbors)
                {
                    //if the neighbor is already visited, ignore it
                    if (neighbor == null || neighbor.Visited)
                        continue;

                    //ignore target
                    if (neighbor == target)
                        continue;

                    //ignore neighbor if it matches any exluded type
                    if (exclude != null && exclude.Contains(neighbor.NodeType))
                        continue;

                    //cumulative distance from the starting node
                    int distance = currentNode.Distance + 1;
                    if (distance > neighbor.Distance)
                    {
                        neighbor.Distance = distance;
                        neighbor.PreviousNodeInPath = currentNode;
                        if (distance > greatestDistance)
                            greatestDistance = distance;
                    }
                }

                //change the current node to the non-visited
                //node with the greatest distance
                currentNode = Nodes.Where(n => !n.Visited && n != target).OrderByDescending(n => n.Distance).FirstOrDefault();
            }

            //fill the result starting from the node with greatest distance
            currentNode = Nodes.Where(n => n != target).OrderByDescending(n => n.Distance).FirstOrDefault();
            while (greatestDistance > 0)
            {
                greatestDistance--;
                result.Add(currentNode.PreviousNodeInPath);
                currentNode = currentNode.PreviousNodeInPath;
            }

            result.Reverse();
            return result.ToList();
        }
        #endregion NavGraph public functions
    }
    #endregion navigation graph

    #region entities
    public class NavEntity
    {
        #region NavEntity properties
        public bool ReachedDestination { get; private set; } = true;
        public bool CanMove { get; private set; } = false;
        public bool IsMoving { get; private set; } = false;
        public Tuple<float, float> Position { get; private set; }
        public Tuple<int, int> LastIndexes { get; private set; }
        public NavNode TargetNode { get; private set; }
        #endregion NavEntity properties

        #region NavEntity private fields
        private float _speed = 1f;
        private Tuple<float, float> _lastPosition;
        private List<NavNode> _path;
        #endregion NavEntity private fields

        #region NavEntity set functions
        /// <summary>
        /// Sets the entity's movement speed multiplier
        /// </summary>
        /// <param name="speed"></param>
        public void SetSpeed(float speed)
        {
            _speed = speed;
        }

        /// <summary>
        /// Sets the entity's position on the grid
        /// Also sets it's last position if it had none
        /// </summary>
        /// <param name="x">X index in the grid</param>
        /// <param name="y">Y index in the grid</param>
        public void SetCurrentPosition(int x, int y)
        {
            Position = new Tuple<float, float>(x, y);

            if (_lastPosition == null)
                _lastPosition = new Tuple<float, float>(x, y);

            if (LastIndexes == null)
                LastIndexes = new Tuple<int, int>(x, y);
        }

        /// <summary>
        /// Sets the entiy's current nav path
        /// </summary>
        /// <param name="path">
        /// The path from where the entity will feed
        /// on nav nodes to determine it's movement
        /// </param>
        public void SetPath(List<NavNode> path)
        {
            if (path == null || path.Count == 0)
                return;

            _path = path;
            if (TargetNode == null)
            {
                TargetNode = path[0];
                path.RemoveAt(0);
            }

            ReachedDestination = false;
        }

        #endregion NavEntity set functions

        #region NavEntity controll functions
        /// <summary>
        /// Sets the entity capable of moving
        /// </summary>
        public void EnableMoving()
        {
            CanMove = true;
        }

        /// <summary>
        /// Sets the entity incapable of moving
        /// Also stops it if it was moving
        /// </summary>
        public void DisableMoving()
        {
            CanMove = false;
        }

        /// <summary>
        /// Move the entity along the set path
        /// </summary>
        /// <param name="deltaTime">The time influence</param>
        public void Move(float deltaTime = 1)
        {
            //there is no path to follow
            if (!CanMove || TargetNode == null || _path == null)
            {
                ReachedDestination = true;
                IsMoving = false;
                return;
            }

            ReachedDestination = false;
            IsMoving = true;

            //calc this frame's translation
            float deltaX = (TargetNode.Indexes.Item1 - _lastPosition.Item1) * deltaTime * _speed;
            float deltaY = (TargetNode.Indexes.Item2 - _lastPosition.Item2) * deltaTime * _speed;
            var deltaPos = new Tuple<float, float>(deltaX, deltaY);

            //apply the translation to current position and fix it compared to last position
            var pos = new Tuple<float, float>(Position.Item1 + deltaPos.Item1, Position.Item2 + deltaPos.Item2);
            if (Math.Abs(pos.Item1 - _lastPosition.Item1) > 1 ||
                Math.Abs(pos.Item2 - _lastPosition.Item2) > 1)
            {
                //snap it to the target node
                Position = new Tuple<float, float>(TargetNode.Indexes.Item1, TargetNode.Indexes.Item2);
            }
            else
            {
                Position = pos;
            }

            //check if reached target position
            if (TargetNode.Indexes.Item1 == Position.Item1 &&
                TargetNode.Indexes.Item2 == Position.Item2)
            {
                _lastPosition = new Tuple<float, float>(TargetNode.Indexes.Item1, TargetNode.Indexes.Item2);
                LastIndexes = new Tuple<int, int>(
                        TargetNode.Indexes.Item1,
                        TargetNode.Indexes.Item2);

                //try feed on the path to continue moving
                if (_path.Count > 0)
                {
                    TargetNode = _path[0];
                    _path.RemoveAt(0);
                }
                else
                {
                    TargetNode = null;
                    ReachedDestination = true;
                    IsMoving = false;
                }
            }
        }
        #endregion NavEntity controll functions
    }
    #endregion entities
}

