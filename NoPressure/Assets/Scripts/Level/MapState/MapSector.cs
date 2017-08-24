using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapSector
{
    const int SECTOR_SIZE = 10;

    const FloorTileType defaultTile = FloorTileType.Blank;
    const SpawnType defaultSpawn = SpawnType.None;

    //FloorTileType[,] mFloorTiles = new FloorTileType[SECTOR_SIZE, SECTOR_SIZE];
    //SpawnType[,] mObsticleLayer = new SpawnType[SECTOR_SIZE, SECTOR_SIZE];

    MapTile[,] Tiles = new MapTile[SECTOR_SIZE, SECTOR_SIZE];

    MapNode mParentNode;
    public Coord mSectorLocation { get; private set; }

    public bool pIsLoaded { get; set; }

    MapSector NorthSector;
    MapSector SouthSector;
    MapSector EastSector;
    MapSector WestSector;

    public void SetTileConnections()
    {
        MapTile NorthTile = null;
        MapTile SouthTile = null;
        MapTile EastTile = null;
        MapTile WestTile = null;

        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
            {
                if (j == 9)
                    if (NorthSector != null)
                        NorthTile = NorthSector.getTileAt(new Coord(i, 0));
                    else
                        NorthTile = null;
                else
                    NorthTile = Tiles[i, j + 1];

                if (j == 0)
                    if (SouthSector != null)
                        SouthTile = SouthSector.getTileAt(new Coord(i, 9));
                    else
                        SouthTile = null;
                else
                    SouthTile = Tiles[i, j - 1];

                if (i == 0)
                    if (WestSector != null)
                        WestTile = WestSector.getTileAt(new Coord(9, j));
                    else
                        WestTile = null;
                else
                    WestTile = Tiles[i - 1, j];

                if (i == 9)
                    if (EastSector != null)
                        EastTile = EastSector.getTileAt(new Coord(0, j));
                    else
                        EastTile = null;
                else
                    EastTile = Tiles[i + 1, j];

                Tiles[i, j].SetupConnections(NorthTile, SouthTile, WestTile, EastTile);

            }
    }

    public MapSector(
        MapNode lParentNode, 
        Coord lSectorLocation, 
        SpawnType[,] lObsticleLayer, 
        FloorTileType[,] lFloorTiles,
        MapSector lNorthSector,
        MapSector lSouthSector,
        MapSector lWestSector,
        MapSector lEastSector)
    {
        // Confirm sector is SECTOR_SIZE x SECTOR_SIZE
        if (lFloorTiles.GetLength(0) != SECTOR_SIZE)
            Debug.LogError("Creating MapSector with improper length: "
                + lFloorTiles.GetLength(0) + "," + lFloorTiles.GetLength(1));

        if (lFloorTiles.GetLength(1) != SECTOR_SIZE)
            Debug.LogError("Creating MapSector with improper length: "
                + lFloorTiles.GetLength(0) + "," + lFloorTiles.GetLength(1));

        if (lObsticleLayer.GetLength(0) != SECTOR_SIZE)
            Debug.LogError("Creating MapSector with improper length: "
                + lObsticleLayer.GetLength(0) + "," + lObsticleLayer.GetLength(1));

        if (lObsticleLayer.GetLength(1) != SECTOR_SIZE)
            Debug.LogError("Creating MapSector with improper length: "
                + lObsticleLayer.GetLength(0) + "," + lObsticleLayer.GetLength(1));

        mSectorLocation = lSectorLocation;

        int baseX = mSectorLocation.x * 10;
        int baseY = mSectorLocation.y * 10;

        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
            {
                Tiles[i, 9 - j] = new MapTile(new Coord(baseX + i, baseY + 9 - j), lFloorTiles[i, j], lObsticleLayer[i, j]);
            }


        NorthSector = lNorthSector;
        SouthSector = lSouthSector;
        WestSector = lWestSector;
        EastSector = lEastSector;

        if (NorthSector != null)
            if (NorthSector.SouthSector == null)
            {
                NorthSector.SouthSector = this;
                NorthSector.SetTileConnections();
            }

        if (SouthSector != null)
            if (SouthSector.NorthSector == null)
            {
                SouthSector.NorthSector = this;
                SouthSector.SetTileConnections();
            }

        if (EastSector != null)
            if (EastSector.WestSector == null)
            {
                EastSector.WestSector = this;
                EastSector.SetTileConnections();
            }
        if (WestSector != null)
            if (WestSector.EastSector == null)
            {
                WestSector.EastSector = this;
                WestSector.SetTileConnections();
            }

        SetTileConnections();
        mParentNode = lParentNode;
    }

    public MapTile getTileAt(Coord location)
    {
        return Tiles[location.x, location.y];
    }

    public void RemoveTileAt(Coord location)
    {
        location = new Coord(location.x % 10, location.y % 10);
        //mFloorTiles[location.x, location.y] = defaultTile;
        Tiles[location.x, location.y].m_TileType = defaultTile;
    }

    public void SetNearby(Coord location, SpawnType toSpawn)
    {
        if (location.x < 9)
        {
            if (Tiles[location.x + 1, location.y].m_SpawnType == defaultSpawn)
            {
                Tiles[location.x + 1, location.y].m_SpawnType = toSpawn;
                return;
            }

            if ((9 - location.y) < 9)
            {
                if (Tiles[location.x + 1, location.y + 1].m_SpawnType == defaultSpawn)
                {
                    Tiles[location.x + 1, location.y + 1].m_SpawnType = toSpawn;
                    return;
                }
            }

            if ((9 - location.y) > 0)
            {
                if (Tiles[location.x + 1, location.y - 1].m_SpawnType == defaultSpawn)
                {
                    Tiles[location.x + 1, location.y - 1].m_SpawnType = toSpawn;
                    return;
                }
            }

        }

        if (location.x > 0)
        {
            if (Tiles[location.x - 1, location.y].m_SpawnType == defaultSpawn)
            {
                Tiles[location.x - 1, location.y].m_SpawnType = toSpawn;
                return;
            }

            if ((9 - location.y) < 9)
            {
                if (Tiles[location.x - 1, location.y + 1].m_SpawnType == defaultSpawn)
                {
                    Tiles[location.x - 1, location.y + 1].m_SpawnType = toSpawn;
                    return;
                }
            }

            if ((9 - location.y) > 0)
            {
                if (Tiles[location.x - 1, location.y - 1].m_SpawnType == defaultSpawn)
                {
                    Tiles[location.x - 1, location.y - 1].m_SpawnType = toSpawn;
                    return;
                }
            }
        }

        if ((9 - location.y) < 9)
        {
            if (Tiles[location.x, location.y + 1].m_SpawnType == defaultSpawn)
            {
                Tiles[location.x, location.y + 1].m_SpawnType = toSpawn;
                return;
            }
        }

        if ((9 - location.y) > 0)
        {
            if (Tiles[location.x, location.y - 1].m_SpawnType == defaultSpawn)
            {
                Tiles[location.x, location.y - 1].m_SpawnType = toSpawn;
                return;
            }
        }
    }

    public MapTile getMapTileAt(Coord location)
    {
        location = new Coord(location.x % 10, location.y % 10);
        return Tiles[location.x, location.y];
    }

    public void SetObjectAt(Coord location, SpawnType toSpawn)
    {
        location = new Coord(location.x % 10, location.y % 10);

        if (Tiles[location.x, location.y].m_SpawnType == defaultSpawn)
        {
            Tiles[location.x, location.y].m_SpawnType = toSpawn;
            return;
        }
        else
            SetNearby(location, toSpawn);
    }

    public void SetTileAt(Coord location, FloorTileType toSpawn)
    {
        location = new Coord(location.x % 10, location.y % 10);
        Tiles[location.x, location.y].m_TileType = toSpawn;
    }

    public void RemoveObjectAt(Coord location)
    {
        location = new Coord(location.x % 10, location.y % 10);
        Tiles[location.x, location.y].m_SpawnType = defaultSpawn;
    }

    public FloorTileType GetFloorTileAt(int x, int y)
    {
        x = x % 10;
        y = y % 10;
        return Tiles[x, y].m_TileType;
    }

    public SpawnType GetSpawnAt(int x, int y)
    {
        x = x % 10;
        y = y % 10;
        return Tiles[x, y].m_SpawnType;
    }

    public MapSector(MapNode lParentNode)
    {
        for (int i = 0; i < SECTOR_SIZE; i++)
        {
            for (int j = 0; j < SECTOR_SIZE; j++)
            {
                Tiles[i, j].m_TileType = defaultTile;
                Tiles[i, j].m_SpawnType = defaultSpawn;
            }
        }
        mParentNode = lParentNode;
    }
}