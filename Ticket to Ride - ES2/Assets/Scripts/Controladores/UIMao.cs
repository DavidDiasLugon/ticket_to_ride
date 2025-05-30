using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class UIMao : MonoBehaviour
{
    public GameObject prefab;
    public RectTransform painelCartas;
    public List<GameObject> cartasInstanciadas = new List<GameObject>();

    public void AtualizaExibicao(Dictionary<string, int> dict)
    {
        if (prefab == null || painelCartas == null)
        {
            Debug.LogError("Prefab ou painelCartas não definido.");
            return;
        }
        foreach (GameObject cartaInstanciada in cartasInstanciadas)
        {
            Destroy(cartaInstanciada);
        }
        cartasInstanciadas.Clear();

        foreach (KeyValuePair<string, int> par in dict)
        {
            string cor = par.Key;
            int quantidade = par.Value;

            if (quantidade > 0)
            {
                Sprite sprite = Resources.Load<Sprite>("Cartas/" + cor);
                GameObject cartaObj = Instantiate(prefab, painelCartas);
                float maxAngle = 5f;
                float randomAngle = Random.Range(-maxAngle, maxAngle);
                cartaObj.transform.localRotation = Quaternion.Euler(0, 0, randomAngle);
                Image componenteImagem = cartaObj.GetComponentInChildren<Image>();
                componenteImagem.sprite = sprite;
                TextMeshProUGUI textoQtd = cartaObj.GetComponentInChildren<TextMeshProUGUI>();
                if (quantidade > 1)
                {
                    textoQtd.text = quantidade.ToString();
                }
                else
                {
                    textoQtd.text = "";
                }
                cartasInstanciadas.Add(cartaObj);
            }
        }
    }
}
