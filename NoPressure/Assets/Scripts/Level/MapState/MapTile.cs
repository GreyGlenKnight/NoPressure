using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile{

    public FloorTileType m_TileType { get; set; }
    public SpawnType m_SpawnType { get; set; }

    public bool isBroken = false;
    public bool isWired = false;
    public bool isSpace = false;

    public Room m_Room { get; set; }
    public MapTile m_NorthTile { get; set; }
    public MapTile m_SouthTile { get; set; }
    public MapTile m_WestTile { get; set; }
    public MapTile m_EastTile { get; set; }

    public float Pressure = 0f;
    public float NewPressure = 0f;
    public float PressureRate = 0f;

    public float bonusSpread = 2f;

    public float EqulizePressureTime = 0.5f;
    public float EqulizePressureCounter = 0.0f;

    public float pressureDecayRate = 0.005f;
    public float BrokenTileDecayRate = 0.8f;

    public float powerGeneration = 0f;
    public float powerConsumption = 0f;

    public PowerNetwork network = null;

    public Coord m_Location;

    

    public float getPower()
    {
        if(network == null)
        {
            //Debug.Log("Not in network");
            return 0f;
        }

        if (network.getPowerRatio() > 0.99f)
        {
            return 1f;
        }

        return network.getPowerRatio();

    }

    public float getPressure()
    {
        return Pressure + NewPressure;
    }

    public void removePowerNetwork()
    {
        network = null;
    }

    public void assignPowerNetwork()
    {
        // find network of neibors and join them

        PowerNetwork networkToJoin = null ;

        if (m_NorthTile != null)
        {
            if (m_NorthTile.network != null && m_NorthTile.isWired)
            {
                if (networkToJoin != null && networkToJoin != m_NorthTile.network)
                {
                    //Debug.Log("Merging " + m_Location + " With North");
                    networkToJoin = m_NorthTile.network.merge(networkToJoin);
                }
                else
                    networkToJoin = m_NorthTile.network;
            }
        }

        if (m_SouthTile != null)
        {
            if (m_SouthTile.network != null && m_SouthTile.isWired)
            {
                if (networkToJoin != null && networkToJoin != m_SouthTile.network)
                {
                    //Debug.Log("Merging " + m_Location + " With South");
                    networkToJoin = m_SouthTile.network.merge(networkToJoin);
                }
                else
                    networkToJoin = m_SouthTile.network; 
            }
        }

        //if (m_Location == new Coord(72, 89))
        //    Debug.Log("72,89 +" + m_WestTile);

        if (m_WestTile != null)
        {

            if (m_WestTile.network != null && m_WestTile.isWired)
            {
                if (networkToJoin != null && networkToJoin != m_WestTile.network)
                {
                    //Debug.Log("Merging " + m_Location + " With West");
                    networkToJoin = m_WestTile.network.merge(networkToJoin);
                }
                else
                    networkToJoin = m_WestTile.network;
            }
        }
        if (m_EastTile != null)
        {
            if (m_EastTile.network != null && m_EastTile.isWired)
            {
                if (networkToJoin != null && networkToJoin != m_EastTile.network)
                {
                    //Debug.Log("Merging " + m_Location + " With East");
                    networkToJoin = m_EastTile.network.merge(networkToJoin);
                }
                else
                    networkToJoin = m_EastTile.network;
            }
        }

        if (networkToJoin == null)
        {
            //Debug.Log("New network at " + m_Location);
            networkToJoin = PowerNetwork.GetNewPowerNetwork();
        }

        network = networkToJoin;
        networkToJoin.Add(this);


    }
    public void resetPressure(float deltaTime)
    {
        Pressure = NewPressure + PressureRate;

        if (isBroken)
            Pressure -= BrokenTileDecayRate * Pressure * deltaTime;
        else
            Pressure -= pressureDecayRate * Pressure * deltaTime;// Pressure decay rate

        if (Pressure > 0)
            Pressure -= deltaTime * 0.1f;

        NewPressure = 0;
    }

    public void SpreadPressure()
    {
        if (Pressure < 2)
        {
            NewPressure += Pressure;
            return;
        }
        //EqulizePressureCounter += deltaTime;
        //if(EqulizePressureCounter > EqulizePressureTime)
        //{
            //EqulizePressureCounter -= EqulizePressureTime;

            //float surroundingPressure = 0f;
            //surroundingPressure += Pressure;

        float pressureDirections = bonusSpread + 1f;

        float lowPressure = getPressure();
        MapTile lowBonus = this;

        if (m_NorthTile != null)
        {
            if (m_NorthTile.isWall() == false)
            {
                pressureDirections += 1;
                if (lowPressure > m_NorthTile.getPressure())
                {
                    lowPressure = m_NorthTile.getPressure();
                    lowBonus = m_NorthTile;
                }
            }

        }

        if (m_SouthTile != null)
        {
            if (m_SouthTile.isWall() == false)
            {
                pressureDirections += 1;
                if(lowPressure> m_SouthTile.getPressure())
                {
                    lowPressure = m_SouthTile.getPressure();
                    lowBonus = m_SouthTile;
                }
            }
        }

        if (m_WestTile != null)
        {
            if (m_WestTile.isWall() == false)
            {
                pressureDirections += 1;
                if (lowPressure > m_WestTile.getPressure())
                {
                    lowPressure = m_WestTile.getPressure();
                    lowBonus = m_WestTile;
                }
            }
        }

        if (m_EastTile != null)
        {
            if (m_EastTile.isWall() == false)
            {
                pressureDirections += 1;
                if (lowPressure > m_EastTile.getPressure())
                {
                    lowPressure = m_EastTile.getPressure();
                    lowBonus = m_EastTile;
                }
            }
        }

    //if (PressureRate > 0)
    //    Debug.Log("pressureDirections == " + pressureDirections);


    float pressureSpread = Pressure / pressureDirections;

        NewPressure += pressureSpread;

        if (m_NorthTile != null)
        {
            if (m_NorthTile.isWall() == false)
                m_NorthTile.NewPressure += pressureSpread;
        }

        if (m_SouthTile != null)
        {
            if (m_SouthTile.isWall() == false)
                m_SouthTile.NewPressure += pressureSpread;
        }

        if (m_WestTile != null)
        {
            if (m_WestTile.isWall() == false)
                m_WestTile.NewPressure += pressureSpread;
        }

        if (m_EastTile != null)
        {
            if (m_EastTile.isWall() == false)
                m_EastTile.NewPressure += pressureSpread;
        }

        lowBonus.NewPressure += pressureSpread * bonusSpread;

        //}
    }

    public void SetupConnections(
        MapTile lNorthTile,
        MapTile lSouthTile,
        MapTile lEastTile,
        MapTile lWestTile)
    {
        m_NorthTile = lNorthTile;
        m_SouthTile = lSouthTile;
        m_EastTile = lEastTile;
        m_WestTile = lWestTile;

        if (m_NorthTile != null)
            m_NorthTile.m_SouthTile = this;
        if (m_SouthTile != null)
            m_SouthTile.m_NorthTile = this;
        if (m_EastTile != null)
            m_EastTile.m_WestTile = this;
        if (m_WestTile != null)
            m_WestTile.m_EastTile = this;

    }

    //public void SetConnections(
    //    MapTile lNorthTile,
    //    MapTile lSouthTile,
    //    MapTile lWestTile,
    //    MapTile lEastTile)
    //{
    //    m_NorthTile = lNorthTile;
    //    m_SouthTile = lSouthTile;
    //    m_WestTile = lWestTile;
    //    m_EastTile = lEastTile;
    //}

    public MapTile (
        Coord location,
        FloorTileType lTileType, 
        SpawnType lSpawnType)
    {
        m_Location = location;

        m_TileType = lTileType;
        m_SpawnType = lSpawnType;



    }
    
    public bool isWall()
    {
        switch(m_SpawnType)
        {
            case SpawnType.Obstacle:
                return true;
            case SpawnType.Wall:
                return true;
            case SpawnType.ForceField:
                return true;
            default:
                return false;
        }
    }



}
