using UnityEngine;

public class EstadoFimTurno : EstadoJogo
{
    public bool fim = false;
    public override void IniciarEstado(Controle controle)
    {
        if (controle.JogadorAtual.Trens <= 2)
        {
            fim = true;
        }
        if (fim)
        {
            controle.TrocaEstado(EstadoFimJogo.CreateInstance<EstadoFimJogo>());
        }
        else
        {
            Debug.Log("Fim do turno de: " + controle.JogadorAtual.Nome);
            controle.Turno++;
            controle.JogadorAtual = controle.getJogadorAtual();
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
