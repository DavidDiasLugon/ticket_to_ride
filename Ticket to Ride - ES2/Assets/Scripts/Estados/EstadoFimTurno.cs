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
            controle.Turno++;
            controle.JogadorAtual = controle.getJogadorAtual();
            controle.TrocaEstado(EstadoEspera.CreateInstance<EstadoEspera>());
            
        }
    }

    public override void RunEstado(Controle controle)
    {
        
    }
}
