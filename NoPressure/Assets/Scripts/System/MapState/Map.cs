using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Map
{

    private MapNode[,] sMapNodes = new MapNode[10, 10];

    public Map()
    {

    }

    public void SaveLocationOnMap(Transform entity)
    {
        if (entity == null)
        {
            Debug.LogError("calling SaveLocationOnMap on a null transform");
        }

        PersistentEntity persistentEntity = entity.GetComponent<PersistentEntity>();
        if (persistentEntity == null)
        {
            Debug.LogError("calling SaveLocationOnMap on a non-persistant entity");
        }
        SpawnType spawnType = persistentEntity.spawnType;
        Coord location = new Coord(
            (int)Math.Round(entity.position.x, 0),
            (int)Math.Round(entity.position.z, 0));

        SetObjectAtTile(spawnType, location);
    }

    public void SetAllSectorsToNotLoaded()
    {
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
            {
                if (sMapNodes[i, j] != null)
                {
                    sMapNodes[i, j].SetAllSectorsUnloaded();
                    sMapNodes[i, j] = null;
                }
            }
    }

    public void SetObjectAtTile(SpawnType itemToSet, Coord location)
    {
        // find the sector
        MapSector Sector = GetSectorAt(WorldSpaceUnit.Tile, location, "Demo");

        Sector.SetObjectAt(location, itemToSet);
    }


    public Coord ConvertToSectorSpace(WorldSpaceUnit unit, Coord location)
    {
        switch (unit)
        {
            case WorldSpaceUnit.Tile:
                return new Coord((int)Math.Floor(location.x / 10f), (int)Math.Floor(location.y / 10f));

            case WorldSpaceUnit.Sector:
                return location;

            // return the bottom left sector
            case WorldSpaceUnit.MpaNode:
                return new Coord(location.x * 4, location.y * 4);

            default:
                Debug.LogError("Unknown Unit: " + unit);
                return new Coord(-1, -1);
        }
    }

    public Coord ConvertToMapNode(WorldSpaceUnit unit, Coord location)
    {
        switch (unit)
        {
            case WorldSpaceUnit.Tile:
                return new Coord((int)Math.Floor(location.x / 40f), (int)Math.Floor(location.y / 40f));

            case WorldSpaceUnit.Sector:
                return new Coord((int)Math.Floor(location.x / 4f), (int)Math.Floor(location.y / 4f));

            // return the bottom left sector
            case WorldSpaceUnit.MpaNode:
                return location;

            default:
                Debug.LogError("Unknown Unit: " + unit);
                return new Coord(-1, -1);
        }
    }

    public MapSector GetSectorAt(WorldSpaceUnit unit, Coord location, string levelName)
    {
        Coord SectorLocation = ConvertToSectorSpace(unit, location);

        Coord MapNodLocation = ConvertToMapNode(WorldSpaceUnit.Sector, SectorLocation);
        MapNode mapNode = GetMapNodeAt(MapNodLocation, levelName);
        MapSector mapSector = null;
        if (mapNode != null)
        {
            Coord SectorInNode = GetMapNodeSectorFromWorldSpaceSector(SectorLocation);
            mapSector = mapNode.getSectorAt(SectorInNode);
        }

        return mapSector;
    }

    public Coord GetMapNodeSectorFromWorldSpaceSector(Coord location)
    {
        return new Coord(location.x % 4, location.y % 4);
    }


    //public void SetMapNode(MapNode toSet, Coord location )
    //{
    //    if (sMapNodes[location.x,location.y] != null)
    //    {
    //        Debug.LogError("Trying to set a map node twice!");
    //    }

    //    sMapNodes[location.x, location.y] = toSet;
    //}

    // Load a file with lMapnameXY.csv and lMapnameXYF.csv if we have not loaded it and 
    // store the info if we need it later
    public MapNode GetMapNodeAt(Coord ILocation, string lMapName)
    {

        //Debug.Log("Loading Map Node: " + lMapName + ILocation);

        if (CheckBounds(ILocation) == false)
        {
            return null;
        }

        if (sMapNodes[ILocation.x, ILocation.y] != null)
            return sMapNodes[ILocation.x, ILocation.y];

        if (sMapNodes[ILocation.x, ILocation.y] == null)
        {
            MapNode currentNode = new MapNode(ILocation, lMapName);

            if (currentNode == null)
                Debug.LogError("Could not load node " + lMapName + " " + ILocation);

            sMapNodes[ILocation.x, ILocation.y] = currentNode;
        }

        return sMapNodes[ILocation.x, ILocation.y];
    }



    private bool CheckBounds(Coord location)
    {
        if (location.x < 0)
        {
            //Debug.Log("Can not load negative value maps :" + location);
            return false;
        }
        if (location.x >= 10)
        {
            //Debug.Log("map value out of bounds:" + location);
            return false;
        }
        if (location.y < 0)
        {
            //Debug.Log("Can not load negative value maps:" + location);
            return false;
        }
        if (location.y >= 10)
        {
            //Debug.Log("map value out of bounds:" + location);
            return false;
        }

        return true;
    }
}