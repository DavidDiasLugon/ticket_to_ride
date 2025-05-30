using UnityEngine;
using UnityEngine.UI;

public class EstadoEspera : EstadoJogo
{
    private Button botaoCompraCarta;
    private Button botaoCompraBilhete;
    public override void IniciarEstado(Controle controle)
    {
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
            controle.TrocaEstado(EstadoCompraCarta1.CreateInstance<EstadoCompraCarta1>());
        });

        botaoCompraBilhete.onClick.AddListener(() =>
        {
            Debug.Log("Botão Compra Bilhete Clicado");
            botaoCompraBilhete.onClick.RemoveAllListeners();
            controle.TrocaEstado(EstadoCompraBilhete.CreateInstance<EstadoCompraBilhete>());
        });
    }

    public override void RunEstado(Controle controle)
    {
    }
}
