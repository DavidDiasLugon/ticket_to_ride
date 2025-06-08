using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameConfig : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }

    public void QuitToMainMenu()
    {
        Debug.Log("Voltando ao menu principal...");
        SceneManager.LoadScene("Menu");
    }
}
