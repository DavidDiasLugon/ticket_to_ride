using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "Controle", menuName = "Scriptable Objects/Controle")]
public class Controle : ScriptableObject
{
    private List<Jogador> jogadores = new List<Jogador>();
    private DeckCartas deckCartas;
    private List<Carta> cartasAbertas =  new List<Carta>();
    private DeckBilhetes deckBilhetes;
    private Jogador jogadorAtual;
    private int turno = 0; 
    private EstadoJogo estadoAtual;

    void OnEnable()
    {
        deckCartas = CreateInstance<DeckCartas>();
        cartasAbertas = new List<Carta>();
        deckBilhetes = CreateInstance<DeckBilhetes>();
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

    public void Preparacao()
    {
        Carta c;
        Bilhete b;
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

        foreach (Jogador jogador in jogadores)
        {
            for (int i = 0; i < 4; i++)
            {
                b = deckBilhetes.CompraBilhete();
                jogador.MaoBilhetes.Add(b);
            }
        }
    }

    public void CartasAbertas()
    {
        if(cartasAbertas.Count > 0)
        {
            for(int i = 0; i < 6; i++)
            {
                deckCartas.Add(cartasAbertas[i]);
            }
            cartasAbertas.Clear();
            deckCartas.Embaralha();
        }
        Carta c;
        for(int i = 0; i < 6; i++)
        {
            c = deckCartas.CompraCarta();
            cartasAbertas[i] = c;
        }
        VerificaLocomotivas();
    }

    public void VerificaLocomotivas()
    {
        int count = 0; 
        foreach(Carta carta in cartasAbertas)
        {
            if(carta.isLocomotive == true)
            {
                count++;
            }
        }
        if(count >= 3)
        {
            CartasAbertas();
        }
    }

    public void CriaCartas()
    {
        List<Carta> cartas = new List<Carta>();
        List<string> cores = new List<string> {"vermelho", "azul", "amarelo", "verde", "rosa", "preto", "laranja", "branco"};
        foreach(string cor in cores)
        {
            for(int i = 0; i < 12; i++)
            {
                Carta c = CreateInstance<Carta>();
                c.Cor = cor;
                c.isLocomotive = false;
                c.Imagem = Resources.Load<Sprite>("Cartas/" + cor);
                cartas.Add(c);
            }
        }

        for(int i = 0; i < 14; i++)
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

        List<Bilhete> bilhetes = new List<Bilhete>();
        for(int i=0; i < 31; i++)
        {
            Bilhete b = CreateInstance<Bilhete>();
            // Cria bilhetes de destino
            bilhetes.Add(b);
        }
        DeckBilhetes bilhetesDeck = CreateInstance<DeckBilhetes>();
        bilhetesDeck.Deck = bilhetes;
        deckBilhetes = bilhetesDeck;
        deckBilhetes.Embaralha();
    }

    public Jogador getJogadorAtual() {
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
}
