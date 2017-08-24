using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Coord pStartLocation = new Coord(55,100);
    public string levelName = "Demo";

    private TheDynamicLoader gDynamicLoader;

    public float deathDelay = 3f;
    private float deathCounter = 0f;

    private void Start()
    {
        gDynamicLoader = TheDynamicLoader.getDynamicLoader();
        gDynamicLoader.ClearQueue();

        // Set the camera on the control manager to allow manual control of the camera
        // or assign camera to have a different focus
        // ^ Not implemented currently
        CameraController cameraController = GetComponent<CameraController>();
        ControlsManager controlsManager = GetComponent<ControlsManager>();
        controlsManager.SetCameraController(cameraController);

        // Assign a Game over condition to the player that is called if the player dies
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.mOnDeathHandler += GameOver;

        // Load the map
        PrefabSpawner prefabSpawner = GetComponent<PrefabSpawner>();
        MapController mapContoller =GetComponent<MapController>();
        mapContoller.Init(WorldSpaceUnit.Tile, pStartLocation, prefabSpawner);
    }

    private void Update()
    {
        deathCounter += Time.deltaTime;
        gDynamicLoader.DoNextAction();
    }

    void GameOver(Transform player)
    {
        deathCounter = 0f;
        player.gameObject.SetActive(false);
        gDynamicLoader.AddActionToQueue(GameOver,Priority.Low);
    }

    // What happens if there is a game over event
    void GameOver()
    {
        if (deathCounter > deathDelay)
        {
            // Non-mono objects retain their state, make sure that relevent data is reset
            GetComponent<MapController>().ResetLevel();

            // Load the loading scene
            SceneManager.LoadScene(1);
        }
        else
            gDynamicLoader.AddActionToQueue(GameOver, Priority.Low);
    }
}
