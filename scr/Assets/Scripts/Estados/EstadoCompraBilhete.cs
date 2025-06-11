using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EstadoCompraBilhete : EstadoJogo
{
    private Button botaoCompraCarta;
    private Button botaoCompraBilhete;
    private Canvas mainCanvas;
    private List<GameObject> bilhetesInstanciados = new List<GameObject>();
    private List<Bilhete> bilhetesDisponiveis = new List<Bilhete>();
    private List<Bilhete> bilhetesSelecionados = new List<Bilhete>();
    private GameObject prefabBilhete;

    private RectTransform cartasAbertas;
    private Button botaoCarta;
    private Button botaoBilhete;
    private GameObject gameBoard;
    private GameObject tracksContainer;
    public override void IniciarEstado(Controle controle)
    {
        mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        prefabBilhete = Resources.Load<GameObject>("Prefabs/Carta/Bilhete");

        cartasAbertas = mainCanvas.transform.Find("CartasAbertas").GetComponent<RectTransform>();
        botaoCarta = mainCanvas.transform.Find("BotaoCarta").GetComponent<Button>();
        botaoBilhete = mainCanvas.transform.Find("BotaoBilhete").GetComponent<Button>();

        gameBoard = GameObject.Find("GameBoard");
        tracksContainer = GameObject.Find("TracksContainer");
        FindAnyObjectByType<GameAudioManager>().Play("DrawCard");
        FindAnyObjectByType<GameAudioManager>().Play("DrawCard");
        FindAnyObjectByType<GameAudioManager>().Play("DrawCard");
        if (controle.JogadorAtual.isAI)
        {
            SelecionaBilhete(controle);
        }
        else
        {
            SelecionaBilhete(controle);
        }
    }

    public override void RunEstado(Controle controle)
    {

    }

    public override void ProcessarSelecao(Controle controle, int indice, Carta cartaSelecionada)
    {
    }

    public void SelecionaBilhete(Controle controle)
    {
        HideObjects();
        RectTransform cardBox = mainCanvas.transform.Find("CardBox").GetComponent<RectTransform>();
        cardBox.gameObject.SetActive(false);
        RectTransform selectionBox = mainCanvas.transform.Find("SelectionBox").GetComponent<RectTransform>();
        foreach (Transform child in selectionBox.transform)
        {
            Destroy(child.gameObject);
        }
        bilhetesInstanciados.Clear();
        bilhetesDisponiveis.Clear();
        bilhetesSelecionados.Clear();

        Button botaoSelect = mainCanvas.transform.Find("BotaoSelect").GetComponent<Button>();
        botaoSelect.gameObject.SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            bilhetesDisponiveis.Add(controle.DeckBilhetes.CompraBilhete());
        }

        if (controle.JogadorAtual.isAI)
        {
            AIController ai = new AIController(controle);

            List<Bilhete> escolhidos = ai.ChooseTickets(bilhetesDisponiveis, 1);


            controle.TrocaEstado(EstadoFimTurno.CreateInstance<EstadoFimTurno>());

        }
        else
        {
            foreach (Bilhete bilhete in bilhetesDisponiveis)
            {
                GameObject bilheteObj = Instantiate(prefabBilhete, selectionBox);
                Button botaoBilhete = bilheteObj.AddComponent<Button>();
                botaoBilhete.onClick.RemoveAllListeners();
                botaoBilhete.onClick.AddListener(() =>
                    {
                        OnBilheteClicked(bilhete, botaoSelect, bilheteObj);
                    });
                bilheteObj.transform.Find("Origem").GetComponent<TextMeshProUGUI>().text = bilhete.Rota[0];
                bilheteObj.transform.Find("Destino").GetComponent<TextMeshProUGUI>().text = bilhete.Rota[1];
                bilheteObj.transform.Find("Pontuacao").GetComponent<TextMeshProUGUI>().text = bilhete.Pontos.ToString();
                bilhetesInstanciados.Add(bilheteObj);
            }
            botaoSelect.onClick.RemoveAllListeners();
            botaoSelect.onClick.AddListener(() =>
            {
                FindAnyObjectByType<GameAudioManager>().Play("DrawCard");
                foreach (Bilhete bilhete in bilhetesSelecionados)
                {
                    controle.JogadorAtual.MaoBilhetes.Add(bilhete);
                }
                List<Bilhete> bilhetesRestantes = bilhetesDisponiveis.Except(bilhetesSelecionados).ToList();
                foreach (Bilhete bilhete in bilhetesRestantes)
                {
                    controle.DeckBilhetes.Deck.Add(bilhete);
                }
                foreach (Transform child in selectionBox.transform)
                {
                    Destroy(child.gameObject);
                }
                bilhetesInstanciados.Clear();
                bilhetesDisponiveis.Clear();
                bilhetesSelecionados.Clear();
                cardBox.gameObject.SetActive(true);
                ShowObjects();
                botaoSelect.onClick.RemoveAllListeners();
                botaoSelect.gameObject.SetActive(false);
                _GameManager.Instance.uiHud.AtualizaMainHud(controle);
                controle.TrocaEstado(EstadoFimTurno.CreateInstance<EstadoFimTurno>());
            });
        }

    }

    public void OnBilheteClicked(Bilhete bilhete, Button botaoSelect, GameObject bilheteObj)
    {
        FindAnyObjectByType<GameAudioManager>().Play("TicketClick");
        if (bilhetesSelecionados.Contains(bilhete))
        {
            bilheteObj.transform.localScale = new Vector3(1f, 1f, 1f);
            bilhetesSelecionados.Remove(bilhete);
        }
        else
        {
            bilheteObj.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            bilhetesSelecionados.Add(bilhete);
        }
        botaoSelect.gameObject.SetActive(bilhetesSelecionados.Count >= 1);
    }

    public void HideObjects()
    {
        cartasAbertas.gameObject.SetActive(false);
        botaoCarta.gameObject.SetActive(false);
        botaoBilhete.gameObject.SetActive(false);
        //gameBoard.SetActive(false);
        //tracksContainer.SetActive(false);
    }

    public void ShowObjects()
    {
        cartasAbertas.gameObject.SetActive(true);
        botaoCarta.gameObject.SetActive(true);
        botaoBilhete.gameObject.SetActive(true);
        //gameBoard.SetActive(true);
        //tracksContainer.SetActive(true);
    }
}
