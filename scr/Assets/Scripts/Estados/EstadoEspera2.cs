using UnityEngine;
using UnityEngine.UI;

public class EstadoEspera2 : EstadoJogo
{
    private Button botaoCompraCarta;

    public override void IniciarEstado(Controle controle)
    {
        Debug.Log("Esperando compra da segunda carta para: " + controle.JogadorAtual.Nome);


        controle.JogadorAtual.UpdateNumeroCartasDict();
        _GameManager.Instance.maoCartas.AtualizaExibicao(controle.JogadorAtual.CartaNmr);


        botaoCompraCarta = GameObject.Find("BotaoCarta")?.GetComponent<Button>();
        if (botaoCompraCarta != null)
        {
            botaoCompraCarta.onClick.RemoveAllListeners();
        }

        // --- LÓGICA DE DECISÃO PRINCIPAL ---
        if (controle.JogadorAtual.isAI)
        {
            // --- LÓGICA DA IA ---
            Debug.Log("Jogador é IA. Escondendo botão e decidindo segunda jogada.");
            if (botaoCompraCarta != null)
            {
                botaoCompraCarta.gameObject.SetActive(false);
            }


            AIController ai = new AIController(controle);
            ai.ExecuteSecondCardDrawAction();
        }
        else
        {

            Debug.Log("Jogador é Humano. Configurando botão para segunda compra.");
            if (botaoCompraCarta != null)
            {
                botaoCompraCarta.gameObject.SetActive(true);
                botaoCompraCarta.onClick.AddListener(() =>
                {
                    Debug.Log("Botão Compra Carta Clicado (Segunda vez)");
                    botaoCompraCarta.onClick.RemoveAllListeners();
                    controle.TrocaEstado(EstadoCompraCarta2.CreateInstance<EstadoCompraCarta2>());
                });
            }
        }
    }

    public override void RunEstado(Controle controle)
    {

    }

    public override void ProcessarSelecao(Controle controle, int indice, Carta cartaSelecionada)
    {

        if (controle.JogadorAtual.isAI) return;

        Debug.Log("Processando seleção de carta (Segunda vez)");


        if (cartaSelecionada.isLocomotive)
        {
            Debug.Log("Ação ilegal: Não pode pegar locomotiva como segunda carta.");

            return;
        }


        controle.JogadorAtual.MaoCartas.Add(cartaSelecionada);
        controle.CartasAbertas.RemoveAt(indice);
        Carta c = controle.DeckCartas.CompraCarta();
        if (c != null) controle.CartasAbertas.Insert(indice, c);

        controle.VerificaLocomotivas();
        _GameManager.Instance.cartasAbertas.AtualizaExibicaoCartasAbertas(controle.CartasAbertas);

        if (botaoCompraCarta != null)
        {
            botaoCompraCarta.onClick.RemoveAllListeners();
        }

        controle.TrocaEstado(EstadoFimTurno.CreateInstance<EstadoFimTurno>());
    }
}