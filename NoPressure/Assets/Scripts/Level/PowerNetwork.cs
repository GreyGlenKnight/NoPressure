using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerNetwork {

    public static List<PowerNetwork> allNetworks = new List<PowerNetwork>();

    public HashSet<MapTile> connectedTiles = new HashSet<MapTile>();

    public float power = 0; 
    public float consumption = 0;
    public int id;
    static int nextId = 0;

    public static void CalcPowerOnAllNetworks()
    {
        //Debug.Log("Number Of networks = " + allNetworks.Count);
        for(int i = 0; i< allNetworks.Count; i++)
        {
            allNetworks[i].calcPower();
        }
    }

    public static PowerNetwork GetNewPowerNetwork()
    {
        PowerNetwork returnMe = new PowerNetwork();
        allNetworks.Add(returnMe);
        nextId++;
        return returnMe;
    }

    private PowerNetwork()
    {
        connectedTiles = new HashSet<MapTile>();
        id = nextId;

    }

    private void clear()
    {
        connectedTiles = null;
        allNetworks.Remove(this);
    }

    public PowerNetwork merge(PowerNetwork otherNetwork)
    {
        MapTile[] otherTiles = new MapTile[otherNetwork.connectedTiles.Count];
        otherNetwork.connectedTiles.CopyTo(otherTiles);

        for (int i = 0; i< otherTiles.Length;i++)
        {
            otherTiles[i].network = this;
        }

        connectedTiles.UnionWith(otherTiles);
        otherNetwork.clear();

        return this;
    }

    public void Add(MapTile toAdd)
    {
        connectedTiles.Add(toAdd);
    }

    public float getPowerRatio()
    {
        if (power > consumption + 0.1f)
            return 1f;
        else
            return power / (consumption + 0.1f);
    }

    private void calcPower()
    {
        power = 0; 
        consumption = 0;

        MapTile[] connectedTilesArray = new MapTile[connectedTiles.Count];
        connectedTiles.CopyTo(connectedTilesArray);
        for (int i = 0; i < connectedTilesArray.Length; i++)
        {
            power += connectedTilesArray[i].powerGeneration;
            //if (connectedTilesArray[i].powerGeneration > 0)
            //    Debug.Log("power found " + power);
            consumption += connectedTilesArray[i].powerConsumption;

        }
        //if(connectedTilesArray.Length >3)
        //Debug.Log(id +" power = " + power + " / " + consumption);

    }

}
