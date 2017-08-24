using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room {

    List<IPressureSource> PressureSources = new List<IPressureSource>();

    List<FloorTile> TilesInRoom = new List<FloorTile>();

    public float m_TotalPressure
    {
        get
        {
            float totalPressure = 0;
            if (PressureSources == null)
                return 0f;
            for(int i = 0; i<PressureSources.Count; i++)
            {
                totalPressure += PressureSources[i].mPressureAmount;
            }
            return totalPressure;
        }
    } 

    public int m_RoomSize { get
        {
            if (TilesInRoom != null)
                return TilesInRoom.Count;
            return 0;
        }
    }

    public float m_TilePressure
    {
        get
        {
            return m_TotalPressure / m_RoomSize;
        }
    }

    public void AddTileToRoom(FloorTile Tile)
    {
        TilesInRoom.Add(Tile); // Increase the size of the list
    }

    public void RemoveTileFromRoom(FloorTile Tile)
    {
        TilesInRoom.Remove(Tile); // Increase the size of the list
    }

    public void MergeRooms(Room otherRoom)
    {
        if(m_RoomSize > otherRoom.m_RoomSize)
        {
            otherRoom.MergeRooms(this);
            return;
        }

        for (int i = 0; i < TilesInRoom.Count; i++)
        {
            //TilesInRoom[i].m_Room = otherRoom;
        }

    }

}
