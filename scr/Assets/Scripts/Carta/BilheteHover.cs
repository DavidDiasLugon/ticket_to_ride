using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BilheteHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string cidadeOrigem;
    private string cidadeDestino;
    public Color corDestaque;
    private List<CityController> cidadesDestacadas = new List<CityController>();

    public void Inicializacao(string origem, string destino)
    {
        this.cidadeOrigem = origem;
        this.cidadeDestino = destino;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cidadesDestacadas.Clear();
        if (BoardManager.Cities.TryGetValue(cidadeOrigem, out CityController city1))
        {
            city1.Destacar(true, corDestaque);
            cidadesDestacadas.Add(city1);
        }
        if (BoardManager.Cities.TryGetValue(cidadeDestino, out CityController city2))
        {
            city2.Destacar(true, corDestaque);
            cidadesDestacadas.Add(city2);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (var cidadeController in cidadesDestacadas)
        {
            if (cidadeController != null)
            {
                cidadeController.Destacar(false, Color.black);
            }
        }
        cidadesDestacadas.Clear();
    }

    void OnDestroy()
    {
        OnPointerExit(null);
    }


}
