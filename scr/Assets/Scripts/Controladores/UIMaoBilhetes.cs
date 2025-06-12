using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMaoBilhetes : MonoBehaviour
{
    public GameObject bilhetePrefab;
    public Transform painelBilhetes;
    private List<GameObject> bilhetesInstanciados = new List<GameObject>();


    public void ExibirBilhetes()
    {
        Debug.Log("Chegou");
        Jogador jogador = _GameManager.Instance.controle.JogadorAtual;
        foreach (Transform child in painelBilhetes)
        {
            Destroy(child.gameObject);
        }
        bilhetesInstanciados.Clear();
        if (jogador.MaoBilhetes.Count > 0)
        {
            Debug.Log("Chegou");
            foreach (Bilhete bilhete in jogador.MaoBilhetes)
            {
                GameObject bilheteObj = Instantiate(bilhetePrefab, painelBilhetes);
                bilheteObj.transform.Find("Origem").GetComponent<TextMeshProUGUI>().text = bilhete.Rota[0];
                bilheteObj.transform.Find("Destino").GetComponent<TextMeshProUGUI>().text = bilhete.Rota[1];
                bilheteObj.transform.Find("Pontuacao").GetComponent<TextMeshProUGUI>().text = bilhete.Pontos.ToString();
                GameObject icone = bilheteObj.transform.Find("Icone").gameObject;
                TextMeshProUGUI textoIcone = icone.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                if (bilhete.Concluido)
                {
                    textoIcone.gameObject.SetActive(true);
                }
                BilheteHover bilheteHover = bilheteObj.GetComponent<BilheteHover>();
                if (bilheteHover != null)
                {
                    bilheteHover.Inicializacao(bilhete.Rota[0], bilhete.Rota[1]);
                }
                bilhetesInstanciados.Add(bilheteObj);
            }
        }
        gameObject.SetActive(true);
        Debug.Log("Chegou");
    }

    public void FecharPainel()
    {
        gameObject.SetActive(false);
    }
}
