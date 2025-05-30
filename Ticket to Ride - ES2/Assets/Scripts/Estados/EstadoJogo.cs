using UnityEngine;

public abstract class EstadoJogo : ScriptableObject
{
    public abstract void IniciarEstado(Controle controle);
    public abstract void RunEstado(Controle controle);
    //Cada fim de Estado tem uma chamada a função do Troca de Estado
}