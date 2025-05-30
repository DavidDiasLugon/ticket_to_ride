using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Bilhete", menuName = "Scriptable Objects/Bilhete")]
public class Bilhete : ScriptableObject
{
    private string[] rota = new string[2];
    private int pontos;

    public int Pontos
    {
        get => pontos;
        set => pontos = value;
    }

    public string[] Rota
    {
        get => rota;
    } 
}
