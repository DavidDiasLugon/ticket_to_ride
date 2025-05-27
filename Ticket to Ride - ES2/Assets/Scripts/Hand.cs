using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hand", menuName = "Scriptable Objects/Hand")]
public class Hand : ScriptableObject
{

    private List<Carta> naMao = new List<Carta>();

    public void Add(Carta c)
    {
        naMao.Add(c);
    }


    public List<Carta> NaMao
    {
        get => naMao;
        set => naMao = value;
    }

}