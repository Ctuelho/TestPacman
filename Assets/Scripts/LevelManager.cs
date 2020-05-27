using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    #region properties
    public Pacman.NavGraph NavGraph { get; private set; }
    #endregion properties

    #region private fields
    [SerializeField]
    private GameObject _pointPrefab;

    [SerializeField]
    private Tilemap _tilemap;
    #endregion private fields

    #region unity event functions
    // Start is called before the first frame update
    public void Initialize()
    {
        NavGraph = new Pacman.NavGraph();

        FillGraph();

        NavGraph.LinkNodes();

        //SpawnPoints();

    }

    public void DrawPath(List<Pacman.NavNode> path)
    {
        StartCoroutine(CreatePointsOfPath(path));
    }

    IEnumerator CreatePointsOfPath(List<Pacman.NavNode> path)
    {
        foreach (var node in path)
        {
            var point = Instantiate(_pointPrefab);
            point.transform.position = new Vector3(
                0.5f + node.Indexes.Item1 - NavGraph.maxX / 2,
                0.5f + node.Indexes.Item2 - NavGraph.maxY / 2,
                0);
            yield return new WaitForSeconds(0.05f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion unity event functions

    #region level montage
    private void FillGraph()
    {
        //iterates through all tiles and create nodes in the graph
        //based on the tile's poisition in the grid
        foreach (var tilePosition in _tilemap.cellBounds.allPositionsWithin)
        {
            var tile = _tilemap.GetTile(tilePosition);

            if (tile != null)
            {
                //only filler and void tiles are considered navigation area
                if (tile.name.Equals("fillerTile") || tile.name.Equals("voidTile"))
                {
                    Pacman.NavNode navNode = new Pacman.NavNode();
                    navNode.Indexes = new System.Tuple<int, int>(tilePosition.x, tilePosition.y);

                    NavGraph.AddNavNode(navNode);
                }
            }
        }
    }

    private void SpawnPoints()
    {
        //iterate through all tiles and spawn points over the appropriate tiles
        foreach (var tilePosition in _tilemap.cellBounds.allPositionsWithin)
        {
            var tile = _tilemap.GetTile(tilePosition);

            if (tile != null)
            {
                if (tile.name.Equals("fillerTile"))
                {
                    var point = Instantiate(_pointPrefab);
                    point.transform.position = _tilemap.GetCellCenterWorld(tilePosition);
                }
            }
        }
    }
    #endregion level montage
}
