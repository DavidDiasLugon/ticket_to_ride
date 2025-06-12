using UnityEngine;

public class EstadoCompraCarta1 : EstadoJogo
{
    public override void IniciarEstado(Controle controle)
    {
        FindAnyObjectByType<AudioManager>().Play("DrawCard");
        Debug.Log("Comprando primeira carta");
        Carta c = controle.DeckCartas.CompraCarta();
        controle.JogadorAtual.MaoCartas.Add(c);
        controle.JogadorAtual.UpdateNumeroCartasDict();
        _GameManager.Instance.maoCartas.AtualizaExibicao(controle.JogadorAtual.CartaNmr);
        controle.TrocaEstado(EstadoEspera2.CreateInstance<EstadoEspera2>());
    }

    public override void RunEstado(Controle controle)
    {

    }
    
    public override void ProcessarSelecao(Controle controle, int indice, Carta cartaSelecionada)
    {
    }
}
