using System.Collections.Generic;
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

    public bool RodadaFinalComecou
    {
        get => rodadaFinalComecou;
    }

    public string NomeJogadorQueIniciouFinal
    {
        get => nomeJogadorQueIniciouFinal;
    }

    public List<Jogador> Jogadores
    {
        get => jogadores;
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
}
