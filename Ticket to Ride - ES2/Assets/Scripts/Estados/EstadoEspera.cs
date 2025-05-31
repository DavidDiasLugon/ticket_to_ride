using UnityEngine;
using UnityEngine.UI;

public class EstadoEspera : EstadoJogo
{
    private Button botaoCompraCarta;
    private Button botaoCompraBilhete;
    public override void IniciarEstado(Controle controle)
    {
        controle.JogadorAtual.UpdateNumeroCartasDict();
        _GameManager.Instance.maoCartas.AtualizaExibicao(controle.JogadorAtual.CartaNmr);
        Debug.Log("Turno de: " + controle.JogadorAtual.Nome);
        botaoCompraCarta = GameObject.Find("BotaoCarta")?.GetComponent<Button>();
        botaoCompraBilhete = GameObject.Find("BotaoBilhete")?.GetComponent<Button>();
        if (botaoCompraCarta == null || botaoCompraBilhete == null)
        {
            Debug.LogError("Botões de compra não encontrados!");
            return;
        }

        botaoCompraCarta.onClick.AddListener(() =>
        {
            Debug.Log("Botão Compra Carta Clicado");
            botaoCompraCarta.onClick.RemoveAllListeners();
            botaoCompraBilhete.onClick.RemoveAllListeners();
            controle.TrocaEstado(EstadoCompraCarta1.CreateInstance<EstadoCompraCarta1>());
        });

        botaoCompraBilhete.onClick.AddListener(() =>
        {
            Debug.Log("Botão Compra Bilhete Clicado");
            botaoCompraBilhete.onClick.RemoveAllListeners();
            botaoCompraCarta.onClick.RemoveAllListeners();
            controle.TrocaEstado(EstadoCompraBilhete.CreateInstance<EstadoCompraBilhete>());
        });
    }

    public override void RunEstado(Controle controle)
    {
    }

    public override void ProcessarSelecao(Controle controle, int indice, Carta cartaSelecionada)
    {
        Debug.Log("Processando seleção de carta");
        controle.JogadorAtual.MaoCartas.Add(cartaSelecionada);
        controle.CartasAbertas.RemoveAt(indice);
        Carta c = controle.DeckCartas.CompraCarta();
        controle.CartasAbertas.Add(c);
        controle.VerificaLocomotivas();
        _GameManager.Instance.cartasAbertas.AtualizaExibicaoCartasAbertas(controle.CartasAbertas);
        botaoCompraCarta.onClick.RemoveAllListeners();
        if (cartaSelecionada.isLocomotive)
        {
            controle.TrocaEstado(EstadoFimTurno.CreateInstance<EstadoFimTurno>());
        }
        else
        {
            controle.TrocaEstado(EstadoEspera2.CreateInstance<EstadoEspera2>());
        }
    }
}
