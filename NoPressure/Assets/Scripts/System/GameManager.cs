using UnityEngine;
using UnityEngine.SceneManagement;
using Pathfinding;

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

        boardScript = GetComponent<BoardManager>();
        controlsManager = GetComponent<ControlsManager>();
        cameraController = GetComponent<CameraController>();
        controlsManager.SetCameraController(cameraController);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerHealth = 10;
        player.mOnDeathHandler += GameOver;
    }

    private void Start()
    {
        Debug.Log("Game Start 5");
        //AstarPath aStarPath = GameObject.Find("_A*").GetComponent<AstarPath>();
        //AstarPath.active.Scan(aStarPath.graphs[0]);

        //GridGraph gridGraph = new GridGraph();
        //gridGraph.SetDimensions(40,40,1);

        //NavGraph navGraph;

        //navGraph = aStarPath.data.CreateGraph(gridGraph.GetType());

        //gridGraph = (GridGraph) aStarPath.data.AddGraph(gridGraph.GetType());

        //gridGraph.SetDimensions(40, 40,1);
        //gridGraph.center = new Vector3 (39.5f,-0.1f, 39.5f);

        //GridGraph moveGraph = (GridGraph)aStarPath.graphs[1];
        //moveGraph.center = new Vector3(59.5f, -0.1f, 139.5f); 

        //AstarPath.active.Scan(aStarPath.graphs[1]);

        PathfindingManager pathfindingmanager = GetComponent<PathfindingManager>();
        pathfindingmanager.Init(new Coord(1,2));


        boardScript.SetUpLevel(BoardManager.LevelType.FromFileMap, new Coord(65, 115)); 

    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            level++;
            loading = true;


            Debug.Log("Scene");
            // TODO: show a loading banner here
            //boardScript.SetUpLevel(BoardManager.LevelType.DebugOpenArea);
            //boardScript.SetUpLevel(BoardManager.LevelType.FromFileMap, new Coord(1,2));
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
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
        Debug.Log("Reloading game");
        SceneManager.LoadScene("Game");
    }
}
