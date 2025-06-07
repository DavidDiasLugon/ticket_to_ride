using UnityEngine;

public abstract class EstadoJogo : ScriptableObject
{
    public abstract void IniciarEstado(Controle controle);
    public abstract void RunEstado(Controle controle);
    //Cada fim de Estado tem uma chamada a função do Troca de Estado
    public abstract void ProcessarSelecao(Controle controle, int indice, Carta cartaSelecionada);
}