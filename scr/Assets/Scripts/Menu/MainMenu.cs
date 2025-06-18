using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public List<GameObject> playerComponents;
    public GameObject SceneLoader;
    public void PlayGame()
    {
        SceneLoader.GetComponent<SceneLoader>().StartCoroutine("Load");
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
        Jogador jogador1 = ScriptableObject.CreateInstance<Jogador>();
        jogador1.Nome = "PLAYER 1";
        jogador1.Cor = "azul";
        jogador1.UpdateNumeroCartasDict();
        GameSettings.jogadores.Add(jogador1);
        foreach (GameObject player in playerComponents)
        {
            Toggle toggle = player.GetComponentInChildren<Toggle>();
            if (toggle.isOn)
            {
                TMP_Dropdown dropdown = player.GetComponentInChildren<TMP_Dropdown>();
                string opcaoSelecionada = dropdown.options[dropdown.value].text;
                if (opcaoSelecionada == "REAL")
                {
                    TextMeshProUGUI nome = player.GetComponentInChildren<TextMeshProUGUI>();
                    Jogador jogador = ScriptableObject.CreateInstance<Jogador>();
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
                    TextMeshProUGUI nomeComponent = player.GetComponentInChildren<TextMeshProUGUI>();
                    Jogador jogadorIA = ScriptableObject.CreateInstance<Jogador>();
                    jogadorIA.Nome = nomeComponent.text + " (IA)";


                    if (nomeComponent.text == "PLAYER 2") jogadorIA.Cor = "verde";
                    else if (nomeComponent.text == "PLAYER 3") jogadorIA.Cor = "rosa";
                    else if (nomeComponent.text == "PLAYER 4") jogadorIA.Cor = "vermelho";
                    else jogadorIA.Cor = "amarelo";


                    jogadorIA.isAI = true;

                    GameSettings.jogadores.Add(jogadorIA);
                    Debug.Log($"Jogador {jogadorIA.Nome} criado como IA.");
                }
            }
        }
        PlayGame();
    }


}
