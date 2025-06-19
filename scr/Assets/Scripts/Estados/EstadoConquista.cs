using System;
using UnityEngine;

public class EstadoConquista : EstadoJogo
{
    public (Draggable carta, TrackController trilho) dadosDaConquista;
    public override void IniciarEstado(Controle controle)
    {
        Jogador jogador = controle.getJogadorAtual();
        TrackController trilho = dadosDaConquista.trilho;
        TrackData dadosTrilho = trilho.trackData;
        string corCarta = dadosDaConquista.carta.corCarta;
        int custo = dadosTrilho.length;

        if (trilho.isClaimed || jogador.Trens < custo)
        {
            Debug.Log("Falha na Conquista");
            FindAnyObjectByType<AudioManager>().Play("FailedConquest");
            FalhaNaConquista(controle);
            return;
        }

        bool podeConquistar = false;

        // Rota Cinza
        if (dadosTrilho.color == TrackColor.Gray)
        {
            // Conquista com cartas de cor e locomotivas
            if (corCarta != "colorido")
            {
                int cartasDaCor = jogador.CartaNmr.ContainsKey(corCarta) ? jogador.CartaNmr[corCarta] : 0;
                int locomotivas = jogador.CartaNmr.ContainsKey("colorido") ? jogador.CartaNmr["colorido"] : 0;
                if (cartasDaCor + locomotivas >= custo)
                {
                    podeConquistar = true;
                }
            }
            // Conquista com locomotivas
            else
            {
                int locomotivas = jogador.CartaNmr.ContainsKey("colorido") ? jogador.CartaNmr["colorido"] : 0;
                if (locomotivas >= custo)
                {
                    podeConquistar = true;
                }
            }
        }
        // Rota Colorida
        else
        {
            string corRequerida = ConverterTrackColorParaString(dadosTrilho.color);

            if (corCarta == corRequerida || corCarta == "colorido")
            {
                int cartasDaCor = jogador.CartaNmr.ContainsKey(corRequerida) ? jogador.CartaNmr[corRequerida] : 0;
                int locomotivas = jogador.CartaNmr.ContainsKey("colorido") ? jogador.CartaNmr["colorido"] : 0;

                if (cartasDaCor + locomotivas >= custo)
                {
                    podeConquistar = true;
                }
            }
        }

        if (!podeConquistar)
        {
            FalhaNaConquista(controle);
            return;
        }

        Debug.Log("Sucesso na conquista da rota");

        string corUsada = (dadosTrilho.color == TrackColor.Gray) ? corCarta : ConverterTrackColorParaString(dadosTrilho.color);
        int cartasDaCorARemover = Mathf.Min(custo, jogador.CartaNmr.ContainsKey(corUsada) ? jogador.CartaNmr[corUsada] : 0);
        jogador.RemoverCartasPorCor(corUsada, cartasDaCorARemover, controle);

        int locomotivasARemover = custo - cartasDaCorARemover;
        if (locomotivasARemover > 0)
        {
            jogador.RemoverCartasPorCor("colorido", locomotivasARemover, controle);
        }
        controle.ReporMesaSeNecessario();

        int [] tabelaPontuacao = { 0, 1, 2, 4, 7, 10, 15 };
        jogador.Pontuacao += tabelaPontuacao[custo];
        jogador.Trens -= custo;

        if (jogador.Trens <= 2 && !controle.RodadaFinalComecou)
        {
            controle.IniciarRodadaFinal(jogador.Nome);
        }

        Color corDoJogador = FindAnyObjectByType<UIHud>().GetColor(jogador);
        trilho.Claim(jogador.Nome, corDoJogador);
        FindAnyObjectByType<AudioManager>().Play("RouteConquered");

        jogador.UpdateNumeroCartasDict();
        _GameManager.Instance.maoCartas.AtualizaExibicao(jogador.CartaNmr);
        _GameManager.Instance.uiHud.AtualizaMainHud(controle);

        VerificarBilhetesCompletos(controle);

        Destroy(dadosDaConquista.carta.gameObject);
        controle.TrocaEstado(EstadoFimTurno.CreateInstance<EstadoFimTurno>());
    }

    public void FalhaNaConquista(Controle controle)
    {
        // Efeito sonoro para a falha
        Debug.Log("Falha na conquista da rota");
        dadosDaConquista.carta.RetornarParaMao();
        controle.TrocaEstado(EstadoEspera.CreateInstance<EstadoEspera>());
    }

    public string ConverterTrackColorParaString(TrackColor cor)
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

    public void VerificarBilhetesCompletos(Controle controle)
    {
        Jogador jogador = controle.getJogadorAtual();
        foreach (Bilhete bilhete in jogador.MaoBilhetes)
        {
            if (!bilhete.Concluido)
            {
                bool caminhoExiste = VerificadorDeRotas.ExisteCaminho(bilhete.Rota[0], bilhete.Rota[1], BoardManager.AllTrackControllers, jogador.Nome);
                if (caminhoExiste)
                {
                    FindAnyObjectByType<AudioManager>().Play("TicketCompleted");
                    bilhete.Concluido = true;
                    Debug.Log("Concluiu bilhete de Destino");
                }
            }
        }
    }

    public override void RunEstado(Controle controle)
    {
    }
    
    public override void ProcessarSelecao(Controle controle, int indice, Carta cartaSelecionada)
    {
    }
}
