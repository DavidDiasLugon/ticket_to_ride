using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UIElements.Image;

public class EstadoEspera : EstadoJogo
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

        controle.JogadorAtual.UpdateNumeroCartasDict();
        _GameManager.Instance.maoCartas.AtualizaExibicao(controle.JogadorAtual.CartaNmr);
        _GameManager.Instance.uiHud.AtualizaMainHud(controle);
        _GameManager.Instance.uiHud.AtualizaOtherPlayerHud(controle);

        Debug.Log("Turno de: " + controle.JogadorAtual.Nome);
        botaoCompraCarta = GameObject.Find("BotaoCarta")?.GetComponent<Button>();
        botaoCompraBilhete = GameObject.Find("BotaoBilhete")?.GetComponent<Button>();
        if (botaoCompraCarta == null || botaoCompraBilhete == null)
        {
            Debug.LogError("Botões de compra não encontrados!");
            return;
        }

        botaoCompraCarta.onClick.AddListener(() =>
        {
            Debug.Log("Botão Compra Carta Clicado");
            botaoCompraCarta.onClick.RemoveAllListeners();
            botaoCompraBilhete.onClick.RemoveAllListeners();
            controle.TrocaEstado(EstadoCompraCarta1.CreateInstance<EstadoCompraCarta1>());
        });

        botaoCompraBilhete.onClick.AddListener(() =>
        {
            Debug.Log("Botão Compra Bilhete Clicado");
            botaoCompraBilhete.onClick.RemoveAllListeners();
            botaoCompraCarta.onClick.RemoveAllListeners();
            controle.TrocaEstado(EstadoCompraBilhete.CreateInstance<EstadoCompraBilhete>());
        });

        if (controle.Turno <= controle.Jogadores.Count - 1)
        {
            SelecionaBilhete(controle);
        }
    }

    public override void RunEstado(Controle controle)
    {
    }

    public override void ProcessarSelecao(Controle controle, int indice, Carta cartaSelecionada)
    {
        Debug.Log("Processando seleção de carta");
        controle.JogadorAtual.MaoCartas.Add(cartaSelecionada);
        controle.CartasAbertas.RemoveAt(indice);
        Carta c = controle.DeckCartas.CompraCarta();
        controle.CartasAbertas.Add(c);
        controle.VerificaLocomotivas();
        _GameManager.Instance.cartasAbertas.AtualizaExibicaoCartasAbertas(controle.CartasAbertas);
        botaoCompraCarta.onClick.RemoveAllListeners();
        if (cartaSelecionada.isLocomotive)
        {
            controle.TrocaEstado(EstadoFimTurno.CreateInstance<EstadoFimTurno>());
        }
        else
        {
            controle.TrocaEstado(EstadoEspera2.CreateInstance<EstadoEspera2>());
        }
    }

    public void SelecionaBilhete(Controle controle)
    {
        HideObjects();
        RectTransform cardBox = mainCanvas.transform.Find("CardBox").GetComponent<RectTransform>();
        cardBox.gameObject.SetActive(false);
        RectTransform selectionBox = mainCanvas.transform.Find("SelectionBox").GetComponent<RectTransform>();
        foreach (GameObject bilheteObj in bilhetesInstanciados)
        {
            Destroy(bilheteObj);
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
            foreach (Bilhete bilhete in bilhetesSelecionados)
            {
                controle.JogadorAtual.MaoBilhetes.Add(bilhete);
            }
            List<Bilhete> bilhetesRestantes = bilhetesDisponiveis.Except(bilhetesSelecionados).ToList();
            foreach (Bilhete bilhete in bilhetesRestantes)
            {
                controle.DeckBilhetes.Deck.Add(bilhete);
            }
            foreach (GameObject bilheteObj in bilhetesInstanciados)
            {
                Destroy(bilheteObj);
            }
            bilhetesInstanciados.Clear();
            bilhetesDisponiveis.Clear();
            bilhetesSelecionados.Clear();
            cardBox.gameObject.SetActive(true);
            ShowObjects();
            botaoSelect.onClick.RemoveAllListeners();
            botaoSelect.gameObject.SetActive(false);
            _GameManager.Instance.uiHud.AtualizaMainHud(controle);
        });
    }

    public void OnBilheteClicked(Bilhete bilhete, Button botaoSelect, GameObject bilheteObj)
    {
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
        botaoSelect.gameObject.SetActive(bilhetesSelecionados.Count >= 2);
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
