using UnityEngine;
using UnityEngine.UI;

public class EstadoEspera2 : EstadoJogo
{
    private Button botaoCompraCarta;
    public override void IniciarEstado(Controle controle)
    {
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
}
