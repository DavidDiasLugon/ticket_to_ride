using System.Collections.Generic;
using System.Linq;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Baralho", menuName = "Scriptable Objects/Baralho")]
public class Baralho : Deck<Carta>
{
    public override void Inicializa(List<Carta> c)
    {
        // Cannot implicitly convert type 'System.Collections.Generic.List<Carta>' to 'System.Collections.Generic.List<CartaTesouro>'CS0029
        this.baralho = c;
    }


    public override Carta CompraCarta()
    {
        if (baralho.Count == 0)
        {
            Debug.Log("Baralho vazio, embaralhando descarte");
            baralho = Embaralha(descarte);
            descarte.Clear();
        }
        Carta cartaComprada = baralho[0];
        baralho.RemoveAt(0);
        return cartaComprada;
    }

    public override List<Carta> Embaralha(List<Carta> l)
    {
        return l.OrderBy(x => UnityEngine.Random.Range(0f, 1f)).ToList();
    }

    public override void Descarte(Carta c)
    {
        descarte.Add(c);
        //        Debug.Log("Descarte " + descarte[^1].Nome);
    }
}
