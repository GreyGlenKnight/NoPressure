using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum FloorTiles
{
    Blank,
    WiredInactive,
    WiredActive,
}

public enum WorldSpaceUnit
{
    Tile, //Smallest unit == 1 Unity Unit
    Sector, // 10x10 Tiles 1/16 of a file
    MpaNode, // 1 File 40x40 Tiles
}


public enum SpawnType
{
    None = 0,
    Wall = 1,
    Player = 2,
    Enemy = 3,
    Obstacle = 4,

    Pistol = 20,
    Rifle = 21,
    Carbine = 22,

    Mine = 30,

    Shield = 40,

    MecanicalTools = 50,
    ElectricalTools = 51,

    PressureStation = 97,
    BrokenPressureStation = 98,
    ElectricTrap = 99,

};




// MapNode knows how to load information with the help of the loader
public class MapNode
{
    // The size of units of game space in sectors
    public const int MAP_NODE_SECTOR_SIZE = 4;
    public const int MAP_NODE_TILE_SIZE = 40;
    public const int SECTOR_TILE_SIZE = MAP_NODE_TILE_SIZE/ MAP_NODE_SECTOR_SIZE;

    // Stores the data needed for a map node 
    MapSector[,] mSectors = new MapSector[MAP_NODE_SECTOR_SIZE, MAP_NODE_SECTOR_SIZE];

    Coord mRoomLocation;

    //MapNode mMapNodeZUp;
    //MapNode mMapNodeZDown;
    //MapNode mMapNodeXUp;
    //MapNode mMapNodeXDown;

    // Only attempt to load once
    //public bool AttemptedLoad { get; private set; }
    //public bool hasLoaded { get; private set; }

    //private MapNode(Coord lRoomLocation, string lMapName)
    //{
    //    mRoomLocation = lRoomLocation;

    //    loadMapDataFromFile(lRoomLocation, lMapName);
    //}

    public MapSector getSectorAt(Coord location)
    {
        return mSectors[location.x, location.y];
    }


    public void SetAllSectorsUnloaded()
    {
        for(int i = 0; i<4; i++)
            for(int j = 0; j<4; j++)
            {
                if (mSectors[i, j] != null)
                    mSectors[i, j].pIsLoaded = false;
            }
    }

    private void CreateBlankSectors()
    {
        for(int i =0; i<4;i++)
        {
            for(int j = 0; j<4; j++)
            {
                mSectors[i, j] = new MapSector(this);
            }
        }
    }


    public MapNode(Coord lStartLocation, string lMapName)
    {
        if (loadMapDataFromFile(lStartLocation, lMapName) == false)
        {

            return;
        }
        mRoomLocation = lStartLocation;
    }


    private bool loadMapDataFromFile(Coord lRoomLocation, string lMapName)
    {
        //int[] DilimLine;
        FileLoader fileLoader = new FileLoader();

        // example Demo23.csv 
        // example Hello11.csv
        string fileName = "Assets\\Levels\\" +lMapName + lRoomLocation.ToString("") + ".csv";

        if (fileLoader.load(fileName) == false)
        {
            //Debug.Log("Failed to load file: " + fileName);
            return false;
        }

        if (fileLoader.getLineCommaDelim() == null) //step passed header line. it is not used by program
        {
            Debug.Log("Failure reading file for level " + lMapName + lRoomLocation.ToString());
            return false;
        }

        

        fileLoader.getLineCommaDelim();//Param line

        List<int[,]> SectorRowObstacles0;
        List<int[,]> SectorRowObstacles1;
        List<int[,]> SectorRowObstacles2;
        List<int[,]> SectorRowObstacles3;

        SectorRowObstacles0 = fileLoader.GetIntLineCommaDelim(SECTOR_TILE_SIZE, SECTOR_TILE_SIZE);
        SectorRowObstacles1 = fileLoader.GetIntLineCommaDelim(SECTOR_TILE_SIZE, SECTOR_TILE_SIZE);
        SectorRowObstacles2 = fileLoader.GetIntLineCommaDelim(SECTOR_TILE_SIZE, SECTOR_TILE_SIZE);
        SectorRowObstacles3 = fileLoader.GetIntLineCommaDelim(SECTOR_TILE_SIZE, SECTOR_TILE_SIZE);

        // Now load floor tile map
        // example Demo23F.csv 
        // example Hello11F.csv
        fileName = "Assets\\Levels\\" + lMapName + lRoomLocation.ToString("") + "F.csv";

        if (fileLoader.load(fileName) == false)
        {
            Debug.LogError("Failed to load file: " + fileName);
        }

        if (fileLoader.getLineCommaDelim() == null) //step passed header line. it is not used by program
            Debug.LogError("Failure reading file for level " + fileName);

        fileLoader.getLineCommaDelim();//Param line

        List<int[,]> SectorRowFloor0;
        List<int[,]> SectorRowFloor1;
        List<int[,]> SectorRowFloor2;
        List<int[,]> SectorRowFloor3;

        SectorRowFloor0 = fileLoader.GetIntLineCommaDelim(SECTOR_TILE_SIZE, SECTOR_TILE_SIZE);
        SectorRowFloor1 = fileLoader.GetIntLineCommaDelim(SECTOR_TILE_SIZE, SECTOR_TILE_SIZE);
        SectorRowFloor2 = fileLoader.GetIntLineCommaDelim(SECTOR_TILE_SIZE, SECTOR_TILE_SIZE);
        SectorRowFloor3 = fileLoader.GetIntLineCommaDelim(SECTOR_TILE_SIZE, SECTOR_TILE_SIZE);

        Coord SectorLocation;

        for (int i = 0; i < 4; i++)
        {
            SectorLocation = new Coord(lRoomLocation.x * 4 + i, (lRoomLocation.y * 4) + 3);

            //Debug.Log(i);
            mSectors[i , 3 ] = new MapSector(this,
                SectorLocation,
                (SpawnType[,])(object)SectorRowObstacles0[i],
                (FloorTiles[,])(object)SectorRowFloor0[i]);
        }


        for (int i = 0; i < 4; i++)
        {
            SectorLocation = new Coord(lRoomLocation.x * 4 + i, (lRoomLocation.y * 4) + 2);

            mSectors[i, 2] = new MapSector(this,
                SectorLocation,
                (SpawnType[,])(object)SectorRowObstacles1[i],
                (FloorTiles[,])(object)SectorRowFloor1[i]);
        }


        for (int i = 0; i < 4; i++)
        {
            SectorLocation = new Coord(lRoomLocation.x * 4 + i, (lRoomLocation.y * 4) + 1);

            mSectors[i, 1] = new MapSector(this,
                SectorLocation,
                (SpawnType[,])(object)SectorRowObstacles2[i],
                (FloorTiles[,])(object)SectorRowFloor2[i]);
        }

        for (int i = 0; i < 4; i++)
        {
            SectorLocation = new Coord(lRoomLocation.x * 4 + i, (lRoomLocation.y * 4) + 0);
            mSectors[i, 0] = new MapSector(this,
                SectorLocation,
                (SpawnType[,])(object)SectorRowObstacles3[i],
                (FloorTiles[,])(object)SectorRowFloor3[i]);
        }
        return true;

    }

}
