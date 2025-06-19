using UnityEngine;

public class EstadoFimTurno : EstadoJogo
{
    public bool fim = false;
    public override void IniciarEstado(Controle controle)
    {
        Debug.Log("Fim do turno de: " + controle.JogadorAtual.Nome);
        controle.Turno++;
        Jogador proximoJogador = controle.getJogadorAtual();

        if (controle.RodadaFinalComecou)
        {
            if (proximoJogador.Nome == controle.NomeJogadorQueIniciouFinal)
            {
                Debug.Log("A rodada final terminou, Fim de Jogo");
                controle.TrocaEstado(EstadoFimJogo.CreateInstance<EstadoFimJogo>());
            }
            else
            {
                Debug.Log($"Turno Final para: {proximoJogador.Nome}");
                controle.JogadorAtual = proximoJogador;
                controle.TrocaEstado(EstadoEspera.CreateInstance<EstadoEspera>());
            }
        }
        else
        {
            controle.JogadorAtual = proximoJogador;
            controle.TrocaEstado(EstadoEspera.CreateInstance<EstadoEspera>());
        }
    }

    public override void RunEstado(Controle controle)
    {

    }
    
    public override void ProcessarSelecao(Controle controle, int indice, Carta cartaSelecionada)
    {
    }
}
