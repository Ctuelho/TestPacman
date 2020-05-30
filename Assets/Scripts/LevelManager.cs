using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        #region properties
        public Pacman.NavGraph NavGraph { get; private set; }
        public List<Collectable> Collectables { get; private set; }
        #endregion properties

        #region private fields
        [SerializeField]
        private GameObject _pelletPrefab;
        [SerializeField]
        private GameObject _bigPelletPrefab;
        [SerializeField]
        private GameObject _bonusPelletPrefab;
        [SerializeField]
        private Tilemap _tilemap;
        #endregion private fields

        #region unity event functions
        #endregion unity event functions

        #region public functions
        public void Initialize()
        {
            NavGraph = new Pacman.NavGraph();

            MountLevel();

            NavGraph.LinkNodes();
        }

        public Collectable GetCollectable(int indexX, int indexY)
        {
            return Collectables.Where(c => c.Index.x == indexX && c.Index.y == indexY).FirstOrDefault();
        }
        #endregion public functions

        #region level montage
        /// <summary>
        /// Create the graph and fills it with the tilemap information
        /// Only "walkable" nodes are considered
        /// </summary>
        private void MountLevel()
        {
            Collectables = new List<Collectable>();

            //iterates through all tiles and create nodes in the graph
            //and elements int he level based on the tile's poisition 
            //in the grid and type. Walls are not considered for the graph
            foreach (var tilePosition in _tilemap.cellBounds.allPositionsWithin)
            {
                var tile = _tilemap.GetTile(tilePosition);

                if (tile != null)
                {
                    bool clearColor = false;
                    Pacman.NavNode navNode = new Pacman.NavNode();
                    if (tile.name.Equals("pelletTile"))
                    {
                        clearColor = true;

                        navNode.NodeType = Pacman.NodeType.Normal;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);

                        var pellet = Instantiate(_pelletPrefab).GetComponent<Pellet>();
                        pellet.PelletType = PelletTypes.Small;
                        pellet.transform.position = _tilemap.GetCellCenterWorld(tilePosition);
                        pellet.Index = new Vector2(tilePosition.x, tilePosition.y);
                        Collectables.Add(pellet);
                    }
                    else if (tile.name.Equals("bigPelletTile"))
                    {
                        clearColor = true;

                        navNode.NodeType = Pacman.NodeType.PowerUp;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);

                        var bigPellet = Instantiate(_bigPelletPrefab).GetComponent<Pellet>();
                        bigPellet.PelletType = PelletTypes.Big;
                        bigPellet.transform.position = _tilemap.GetCellCenterWorld(tilePosition);
                        bigPellet.Index = new Vector2(tilePosition.x, tilePosition.y);
                        Collectables.Add(bigPellet);
                    }
                    else if (tile.name.Equals("emptyTile"))
                    {
                        clearColor = true;

                        navNode.NodeType = Pacman.NodeType.Normal;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);
                    }
                    else if (tile.name.Equals("bonusPointTile"))
                    {
                        clearColor = true;

                        navNode.NodeType = Pacman.NodeType.BonusPoint;
                        NavGraph.BonusPointNode = navNode;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);

                        var bonusPellet = Instantiate(_bonusPelletPrefab).GetComponent<Pellet>();
                        bonusPellet.PelletType = PelletTypes.Bonus;
                        bonusPellet.transform.position = _tilemap.GetCellCenterWorld(tilePosition);
                        bonusPellet.Index = new Vector2(tilePosition.x, tilePosition.y);
                        Collectables.Add(bonusPellet);
                    }
                    else if (tile.name.Equals("playerTile"))
                    {
                        clearColor = true;

                        navNode.NodeType = Pacman.NodeType.PlayerSpawn;
                        NavGraph.PlayerSpawnNode = navNode;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);
                    }
                    else if (tile.name.Equals("enemy1"))
                    {
                        clearColor = true;

                        navNode.NodeType = Pacman.NodeType.EnemyPosition1;
                        NavGraph.EnemyPosition1Node = navNode;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);
                    }
                    else if (tile.name.Equals("enemy2"))
                    {
                        clearColor = true;

                        navNode.NodeType = Pacman.NodeType.EnemyPosition2;
                        NavGraph.EnemyPosition2Node = navNode;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);
                    }
                    else if (tile.name.Equals("enemy3"))
                    {
                        clearColor = true;

                        navNode.NodeType = Pacman.NodeType.EnemyPosition3;
                        NavGraph.EnemyPosition3Node = navNode;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);
                    }
                    else if (tile.name.Equals("enemy4"))
                    {
                        clearColor = true;

                        navNode.NodeType = Pacman.NodeType.EnemyPosition4;
                        NavGraph.EnemyPosition4Node = navNode;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);
                    }
                    else if (tile.name.Equals("warperDown"))
                    {
                        //clearColor = true;

                        navNode.NodeType = Pacman.NodeType.WarperDown;
                        NavGraph.WarperDown = navNode;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);
                    }
                    else if (tile.name.Equals("warperUp"))
                    {
                        //clearColor = true;

                        navNode.NodeType = Pacman.NodeType.WarperUp;
                        NavGraph.WarperUp = navNode;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);
                    }
                    else if (tile.name.Equals("warperLeft"))
                    {
                        //clearColor = true;

                        navNode.NodeType = Pacman.NodeType.WarperLeft;
                        NavGraph.WarperLeft = navNode;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);
                    }
                    else if (tile.name.Equals("warperRight"))
                    {
                        //clearColor = true;

                        navNode.NodeType = Pacman.NodeType.WarperRight;
                        NavGraph.WarperRight = navNode;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);
                    }
                    else if (tile.name.Equals("nonPlayableTile"))
                    {
                        clearColor = true;

                        navNode.NodeType = Pacman.NodeType.NonWalkable;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);
                    }
                    else if (tile.name.Equals("EnemyExitTile"))
                    {
                        clearColor = true;

                        navNode.NodeType = Pacman.NodeType.Normal;
                        NavGraph.EnemyExitTargetNode = navNode;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);
                    }
                    
                    if (clearColor)
                    {
                        _tilemap.SetTileFlags(tilePosition, TileFlags.None);
                        _tilemap.SetColor(tilePosition, new Color(0, 0, 0, 0));
                    }
                }
            }


            //link warp nodes
            NavGraph.WarperLeft.Left = NavGraph.WarperRight;
            NavGraph.WarperRight.Right = NavGraph.WarperLeft;
            NavGraph.WarperUp.Up = NavGraph.WarperDown;
            NavGraph.WarperDown.Down = NavGraph.WarperUp;
        }
        #endregion level montage
    }
}