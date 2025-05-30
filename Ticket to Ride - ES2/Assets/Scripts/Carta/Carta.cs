using UnityEngine;

[CreateAssetMenu(fileName = "Carta", menuName = "Scriptable Objects/Carta")]
public class Carta : ScriptableObject
{
    private string cor;
    public bool isLocomotive;
    public Sprite imagem;

    public string Cor
    {
        get => cor;
        set => cor = value;
    }

    public Sprite Imagem
    {
        get => imagem;
        set => imagem = value;
    }
}
