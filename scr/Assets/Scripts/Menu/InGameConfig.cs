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
        GameSettings.jogadores.Clear();
        GameSettings.vencedor = null;
        SceneManager.LoadScene("MainMenu");
    }

    public void ClickSound()
    {
        FindAnyObjectByType<AudioManager>().Play("Click");
    }
}
