using UnityEngine;

public abstract class Carta : ScriptableObject
{
    [SerializeField] private string nome;
    [SerializeField] private string cor;
    [SerializeField] private string cartaPath;


    public string Nome
    {
        get => nome;
        set => nome = value;
    }

    public string Cor
    {
        get => cor;
        set => cor = value;
    }

    public string CartaPath
    {
        get => cartaPath;
        set => cartaPath = value;
    }

}
