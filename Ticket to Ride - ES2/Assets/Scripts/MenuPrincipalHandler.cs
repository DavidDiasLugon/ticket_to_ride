using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalHandler : MonoBehaviour
{
    [SerializeField] private string nomeDaSceneJogo;
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelMenuOpcoes;
    [SerializeField] private GameObject painelMenuConfig;
    
    public void Jogar()
    {
        painelMenuInicial.SetActive(false);
        painelMenuConfig.SetActive(true);
    }

    public void Opcoes()
    {
        painelMenuInicial.SetActive(false);
        painelMenuOpcoes.SetActive(true);
    }

    public void SairOpcoes()
    {
        painelMenuOpcoes.SetActive(false);
        painelMenuInicial.SetActive(true);
    }

    public void SairConfig()
    {
        painelMenuConfig.SetActive(false);
        painelMenuInicial.SetActive(true);
    }

    public void IniciarJogo()
    {
        SceneManager.LoadScene(nomeDaSceneJogo);
    }

    public void SairJogo()
    {
        Debug.Log("Sair do Jogo");
        Application.Quit();
    }

}
