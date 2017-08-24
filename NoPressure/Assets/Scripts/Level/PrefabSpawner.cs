using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public class PrefabSpawner : MonoBehaviour {

    public Transform MovingUnitsHolder;
    public Transform BoardHolder;

    private List<Transform> NoSpawnTilesList; //TODO add in sector logic

    public Transform CratePrefab;
    public Transform[] enemies; 

    private List<Transform> movingEntities = new List<Transform>();

    private Action<Transform> mSaveLocationOnMap;
    public List<SectorManager> mSectors = new List<SectorManager>();
    private TileSpawner m_TileSpawner;
    private ItemManager itemManager;

    private void Awake()
    {
        NoSpawnTilesList = new List<Transform>();

    }

    public void Init(Action<Transform> lSaveLocationOnMap)
    {
        mSaveLocationOnMap = lSaveLocationOnMap;
        m_TileSpawner = GetComponent<TileSpawner>();
        if (m_TileSpawner == null)
            Debug.LogError("Can not find Floor Tile Spawner");

        itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();

    }


    public void FindPowerNetworks()
    {
        for(int i = 0;i< mSectors.Count; i++)
        {
            mSectors[i].FindWires();
        }

        for (int j = 0; j < mSectors.Count; j++)
        {
            mSectors[j].RemovePowerNetworks();
        }

        for (int j = 0; j < mSectors.Count; j++)
        {
            mSectors[j].AssignPowerNetworks();
        }

    }

    public void FindPowerNetworks(int i)
    {
        mSectors[i].FindWires();
        mSectors[i].AssignPowerNetworks();
        //mSectors[i].SpreadPressure();
        //mSectors[i].ResetPressure();
    }

    public void GeneratePressureUpkeep(int i)
    {
        mSectors[i].SpreadPressure();
        mSectors[i].ResetPressure();
        //mSectors[i].SpreadPressure();
        //mSectors[i].ResetPressure();
    }

    public void GeneratePressureUpkeep()
    {
        for(int i = 0;i< mSectors.Count;i++)
        {
            mSectors[i].SpreadPressure();
        }
        for (int i = 0; i < mSectors.Count; i++)
        {
            mSectors[i].ResetPressure();
        }
    }

    public void SpawnSector(MapSector SpawnSector )
    {
        if (SpawnSector.pIsLoaded == true)
            return;

        Coord SectorLocation = SpawnSector.mSectorLocation;
        GameObject SectorContainer = GameObject.Find("Sector" + SectorLocation.ToString("_"));

        GameObject SectorTileContainer = GameObject.Find("SectorTiles" + SectorLocation.ToString("_"));

        if (SectorContainer == null)
        {
            if (BoardHolder == null)
                BoardHolder = GameObject.Find("BoardHolder").transform;

            // Create Container
            SectorContainer = Instantiate(BoardHolder.gameObject);
            SectorContainer.name = "Sector" + SectorLocation.ToString("_"); 
        }

        if (SectorTileContainer == null)
        {
            if (BoardHolder == null)
                BoardHolder = GameObject.Find("BoardHolder").transform;

            // Create Container
            SectorTileContainer = Instantiate(BoardHolder.gameObject);
            SectorTileContainer.name = "SectorTiles" + SectorLocation.ToString("_");
        }

        // Spawn Tiles for the sector
        //mTileCreator.SpawnTilesForSector(SpawnSector);
        FloorTile[,] l_FloorTiles = new FloorTile[10, 10];

        for (int i = 0; i< 10; i++)
        {
            for (int j = 0; j<10; j++)
            {

                // Spawn entity layer
                PersistentEntity entity;
                SpawnType spawnType = SpawnSector.GetSpawnAt(i, j);

                entity = SpawnPrefab(spawnType, new Coord(SectorLocation.x*10 + i, SectorLocation.y*10 + j));
                
                if (entity != null)
                {
                    entity.spawnType = spawnType;
                    entity.mOnDeathHandler += DespawnObject;

                    GeneratePressure generatePressure = entity.GetComponent<GeneratePressure>();



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

                // Spawn Tile Layer
                
                FloorTile tile;
                MapTile tileData = SpawnSector.getTileAt(new Coord(i, j));
                FloorTileType tileType = tileData.m_TileType; //pawnSector.GetFloorTileAt(i, j);

                tile = m_TileSpawner.SpawnTile(tileData, new Coord(SectorLocation.x * 10 + i, SectorLocation.y * 10 + j));

                //tile.TileData = tileData;
                l_FloorTiles[i, j] = tile;
                tile.transform.SetParent(SectorContainer.transform);

            }
        }

        SpawnSector.pIsLoaded = true;

        SectorManager sectorManager = new SectorManager(l_FloorTiles, SpawnSector);
        mSectors.Add(sectorManager);

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

        int DespawnSectorIndex = -1 ;

        for (int i = 0; i < mSectors.Count; i++)
        {
            if (mSectors[i].SectorData == SpawnSector)
            {
                DespawnSectorIndex = i;

                break;
            }
        }

        if (DespawnSectorIndex == -1)
            Debug.LogError("Could not find sector to despawn");

        mSectors[DespawnSectorIndex].DespawnSectorTiles(SpawnSector);

        GameObject SectorContainer = GameObject.Find("Sector" + SectorLocation.ToString("_"));

        List<PersistentEntity> childList = new List<PersistentEntity>();

        if (SectorContainer == null)
            return;

        for (int i = 0; i< SectorContainer.transform.childCount; i++)
        {
            PersistentEntity entity = SectorContainer.transform.GetChild(i).GetComponent<PersistentEntity>();
            if (entity != null)
                childList.Add(entity);
        }

        for (int i = 0; i< childList.Count; i++ )
        {
            // Destroy all the children... Damn... thats brutal.
            childList[i].Despawn();
            childList[i].gameObject.SetActive(false);
            Destroy(childList[i].gameObject);
        }

        SpawnSector.pIsLoaded = false;
        mSectors.RemoveAt(DespawnSectorIndex);
    }


    public PersistentEntity SpawnPrefab(SpawnType itemTypeToSpawn, Coord SpawnLocation)
    {
        switch (itemTypeToSpawn)
        {
            case SpawnType.None:
                return null;
            case SpawnType.Wall:
                return CreateWall(SpawnLocation);
            case SpawnType.Enemy:
                return CreateEnemy(SpawnLocation);
            case SpawnType.Obstacle:
                return CreateObstacle(SpawnLocation);
            case SpawnType.ForceField:
                return CreateForceField(SpawnLocation);
            case SpawnType.Pistol:
                return CreateCrate(SpawnLocation, SpawnType.Pistol);
            case SpawnType.Rifle:
                return CreateCrate(SpawnLocation, SpawnType.Rifle);
            case SpawnType.Carbine:
                return CreateCrate(SpawnLocation, SpawnType.Carbine);

            case SpawnType.PlasmaThrower:
                return CreateCrate(SpawnLocation, SpawnType.PlasmaThrower);
            case SpawnType.RocketLauncher:
                return CreateCrate(SpawnLocation, SpawnType.RocketLauncher);

            case SpawnType.PortableGenerator:
                return CreateCrate(SpawnLocation, SpawnType.PortableGenerator);
            case SpawnType.PortablePressure:
                return CreateCrate(SpawnLocation, SpawnType.PortablePressure);

            case SpawnType.Shield:
                return CreateCrate(SpawnLocation, SpawnType.Shield);
            case SpawnType.Mine:
                return CreateCrate(SpawnLocation, SpawnType.Mine);
            case SpawnType.MecanicalTools:
                return CreateCrate(SpawnLocation, SpawnType.MecanicalTools);
            case SpawnType.ElectricalTools:
                return CreateCrate(SpawnLocation, SpawnType.ElectricalTools);



            case SpawnType.PressureStation:
                return CreatePressureCube(SpawnLocation);
            case SpawnType.PowerCube:
                return CreatePowerCube(SpawnLocation);
            case SpawnType.BrokenPressureStation:
                return null; //CreateBrokenPressureStationTile(SpawnLocation);

            default:
                Debug.Log("Invalid item spawn number: " + itemTypeToSpawn);
                return null;
        }
    }

    public void RemoveCrateFromList(Transform CrateToRemove)
    {
        NoSpawnTilesList.Remove(CrateToRemove);
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
        IInventoryItem itemToSpawn = itemManager.SpawnItem(contents);

        if (itemToSpawn == null)
        {
            Debug.Log("can not find item type: " + contents);
            return null;
        }
        return CreateCrate(x, z, itemManager.SpawnItem(contents));
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

    public PersistentEntity CreateForceField(Coord location)
    {
        return CreateForceField(location.x, location.y);
    }

    public PersistentEntity CreateForceField(int x, int z)
    {
        //TileInfo obstacleTileInfo = roomSlots.Dequeue();
        //obstacleTileInfo.prefabType = PrefabType.Obstacle;
        Transform obstaclePrefab = GameObject.Find("ForceField").transform;
        //Transform obstaclePrefab = obstacleTiles[Random.Range(0, obstacleTiles.Length - 1)];
        Transform newObstacle = Instantiate(obstaclePrefab, new Vector3(x, 1, z), Quaternion.identity);
        NoSpawnTilesList.Add(newObstacle);

        //GraphUpdateObject GUO = new GraphUpdateObject();
        //GUO.bounds = new Bounds(new Vector3(x, 0.0f, z), new Vector3(1.4f, 2f, 1.4f));
        //GUO.setWalkability = false;
        //GUO.modifyWalkability = true;


        //AstarPath.active.UpdateGraphs(GUO);

        PersistentEntity returnVal = newObstacle.GetComponent<DestructibleObstacle>();

        return returnVal;
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


    public PersistentEntity CreatePressureCube(Coord location)
    {
        return CreatePressureCube(location.x, location.y);
    }

    public PersistentEntity CreatePressureCube(int x, int z)
    {
        Transform pressurePrefab = GameObject.Find("PressureCube").transform;
        Transform pressureCube = Instantiate(pressurePrefab, new Vector3(x, 1, z), Quaternion.identity);

        GraphUpdateObject GUO = new GraphUpdateObject();
        GUO.bounds = new Bounds(new Vector3(x, 0.0f, z), new Vector3(1.4f, 2f, 1.4f));
        GUO.setWalkability = false;
        GUO.modifyWalkability = true;

        if (AstarPath.active == null)
            AstarPath.active = GameObject.Find("_A*").GetComponent<AstarPath>();
        AstarPath.active.UpdateGraphs(GUO);

        NoSpawnTilesList.Add(pressureCube);

        return pressureCube.GetComponent<DestructibleObstacle>();
    }

    public PersistentEntity CreatePowerCube(Coord location)
    {
        return CreatePowerCube(location.x, location.y);
    }

    public PersistentEntity CreatePowerCube(int x, int z)
    {
        Transform pressurePrefab = GameObject.Find("PowerCube").transform;
        Transform pressureCube = Instantiate(pressurePrefab, new Vector3(x, 1, z), Quaternion.identity);

        GraphUpdateObject GUO = new GraphUpdateObject();
        GUO.bounds = new Bounds(new Vector3(x, 0.0f, z), new Vector3(1.4f, 2f, 1.4f));
        GUO.setWalkability = false;
        GUO.modifyWalkability = true;

        if (AstarPath.active == null)
            AstarPath.active = GameObject.Find("_A*").GetComponent<AstarPath>();
        AstarPath.active.UpdateGraphs(GUO);

        NoSpawnTilesList.Add(pressureCube);

        return pressureCube.GetComponent<DestructibleObstacle>();
    }
}
