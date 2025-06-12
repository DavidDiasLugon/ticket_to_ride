using UnityEngine;
using UnityEngine.UI;

public class EstadoEspera2 : EstadoJogo
{
    private Button botaoCompraCarta;
    public override void IniciarEstado(Controle controle)
    {
        Debug.Log("Esperando compra da segunda carta");
        controle.JogadorAtual.UpdateNumeroCartasDict();
        _GameManager.Instance.maoCartas.AtualizaExibicao(controle.JogadorAtual.CartaNmr);
        botaoCompraCarta = GameObject.Find("BotaoCarta")?.GetComponent<Button>();

        botaoCompraCarta.onClick.AddListener(() =>
        {
            Debug.Log("Botão Compra Carta Clicado");
            botaoCompraCarta.onClick.RemoveAllListeners();
            controle.TrocaEstado(EstadoCompraCarta2.CreateInstance<EstadoCompraCarta2>());
        });
    }

    public override void RunEstado(Controle controle)
    {
    }

    public override void ProcessarSelecao(Controle controle, int indice, Carta cartaSelecionada)
    {
        FindAnyObjectByType<AudioManager>().Play("DrawCard");
        Debug.Log("Processando seleção de carta");
        controle.JogadorAtual.MaoCartas.Add(cartaSelecionada);
        controle.CartasAbertas.RemoveAt(indice);
        Carta c = controle.DeckCartas.CompraCarta();
        controle.CartasAbertas.Add(c);
        controle.VerificaLocomotivas();
        _GameManager.Instance.cartasAbertas.AtualizaExibicaoCartasAbertas(controle.CartasAbertas);
        botaoCompraCarta.onClick.RemoveAllListeners();
        controle.TrocaEstado(EstadoFimTurno.CreateInstance<EstadoFimTurno>());
    }
}
