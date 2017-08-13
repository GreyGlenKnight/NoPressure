using UnityEngine;
using UnityEngine.SceneManagement;


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
        Debug.Log("Game Start 3");
        boardScript.SetUpLevel(BoardManager.LevelType.FromFileMap, new Coord(53, 103));
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
