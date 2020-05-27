using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        #region properties
        public Pacman.NavGraph NavGraph { get; private set; }
        #endregion properties

        #region private fields
        [SerializeField]
        private GameObject _pelletPrefab;

        [SerializeField]
        private GameObject _bigPelletPrefab;

        [SerializeField]
        private Tilemap _tilemap;

        List<Pellet> _pellets;
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
        #endregion public functions

        #region level montage
        /// <summary>
        /// Create the graph and fills it with the tilemap information
        /// Only "walkable" nodes are considered
        /// </summary>
        private void MountLevel()
        {
            _pellets = new List<Pellet>();

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
                        pellet.PelletType = PelletType.Small;
                        pellet.transform.position = _tilemap.GetCellCenterWorld(tilePosition);
                        _pellets.Add(pellet);
                    }
                    else if (tile.name.Equals("bigPelletTile"))
                    {
                        clearColor = true;

                        navNode.NodeType = Pacman.NodeType.PowerUp;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);

                        var bigPellet = Instantiate(_bigPelletPrefab).GetComponent<Pellet>();
                        bigPellet.PelletType = PelletType.Big;
                        bigPellet.transform.position = _tilemap.GetCellCenterWorld(tilePosition);
                        _pellets.Add(bigPellet);
                    }
                    else if (tile.name.Equals("emptyTile"))
                    {
                        clearColor = true;

                        navNode.NodeType = Pacman.NodeType.Normal;
                        navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);
                        NavGraph.AddNavNode(navNode);
                    }
                    else if (tile.name.Equals("playerTile"))
                    {
                        clearColor = true;

                        navNode.NodeType = Pacman.NodeType.PlayerSpawn;
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
        }

        private void SpawnPoints()
        {
            if(_pellets == null)
            {
                _pellets = new List<Pellet>();

                //iterate through all tiles and spawn points over the appropriate tiles
                foreach (var tilePosition in _tilemap.cellBounds.allPositionsWithin)
                {
                    var tile = _tilemap.GetTile(tilePosition);

                    if (tile != null)
                    {
                        if (tile.name.Equals("pelletTile"))
                        {
                            
                        }
                        else if (tile.name.Equals("bigPelletTile"))
                        {
                            
                        }
                    }
                }
            }
            else
            {
                //reactivate the pellets
                foreach(var pellet in _pellets)
                {
                    pellet.gameObject.SetActive(true);
                }
            }
        }
        #endregion level montage
    }
}