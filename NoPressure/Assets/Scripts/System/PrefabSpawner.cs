using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public class PrefabSpawner : MonoBehaviour {

    public Transform MovingUnitsHolder;
    public Transform BlankObject;
    public Transform BoardHolder;

    private List<Transform> NoSpawnTilesList; //TODO add in sector logic

    public Transform ElectricTrapTile;
    public Transform PressureTile;
    public Transform BrokenPressureStationTile;

    public Transform CratePrefab;
    public Transform[] enemies;

    private List<Transform> movingEntities = new List<Transform>();

    private Action<Transform> mSaveLocationOnMap;

    private void Awake()
    {
        NoSpawnTilesList = new List<Transform>();
        BoardHolder = transform.Find("BoardHolder").transform;
    }

    public void Init(Action<Transform> lSaveLocationOnMap)
    {
        mSaveLocationOnMap = lSaveLocationOnMap;
    }

    public void SpawnSector(MapSector SpawnSector )
    {
        if (SpawnSector.pIsLoaded == true)
            return;

        Coord SectorLocation = SpawnSector.mSectorLocation;

        //Debug.Log("spawning Sector: " + SectorLocation.ToString("_"));
        GameObject SectorContainer = GameObject.Find("Sector" + SectorLocation.ToString("_"));

        if (SectorContainer == null)
        {
            if (BoardHolder == null)
                BoardHolder = GameObject.Find("BoardHolder").transform;

            // Create Container
            SectorContainer = Instantiate(BoardHolder.gameObject);
            SectorContainer.name = "Sector" + SectorLocation.ToString("_"); 
        }

        for (int i = 0; i< 10; i++)
        {
            for (int j = 0; j<10; j++)
            {
                PersistentEntity entity;
                SpawnType spawnType = SpawnSector.GetSpawnAt(i, j);

                entity = SpawnPrefab(spawnType, new Coord(SectorLocation.x*10 + i, SectorLocation.y*10 + j));
                
                if (entity != null)
                {
                    entity.spawnType = spawnType;
                    entity.mOnDeathHandler += DespawnObject;
                    if (entity is MovingEntity)
                    {
                        movingEntities.Add(entity.transform);
                        entity.transform.parent = MovingUnitsHolder;
                        entity.SetSpawnPoint(SpawnSector,new Coord(i, j));
                        SpawnSector.RemoveObjectAt(new Coord(i, j));
                        entity.mOnDespawn += mSaveLocationOnMap;
                    }
                    else
                    {
                        entity.transform.parent = SectorContainer.transform;
                        entity.SetSpawnPoint(SpawnSector, new Coord(i, j));
                    }
                }
            }
        }
        SpawnSector.pIsLoaded = true;
    }

    public List<Transform> FindEntityLeftOf (int sector)
    {
        List<Transform> returnVal = new List<Transform>();
        for (int i = 0; i < movingEntities.Count; i++)
        {
            if (movingEntities[i].position.x < sector * 10)
            {
                returnVal.Add(movingEntities[i]);
                //movingEntities.RemoveAt(i);
            }
        }
        return returnVal;
    }


    public void DespawnAbove(int sector)
    {
        List<Transform> despawnList = FindEntityAbove(sector);
        DespawnList(despawnList);
    }

    public void DespawnBelow(int sector)
    {
        List<Transform> despawnList = FindEntityBelow(sector);
        DespawnList(despawnList);
    }

    public void DespawnRightOf(int sector)
    {
        List<Transform> despawnList = FindEntityRightOf(sector);
        DespawnList(despawnList);
    }

    public void DespawnLeftOf(int sector)
    {
        List<Transform> despawnList = FindEntityLeftOf(sector);
        DespawnList(despawnList);
    }


    public List<Transform> FindEntityRightOf(int sector)
    {
        List<Transform> returnVal = new List<Transform>();
        for (int i = 0; i < movingEntities.Count; i++)
        {
            if( movingEntities[i] == null)
            {
                movingEntities.RemoveAt(i);
                i--;
            }
            else if (movingEntities[i].position.x > (sector * 10) +10)
            {
                returnVal.Add(movingEntities[i]);
                //movingEntities.RemoveAt(i);
            }
        }
        return returnVal;
    }

    public List<Transform> FindEntityAbove(int sector)
    {
        List<Transform> returnVal = new List<Transform>();
        for (int i = 0; i < movingEntities.Count; i++)
        {
            if (movingEntities[i].position.z > (sector * 10) +10)
            {
                returnVal.Add(movingEntities[i]);
                //movingEntities.RemoveAt(i);
            }
        }
        return returnVal;
    }

    public List<Transform> FindEntityBelow(int sector)
    {
        List<Transform> returnVal = new List<Transform>();
        for (int i = 0; i < movingEntities.Count; i++)
        {
            if (movingEntities[i].position.z < sector * 10)
            {
                returnVal.Add(movingEntities[i]);
                //movingEntities.RemoveAt(i);
            }
        }
        return returnVal;
    }

    public void DespawnList(List<Transform> despawn)
    {
        for(int i=0; i<despawn.Count; i++)
        {
            MovingEntity entity = despawn[i].GetComponent<MovingEntity>();
            SpawnType spawnType = entity.spawnType;
            Coord location = new Coord((int)Math.Round(despawn[i].position.x, 0), (int)Math.Round(despawn[i].position.z, 0));

            entity.Despawn();
            movingEntities.Remove(despawn[i]);
            despawn[i].gameObject.SetActive(false);

            Destroy(despawn[i].gameObject);
        }
    }

    public void DespawnSector(MapSector SpawnSector)
    {
        if (SpawnSector == null)
            return;

        // Dont do anything if the sector was already despawned or was never spawned
        if (SpawnSector.pIsLoaded == false)
            return;

        Coord SectorLocation = SpawnSector.mSectorLocation;

        //Debug.Log("Despawning Sector: " + SectorLocation.ToString("_"));

        GameObject SectorContainer = GameObject.Find("Sector" + SectorLocation.ToString("_"));

        List<Transform> childList = new List<Transform>();

        if (SectorContainer == null)
            return;

        for (int i = 0; i< SectorContainer.transform.childCount; i++)
        {
            childList.Add(SectorContainer.transform.GetChild(i));
        }

        for (int i = 0; i< childList.Count; i++ )
        {
            // Destroy all the children... Damn... thats brutal.
            childList[i].gameObject.SetActive(false);
            Destroy(childList[i].gameObject);
        }

        SpawnSector.pIsLoaded = false;
    }

    public PersistentEntity SpawnPrefab(SpawnType itemTypeToSpawn, Coord SpawnLocation)
    {
        switch (itemTypeToSpawn)
        {
            case SpawnType.None:
                return CreateFloor(SpawnLocation);
            case SpawnType.Wall:
                return CreateWall(SpawnLocation);
            case SpawnType.Enemy:
                return CreateEnemy(SpawnLocation);
            case SpawnType.Obstacle:
                return CreateObstacle(SpawnLocation);
            case SpawnType.Pistol:
                return CreateCrate(SpawnLocation, SpawnType.Pistol);
            case SpawnType.Rifle:
                return CreateCrate(SpawnLocation, SpawnType.Rifle);
            case SpawnType.Carbine:
                return CreateCrate(SpawnLocation, SpawnType.Carbine);
            case SpawnType.Shield:
                return CreateCrate(SpawnLocation, SpawnType.Shield);
            case SpawnType.Mine:
                return CreateCrate(SpawnLocation, SpawnType.Mine);
            case SpawnType.MecanicalTools:
                return CreateCrate(SpawnLocation, SpawnType.MecanicalTools);
            case SpawnType.ElectricalTools:
                return CreateCrate(SpawnLocation, SpawnType.ElectricalTools);
            case SpawnType.PressureStation:
                return CreatePressureTile(SpawnLocation);
            case SpawnType.BrokenPressureStation:
                return CreateBrokenPressureStationTile(SpawnLocation);
            case SpawnType.ElectricTrap:
                return CreateElectricTrapTile(SpawnLocation);
            default:
                Debug.Log("Invalid item spawn number: " + itemTypeToSpawn);
                return null;
        }
    }

    public void RemoveCrateFromList(Transform CrateToRemove)
    {
        NoSpawnTilesList.Remove(CrateToRemove);
    }

    private PersistentEntity CreatePressureTile(Coord location)
    {
        return CreatePressureTile(location.x, location.y);
    }

    private PersistentEntity CreatePressureTile(int x, int z)
    {
        if (PressureTile == null)
        {
            PressureTile = GameObject.Find("PressureStation").transform;
            if (PressureTile == null)
            {
                Debug.Log("Failed loading Pressure Tile");
                return null;
            }
        }

        Transform newTile = Instantiate(PressureTile, new Vector3(x, 0.2f, z), Quaternion.identity);
        NoSpawnTilesList.Add(newTile);

        return newTile.GetComponent<EffectTile>();
    }

    public PersistentEntity CreateBrokenPressureStationTile(Coord location)
    {
        return CreateBrokenPressureStationTile(location.x, location.y);
    }

    public PersistentEntity CreateBrokenPressureStationTile(int x, int z)
    {
        if (BrokenPressureStationTile == null)
        {
            BrokenPressureStationTile = GameObject.Find("BrokenPressureStation").transform;
            if (BrokenPressureStationTile == null)
            {
                Debug.Log("Failed loading broken pressure stations 2");
                return null;
            }
        }
        Transform crateObject = Instantiate(BrokenPressureStationTile, new Vector3(x, 0.2f, z), Quaternion.identity);
        NoSpawnTilesList.Add(crateObject);

        return crateObject.GetComponent<EffectTile>();
    }

    public PersistentEntity CreateElectricTrapTile(Coord location)
    {
        return CreateElectricTrapTile(location.x, location.y);
    }

    public PersistentEntity CreateElectricTrapTile(int x, int z)
    {
        if (ElectricTrapTile == null)
        {
            ElectricTrapTile = GameObject.Find("ElectricTrap").transform;
            if (ElectricTrapTile == null)
            {
                Debug.Log("Failed loading electric traps");
                return null;
            }
        }
        Transform crateObject = Instantiate(ElectricTrapTile, new Vector3(x, 0.2f, z), Quaternion.identity);
        NoSpawnTilesList.Add(crateObject);

        return crateObject.GetComponent<EffectTile>();

    }

    public PersistentEntity CreateCrate(Transform location, IInventoryItem contents)
    {
        return CreateCrate(
            (int) Math.Round(location.position.x, 0),
            (int) Math.Round(location.position.z, 0),
            contents);
    }

    public PersistentEntity CreateCrate(Coord location, IInventoryItem contents)
    {
        return CreateCrate(location.x, location.y, contents);
    }

    public PersistentEntity CreateCrate(int x, int z, IInventoryItem contents)
    {
        if (CratePrefab == null)
        {
            CratePrefab = GameObject.Find("Crate").transform;
            if (CratePrefab == null)
            {
                Debug.Log("Crate Prefab is null, assign in editor");
                return null; ;
            }
        }
        Vector2 SpawnLocation;
        if (FindNearestEmptyTile(x, z, out SpawnLocation) == false)
            return null;

        Transform crateObject = Instantiate(CratePrefab, new Vector3(SpawnLocation.x, 1, SpawnLocation.y), Quaternion.identity);
        Crate newCrate = crateObject.GetComponent<Crate>();
        if (newCrate == null)
            Debug.LogError("new create item does not have crate component");

        newCrate.SetCrateItem(contents);

        NoSpawnTilesList.Add(crateObject);
        return crateObject.GetComponent<Crate>();
    }

    public PersistentEntity CreateCrate(Coord location, SpawnType contents)
    {
        return CreateCrate(location.x, location.y, contents);
    }

    public PersistentEntity CreateCrate(int x, int z, SpawnType contents)
    {
        IInventoryItem itemToSpawn = ItemManager.SpawnItem(contents);

        if (itemToSpawn == null)
        {
            Debug.Log("can not find item type: " + contents);
            return null;
        }
        return CreateCrate(x, z, ItemManager.SpawnItem(contents));
    }

    private bool FindNearestEmptyTile(int x, int z, out Vector2 emptyTile)
    {

        int xBoundHigh = x + 1;
        int xBoundLow = x - 1;
        int zBoundHigh = z + 1;
        int zBoundLow = z - 1;

        List<Vector2> closeNonEmptyTiles = new List<Vector2>();

        for (int i = 0; i < NoSpawnTilesList.Count; i++)
        {
            if (NoSpawnTilesList[i] == null)
                NoSpawnTilesList.RemoveAt(i);
            else
            {
                int tileX = (int)Math.Round(NoSpawnTilesList[i].position.x, 0);
                int tileZ = (int)Math.Round(NoSpawnTilesList[i].position.z, 0);
                if (tileX <= xBoundHigh && tileX >= xBoundLow)
                {
                    if (tileZ <= zBoundHigh && tileZ >= zBoundLow)
                    {
                        closeNonEmptyTiles.Add(new Vector2(tileX, tileZ));
                    }
                }
            }
        }

        if (!closeNonEmptyTiles.Contains(new Vector2(x, z)))
        {
            //Debug.Log("return "+x + "," + z);
            emptyTile = new Vector2(x, z);
            return true;
        }
        if (!closeNonEmptyTiles.Contains(new Vector2(x + 1, z)))
        {
            emptyTile = new Vector2(x + 1, z);
            return true;
        }
        if (!closeNonEmptyTiles.Contains(new Vector2(x - 1, z)))
        {
            emptyTile = new Vector2(x - 1, z);
            return true;
        }
        if (!closeNonEmptyTiles.Contains(new Vector2(x, z + 1)))
        {
            emptyTile = new Vector2(x, z + 1);
            return true;
        }
        if (!closeNonEmptyTiles.Contains(new Vector2(x, z - 1)))
        {
            emptyTile = new Vector2(x, z - 1);
            return true;
        }
        if (!closeNonEmptyTiles.Contains(new Vector2(x + 1, z + 1)))
        {
            emptyTile = new Vector2(x + 1, z + 1);
            return true;
        }
        if (!closeNonEmptyTiles.Contains(new Vector2(x + 1, z - 1)))
        {
            emptyTile = new Vector2(x + 1, z - 1);
            return true;
        }
        if (!closeNonEmptyTiles.Contains(new Vector2(x - 1, z + 1)))
        {
            emptyTile = new Vector2(x - 1, z + 1);
            return true;
        }

        if (!closeNonEmptyTiles.Contains(new Vector2(x - 1, z - 1)))
        {
            emptyTile = new Vector2(x - 1, z - 1);
            return true;
        }

        Debug.Log("no nearby tiles found, failed spawning item");
        emptyTile = new Vector2(x + 3, z);
        return false;
    }

    public PersistentEntity CreateObstacle(Coord location)
    {
        return CreateObstacle(location.x, location.y);
    }

    public PersistentEntity CreateObstacle(int x, int z)
    {
        //TileInfo obstacleTileInfo = roomSlots.Dequeue();
        //obstacleTileInfo.prefabType = PrefabType.Obstacle;
        Transform obstaclePrefab = GameObject.Find("DestructibleObstacle").transform;
        //Transform obstaclePrefab = obstacleTiles[Random.Range(0, obstacleTiles.Length - 1)];
        Transform newObstacle = Instantiate(obstaclePrefab, new Vector3(x, 1, z), Quaternion.identity);
        NoSpawnTilesList.Add(newObstacle);

        GraphUpdateObject GUO = new GraphUpdateObject();
        GUO.bounds = new Bounds(new Vector3(x , 0.0f, z ), new Vector3(1.4f, 2f, 1.4f));
        GUO.setWalkability = false;
        GUO.modifyWalkability = true;


        AstarPath.active.UpdateGraphs(GUO);

        PersistentEntity returnVal = newObstacle.GetComponent<DestructibleObstacle>();

        return returnVal;
    }

    public PersistentEntity CreateFloor(Coord location)
    {
        return CreateFloor(location.x, location.y);
    }


    public PersistentEntity CreateFloor(int x, int z)
    {

        Transform floorPrefab = GameObject.Find("Floor").transform;

        //Transform floorPrefab = floorTiles[Random.Range(0, floorTiles.Length - 1)];
        Transform floorTile = Instantiate(floorPrefab, new Vector3(x, -.25f, z), Quaternion.identity);

        return floorTile.GetComponent<FloorTile>();
    }

    public PersistentEntity CreateWall(Coord location)
    {
        return CreateWall(location.x, location.y);
    }


    public PersistentEntity CreateWall(int x, int z)
    {
        Transform wallPrefab = GameObject.Find("Wall").transform;
        Transform wallTile = Instantiate(wallPrefab, new Vector3(x, 1, z), Quaternion.identity);

        GraphUpdateObject GUO = new GraphUpdateObject();
        GUO.bounds = new Bounds(new Vector3(x , 0.0f, z), new Vector3(1.4f, 2f, 1.4f));
        GUO.setWalkability = false;
        GUO.modifyWalkability = true;

        if (AstarPath.active == null) 
            AstarPath.active = GameObject.Find("_A*").GetComponent<AstarPath>();
        AstarPath.active.UpdateGraphs(GUO);

        NoSpawnTilesList.Add(wallTile);

        return wallTile.GetComponent<DestructibleObstacle>();
    }

    public PersistentEntity CreateEnemy(Coord location)
    {
        return CreateEnemy(location.x, location.y);
    }

    public PersistentEntity CreateEnemy(int x, int z)
    {
        Transform enemyPrefab = enemies[UnityEngine.Random.Range(0, enemies.Length - 1)];
        Transform enemy = Instantiate(enemyPrefab, new Vector3(x, 0.8f, z), Quaternion.identity);

        int enemyHealth = 5;
        int enemyDamage = 1;

        Color enemyColor = Color.red;
        Enemy newEnemy = enemy.GetComponent<Enemy>();
        newEnemy.SetUpEnemy(enemyHealth, enemyDamage, enemyColor, 1, 2);
        //newEnemy.mOnDeathHandler +=

        return newEnemy;
    }

    public void DespawnObject(Transform despawn)
    {
        DestructibleObstacle Obstacle = despawn.GetComponent<DestructibleObstacle>();
        if (Obstacle != null)
        {
            GraphUpdateObject GUO = new GraphUpdateObject();
            GUO.bounds = new Bounds(
                new Vector3(despawn.transform.position.x, 0.0f, despawn.transform.position.z), 
                new Vector3(1.4f, 2f, 1.4f));
            GUO.setWalkability = true;
            GUO.modifyWalkability = true;
            AstarPath.active.UpdateGraphs(GUO);
        }

        despawn.gameObject.SetActive(false);
    }
}
