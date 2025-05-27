using System.Collections.Generic;
using UnityEngine;

public abstract class Deck<T> : ScriptableObject where T : Carta
{
    protected List<T> descarte = new List<T>();
    protected List<T> baralho = new List<T>();

    public abstract void Inicializa(List<T> c);

    public abstract Carta CompraCarta();

    public abstract List<T> Embaralha(List<T> l);

    public abstract void Descarte(T c);
    

}
