using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelAssembler : MonoBehaviour
{
    [SerializeField]
    private GameObject _pointPrefab;

    [SerializeField]
    private Tilemap _tilemap;

    #region unity event functions
    // Start is called before the first frame update
    void Start()
    {
        //iterate through all tiles and spawn points over the appropriate ones
        foreach(var tilePosition in _tilemap.cellBounds.allPositionsWithin)
        {
            var tile = _tilemap.GetTile(tilePosition);

            if(tile != null)
            {
                if(tile.name.Equals("fillerTile"))
                {
                    var point = Instantiate(_pointPrefab);
                    point.transform.position = _tilemap.GetCellCenterWorld(tilePosition);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion unity event functions
}
