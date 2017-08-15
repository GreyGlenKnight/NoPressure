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
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
