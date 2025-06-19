using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EstadoFimJogo : EstadoJogo
{
    public GameObject SceneLoader;
    public override void IniciarEstado(Controle controle)
    {
        SceneLoader = GameObject.Find("LevelLoader");
        Debug.Log("Fim do jogo, calculando rota mais longa");
        int maiorRota = 0;
        List<Jogador> jogadoresComMaiorRota = new List<Jogador>();
        foreach (Jogador jogador in controle.Jogadores)
        {
            int rotaAtual = VerificadorDeRotas.MaiorCaminhoContinuo(BoardManager.AllTrackControllers, jogador.Nome);
            if (rotaAtual > maiorRota)
            {
                maiorRota = rotaAtual;
            }
        }

        foreach (Jogador jogador in controle.Jogadores)
        {
            int rotaAtual = VerificadorDeRotas.MaiorCaminhoContinuo(BoardManager.AllTrackControllers, jogador.Nome);
            if (rotaAtual == maiorRota && maiorRota > 0)
            {
                jogadoresComMaiorRota.Add(jogador);
            }
        }
        DadosFimJogo.jogadores = controle.Jogadores;
        DadosFimJogo.jogadoresComMaiorCaminho = jogadoresComMaiorRota;
        SceneLoader.GetComponent<SceneLoader>().StartCoroutine("LoadEndGame");
    }

    public override void RunEstado(Controle controle)
    {

    }

    public override void ProcessarSelecao(Controle controle, int indice, Carta cartaSelecionada)
    {
    }
}
