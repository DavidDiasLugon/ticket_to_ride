using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "Jogador", menuName = "Scriptable Objects/Jogador")]
public class Jogador : ScriptableObject
{
    private string cor;
    private string nome;
    private int pontuacao = 0;
    private int trens = 45;
    private List<Carta> maoCartas = new List<Carta>();
    private Dictionary<string, int> cartaNmr = new Dictionary<string, int>();
    private List<Bilhete> maoBilhetes = new List<Bilhete>();
    private bool selecionouBilhetes = false;

    public string Nome
    {
        get => nome;
        set => nome = value;
    }

    public bool SelecionouBilhetes
    {
        get => selecionouBilhetes;
        set => selecionouBilhetes = value;
    }

    public Dictionary<string, int> CartaNmr
    {
        get => cartaNmr;
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

    public void StartDict()
    {
        cartaNmr["amarelo"] = 0;
        cartaNmr["verde"] = 0;
        cartaNmr["vermelho"] = 0;
        cartaNmr["azul"] = 0;
        cartaNmr["rosa"] = 0;
        cartaNmr["laranja"] = 0;
        cartaNmr["branco"] = 0;
        cartaNmr["preto"] = 0;
        cartaNmr["colorido"] = 0;
    }

    public void UpdateNumeroCartasDict()
    {
        List<string> dictKeys = cartaNmr.Keys.ToList();
        foreach (string cor in dictKeys)
        {
            cartaNmr[cor] = maoCartas.Count(carta => carta.Cor == cor);
        }
    }

    public void RemoverCartasPorCor(string cor, int quantidade, Controle controle)
    {
        if (quantidade <= 0) return;

        List<Carta> cartasARemover = maoCartas.Where(carta => carta.Cor == cor).Take(quantidade).ToList();
        foreach (Carta carta in cartasARemover)
        {
            maoCartas.Remove(carta);
            controle.DeckCartas.Add(carta);
        }
        UpdateNumeroCartasDict();
    }
}
