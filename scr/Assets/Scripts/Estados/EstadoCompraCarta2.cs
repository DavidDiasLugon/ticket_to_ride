using UnityEngine;

public class EstadoCompraCarta2 : EstadoJogo
{
    public override void IniciarEstado(Controle controle)
    {
        FindAnyObjectByType<GameAudioManager>().Play("DrawCard");
        Carta c = controle.DeckCartas.CompraCarta();
        controle.JogadorAtual.MaoCartas.Add(c);
        controle.JogadorAtual.UpdateNumeroCartasDict();
        _GameManager.Instance.maoCartas.AtualizaExibicao(controle.JogadorAtual.CartaNmr);
        controle.TrocaEstado(EstadoFimTurno.CreateInstance<EstadoFimTurno>());

    }

    public override void RunEstado(Controle controle)
    {

    }
    
    public override void ProcessarSelecao(Controle controle, int indice, Carta cartaSelecionada)
    {
    }
}
