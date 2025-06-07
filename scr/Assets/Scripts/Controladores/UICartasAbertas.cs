using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICartasAbertas : MonoBehaviour
{
    public GameObject prefab;
    public RectTransform painelCartasAbertas;
    public List<GameObject> cartasInstanciadas = new List<GameObject>();

    public void AtualizaExibicaoCartasAbertas(List<Carta> cartas)
    {
        if (prefab == null || painelCartasAbertas == null)
        {
            Debug.LogError("Prefab ou painelCartasAbertas não definido.");
            return;
        }

        foreach (GameObject cartaInstanciada in cartasInstanciadas)
        {
            Destroy(cartaInstanciada);
        }
        cartasInstanciadas.Clear();

        foreach (Carta carta in cartas)
        {
            if (carta != null)
            {
                Sprite sprite = Resources.Load<Sprite>("Cartas/" + carta.Cor);
                GameObject cartaObj = Instantiate(prefab, painelCartasAbertas);
                cartaObj.transform.localRotation = Quaternion.Euler(0, 0, 90f);
                Image componenteImagem = cartaObj.GetComponentInChildren<Image>();
                componenteImagem.sprite = sprite;
                Button botaoCarta = cartaObj.AddComponent<Button>();
                botaoCarta.targetGraphic = componenteImagem;
                botaoCarta.onClick.RemoveAllListeners();
                int index = cartas.IndexOf(carta);
                botaoCarta.onClick.AddListener(() =>
                {
                    botaoCarta.onClick.RemoveAllListeners();
                    OnCartaAbertaClicada(index, carta);
                });
                cartasInstanciadas.Add(cartaObj);
            }
        }
    }


    public void OnCartaAbertaClicada(int indice, Carta carta)
    {
        _GameManager.Instance.controle.CartaSelecionada(indice, carta);
        Debug.Log("Carta aberta clicada");
    }
}
