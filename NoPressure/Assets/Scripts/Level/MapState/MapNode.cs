using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// MapNode knows how to load information with the help of the loader
public class MapNode
{
    // The size of units of game space in sectors
    public const int MAP_NODE_SECTOR_SIZE = 4;
    public const int MAP_NODE_TILE_SIZE = 40;
    public const int SECTOR_TILE_SIZE = MAP_NODE_TILE_SIZE/ MAP_NODE_SECTOR_SIZE;

    public MapNode NorthNode;
    public MapNode SouthNode;
    public MapNode WestNode;
    public MapNode EastNode;

    // Stores the data needed for a map node 
    MapSector[,] mSectors = new MapSector[MAP_NODE_SECTOR_SIZE, MAP_NODE_SECTOR_SIZE];

    Coord mRoomLocation;

    public MapNode(Coord lStartLocation, 
        string lMapName,
        MapNode lNorthNode,
        MapNode lSouthNode,
        MapNode lWestNode,
        MapNode lEastNode)
    {
        if (loadMapDataFromFile(lStartLocation, lMapName) == false)
        {
            return;
        }

        NorthNode = lNorthNode;
        SouthNode = lSouthNode;
        WestNode = lWestNode;
        EastNode = lEastNode;

        if (NorthNode != null)
            NorthNode.SouthNode = this;

        if (SouthNode != null)
            SouthNode.NorthNode = this;

        if (EastNode != null)
            EastNode.WestNode = this;

        if (WestNode != null)
            WestNode.EastNode = this;

        mRoomLocation = lStartLocation;
    }

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



    private bool loadMapDataFromFile(Coord lRoomLocation, string lMapName)
    {
        FileLoader fileLoader = new FileLoader();

        // Example: Demo23.csv 
        // Example: Hello11.csv
        string fileName = "Assets/Levels/" +lMapName + lRoomLocation.ToString("") + ".csv";

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
        // Example: Demo23F.csv 
        // Example: Hello11F.csv
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

        MapSector NorthSector = null ;
        MapSector SouthSector = null;
        MapSector WestSector = null;
        MapSector EastSector = null;

        for (int i = 0; i < 4; i++)
        {
            SectorLocation = new Coord(lRoomLocation.x * 4 + i, (lRoomLocation.y * 4) + 3);

            if (NorthNode != null)
                NorthSector = NorthNode.getSectorAt(new Coord(i, 0));
            else
                NorthNode = null;

            if (i == 0)
                if (WestNode != null)
                    WestSector = WestNode.getSectorAt(new Coord(3, 3));
                else
                    WestSector = null;
            else
                WestSector = mSectors[i - 1, 3];

            if (i == 3)
                if (EastSector != null)
                    EastSector = EastNode.getSectorAt(new Coord(0, 3));
                else
                    EastSector = null;
            else
                EastSector = mSectors[i + 1, 3];// always null

            SouthSector = mSectors[i, 2];// always null

            //Debug.Log(i);
            mSectors[i, 3] = new MapSector(this,
                SectorLocation,
                (SpawnType[,])(object)SectorRowObstacles0[i],
                (FloorTileType[,])(object)SectorRowFloor0[i],
                NorthSector,
                SouthSector,
                WestSector,
                EastSector);
                
        }

        for (int i = 0; i < 4; i++)
        {
            SectorLocation = new Coord(lRoomLocation.x * 4 + i, (lRoomLocation.y * 4) + 2);

            NorthSector = mSectors[i, 3];

            if (i == 0)
                if (WestNode != null)
                    WestSector = WestNode.getSectorAt(new Coord(3, 2));
                else
                    WestSector = null;
            else
                WestSector = mSectors[i - 1, 2];

            if (i == 3)
                if (EastNode != null)
                    EastSector = EastNode.getSectorAt(new Coord(0, 2));
                else
                    EastSector = null;
            else
                EastSector = mSectors[i + 1, 2];// always null

            SouthSector = mSectors[i, 1];// always null

            mSectors[i, 2] = new MapSector(this,
                SectorLocation,
                (SpawnType[,])(object)SectorRowObstacles1[i],
                (FloorTileType[,])(object)SectorRowFloor1[i],
                NorthSector,
                SouthSector,
                WestSector,
                EastSector);
        }

        for (int i = 0; i < 4; i++)
        {
            SectorLocation = new Coord(lRoomLocation.x * 4 + i, (lRoomLocation.y * 4) + 1);

            NorthSector = mSectors[i, 2];

            if (i == 0)
                if (WestNode != null)
                    WestSector = WestNode.getSectorAt(new Coord(3, 1));
                else
                    WestSector = null;
            else
                WestSector = mSectors[i - 1, 1];

            if (i == 3)
                if (EastNode != null)
                    EastSector = EastNode.getSectorAt(new Coord(0, 1));
                else
                    EastSector = null;
            else
                EastSector = mSectors[i + 1, 1];// always null

            SouthSector = mSectors[i, 0];// always null

            mSectors[i, 1] = new MapSector(this,
                SectorLocation,
                (SpawnType[,])(object)SectorRowObstacles2[i],
                (FloorTileType[,])(object)SectorRowFloor2[i],
                NorthSector,
                SouthSector,
                WestSector,
                EastSector); 
        }

        for (int i = 0; i < 4; i++) 
        {
            SectorLocation = new Coord(lRoomLocation.x * 4 + i, (lRoomLocation.y * 4) + 0);

            NorthSector = mSectors[i, 1];

            if (i == 0)
                if (WestNode != null)
                    WestSector = WestNode.getSectorAt(new Coord(3, 0));
                else
                    WestSector = null;
            else
                WestSector = mSectors[i - 1, 0];

            if (i == 3)
                if (EastNode != null)
                    EastSector = EastNode.getSectorAt(new Coord(0, 0));
                else
                    EastSector = null;
            else
                EastSector = mSectors[i + 1, 0];// always null

            if (SouthNode != null)
                SouthSector = SouthNode.getSectorAt(new Coord(i, 3));
            else
                SouthSector = null;


            mSectors[i, 0] = new MapSector(this,
                SectorLocation,
                (SpawnType[,])(object)SectorRowObstacles3[i],
                (FloorTileType[,])(object)SectorRowFloor3[i],
                NorthSector,
                SouthSector,
                WestSector,
                EastSector);
        }
        return true;
    }
}
