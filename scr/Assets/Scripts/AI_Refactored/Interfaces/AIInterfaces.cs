using System.Collections.Generic;

/// <summary>
/// Descreve um objeto que pode fornecer dados sobre o estado do tabuleiro.
/// Usado pela IA para ver as rotas disponíveis sem conhecer o BoardManager.
/// </summary>
public interface IBoardStateProvider
{
    IEnumerable<TrackController> GetAvailableTracks();
}

/// <summary>
/// Descreve um objeto que pode fornecer dados sobre os recursos de um jogador.
/// Usado pela IA para ver sua própria mão sem conhecer a classe 'Jogador' diretamente.
/// </summary>
public interface IPlayerData
{
    int Trens { get; }
    IReadOnlyDictionary<string, int> CartasNaMao { get; }
    IEnumerable<Bilhete> BilhetesNaMao { get; }
}

/// <summary>
/// Representa uma decisão tomada pela IA.
/// Isso separa a lógica de "pensar" da lógica de "executar".
/// O AIController_Refactored cria e retorna esta classe, e o 'Controle' a interpreta.
/// </summary>
public class AIAction
{
    // Enum para os tipos de ações possíveis que a IA pode decidir.
    public enum ActionType
    {
        ClaimRoute,
        DrawTickets,
        DrawCards,
        DrawFirstCard,
        DrawSecondCard,
        ChooseTickets
    }

    public ActionType Type { get; set; }

    // 'object' genérico para guardar dados adicionais sobre a ação.
    // Ex: Se o Type for ClaimRoute, Data conterá o TrackController da rota.
    // Ex: Se o Type for ChooseTickets, Data conterá a List<Bilhete> escolhida.
    public object Data { get; set; }
}