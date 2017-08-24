using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour {

    public Transform EffectTile;
    //public Transform PressureTile;

    //public Transform WiredTile;
    //public Transform WiredPressureTile;
    public Transform BoardHolder;

    //public Transform BrokenAndWired;

    // Use this for initialization
    void Start () {
        if (BoardHolder == null)
            BoardHolder = GameObject.Find("BoardHolder").transform;
    }

    void RemoveTile(FloorTile tile)
    {
        tile.Despawn();
        tile.gameObject.SetActive(false);
        Destroy(tile);
    }

    public FloorTile SpawnTile(MapTile tileData, Coord SpawnLocation)
    {
        switch (tileData.m_TileType)
        {
            case FloorTileType.Blank:
                return CreateTile(tileData, SpawnLocation);
            case FloorTileType.Broken:
                return CreateBrokenTile(tileData, SpawnLocation);
            case FloorTileType.Wired:
                return CreateWiredTile(tileData, SpawnLocation);
            case FloorTileType.WiredAndBroken:
                return CreateBrokenAndWired(tileData, SpawnLocation);

            default:
                Debug.Log("Invalid floor tile: " + tileData.m_TileType);
                return null;
        }
    }

    public FloorTile CreateWiredTile(MapTile tileData, Coord location)
    {
        return CreateWiredTile(tileData, location.x, location.y);
    }

    public FloorTile CreateWiredTile(MapTile tileData, int x, int z)
    {

        //Transform floorPrefab = GameObject.Find("Floor").transform;

        //Transform floorPrefab = floorTiles[Random.Range(0, floorTiles.Length - 1)];
        Transform floorTile = Instantiate(EffectTile, new Vector3(x, -.25f, z), Quaternion.identity);

        floorTile.GetComponent<FloorTile>().TileData = tileData;
        floorTile.GetComponent<EffectTile>().TileData.isWired = true;

        return floorTile.GetComponent<FloorTile>();
    }

    public FloorTile CreateBrokenAndWired(MapTile tileData, Coord location)
    {
        return CreateBrokenAndWired(tileData, location.x, location.y);
    }

    public FloorTile CreateBrokenAndWired(MapTile tileData, int x, int z)
    {

        //Transform floorPrefab = GameObject.Find("Floor").transform;

        //Transform floorPrefab = floorTiles[Random.Range(0, floorTiles.Length - 1)];
        Transform floorTile = Instantiate(EffectTile, new Vector3(x, -.25f, z), Quaternion.identity);

        floorTile.GetComponent<FloorTile>().TileData = tileData;
        floorTile.GetComponent<EffectTile>().TileData.isWired = true;
        floorTile.GetComponent<EffectTile>().TileData.isBroken = true;

        return floorTile.GetComponent<FloorTile>();
    }

    public FloorTile CreateBrokenTile(MapTile tileData, Coord location)
    {
        return CreateBrokenTile(tileData, location.x, location.y);
    }

    public FloorTile CreateBrokenTile(MapTile tileData, int x, int z)
    {

        //Transform floorPrefab = GameObject.Find("Floor").transform;

        //Transform floorPrefab = floorTiles[Random.Range(0, floorTiles.Length - 1)];
        Transform floorTile = Instantiate(EffectTile, new Vector3(x, -.25f, z), Quaternion.identity);

        floorTile.GetComponent<FloorTile>().TileData = tileData;
        floorTile.GetComponent<EffectTile>().TileData.isBroken = true;

        return floorTile.GetComponent<FloorTile>();
    }

    private FloorTile CreateTile(MapTile tileData,Coord location)
    {
        return CreateTile(tileData,location.x, location.y);
    }

    private FloorTile CreateTile(MapTile TileData,int x, int z)
    {
        //if (PressureTile == null)
        //{
        //    PressureTile = GameObject.Find("PressureStation").transform;
        //    if (PressureTile == null)
        //    {
        //        Debug.Log("Failed loading Pressure Tile");
        //        return null;
        //    }
        //}

        //Transform newTile = Instantiate(PressureTile, new Vector3(x, 0.2f, z), Quaternion.identity);
        //NoSpawnTilesList.Add(newTile);

        //return newTile.GetComponent<EffectTile>();

        Transform floorTile = Instantiate(EffectTile, new Vector3(x, -.25f, z), Quaternion.identity);

        floorTile.GetComponent<FloorTile>().TileData = TileData;

        return floorTile.GetComponent<FloorTile>();
    }

    //public FloorTile CreateFloor(Coord location)
    //{
    //    return CreateFloor(location.x, location.y);
    //}

    //public FloorTile CreateFloor(int x, int z)
    //{

    //    // Transform floorPrefab = GameObject.Find("Floor").transform;

    //    //Transform floorPrefab = floorTiles[Random.Range(0, floorTiles.Length - 1)];
    //    Transform floorTile = Instantiate(BlankTile, new Vector3(x, -.25f, z), Quaternion.identity);

    //    return floorTile.GetComponent<FloorTile>();
    //}

    //public FloorTile CreateWiredFloor(Coord location)
    //{
    //    return CreateWiredFloor(location.x, location.y);
    //}

    //public FloorTile CreateWiredFloor(int x, int z)
    //{

    //    //Transform floorPrefab = GameObject.Find("Floor").transform;

    //    //Transform floorPrefab = floorTiles[Random.Range(0, floorTiles.Length - 1)];
    //    Transform floorTile = Instantiate(WiredTile, new Vector3(x, -.25f, z), Quaternion.identity);

    //    return floorTile.GetComponent<FloorTile>();
    //}






    //public FloorTile CreateBrokenWiredFloor(Coord location)
    //{
    //    return CreateBrokenWiredFloor(location.x, location.y);
    //}

    //public FloorTile CreateBrokenWiredFloor(int x, int z)
    //{

    //    //Transform floorPrefab = GameObject.Find("Floor").transform;

    //    //Transform floorPrefab = floorTiles[Random.Range(0, floorTiles.Length - 1)];
    //    Transform floorTile = Instantiate(WiredTile, new Vector3(x, -.25f, z), Quaternion.identity);

    //    return floorTile.GetComponent<FloorTile>();
    //}




    //public FloorTile CreateBrokenPressureStationTile(Coord location)
    //{
    //    return CreateBrokenPressureStationTile(location.x, location.y);
    //}

    //public FloorTile CreateBrokenPressureStationTile(int x, int z)
    //{
    //    //if (BrokenPressureStationTile == null)
    //    //{
    //    //    BrokenPressureStationTile = GameObject.Find("BrokenPressureStation").transform;
    //    //    if (BrokenPressureStationTile == null)
    //    //    {
    //    //        Debug.Log("Failed loading broken pressure stations 2");
    //    //        return null;
    //    //    }
    //    //}
    //    //Transform crateObject = Instantiate(BrokenPressureStationTile, new Vector3(x, 0.2f, z), Quaternion.identity);
    //    //NoSpawnTilesList.Add(crateObject);

    //    //return crateObject.GetComponent<EffectTile>();

    //    Transform floorTile = Instantiate(PressureTile, new Vector3(x, -.25f, z), Quaternion.identity);

    //    EffectTile floorTileEfect = floorTile.GetComponent<EffectTile>();
    //    floorTileEfect.isBroken = true;

    //    return floorTileEfect;


    //}

    //public FloorTile CreateElectricTrapTile(Coord location)
    //{
    //    return CreateElectricTrapTile(location.x, location.y);
    //}

    //public FloorTile CreateElectricTrapTile(int x, int z)
    //{
    //    //if (ElectricTrapTile == null)
    //    //{
    //    //    ElectricTrapTile = GameObject.Find("ElectricTrap").transform;
    //    //    if (ElectricTrapTile == null)
    //    //    {
    //    //        Debug.Log("Failed loading electric traps");
    //    //        return null;
    //    //    }
    //    //}
    //    //Transform crateObject = Instantiate(ElectricTrapTile, new Vector3(x, 0.2f, z), Quaternion.identity);
    //    //NoSpawnTilesList.Add(crateObject);

    //    //return crateObject.GetComponent<EffectTile>();

    //    Transform floorTile = Instantiate(WiredTile, new Vector3(x, -.25f, z), Quaternion.identity);

    //    EffectTile floorTileEfect = floorTile.GetComponent<EffectTile>();
    //    floorTileEfect.isBroken = true;

    //    return floorTileEfect;


    //}

}
