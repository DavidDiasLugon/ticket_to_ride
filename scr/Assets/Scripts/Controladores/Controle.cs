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

    public void ProcessarAcao_AIPegaCartaVisivel(int indiceDaCarta)
    {
        // Pega a referência da carta antes de removê-la
        Carta cartaSelecionada = CartasAbertas[indiceDaCarta];

        Debug.Log($"AÇÃO IA: Pegando carta '{cartaSelecionada.Cor}' do índice {indiceDaCarta}.");

        // Adiciona a carta à mão da IA
        JogadorAtual.MaoCartas.Add(cartaSelecionada);

        // Remove a carta da lista de cartas abertas
        CartasAbertas.RemoveAt(indiceDaCarta);

        // Compra uma nova carta do baralho para repor
        Carta c = DeckCartas.CompraCarta();
        if (c != null)
        {
            CartasAbertas.Insert(indiceDaCarta, c);
        }

        // Atualiza a UI e verifica se precisa embaralhar as cartas abertas
        VerificaLocomotivas();
        _GameManager.Instance.cartasAbertas.AtualizaExibicaoCartasAbertas(CartasAbertas);

        // Lógica de transição de estado, agora dentro de um fluxo controlado
        if (cartaSelecionada.isLocomotive)
        {
            TrocaEstado(EstadoFimTurno.CreateInstance<EstadoFimTurno>());
        }
        else
        {
            TrocaEstado(EstadoEspera2.CreateInstance<EstadoEspera2>());
        }
    }
}
