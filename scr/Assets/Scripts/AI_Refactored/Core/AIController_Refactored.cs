using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Versão refatorada do AIController.
/// Contém apenas a lógica de decisão, sem efeitos colaterais (sem esperas, sem alterar a UI).
/// Recebe suas dependências (dados do jogador e do tabuleiro) via interfaces.
/// </summary>
public class AIController_Refactored
{
    private readonly IPlayerData aiPlayer;
    private readonly IBoardStateProvider board;

    private TrackController melhorRotaParaComprar = null;

    // A IA recebe suas dependências no construtor (Injeção de Dependência).
    public AIController_Refactored(IPlayerData player, IBoardStateProvider boardState)
    {
        this.aiPlayer = player;
        this.board = boardState;
    }

    /// <summary>
    /// Método principal de decisão. Avalia as opções e retorna a melhor como um AIAction.
    /// </summary>
    public AIAction DecideAcaoPrincipal()
    {
        float claimScore = ScoreClaimRouteAction();
        float drawTicketsScore = ScoreDrawTicketsAction();
        float drawCardsScore = ScoreDrawCardsAction();

        if (claimScore > drawCardsScore && claimScore > drawTicketsScore && melhorRotaParaComprar != null)
        {
            return new AIAction { Type = AIAction.ActionType.ClaimRoute, Data = melhorRotaParaComprar };
        }

        if (drawTicketsScore > drawCardsScore)
        {
            return new AIAction { Type = AIAction.ActionType.DrawTickets };
        }

        return new AIAction { Type = AIAction.ActionType.DrawCards };
    }

    /// <summary>
    /// Avalia e pontua a ação de conquistar uma rota.
    /// Público para permitir testes diretos.
    /// </summary>
    public float ScoreClaimRouteAction()
    {
        melhorRotaParaComprar = null;
        Bilhete objetivo = aiPlayer.BilhetesNaMao.Where(b => !b.Concluido).OrderBy(b => b.Pontos).FirstOrDefault();
        if (objetivo == null) return 0f;

        foreach (var rotaController in board.GetAvailableTracks())
        {
            TrackData dadosDaRota = rotaController.trackData;
            bool rotaUtil = objetivo.Rota.Contains(dadosDaRota.city1Name) || objetivo.Rota.Contains(dadosDaRota.city2Name);

            if (rotaUtil && PodeConquistar(dadosDaRota))
            {
                melhorRotaParaComprar = rotaController;
                return 50f;
            }
        }
        return 0f;
    }

    /// <summary>
    /// Avalia e pontua a ação de comprar novos bilhetes.
    /// </summary>
    public float ScoreDrawTicketsAction()
    {
        if (aiPlayer.BilhetesNaMao.Count() < 2 && aiPlayer.Trens > 20)
        {
            return 45f;
        }
        return 0f;
    }

    /// <summary>
    /// Retorna a pontuação base para a ação de comprar cartas.
    /// </summary>
    public float ScoreDrawCardsAction()
    {
        return 40f;
    }

    /// <summary>
    /// Decide quais bilhetes manter de uma lista.
    /// Retorna um AIAction com a lista de bilhetes escolhidos.
    /// </summary>
    public AIAction DecideQuaisBilhetesManter(List<Bilhete> bilhetesDisponiveis, int minToKeep)
    {
        var bilhetesEscolhidos = bilhetesDisponiveis.OrderBy(b => b.Pontos).Take(minToKeep).ToList();
        return new AIAction { Type = AIAction.ActionType.ChooseTickets, Data = bilhetesEscolhidos };
    }

    /// <summary>
    /// Verifica se o jogador tem cartas suficientes para conquistar uma rota.
    /// </summary>
    public bool PodeConquistar(TrackData rota)
    {
        int custo = rota.length;
        int locomotivas = aiPlayer.CartasNaMao.ContainsKey("colorido") ? aiPlayer.CartasNaMao["colorido"] : 0;

        if (rota.color == TrackColor.Gray)
        {
            foreach (var par in aiPlayer.CartasNaMao)
            {
                if (par.Key != "colorido" && par.Value + locomotivas >= custo) return true;
            }
            return locomotivas >= custo;
        }
        else
        {
            string cor = ConverteTrackColorParaString(rota.color);
            int cartasDaCor = aiPlayer.CartasNaMao.ContainsKey(cor) ? aiPlayer.CartasNaMao[cor] : 0;
            return cartasDaCor + locomotivas >= custo;
        }
    }

    // Método de apoio estático, não depende de estado.
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
}