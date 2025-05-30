using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BaralhoBilhete", menuName = "Scriptable Objects/BaralhoBilhete")]
public class DeckBilhetes : ScriptableObject
{
    private List<Bilhete> deckBilhetes = new List<Bilhete>();

    public List<Bilhete> Deck
    {
        get => deckBilhetes;
        set => deckBilhetes = value;
    }

    public List<Bilhete> Embaralha()
    {
        return deckBilhetes.OrderBy(x => UnityEngine.Random.Range(0f, 1f)).ToList();
    }

    public Bilhete CompraBilhete()
    {
        Bilhete bilheteComprado = deckBilhetes[0];
        deckBilhetes.RemoveAt(0);
        return bilheteComprado;
    }

    public void Add(Bilhete b)
    {
        deckBilhetes.Add(b);
    }
}
