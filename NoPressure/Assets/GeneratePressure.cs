using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GeneratePressure : PowerSource
{
    //public MapTile linkedTile;

    //public float PressureRemaining = 1000f;

    public Material OnCharge4;
    public Material OnCharge3;
    public Material OnCharge2;
    public Material OnCharge1;
    float amountCharged = 0;

    public float ChargeRequired = 200;
    public float PressureAmount = 200;

    //public void onDeathDelegate(Transform dieingUnit)
    //{
        
    //    Debug.Log("Stop the Pressure");
    //    linkedTile.PressureRate = 0;
    //}

    protected override void Update()
    {

        if (isDamaged == true)
        {
            ChangeMateral(BrokenMateral);
            return;
        }

        else if (isOn == false)
        {
            ChangeMateral(OffMateral);
        }

        else
        {
            if(amountCharged < ChargeRequired/4)
                ChangeMateral(OnCharge1);
            else if (amountCharged < (ChargeRequired / 2))
                ChangeMateral(OnCharge2);
            else if (3 * amountCharged < (ChargeRequired / 4))
                ChangeMateral(OnCharge3);
            else
                ChangeMateral(OnCharge4);

        }
            
        if (linkedTile == null)
            return;

        if (isOn == true)
        {
            linkedTile.powerConsumption = consumption;
            amountCharged += linkedTile.getPower() * consumption * Time.deltaTime;

            if (amountCharged > ChargeRequired)
            {
                linkedTile.NewPressure += PressureAmount;
                amountCharged -= ChargeRequired;
            }
        }
        else
        {
            linkedTile.powerConsumption = 0;
        }
    }


}