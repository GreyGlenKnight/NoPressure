using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorManager {

    public MapSector SectorData;
    public FloorTile[,] Tiles = new FloorTile[10,10];
    public List<FloorTile> wiredTiles = new List<FloorTile>();


    public void RemovePowerNetworks()
    {
        for (int i = 0; i < wiredTiles.Count; i++)
            wiredTiles[i].RemoveNetwork();
    }


    public void AssignPowerNetworks()
    {
        for (int i = 0; i < wiredTiles.Count; i++)
        {
            wiredTiles[i].AssignNetwork();
        }
    }

    public void FindWires()
    {
        wiredTiles = new List<FloorTile>();
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
            {
                if (Tiles[i, j].TileData.isWired == true)
                {
                    wiredTiles.Add(Tiles[i, j]);
                }
            }
    }

    public void ResetPressure()
    {
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
            {
                Tiles[i, j].ResetPressure();
            }
    }

    public void SpreadPressure()
    {
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
            {
                Tiles[i, j].SpreadPressure();
            }
    }

    public SectorManager(FloorTile[,] lTiles, MapSector lSector)
    {
        Tiles = lTiles;
        SectorData = lSector;
    }

    public void DespawnSectorTiles(MapSector SpawnSector)
    {
        Coord SectorLocation = SpawnSector.mSectorLocation;

        GameObject SectorContainer = GameObject.Find("SectorTiles" + SectorLocation.ToString("_"));

        List<PersistentEntity> childList = new List<PersistentEntity>();

        if (SectorContainer == null)
            return;

        for (int i = 0; i < SectorContainer.transform.childCount; i++)
        {
            childList.Add(SectorContainer.transform.GetChild(i).GetComponent<PersistentEntity>());
        }

        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].GetComponent<FloorTile>().Die();
        }
    }

    //public void SpawnTilesForSector(MapSector spawnSector)
    //{
    //    Coord sectorLocation = spawnSector.mSectorLocation;
    //    GameObject SectorContainer = GameObject.Find("SectorTiles" + sectorLocation.ToString("_"));

    //    if (SectorContainer == null)
    //    {
    //        if (BoardHolder == null)
    //            BoardHolder = GameObject.Find("BoardHolder").transform;

    //        // Create Container
    //        SectorContainer = Instantiate(BoardHolder.gameObject);
    //        SectorContainer.name = "SectorTiles" + sectorLocation.ToString("_");

    //        SectorContainer.transform.parent = GameObject.Find("Sector" + sectorLocation.ToString("_")).transform;
    //    }

    //    for (int i = 0; i < 10; i++)
    //    {
    //        for (int j = 0; j < 10; j++)
    //        {

    //        }
    //    }
    //}


}
