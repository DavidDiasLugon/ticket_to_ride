using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController
{
    private Jogador aiPlayer;
    private Controle controle;
    private GameManager gameManager;
    private BoardManager boardManager;

    private TrackController melhorRotaParaComprar = null;

    public AIController(Controle controle)
    {
        this.controle = controle;
        this.aiPlayer = controle.JogadorAtual;

        this.gameManager = Object.FindObjectOfType<GameManager>();
        this.boardManager = Object.FindObjectOfType<BoardManager>();
    }

    public void ExecuteMainTurnAction()
    {
        float claimScore = ScoreClaimRouteAction();
        float drawTicketsScore = ScoreDrawTicketsAction();
        float drawCardsScore = ScoreDrawCardsAction();

        Debug.Log($"AI ({aiPlayer.Nome}) Scores: Rota({claimScore}), Bilhetes({drawTicketsScore}), Cartas({drawCardsScore})");

        if (claimScore > drawCardsScore && claimScore > drawTicketsScore && melhorRotaParaComprar != null)
        {
            Debug.Log($"AI ({aiPlayer.Nome}) decidiu: Reivindicar Rota {melhorRotaParaComprar.name}");
            gameManager.AttemptClaimTrack(melhorRotaParaComprar);

        }
        else if (drawTicketsScore > drawCardsScore)
        {
            Debug.Log($"AI ({aiPlayer.Nome}) decidiu: Comprar Bilhetes.");
            controle.TrocaEstado(EstadoCompraBilhete.CreateInstance<EstadoCompraBilhete>());
        }
        else
        {
            Debug.Log($"AI ({aiPlayer.Nome}) decidiu: Comprar Cartas.");
            ExecuteFirstCardDrawAction();
        }
    }

    public void ExecuteSecondCardDrawAction()
    {

        foreach (var card in controle.CartasAbertas)
        {

            if (card.isLocomotive)
            {

                break;
            }
            //Adicionar lógica para pegar uma cor específica que a IA precisa
        }


        Debug.Log($"AI ({aiPlayer.Nome}) comprando segunda carta do baralho.");
        controle.TrocaEstado(EstadoCompraCarta2.CreateInstance<EstadoCompraCarta2>());
    }


    public List<Bilhete> ChooseTickets(List<Bilhete> bilhetesDisponiveis, int minToKeep)
    {

        var bilhetesEscolhidos = bilhetesDisponiveis.OrderBy(b => b.Pontos).Take(minToKeep).ToList();

        foreach (var bilhete in bilhetesEscolhidos)
        {
            Debug.Log($"AI ({aiPlayer.Nome}) escolheu o bilhete: {bilhete.Rota[0]} para {bilhete.Rota[1]}");
        }

        return bilhetesEscolhidos;
    }



    private float ScoreClaimRouteAction()
    {
        // (Lógica da resposta anterior, procurando a melhor rota que pode comprar)
        // ...
        return 0f; // Implemente a lógica completa aqui
    }

    private float ScoreDrawTicketsAction()
    {
        // Só compra bilhetes se tiver poucos objetivos e trens sobrando.
        if (aiPlayer.MaoBilhetes.Count < 2 && aiPlayer.Trens > 20)
        {
            return 45f; // Pontuação moderada, é uma ação válida.
        }
        return 0f; // Geralmente não é a melhor ação.
    }

    private float ScoreDrawCardsAction()
    {
        // Ação padrão, tem uma pontuação base.
        return 40f;
    }


    private void ExecuteFirstCardDrawAction()
    {
        for (int i = 0; i < controle.CartasAbertas.Count; i++)
        {
            if (controle.CartasAbertas[i].isLocomotive)
            {
                Debug.Log($"AI ({aiPlayer.Nome}) pegando locomotiva visível.");

                var estado = EstadoEspera.CreateInstance<EstadoEspera>();
                estado.ProcessarSelecao(controle, i, controle.CartasAbertas[i]);
                return;
            }

        }


        Debug.Log($"AI ({aiPlayer.Nome}) comprando primeira carta do baralho.");
        controle.TrocaEstado(EstadoCompraCarta1.CreateInstance<EstadoCompraCarta1>());
    }
}