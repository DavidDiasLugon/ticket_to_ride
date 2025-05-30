using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public List<GameObject> playerComponents;
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public void ButtonClick()
    {
        FindAnyObjectByType<AudioManager>().Play("Click");
    }

    public void StartSfx()
    {
        FindAnyObjectByType<AudioManager>().Play("Start");
    }

    public void ConfigurarJogo()
    {
        Jogador jogador1 = new Jogador();
        jogador1.Nome = "PLAYER 1";
        jogador1.Cor = "azul";
        GameSettings.jogadores.Add(jogador1);
        foreach (GameObject player in playerComponents)
        {
            Toggle toggle = player.GetComponentInChildren<Toggle>();
            if (toggle.isOn)
            {
                TMP_Dropdown dropdown = player.GetComponentInChildren<TMP_Dropdown>();
                string opcaoSelecionada = dropdown.options[dropdown.value].text;
                if (opcaoSelecionada == "Real")
                {
                    TextMeshProUGUI nome = player.GetComponentInChildren<TextMeshProUGUI>();
                    Jogador jogador = new Jogador();
                    jogador.Nome = nome.text;
                    if (jogador.Nome == "PLAYER 2")
                    {
                        jogador.Cor = "verde";
                    }
                    else if (jogador.Nome == "PLAYER 3")
                    {
                        jogador.Cor = "rosa";
                    }
                    else if (jogador.Nome == "PLAYER 4")
                    {
                        jogador.Cor = "vermelho";
                    }
                    else
                    {
                        jogador.Cor = "amarelo";
                    }
                    GameSettings.jogadores.Add(jogador);
                }
                else if (opcaoSelecionada == "IA")
                {
                    // Criação de jogador IA
                }
            }
        }
    }
    
    
}
