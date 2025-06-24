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
        HideMaoBilhetes();
        mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        prefabBilhete = Resources.Load<GameObject>("Prefabs/Carta/BilheteNew");

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
        
        if (controle.JogadorAtual.isAI)
        {
            _GameManager.Instance.canvasGroupCartasAbertas.blocksRaycasts = false;
            _GameManager.Instance.canvasGroupCartasAbertas.interactable = false;
            _GameManager.Instance.canvasGroupGameBoard.blocksRaycasts = false;
            _GameManager.Instance.canvasGroupGameBoard.interactable = false;
            foreach (Transform child in _GameManager.Instance.MaoJogador.gameObject.transform)
            {
                GameObject carta = child.gameObject;
                carta.GetComponent<CanvasGroup>().interactable = false;
                carta.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            _GameManager.Instance.canvasGroupMaoJogador.alpha = 0;
            _GameManager.Instance.canvasGroupMainCanvas.blocksRaycasts = false;
            _GameManager.Instance.canvasGroupMainCanvas.interactable = false;
            _GameManager.Instance.canvasGroupTrackContainer.blocksRaycasts = false;
            _GameManager.Instance.canvasGroupTrackContainer.interactable = false;
            TextMeshProUGUI anuncio = _GameManager.Instance.anuncios;
            anuncio.gameObject.SetActive(true);
            _GameManager.Instance.StartCoroutine(ExecutarTurnoIA(controle));
            return;
        }

        _GameManager.Instance.canvasGroupCartasAbertas.blocksRaycasts = true;
        _GameManager.Instance.canvasGroupCartasAbertas.interactable = true;
        _GameManager.Instance.canvasGroupGameBoard.blocksRaycasts = true;
        _GameManager.Instance.canvasGroupGameBoard.interactable = true;
        foreach (Transform child in _GameManager.Instance.MaoJogador.gameObject.transform)
        {
            GameObject carta = child.gameObject;
            carta.GetComponent<CanvasGroup>().interactable = true;
            carta.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        _GameManager.Instance.canvasGroupMaoJogador.alpha = 1;
        _GameManager.Instance.canvasGroupMainCanvas.blocksRaycasts = true;
        _GameManager.Instance.canvasGroupMainCanvas.interactable = true;
        _GameManager.Instance.canvasGroupTrackContainer.blocksRaycasts = true;
        _GameManager.Instance.canvasGroupTrackContainer.interactable = true;
        botaoCompraCarta = GameObject.Find("BotaoCarta")?.GetComponent<Button>();
        botaoCompraBilhete = GameObject.Find("BotaoBilhete")?.GetComponent<Button>();
        if (botaoCompraCarta == null || botaoCompraBilhete == null)
        {
            Debug.LogError("Botões de compra não encontrados!");
            return;
        }
        if (controle.DeckCartas.Deck.Count > 0)
        {
            botaoCompraCarta.interactable = true;
        }
        else
        {
            botaoCompraCarta.interactable = false;
        }
        botaoCompraCarta.onClick.RemoveAllListeners();
        botaoCompraCarta.onClick.AddListener(() =>
        {
            Debug.Log("Botão Compra Carta Clicado");
            botaoCompraCarta.onClick.RemoveAllListeners();
            botaoCompraBilhete.onClick.RemoveAllListeners();
            controle.TrocaEstado(EstadoCompraCarta1.CreateInstance<EstadoCompraCarta1>());
        });
        if (controle.DeckBilhetes.Deck.Count == 0)
        {
            botaoCompraBilhete.interactable = false;
        }
        else
        {
            botaoCompraBilhete.interactable = true;
        }
        botaoCompraBilhete.onClick.RemoveAllListeners();
        botaoCompraBilhete.onClick.AddListener(() =>
        {
            Debug.Log("Botão Compra Bilhete Clicado");
            botaoCompraBilhete.onClick.RemoveAllListeners();
            botaoCompraCarta.onClick.RemoveAllListeners();
            controle.TrocaEstado(EstadoCompraBilhete.CreateInstance<EstadoCompraBilhete>());
        });

        if (!controle.JogadorAtual.SelecionouBilhetes)
        {
            SelecionaBilhete(controle);
        }
    }

    public override void RunEstado(Controle controle)
    {
    }

    public override void ProcessarSelecao(Controle controle, int indice, Carta cartaSelecionada)
    {
        FindAnyObjectByType<AudioManager>().Play("DrawCard");
        Debug.Log("Processando seleção de carta");
        controle.JogadorAtual.MaoCartas.Add(cartaSelecionada);
        controle.CartasAbertas.RemoveAt(indice);
        Carta c = controle.DeckCartas.CompraCarta();
        if (c != null)
        {
            controle.CartasAbertas.Add(c);
        }
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

        foreach (Bilhete bilhete in bilhetesDisponiveis)
        {
            GameObject bilheteObj = Instantiate(prefabBilhete, selectionBox);

            BilheteHover bilheteHover = bilheteObj.GetComponent<BilheteHover>();
            if (bilheteHover != null)
            {
                bilheteHover.Inicializacao(bilhete.Rota[0], bilhete.Rota[1]);
            }

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
            HideMaoBilhetes();
            controle.JogadorAtual.SelecionouBilhetes = true;
            FindAnyObjectByType<AudioManager>().Play("DrawCard");
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
        });
    }

    public void OnBilheteClicked(Bilhete bilhete, Button botaoSelect, GameObject bilheteObj)
    {
        HideMaoBilhetes();
        FindAnyObjectByType<AudioManager>().Play("TicketClick");
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

    public void HideMaoBilhetes()
    {
        UIMaoBilhetes maoBilhetes = FindAnyObjectByType<UIMaoBilhetes>();
        if (maoBilhetes != null)
        {
            maoBilhetes.FecharPainel();
        }
    }

    IEnumerator ExecutarTurnoIA(Controle controle)
    {
        TextMeshProUGUI anuncio = _GameManager.Instance.anuncios;
        anuncio.gameObject.SetActive(true);

        anuncio.text = $"Início do turno de {controle.JogadorAtual.Nome}";
        yield return new WaitForSeconds(3.0f);

        anuncio.text = $"{controle.JogadorAtual.Nome} está decidindo sua jogada";
        yield return new WaitForSeconds(3.0f);

        AIController ai = new AIController(controle);

        if (!controle.JogadorAtual.SelecionouBilhetes)
        {
            List<Bilhete> bilhetesDisponíveis = new List<Bilhete>();
            for (int i = 0; i < 3; i++)
            {
                Bilhete b = controle.DeckBilhetes.CompraBilhete();
                if (b != null)
                {
                    bilhetesDisponíveis.Add(b);
                }
            }
            int minTokeep = 2;
            List<Bilhete> bilhetesEscolhidos = ai.ChooseTickets(bilhetesDisponíveis, minTokeep);
            foreach (var bilhete in bilhetesEscolhidos)
            {
                controle.JogadorAtual.MaoBilhetes.Add(bilhete);
            }
            List<Bilhete> bilhetesRestantes = bilhetesDisponíveis.Except(bilhetesEscolhidos).ToList();
            foreach (var bilhete in bilhetesRestantes)
            {
                controle.DeckBilhetes.Deck.Add(bilhete);
            }
            controle.DeckBilhetes.Embaralha();
            _GameManager.Instance.uiHud.AtualizaMainHud(controle);
            controle.JogadorAtual.SelecionouBilhetes = true;
        }
        ai.ExecuteMainTurnAction();
        yield return _GameManager.Instance.StartCoroutine(ai.ExecuteMainTurnAction());
    }
}
