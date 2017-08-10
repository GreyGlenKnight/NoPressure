using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    private void Start()
    {
        Cursor.visible = true;
    }

    public void Play()
    {
        Debug.Log("PlayGame");
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
