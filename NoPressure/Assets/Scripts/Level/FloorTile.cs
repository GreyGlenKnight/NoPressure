using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : PersistentEntity {

    public Material FloorMaterial;

    public MapTile TileData;

    public float m_LastUpdateTime;

    public Material WiredPower0;
    public Material WiredPower1;
    public Material WiredPower2;
    public Material WiredPower3;
    public Material WiredPower4;

    public Material Wired0;
    public Material Wired1;
    public Material Wired2;
    public Material Wired3;
    public Material Wired4;

    public Material WiredSemiActive0;
    public Material WiredSemiActive1;
    public Material WiredSemiActive2;
    public Material WiredSemiActive3;
    public Material WiredSemiActive4;

    public Material Power0;
    public Material Power1;
    public Material Power2;
    public Material Power3;
    public Material Power4;

    public Material BrokenActive0;
    public Material BrokenActive1;
    public Material BrokenActive2;
    public Material BrokenActive3;
    public Material BrokenActive4;

    public Material Broken0;
    public Material Broken1;
    public Material Broken2;
    public Material Broken3;
    public Material Broken4;

    public Material BrokenWire0;
    public Material BrokenWire1;
    public Material BrokenWire2;
    public Material BrokenWire3;
    public Material BrokenWire4; 

    public void updateMat()
    {
        if (TileData.isBroken == false)
        {
            if (TileData.isWired == true)
            {
                if (TileData.getPower() > 0.99f)
                {
                    if (TileData.getPressure() < 10f)
                    {
                        ChangeMateral(WiredPower0);
                    }

                    else if (TileData.getPressure() < 25f)
                    {
                        ChangeMateral(WiredPower1);
                    }
                    else if (TileData.getPressure() < 45f)
                    {
                        ChangeMateral(WiredPower2);
                    }
                    else if (TileData.getPressure() < 70f)
                    {
                        ChangeMateral(WiredPower3);
                    }
                    else
                    {
                        ChangeMateral(WiredPower4);
                    }
                }
                else if (TileData.getPower() < 0.01f)
                {
                    if (TileData.getPressure() < 10f)
                    {
                        ChangeMateral(Wired0);
                    }

                    else if (TileData.getPressure() < 25f)
                    {
                        ChangeMateral(Wired1);
                    }
                    else if (TileData.getPressure() < 45f)
                    {
                        ChangeMateral(Wired2);
                    }
                    else if (TileData.getPressure() < 70f)
                    {
                        ChangeMateral(Wired3);
                    }
                    else
                    {
                        ChangeMateral(Wired4);
                    }
                }
                else
                {
                    if (TileData.getPressure() < 10f)
                    {
                        ChangeMateral(WiredSemiActive0);
                    }

                    else if (TileData.getPressure() < 25f)
                    {
                        ChangeMateral(WiredSemiActive1);
                    }
                    else if (TileData.getPressure() < 45f)
                    {
                        ChangeMateral(WiredSemiActive2);
                    }
                    else if (TileData.getPressure() < 70f)
                    {
                        ChangeMateral(WiredSemiActive3);
                    }
                    else
                    {
                        ChangeMateral(WiredSemiActive4);
                    }
                }
            }
            else
            {
                if (TileData.getPressure() < 10f)
                {
                    ChangeMateral(Power0);
                }

                else if (TileData.getPressure() < 25f)
                {
                    ChangeMateral(Power1);
                }
                else if (TileData.getPressure() < 45f)
                {
                    ChangeMateral(Power2);
                }
                else if (TileData.getPressure() < 70f)
                {
                    ChangeMateral(Power3);
                }
                else
                {
                    ChangeMateral(Power4);
                }
            }
        }
        else if (TileData.isBroken == true)
        {
            if (TileData.isWired == true)
            {
                if (TileData.getPower() > 0.1f)
                {
                    if (TileData.getPressure() < 10f)
                    {
                        ChangeMateral(BrokenActive0);
                    }

                    else if (TileData.getPressure() < 25f)
                    {
                        ChangeMateral(BrokenActive1);
                    }
                    else if (TileData.getPressure() < 45f)
                    {
                        ChangeMateral(BrokenActive2);
                    }
                    else if (TileData.getPressure() < 70f)
                    {
                        ChangeMateral(BrokenActive3);
                    }
                    else
                    {
                        ChangeMateral(BrokenActive4);
                    }
                }
                else
                {
                    if (TileData.getPressure() < 10f)
                    {
                        ChangeMateral(BrokenWire0);
                    }

                    else if (TileData.getPressure() < 25f)
                    {
                        ChangeMateral(BrokenWire1);
                    }
                    else if (TileData.getPressure() < 45f)
                    {
                        ChangeMateral(BrokenWire2);
                    }
                    else if (TileData.getPressure() < 70f)
                    {
                        ChangeMateral(BrokenWire3);
                    }
                    else
                    {
                        ChangeMateral(BrokenWire4);
                    }
                }
            }
            else
            {
                if (TileData.getPressure() < 10f)
                {
                    ChangeMateral(Broken0);
                }

                else if (TileData.getPressure() < 25f)
                {
                    ChangeMateral(Broken1);
                }
                else if (TileData.getPressure() < 45f)
                {
                    ChangeMateral(Broken2);
                }
                else if (TileData.getPressure() < 70f)
                {
                    ChangeMateral(Broken3);
                }
                else
                {
                    ChangeMateral(Broken4);
                }
            }

        }
    }

    public void RemoveNetwork()
    {
        TileData.removePowerNetwork();
    }

    public void AssignNetwork()
    {
        //if (isWired == false)
        //    Debug.Log("only wired tiles should be assigned a network!");
        //else
        TileData.assignPowerNetwork();
    }

    public void ResetPressure()
    {
        float deltaTime = Time.timeSinceLevelLoad - m_LastUpdateTime;
        m_LastUpdateTime = Time.timeSinceLevelLoad;
        //Debug.Log(TileData.getPressure() + "," + "0988");
        TileData.resetPressure(deltaTime);
        updateMat();
    }

    public void SpreadPressure()
    {
        float deltaTime = Time.timeSinceLevelLoad - m_LastUpdateTime;
        //m_LastUpdateTime = Time.timeSinceLevelLoad;

        TileData.SpreadPressure();
    }

    public void ManualUpdate()
    {
    }

    public void ChangeMateral(Material newMateral)
    {
        Material[] mats;
        mats = GetComponent<Renderer>().materials;
        mats[0] = newMateral;
        GetComponent<Renderer>().materials = mats;
    }

    public void RemoveWire()
    {

    }

    public void AddWireToTile()
    {

    }

    public void PowerOn()
    {
        // If this tile already has power, we stop propagating the current


        // If this tile was off and is turning on, propagate power to all neighbors

    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
	}
	
}
