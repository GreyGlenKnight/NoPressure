using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TileType
{
    Blank,
    WiredButInactive,
    WiredAndActive,

}

public class FloorMap : MonoBehaviour {

    // Singleton for keeping track of the floor
    private static FloorMap instance;

    Transform[][] FloorTileObjects;
    TileType[][] FloorInfo;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("SelfDestroy");
            Destroy(gameObject);
        }
        // Persist the GameManager instance across scenes
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
