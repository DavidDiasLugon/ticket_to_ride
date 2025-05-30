using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Jogador", menuName = "Scriptable Objects/Jogador")]
public class Jogador : ScriptableObject
{
    private string cor;
    private string nome;
    private int pontuacao = 0;
    private int trens = 45;
    private List<Carta> maoCartas = new List<Carta>();
    private List<Bilhete> maoBilhetes = new List<Bilhete>();

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

    public int Pontuacao
    {
        get => pontuacao;
        set => pontuacao = value;
    }

    public int Trens
    {
        get => trens;
        set => trens = value;
    }

    public List<Carta> MaoCartas
    {
        get => maoCartas;
    }

    public List<Bilhete> MaoBilhetes
    {
        get => maoBilhetes;
    }
}
