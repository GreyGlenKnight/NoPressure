using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {

    public GameObject Pistol;
    public GameObject Rifle;
    public GameObject Carbine;
    public GameObject PlasmaThrower;
    public GameObject RocketLauncher;

    public GameObject PortableGenerator;
    public GameObject PortablePressure;

    public GameObject Shield;
    public GameObject MecanicalTools;
    public GameObject ElectricalTools;

    public IInventoryItem SpawnItem(SpawnType typeToSpawn)
    {

        IInventoryItem ItemToSpawn;

        switch (typeToSpawn)
        {
            case SpawnType.Pistol:
                ItemToSpawn = Instantiate(Pistol).GetComponent<IInventoryItem>();
                ItemToSpawn.mCharges = Instantiate(Pistol.GetComponent<IInventoryItem>().mCharges);
                break;

            case SpawnType.Rifle:
                ItemToSpawn = Instantiate(Rifle).GetComponent<IInventoryItem>();
                ItemToSpawn.mCharges = Instantiate(Rifle.GetComponent<IInventoryItem>().mCharges);
                break;

            case SpawnType.Carbine:
                ItemToSpawn = Instantiate(Carbine).GetComponent<IInventoryItem>();
                ItemToSpawn.mCharges = Instantiate(Carbine.GetComponent<IInventoryItem>().mCharges);
                break;

            case SpawnType.PlasmaThrower:
                ItemToSpawn = Instantiate(PlasmaThrower).GetComponent<IInventoryItem>();
                ItemToSpawn.mCharges = Instantiate(PlasmaThrower.GetComponent<IInventoryItem>().mCharges);
                break;

            case SpawnType.RocketLauncher:
                ItemToSpawn = Instantiate(RocketLauncher).GetComponent<IInventoryItem>();
                ItemToSpawn.mCharges = Instantiate(RocketLauncher.GetComponent<IInventoryItem>().mCharges);
                break;

            case SpawnType.PortableGenerator:
                ItemToSpawn = Instantiate(PortableGenerator).GetComponent<IInventoryItem>();
                ItemToSpawn.mCharges = Instantiate(PortableGenerator.GetComponent<IInventoryItem>().mCharges);
                break;

            case SpawnType.PortablePressure:
                ItemToSpawn = Instantiate(PortablePressure).GetComponent<IInventoryItem>();
                ItemToSpawn.mCharges = Instantiate(PortablePressure.GetComponent<IInventoryItem>().mCharges);
                break;

            case SpawnType.Shield:
                ItemToSpawn = Instantiate(Shield).GetComponent<IInventoryItem>();
                ItemToSpawn.mCharges = Instantiate(Shield.GetComponent<IInventoryItem>().mCharges);
                break;

            case SpawnType.MecanicalTools:
                ItemToSpawn = Instantiate(MecanicalTools).GetComponent<IInventoryItem>();
                //ItemToSpawn.mCharges = Instantiate(Pistol.GetComponent<IInventoryItem>().mCharges);
                break;

            case SpawnType.ElectricalTools:
                ItemToSpawn = Instantiate(ElectricalTools).GetComponent<IInventoryItem>();
                //ItemToSpawn.mCharges = Instantiate(Pistol.GetComponent<IInventoryItem>().mCharges);
                break;

            default:
                ItemToSpawn = null;
                Debug.Log("Unassigned item type: " + typeToSpawn);
                break;
        }

        return ItemToSpawn;
    }

}
