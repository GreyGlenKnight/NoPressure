using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour {

    public Transform EffectTile;
    public Transform BoardHolder;
    public PrefabPooler prefabPooler;

    public Transform spaceTile;

    // Use this for initialization
    void Start () {
        if (BoardHolder == null)
            BoardHolder = GameObject.Find("BoardHolder").transform;

        prefabPooler = PrefabPooler.instance;
    }

    void RemoveTile(FloorTile tile)
    {
        tile.Despawn();
        tile.gameObject.SetActive(false);
        //Destroy(tile);
    }

    public FloorTile SpawnTile(MapTile tileData, Coord SpawnLocation)
    {
        switch (tileData.m_TileType)
        {
            case FloorTileType.Blank:
                return prefabPooler.SpawnTile(tileData, SpawnLocation);
            case FloorTileType.Broken:
                return prefabPooler.CreateBrokenTile(tileData, SpawnLocation);
            case FloorTileType.Wired:
                return prefabPooler.CreateWiredTile(tileData, SpawnLocation);
            case FloorTileType.WiredAndBroken:
                return prefabPooler.CreateBrokenAndWired(tileData, SpawnLocation);
            case FloorTileType.Space: 
                return prefabPooler.CreateDeadSpaceTile(tileData, SpawnLocation);
            //return prefabPooler.CreateDeadSpaceTile(tileData, SpawnLocation);
            default:
                Debug.Log("Invalid floor tile: " + tileData.m_TileType);
                return null;
        }
    }

}
