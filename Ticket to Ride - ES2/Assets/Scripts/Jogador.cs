using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Jogador", menuName = "Scriptable Objects/Jogador")]
public class Jogador : ScriptableObject
{
    private string nome;
    private Hand mao;

    public void OnEnable()
    {
        mao = Hand.CreateInstance<Hand>();
    }




    public Hand Mao
    {
        get => mao;
        set => mao = value;
    }

    public string Nome
    {
        get => nome;
        set => nome = value;
    }


}
