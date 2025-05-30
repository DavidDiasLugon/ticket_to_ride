using UnityEngine;

[CreateAssetMenu(fileName = "Carta", menuName = "Scriptable Objects/Carta")]
public class Carta : ScriptableObject
{
    private string cor;
    public bool isLocomotive;

    public string Cor
    {
        get => cor;
        set => cor = value;
    }
}
