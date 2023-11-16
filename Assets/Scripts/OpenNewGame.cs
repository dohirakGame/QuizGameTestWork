using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenNewGame : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("Game");
    }
}


