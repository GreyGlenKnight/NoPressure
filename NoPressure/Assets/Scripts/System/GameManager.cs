using UnityEngine;
using UnityEngine.SceneManagement;
using Pathfinding;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    private BoardManager boardScript;
    private Player player;

    private ControlsManager controlsManager;
    private CameraController cameraController;

    public int level = 0;
    public bool loading = true;

    public float playerHealth;
    public float playerXP;

    private bool isLoaded = false;


    private void Awake()
    {
        Debug.Log("Awake");
        // Ensure the instance is of the type GameManager
        //if (instance == null)
            instance = this;
        //else if (instance != this)
        //{
        //    Debug.Log("SelfDestroy");
        //    Destroy(gameObject);
        //}
        //// Persist the GameManager instance across scenes
        //DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();
        controlsManager = GetComponent<ControlsManager>();
        cameraController = GetComponent<CameraController>();
    }

    private void Start()
    {
        Debug.Log("Start");
        //LoadLevel();


    }

    private void LoadLevel()
    {

        controlsManager.SetCameraController(cameraController);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        //playerHealth = 10;
        player.mOnDeathHandler += GameOver;

        Debug.Log("Game Start 15");

        PathfindingManager pathfindingmanager = PathfindingManager.getPathfindingManager();
        //pathfindingmanager.Init(new Coord(1, 2));

        boardScript.SetUpLevel(BoardManager.LevelType.FromFileMap, new Coord(65, 115));
    }

    private void OnUpdate()
    {
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 2)
        {
            Debug.Log("Loading not Game scene");
            return;
        }
        Debug.Log("Scene Loaded");
        LoadLevel();
    }

    //void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    //{
    //    if (scene.name == "Game")
    //    {
    //        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    //        level++;
    //        loading = true;


    //        Debug.Log("Scene");
    //        // TODO: show a loading banner here
    //        //boardScript.SetUpLevel(BoardManager.LevelType.DebugOpenArea);
    //        //boardScript.SetUpLevel(BoardManager.LevelType.FromFileMap, new Coord(1,2));
    //    }
    //}


    private void OnEnable()
    {
        //SceneManager.sceneLoaded += OnLevelFinishedLoading;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void Restart()
    {
        // compute new health as fraction of xp
        float currentHealth = player.mHealth;
        //float currentXP = player.mExperience;

        //float remainingXP = currentXP % 5;
        //float healthGain = currentXP / 5;

        // store player stats
        //playerHealth = currentHealth + healthGain;
        //playerXP = remainingXP;

        boardScript.DestroyLevel();
        SceneManager.LoadScene("Loading");
    }

    void GameOver()
    {
        isLoaded = false;
        Debug.Log("Reloading game");
        MapController.GetMapController().ResetLevel();
        SceneManager.LoadScene(1);
        //SceneManager.LoadSceneAsync("Game");
    }


}
