using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapSector
{
    const int SECTOR_SIZE = 10;

    const FloorTiles defaultTile = FloorTiles.Blank;
    const SpawnType defaultSpawn = SpawnType.None;

    FloorTiles[,] mFloorTiles = new FloorTiles[SECTOR_SIZE, SECTOR_SIZE];
    SpawnType[,] mObsticleLayer = new SpawnType[SECTOR_SIZE, SECTOR_SIZE];

    MapNode mParentNode;
    public Coord mSectorLocation { get; private set; }

    public bool pIsLoaded { get; set; }

    public void RemoveTileAt(Coord location)
    {
        location = new Coord(location.x % 10, location.y % 10);
        mFloorTiles[location.x, 9 - location.y] = defaultTile;
    }

    public void SetNearby(Coord location, SpawnType toSpawn)
    {
        if (location.x < 9)
        {
            if (mObsticleLayer[location.x + 1, 9 - location.y] == defaultSpawn)
            {
                mObsticleLayer[location.x + 1, 9 - location.y] = toSpawn;
                return;
            }

            if ((9 - location.y) < 9)
            {
                if (mObsticleLayer[location.x + 1, (9 - location.y) + 1] == defaultSpawn)
                {
                    mObsticleLayer[location.x + 1, (9 - location.y) + 1] = toSpawn;
                    return;
                }
            }

            if ((9 - location.y) > 0)
            {
                if (mObsticleLayer[location.x + 1, (9 - location.y) - 1] == defaultSpawn)
                {
                    mObsticleLayer[location.x + 1, (9 - location.y) - 1] = toSpawn;
                    return;
                }
            }

        }

        if (location.x > 0)
        {
            if (mObsticleLayer[location.x - 1, 9 - location.y] == defaultSpawn)
            {
                mObsticleLayer[location.x - 1, 9 - location.y] = toSpawn;
                return;
            }

            if ((9 - location.y) < 9)
            {
                if (mObsticleLayer[location.x - 1, (9 - location.y) + 1] == defaultSpawn)
                {
                    mObsticleLayer[location.x - 1, (9 - location.y) + 1] = toSpawn;
                    return;
                }
            }

            if ((9 - location.y) > 0)
            {
                if (mObsticleLayer[location.x - 1, (9 - location.y) - 1] == defaultSpawn)
                {
                    mObsticleLayer[location.x - 1, (9 - location.y) - 1] = toSpawn;
                    return;
                }
            }
        }

        if ((9 - location.y) < 9)
        {
            if (mObsticleLayer[location.x, (9 - location.y) + 1] == defaultSpawn)
            {
                mObsticleLayer[location.x, (9 - location.y) + 1] = toSpawn;
                return;
            }
        }

        if ((9 - location.y) > 0)
        {
            if (mObsticleLayer[location.x, (9 - location.y) - 1] == defaultSpawn)
            {
                mObsticleLayer[location.x, (9 - location.y) - 1] = toSpawn;
                return;
            }
        }
    }

    public void SetObjectAt(Coord location, SpawnType toSpawn)
    {
        location = new Coord(location.x % 10, location.y % 10);

        if (mObsticleLayer[location.x, 9 - location.y] == defaultSpawn)
        {
            mObsticleLayer[location.x, 9 - location.y] = toSpawn;
            return;
        }
        else
            SetNearby(location, toSpawn);

    }

    public void SetTileAt(Coord location, FloorTiles toSpawn)
    {

        location = new Coord(location.x % 10, location.y % 10);
        mFloorTiles[location.x, 9 - location.y] = toSpawn;
    }

    public void RemoveObjectAt(Coord location)
    {
        Debug.Log(mObsticleLayer[location.x, 9 - location.y] + " -> " + defaultSpawn);
        location = new Coord(location.x % 10, location.y % 10);
        mObsticleLayer[location.x, 9 - location.y] = defaultSpawn;
    }

    public FloorTiles GetFloorTileAt(int x, int y)
    {
        x = x % 10;
        y = y % 10;
        return mFloorTiles[x, 9 - y];
    }

    public SpawnType GetSpawnAt(int x, int y)
    {
        x = x % 10;
        y = y % 10;
        return mObsticleLayer[x, 9 - y];
    }

    public MapSector(MapNode lParentNode, Coord lSectorLocation, SpawnType[,] lObsticleLayer, FloorTiles[,] lFloorTiles)
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

        mFloorTiles = lFloorTiles;
        mObsticleLayer = lObsticleLayer;

        mParentNode = lParentNode;
    }

    public MapSector(MapNode lParentNode)
    {
        for (int i = 0; i < SECTOR_SIZE; i++)
        {
            for (int j = 0; j < SECTOR_SIZE; j++)
            {
                mFloorTiles[i, j] = defaultTile;
                mObsticleLayer[i, j] = defaultSpawn;
            }
        }
        mParentNode = lParentNode;
    }
}