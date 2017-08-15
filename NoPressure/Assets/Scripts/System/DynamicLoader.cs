using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicLoader : MonoBehaviour {

    static DynamicLoader instance;

    PathfindingManager pathfindingManager;
    MapController mapController;
    public bool Rest = false;

    private void Awake()
    {
        // Ensure the instance is of the type GameManager
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        // Persist the GameManager instance across scenes
        DontDestroyOnLoad(gameObject);

    }

    public static DynamicLoader getDynamicLoader()
    {
        if (instance == null)
        {
            Debug.Log("Trying to access Dynamic loader before it loaded");
            return null;
        }

        return instance;
    }

    // Use this for initialization
    void Start () {
        pathfindingManager = PathfindingManager.getPathfindingManager();
        mapController = MapController.GetMapController();
    }
	
	// Update is called once per frame
	void Update () {

        if (Rest == true)
        {
            Rest = false;
            return;
        }

        if(mapController.LoadNextSector() == true)
        {
            //Debug.Log("Spawning");
            return;
        }

        if (pathfindingManager.ScanNextQueue() == true)
        {
            //Debug.Log("Scanning");
            return;
        }

        if (mapController.DespawnNextSector() == true)
        {
            //Debug.Log("despawning");
            return;
        }
    }



}
