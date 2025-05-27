using System.Collections;
using System.Collections.Generic;
using System.Linq; // Necessário para .OrderBy e .ToList
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("Configuração do Baralho")]
    [Tooltip("Todas as cartas que podem compor o baralho. Arraste suas Carta ScriptableObjects aqui.")]
    public List<Carta> allPossibleCartas = new List<Carta>();

    [Tooltip("O número máximo de cartas que o baralho principal pode conter.")]
    public int maxDeckSize = 40;

    [Header("Status do Baralho")]
    [Tooltip("Cartas atualmente no baralho principal (prontas para serem compradas).")]
    public List<Carta> mainDeck = new List<Carta>();

    [Tooltip("Cartas que foram descartadas.")]
    public List<Carta> discardPile = new List<Carta>();


    public static event System.Action OnDeckShuffled;
    public static event System.Action OnCartadiscarded;


    public static event System.Action OnDeckRestocked;

    void Awake()
    {

        InitializeDeck();
    }

    public void InitializeDeck()
    {
        mainDeck.Clear();
        discardPile.Clear();

        foreach (Carta Carta in allPossibleCartas)
        {
            if (mainDeck.Count < maxDeckSize)
            {
                mainDeck.Add(Carta);
            }
            else
            {
                Debug.LogWarning($"Max deck size ({maxDeckSize}) reached. Not all possible Cartas were added to the initial deck.");
                break;
            }
        }

        ShuffleDeck();
    }


    public void ShuffleDeck()
    {
        if (mainDeck.Count == 0)
        {
            Debug.LogWarning("Cannot shuffle an empty deck.");
            return;
        }


        for (int i = 0; i < mainDeck.Count; i++)
        {
            Carta temp = mainDeck[i];
            int randomIndex = Random.Range(i, mainDeck.Count);
            mainDeck[i] = mainDeck[randomIndex];
            mainDeck[randomIndex] = temp;
        }

        Debug.Log("Baralho embaralhado!");
        OnDeckShuffled?.Invoke();
    }


    public Carta DrawCarta()
    {
        if (mainDeck.Count == 0)
        {
            Debug.LogWarning("Baralho vazio! Não há cartas para comprar.");
            return null;
        }

        Carta drawnCarta = mainDeck[0];
        mainDeck.RemoveAt(0);

        Debug.Log($"Carta comprada: {drawnCarta.Nome}. Cartas restantes no baralho: {mainDeck.Count}");
        return drawnCarta;
    }


    public void discardCarta(Carta CartaTodiscard)
    {
        if (CartaTodiscard == null)
        {
            Debug.LogWarning("Tentativa de descartar uma carta nula.");
            return;
        }


        discardPile.Add(CartaTodiscard);
        Debug.Log($"Carta descartada: {CartaTodiscard.Nome}. Cartas no descarte: {discardPile.Count}");
        OnCartadiscarded?.Invoke();
    }

    public bool IsDeckFull()
    {
        return mainDeck.Count >= maxDeckSize;
    }


    public int GetCurrentDeckCount()
    {
        return mainDeck.Count;
    }


    public void RestockDeckFromdiscard()
    {
        if (discardPile.Count == 0)
        {
            Debug.Log("Pilha de descarte vazia. Não há cartas para repor no baralho.");
            return;
        }

        // Adiciona todas as cartas do descarte ao baralho principal
        foreach (Carta discardedCarta in discardPile)
        {
            if (mainDeck.Count < maxDeckSize)
            {
                mainDeck.Add(discardedCarta);
            }
            else
            {
                Debug.LogWarning($"Não foi possível repor todas as cartas do descarte. O baralho principal atingiu o limite de {maxDeckSize} cartas.");
                break;
            }
        }

        discardPile.Clear();
        ShuffleDeck();

        Debug.Log($"Baralho reposto com {mainDeck.Count} cartas da pilha de descarte. Pilha de descarte agora vazia.");
        OnDeckRestocked?.Invoke();
    }


    public void PrintMainDeckContents()
    {
        Debug.Log("Conteúdo do Baralho Principal:");
        if (mainDeck.Count == 0)
        {
            Debug.Log("Vazio.");
            return;
        }
        for (int i = 0; i < mainDeck.Count; i++)
        {
            Debug.Log($"- [{i}] {mainDeck[i].Nome}");
        }
    }

    public void PrintdiscardPileContents()
    {
        Debug.Log("Conteúdo da Pilha de Descarte:");
        if (discardPile.Count == 0)
        {
            Debug.Log("Vazio.");
            return;
        }
        for (int i = 0; i < discardPile.Count; i++)
        {
            Debug.Log($"- [{i}] {discardPile[i].Nome}");
        }
    }
}