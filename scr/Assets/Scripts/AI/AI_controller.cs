using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

        //this.gameManager = Object.FindObjectOfType<GameManager>();
        //this.boardManager = Object.FindObjectOfType<BoardManager>();
    }

    public IEnumerator ExecuteMainTurnAction()
    {
        TextMeshProUGUI anuncio = _GameManager.Instance.anuncios;
        anuncio.gameObject.SetActive(true);

        float claimScore = ScoreClaimRouteAction();
        float drawTicketsScore = ScoreDrawTicketsAction();
        float drawCardsScore = ScoreDrawCardsAction();

        Debug.Log($"AI ({aiPlayer.Nome}) Scores: Rota({claimScore}), Bilhetes({drawTicketsScore}), Cartas({drawCardsScore})");

        if (claimScore > drawCardsScore && claimScore > drawTicketsScore && melhorRotaParaComprar != null)
        {
            Debug.Log($"AI ({aiPlayer.Nome}) decidiu: Reivindicar Rota {melhorRotaParaComprar.name}");
            anuncio.text = $"IA vai conquistar a rota de {melhorRotaParaComprar.trackData.city1Name} para {melhorRotaParaComprar.trackData.city2Name}";
            yield return new WaitForSeconds(2.0f);
            controle.ProcessarAcaoAIConquistaRota(melhorRotaParaComprar);

        }
        else if (drawTicketsScore > drawCardsScore)
        {
            Debug.Log($"AI ({aiPlayer.Nome}) decidiu: Comprar Bilhetes.");
            anuncio.text = "IA vai comprar novos bilhetes de destino!";
            yield return new WaitForSeconds(2.0f);
            controle.ProcessarAcaoAIComprabilhete();
        }
        else
        {
            Debug.Log($"AI ({aiPlayer.Nome}) decidiu: Comprar Cartas.");
            anuncio.text = "IA vai comprar cartas de vagão";
            yield return new WaitForSeconds(2.0f);
            controle.ProcessarAcaoAICompraCarta();
        }
        yield return new WaitForSeconds(2.0f);
        anuncio.text = $"Fim do turno de {aiPlayer.Nome}";
        yield return new WaitForSeconds(2.0f);
        anuncio.gameObject.SetActive(false);
        controle.TrocaEstado(EstadoFimTurno.CreateInstance<EstadoFimTurno>());
    }

    public int ExecuteSecondCardDrawAction(Controle controle)
    {

        List<string> coresNecessarias = ObterCoresNecessarias(controle);
        for (int i = 0; i < controle.CartasAbertas.Count; i++)
        {
            Carta cartaAtual = controle.CartasAbertas[i];
            if (cartaAtual != null)
            {
                if (!cartaAtual.isLocomotive && coresNecessarias.Contains(cartaAtual.Cor))
                {
                    return i;
                }
            }
        }
        for (int i = 0; i < controle.CartasAbertas.Count; i++)
        {
            if (controle.CartasAbertas[i] != null && !controle.CartasAbertas[i].isLocomotive)
            {
                return i;
            }
        }
        return -1;
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
        melhorRotaParaComprar = null;
        Bilhete objetivo = aiPlayer.MaoBilhetes.Where(b => !b.Concluido).OrderBy(b => b.Pontos).FirstOrDefault();
        if (objetivo == null)
        {
            return 0f;
        }
        foreach (var rotaController in BoardManager.AllTrackControllers)
        {
            if (!rotaController.isClaimed)
            {
                TrackData dadosDaRota = rotaController.trackData;
                bool rotaUtil = objetivo.Rota.Contains(dadosDaRota.city1Name) || objetivo.Rota.Contains(dadosDaRota.city2Name);

                if (rotaUtil)
                {
                    if (PodeConquistar(aiPlayer, dadosDaRota))
                    {
                        melhorRotaParaComprar = rotaController;
                        return 50f;
                    }
                }
            }
        }
        return 0f;
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


    public int ExecuteFirstCardDrawAction(Controle controle)
    {
        for (int i = 0; i < controle.CartasAbertas.Count; i++)
        {
            if (controle.CartasAbertas[i] != null && controle.CartasAbertas[i].isLocomotive)
            {
                return i;
            }
        }

        List<string> coresNecessarias = ObterCoresNecessarias(controle);
        for (int i = 0; i < controle.CartasAbertas.Count; i++)
        {
            if (controle.CartasAbertas[i] != null)
            {
                if (coresNecessarias.Contains(controle.CartasAbertas[i].Cor))
                {
                    return i;
                }
            }
        }
        return -1;
    }

    public bool PodeConquistar(Jogador ia, TrackData rota)
    {
        int custo = rota.length;
        int locomotivas = ia.CartaNmr.ContainsKey("colorido") ? ia.CartaNmr["colorido"] : 0;
        if (rota.color == TrackColor.Gray)
        {
            foreach (var par in ia.CartaNmr)
            {
                if (par.Key != "colorido" && par.Value + locomotivas >= custo)
                {
                    return true;
                }
            }
            if (locomotivas >= custo)
            {
                return true;
            }
            return false;
        }
        else
        {
            string cor = ConverteTrackColorParaString(rota.color);
            int cartasDaCor = ia.CartaNmr.ContainsKey(cor) ? ia.CartaNmr[cor] : 0;
            return cartasDaCor + locomotivas >= custo;
        }
    }

    public static string ConverteTrackColorParaString(TrackColor cor)
    {
        switch (cor)
        {
            case TrackColor.Red: return "vermelho";
            case TrackColor.Blue: return "azul";
            case TrackColor.Green: return "verde";
            case TrackColor.Yellow: return "amarelo";
            case TrackColor.Black: return "preto";
            case TrackColor.White: return "branco";
            case TrackColor.Orange: return "laranja";
            case TrackColor.Pink: return "rosa";
            default: return "";
        }
    }

    public List<string> ObterCoresNecessarias(Controle controle)
    {
        var coresNecessarias = new List<string>();
        var jogadorIA = controle.JogadorAtual;

        foreach (var bilhete in jogadorIA.MaoBilhetes.Where(b => !b.Concluido))
        {
            foreach (var rota in BoardManager.AllTrackControllers)
            {
                if (!rota.isClaimed)
                {
                    if (bilhete.Rota.Contains(rota.trackData.city1Name) || bilhete.Rota.Contains(rota.trackData.city2Name))
                    {
                        if (rota.trackData.color != TrackColor.Gray)
                        {
                            string corDaRota = ConverteTrackColorParaString(rota.trackData.color);
                            if (!coresNecessarias.Contains(corDaRota))
                            {
                                coresNecessarias.Add(corDaRota);
                            }
                        }
                    }
                }
            }
        }
        return coresNecessarias;
    }

}