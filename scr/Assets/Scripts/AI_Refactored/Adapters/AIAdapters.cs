using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Adaptador que implementa IPlayerData.
/// Ele "envolve" um objeto 'Jogador' real para fornecer os dados à IA.
/// </summary>
public class PlayerDataAdapter : IPlayerData
{
    private readonly Jogador jogadorReal;

    public PlayerDataAdapter(Jogador jogador)
    {
        this.jogadorReal = jogador;
    }

    public int Trens => jogadorReal.Trens;
    public IReadOnlyDictionary<string, int> CartasNaMao => jogadorReal.CartaNmr;
    public IEnumerable<Bilhete> BilhetesNaMao => jogadorReal.MaoBilhetes;
}

/// <summary>
/// Adaptador que implementa IBoardStateProvider.
/// Ele "conversa" com o Singleton BoardManager para obter os dados do tabuleiro.
/// </summary>
public class BoardStateAdapter : IBoardStateProvider
{
    public IEnumerable<TrackController> GetAvailableTracks()
    {
        // Acessa o Singleton estático do seu jogo para obter os dados reais.
        return BoardManager.AllTrackControllers.Where(t => !t.isClaimed);
    }
}