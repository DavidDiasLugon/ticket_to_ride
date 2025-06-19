using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BaralhoCarta", menuName = "Scriptable Objects/BaralhoCarta")]
public class DeckCartas : ScriptableObject
{
    private List<Carta> deckCartas = new List<Carta>();

    public List<Carta> Deck
    {
        get => deckCartas;
        set => deckCartas = value;
    }

    public void Embaralha()
    {
        deckCartas = deckCartas.OrderBy(x => UnityEngine.Random.Range(0f, 1f)).ToList();
    }

    public Carta CompraCarta()
    {
        if (Deck.Count == 0)
        {
            return null;
        }
        Carta cartaComprada = deckCartas[0];
        deckCartas.RemoveAt(0);
        return cartaComprada;
    }

    public void Add(Carta c)
    {
        deckCartas.Add(c);
    }
}
