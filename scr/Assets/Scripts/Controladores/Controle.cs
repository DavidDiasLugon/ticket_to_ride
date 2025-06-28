using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "Controle", menuName = "Scriptable Objects/Controle")]
public class Controle : ScriptableObject
{
    private List<Jogador> jogadores = new List<Jogador>();
    private DeckCartas deckCartas;
    private List<Carta> cartasAbertas = new List<Carta>();
    private DeckBilhetes deckBilhetes;
    private Jogador jogadorAtual;
    private int turno = 0;
    private EstadoJogo estadoAtual;
    private TextAsset ticketsTxt;

    private bool rodadaFinalComecou = false;
    private string nomeJogadorQueIniciouFinal = "";

    void OnEnable()
    {
        deckCartas = CreateInstance<DeckCartas>();
        cartasAbertas = new List<Carta>();
        deckBilhetes = CreateInstance<DeckBilhetes>();
        ticketsTxt = Resources.Load<TextAsset>("Tickets");
    }

    public List<Jogador> Jogadores
    {
        get => jogadores;
    }

    public bool RodadaFinalComecou
    {
        get => rodadaFinalComecou;
    }

    public string NomeJogadorQueIniciouFinal
    {
        get => nomeJogadorQueIniciouFinal;
    }

    public EstadoJogo EstadoAtual
    {
        get => estadoAtual;
        set => estadoAtual = value;
    }

    public DeckCartas DeckCartas
    {
        get => deckCartas;
    }

    public DeckBilhetes DeckBilhetes
    {
        get => deckBilhetes;
    }

    public Jogador JogadorAtual
    {
        get => jogadorAtual;
        set => jogadorAtual = value;
    }

    public int Turno
    {
        get => turno;
        set => turno = value;
    }

    public List<Carta> CartasAbertas
    {
        get => cartasAbertas;
    }

    public void Preparacao()
    {
        Carta c;
        foreach (Jogador jogador in jogadores)
        {
            for (int i = 0; i < 5; i++)
            {
                c = deckCartas.CompraCarta();
                jogador.MaoCartas.Add(c);
            }
            jogador.StartDict();
            jogador.UpdateNumeroCartasDict();
        }
    }

    public void AtualizarCartasAbertas()
    {
        if (cartasAbertas.Count > 0)
        {
            foreach (Carta carta in cartasAbertas)
            {
                if (carta != null)
                {
                    deckCartas.Add(carta);
                }
            }
            cartasAbertas.Clear();
            deckCartas.Embaralha();
        }
        Carta c;
        for (int i = 0; i < 5; i++)
        {
            c = deckCartas.CompraCarta();
            if (c != null)
            {
                cartasAbertas.Add(c);
            }
        }
        VerificaLocomotivas();
    }

    public void VerificaLocomotivas()
    {
        int count = 0;
        foreach (Carta carta in cartasAbertas)
        {
            if (carta.isLocomotive == true)
            {
                count++;
            }
        }
        if (count >= 3)
        {
            AtualizarCartasAbertas();
        }
    }

    public void CriaCartas()
    {
        List<Carta> cartas = new List<Carta>();
        List<string> cores = new List<string> { "vermelho", "azul", "amarelo", "verde", "rosa", "preto", "laranja", "branco" };
        foreach (string cor in cores)
        {
            for (int i = 0; i < 12; i++)
            {
                Carta c = CreateInstance<Carta>();
                c.Cor = cor;
                c.isLocomotive = false;
                c.Imagem = Resources.Load<Sprite>("Cartas/" + cor);
                cartas.Add(c);
            }
        }

        for (int i = 0; i < 14; i++)
        {
            Carta c = CreateInstance<Carta>();
            c.Cor = "colorido";
            c.isLocomotive = true;
            cartas.Add(c);
        }

        DeckCartas cartasDeck = CreateInstance<DeckCartas>();
        cartasDeck.Deck = cartas;
        deckCartas = cartasDeck;
        deckCartas.Embaralha();

        string[] lines = ticketsTxt.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        List<Bilhete> bilhetes = new List<Bilhete>();
        foreach (string line in lines)
        {
            string[] parts = line.Split("_");
            string origem = parts[0].Trim();
            string destino = parts[1].Trim();
            int pontos = int.Parse(parts[2].Trim());
            Bilhete b = CreateInstance<Bilhete>();
            b.Rota = new string[] { origem, destino };
            b.Pontos = pontos;
            bilhetes.Add(b);
        }
        DeckBilhetes bilhetesDeck = CreateInstance<DeckBilhetes>();
        bilhetesDeck.Deck = bilhetes;
        deckBilhetes = bilhetesDeck;
        deckBilhetes.Embaralha();
    }

    public Jogador getJogadorAtual()
    {
        int index = turno % jogadores.Count;
        return jogadores[index];
    }

    public void AtribuiJogadores()
    {
        jogadores = GameSettings.jogadores;
    }

    public void TrocaEstado(EstadoJogo novoEstado)
    {
        estadoAtual = novoEstado;
        estadoAtual.IniciarEstado(this);
    }

    public void RunEstadoAtual()
    {
        estadoAtual.RunEstado(this);
    }

    public void CartaSelecionada(int index, Carta cartaSelecionada)
    {
        estadoAtual.ProcessarSelecao(this, index, cartaSelecionada);
    }

    public void ConquistarRota(Draggable draggable, TrackController trackController)
    {
        if (estadoAtual is EstadoEspera)
        {
            EstadoConquista estadoConquista = CreateInstance<EstadoConquista>();
            estadoConquista.dadosDaConquista = (draggable, trackController);
            TrocaEstado(estadoConquista);
        }
        else
        {
            Debug.Log("Não é possível conquistar");
            FindAnyObjectByType<AudioManager>().Play("FailedConquest");
            draggable.RetornarParaMao();
        }
    }

    public void ReporMesaSeNecessario()
    {
        while (cartasAbertas.Count < 5 && deckCartas.Deck.Count > 0)
        {
            Carta c = deckCartas.CompraCarta();
            if (c != null)
            {
                cartasAbertas.Add(c);
            }
            else
            {
                break;
            }
        }
        VerificaLocomotivas();
        _GameManager.Instance.cartasAbertas.AtualizaExibicaoCartasAbertas(cartasAbertas);
    }

    public void IniciarRodadaFinal(string nomeDoJogador)
    {
        if (!rodadaFinalComecou)
        {
            rodadaFinalComecou = true;
            nomeJogadorQueIniciouFinal = nomeDoJogador;
            Debug.Log($"Rodada Final Iniciada por: {nomeDoJogador}");
        }
    }

    public void ProcessarAcaoAIConquistaRota(TrackController trilho)
    {
        Jogador jogadorAI = JogadorAtual;
        TrackData dadosTrilho = trilho.trackData;
        int custo = dadosTrilho.length;

        string corAPagar = "";
        if (dadosTrilho.color == TrackColor.Gray)
        {
            corAPagar = jogadorAI.CartaNmr.Where(par => par.Key != "colorido").OrderByDescending(par => par.Value).FirstOrDefault().Key;
        }
        else
        {
            corAPagar = AIController.ConverteTrackColorParaString(dadosTrilho.color);
        }

        int cartasDaCor = jogadorAI.CartaNmr.ContainsKey(corAPagar) ? jogadorAI.CartaNmr[corAPagar] : 0;
        int cartasParaRemover = Mathf.Min(custo, cartasDaCor);
        jogadorAI.RemoverCartasPorCor(corAPagar, cartasParaRemover, this);

        int locomotivasARemover = custo - cartasParaRemover;
        if (locomotivasARemover > 0)
        {
            jogadorAI.RemoverCartasPorCor("colorido", locomotivasARemover, this);
        }
        int[] tabelaPontos = { 0, 1, 2, 4, 7, 10, 15 };
        jogadorAI.Pontuacao += tabelaPontos[custo];
        jogadorAI.Trens -= custo;

        Color corDoPlayer = FindAnyObjectByType<UIHud>().GetColor(jogadorAI);
        trilho.Claim(jogadorAI.Nome, corDoPlayer);
        FindAnyObjectByType<AudioManager>().Play("RouteConquered");

        jogadorAI.UpdateNumeroCartasDict();
        _GameManager.Instance.maoCartas.AtualizaExibicao(jogadorAI.CartaNmr);
        _GameManager.Instance.uiHud.AtualizaMainHud(this);
        ReporMesaSeNecessario();
        EstadoConquista ec = EstadoConquista.CreateInstance<EstadoConquista>();
        ec.VerificarBilhetesCompletos(this);
        if (jogadorAI.Trens <= 2 && !this.RodadaFinalComecou)
        {
            this.IniciarRodadaFinal(jogadorAI.Nome);
        }
        //TrocaEstado(EstadoFimTurno.CreateInstance<EstadoFimTurno>());
    }

    public void ExecutarLogicaConquistaRota(TrackController trilho, Jogador jogador)
    {
        TrackData dadosTrilho = trilho.trackData;
        int custo = dadosTrilho.length;

        // Lógica para determinar a cor do pagamento (simplificada para o exemplo)
        string corAPagar = "";
        if (dadosTrilho.color == TrackColor.Gray)
        {
            corAPagar = jogador.CartaNmr.Where(par => par.Key != "colorido").OrderByDescending(par => par.Value).FirstOrDefault().Key;
        }
        else
        {
            corAPagar = AIController.ConverteTrackColorParaString(dadosTrilho.color);
        }

        // Lógica pura de dedução de recursos
        int cartasDaCor = jogador.CartaNmr.ContainsKey(corAPagar) ? jogador.CartaNmr[corAPagar] : 0;
        int cartasParaRemover = Mathf.Min(custo, cartasDaCor);
        jogador.RemoverCartasPorCor(corAPagar, cartasParaRemover, this);

        int locomotivasARemover = custo - cartasParaRemover;
        if (locomotivasARemover > 0)
        {
            jogador.RemoverCartasPorCor("colorido", locomotivasARemover, this);
        }

        int[] tabelaPontos = { 0, 1, 2, 4, 7, 10, 15 };
        jogador.Pontuacao += tabelaPontos[custo];
        jogador.Trens -= custo;

        // A única dependência externa é o próprio trilho, o que é perfeito para o teste.
        // Nota: O Claim() precisará ser ajustado para não depender da cor, ou passamos uma cor padrão.
        // Vamos assumir que podemos passar uma cor padrão para o teste.
        trilho.Claim(jogador.Nome, Color.black); // Usamos uma cor qualquer, já que a UI não será vista.

        // Lógica de verificação de bilhetes e fim de jogo
        EstadoConquista.CreateInstance<EstadoConquista>().VerificarBilhetesCompletos(this);
        if (jogador.Trens <= 2 && !this.RodadaFinalComecou)
        {
            this.IniciarRodadaFinal(jogador.Nome);
        }
    }

    public void ProcessarAcaoAIComprabilhete()
    {
        List<Bilhete> bilhetesDisponíveis = new List<Bilhete>();
        for (int i = 0; i < 3; i++)
        {
            Bilhete b = DeckBilhetes.CompraBilhete();
            if (b != null)
            {
                bilhetesDisponíveis.Add(b);
            }
        }
        if (bilhetesDisponíveis.Count == 0)
        {
            ProcessarAcaoAICompraCarta();
            return;
        }
        AIController ai = new AIController(this);
        int minTokeep = 1;
        List<Bilhete> bilhetesEscolhidos = ai.ChooseTickets(bilhetesDisponíveis, minTokeep);
        foreach (var bilhete in bilhetesEscolhidos)
        {
            JogadorAtual.MaoBilhetes.Add(bilhete);
            FindAnyObjectByType<AudioManager>().Play("DrawCard");
        }
        List<Bilhete> bilhetesRestantes = bilhetesDisponíveis.Except(bilhetesEscolhidos).ToList();
        foreach (var bilhete in bilhetesRestantes)
        {
            DeckBilhetes.Deck.Add(bilhete);
        }
        DeckBilhetes.Embaralha();
        _GameManager.Instance.uiHud.AtualizaMainHud(this);
        //TrocaEstado(EstadoFimTurno.CreateInstance<EstadoFimTurno>());
    }

    public void ProcessarAcaoAICompraCarta()
    {
        AIController ai = new AIController(this);
        int indicePrimeiraCarta = ai.ExecuteFirstCardDrawAction(this);
        Carta primeiraCarta;
        if (indicePrimeiraCarta == -1)
        {
            primeiraCarta = DeckCartas.CompraCarta();
        }
        else
        {
            primeiraCarta = CartasAbertas[indicePrimeiraCarta];
            CartasAbertas.RemoveAt(indicePrimeiraCarta);
            Carta reposicao1 = DeckCartas.CompraCarta();
            if (reposicao1 != null)
            {
                cartasAbertas.Insert(indicePrimeiraCarta, reposicao1);
            }
        }

        if (primeiraCarta != null)
        {
            JogadorAtual.MaoCartas.Add(primeiraCarta);
            FindAnyObjectByType<AudioManager>().Play("DrawCard");
        }

        if (indicePrimeiraCarta != -1 && primeiraCarta != null && primeiraCarta.isLocomotive)
        {
            FinalizarTurnoIA();
            return;
        }


        int indiceSegundaCarta = ai.ExecuteSecondCardDrawAction(this);
        Carta segundaCarta;
        if (indiceSegundaCarta == -1)
        {
            segundaCarta = DeckCartas.CompraCarta();
        }
        else
        {
            segundaCarta = cartasAbertas[indiceSegundaCarta];
            CartasAbertas.RemoveAt(indiceSegundaCarta);
            Carta reposicao2 = DeckCartas.CompraCarta();
            if (reposicao2 != null)
            {
                CartasAbertas.Insert(indiceSegundaCarta, reposicao2);
            }
            if (segundaCarta != null)
            {
                JogadorAtual.MaoCartas.Add(segundaCarta);
                FindAnyObjectByType<AudioManager>().Play("DrawCard");
            }
            FinalizarTurnoIA();
        }
    }

    private void FinalizarTurnoIA()
    {
        JogadorAtual.UpdateNumeroCartasDict();
        _GameManager.Instance.maoCartas.AtualizaExibicao(JogadorAtual.CartaNmr);
        _GameManager.Instance.cartasAbertas.AtualizaExibicaoCartasAbertas(CartasAbertas);
        VerificaLocomotivas();
        _GameManager.Instance.uiHud.AtualizaMainHud(this);
        //TrocaEstado(EstadoFimTurno.CreateInstance<EstadoFimTurno>());
    }
}
