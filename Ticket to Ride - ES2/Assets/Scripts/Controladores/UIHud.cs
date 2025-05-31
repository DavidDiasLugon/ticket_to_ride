using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class UIHud : MonoBehaviour
{
    public GameObject mainHudPrefab;
    public RectTransform mainHudPanel;
    private GameObject mainHudObj;
    public GameObject otherPlayerHudPrefab;
    public RectTransform otherPlayerHudPanel;
    private List<GameObject> otherPlayerHudObjs = new List<GameObject>(); 
    private List<Jogador> remainingPlayers = new List<Jogador>();

    public void AtualizaMainHud(Controle controle)
    {
        if (mainHudPrefab == null || mainHudPanel == null)
        {
            Debug.LogError("Prefab ou painel não definido.");
            return;
        }
        if (mainHudObj != null)
        {
            Destroy(mainHudObj);
        }
        mainHudObj = Instantiate(mainHudPrefab, mainHudPanel, false);
        Color cor = GetColor(controle.JogadorAtual);

        Image nameBox = mainHudObj.transform.Find("NameBox").GetComponent<Image>();
        nameBox.color = cor;
        TextMeshProUGUI name = nameBox.GetComponentInChildren<TextMeshProUGUI>();
        name.text = controle.JogadorAtual.Nome;

        Image pointsBox = mainHudObj.transform.Find("PointsBox").GetComponent<Image>();
        TextMeshProUGUI points = pointsBox.GetComponentInChildren<TextMeshProUGUI>();
        points.text = controle.JogadorAtual.Pontuacao.ToString();

        Image trainBox = mainHudObj.transform.Find("TrainsBox").GetComponent<Image>();
        trainBox.color = cor;
        TextMeshProUGUI trains = trainBox.GetComponentInChildren<TextMeshProUGUI>();
        trains.text = controle.JogadorAtual.Trens.ToString();

        Image ticketBox = mainHudObj.transform.Find("TicketBox").GetComponent<Image>();
        ticketBox.color = cor;
        TextMeshProUGUI tickets = ticketBox.GetComponentInChildren<TextMeshProUGUI>();
        tickets.text = controle.JogadorAtual.MaoBilhetes.Count.ToString();
    }

    public void AtualizaOtherPlayerHud(Controle controle)
    {
        remainingPlayers = GetRemainingPlayers(controle);
        if (otherPlayerHudPrefab == null || otherPlayerHudPanel == null)
        {
            Debug.LogError("Prefab ou painel não definido.");
            return;
        }
        foreach (GameObject hudObj in otherPlayerHudObjs)
        {
            Destroy(hudObj);
        }
        otherPlayerHudObjs.Clear();
        foreach (Jogador jogador in remainingPlayers)
        {
            GameObject hudObj = Instantiate(otherPlayerHudPrefab, otherPlayerHudPanel, false);
            Color cor = GetColor(jogador);

            Image nameBox = hudObj.transform.Find("NameBox").GetComponent<Image>();
            nameBox.color = cor;
            TextMeshProUGUI name = nameBox.GetComponentInChildren<TextMeshProUGUI>();
            name.text = jogador.Nome;

            Image pointsBox = hudObj.transform.Find("PointsBox").GetComponent<Image>();
            TextMeshProUGUI points = pointsBox.GetComponentInChildren<TextMeshProUGUI>();
            points.text = jogador.Pontuacao.ToString();

            Image trainBox = hudObj.transform.Find("TrainsBox").GetComponent<Image>();
            trainBox.color = cor;
            TextMeshProUGUI trains = trainBox.GetComponentInChildren<TextMeshProUGUI>();
            trains.text = jogador.Trens.ToString();

            otherPlayerHudObjs.Add(hudObj);
        }

    }

    public Color GetColor(Jogador jogador)
    {
        string cor = jogador.Cor;
        switch (cor)
        {
            case "azul":
                return new Color(55f / 255f, 182f / 255f, 243f / 255f);
            case "amarelo":
                return new Color(254f / 255f, 222f / 255f, 90f / 255f, 1f);
            case "vermelho":
                return new Color(254f / 255f, 48f / 255f, 48f / 255f, 1f);
            case "rosa":
                return new Color(254f / 255f, 101f / 255f, 195f / 255f, 1f);
            case "verde":
                return new Color(0f / 255f, 192f / 255f, 99f / 255f, 1f);
            default:
                return Color.white;
        }
    }

    public List<Jogador> GetRemainingPlayers(Controle controle)
    {
        remainingPlayers.Clear();
        foreach (Jogador jogador in controle.Jogadores)
        {
            if (jogador != controle.JogadorAtual)
            {
                remainingPlayers.Add(jogador);
            }
        }
        return remainingPlayers;
    }

}
