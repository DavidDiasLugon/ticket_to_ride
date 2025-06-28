using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;
using System.Reflection;

public class MockBoardState : IBoardStateProvider
{
    public List<TrackController> MockTracks = new List<TrackController>();

    public IEnumerable<TrackController> GetAvailableTracks()
    {
        return MockTracks.Where(t => t != null && !t.isClaimed);
    }
}


public class AI_Integration_Tests
{
    private Controle controle;
    private Jogador aiPlayer;
    private MockBoardState mockBoard;
    private List<GameObject> trilhosGOs;

    private TrackController CriarTrilhoReal(string cidade1, string cidade2, int custo, TrackColor cor)
    {
        var trackGO = new GameObject($"Track_{cidade1}_{cidade2}");
        var trackController = trackGO.AddComponent<TrackController>();
        var trackData = ScriptableObject.CreateInstance<TrackData>();
        trackData.city1Name = cidade1;
        trackData.city2Name = cidade2;
        trackData.length = custo;
        trackData.color = cor;

        trackController.Initialize(trackData, new List<TrackSegmentController>());
        return trackController;
    }

    [SetUp]
    public void Setup()
    {
        controle = ScriptableObject.CreateInstance<Controle>();
        aiPlayer = ScriptableObject.CreateInstance<Jogador>();
        aiPlayer.Nome = "IA de Teste";
        aiPlayer.isAI = true;
        aiPlayer.Trens = 45;
        aiPlayer.StartDict();

        controle.Jogadores.Add(aiPlayer);
        controle.JogadorAtual = aiPlayer;

        mockBoard = new MockBoardState();
        trilhosGOs = new List<GameObject>();
    }

    [TearDown]
    public void Teardown()
    {
        foreach (var go in trilhosGOs)
        {
            if (go != null)
            {
                UnityEngine.Object.Destroy(go);
            }
        }
        // Limpa a propriedade estática para não afetar outros testes
        SetStaticBoardManagerTracks(null);
    }

    private void SetStaticBoardManagerTracks(List<TrackController> tracks)
    {
        var propertyInfo = typeof(BoardManager).GetProperty("AllTrackControllers", BindingFlags.Public | BindingFlags.Static);
        if (propertyInfo != null)
        {
            propertyInfo.SetValue(null, tracks);
        }
        else
        {
            Assert.Fail("Não foi possível encontrar a propriedade estática 'AllTrackControllers' via Reflection.");
        }
    }

    [UnityTest]
    public IEnumerator IA_DecideConquistarRota_E_ControleExecutaCorretamente()
    {
        LogAssert.Expect(LogType.Error, "TrackController não conseguiu encontrar o GameManager!");
        // --- ARRANGE ---
        var rotaAlvoController = CriarTrilhoReal("Lisboa", "Madrid", 3, TrackColor.Black);
        trilhosGOs.Add(rotaAlvoController.gameObject);

        // CRIA UMA LISTA COM OS TRILHOS DO CENÁRIO
        var listaDeTrilhosDoCenario = new List<TrackController> { rotaAlvoController };

        // 1. Alimenta o board falso para a IA decidir a jogada
        mockBoard.MockTracks = listaDeTrilhosDoCenario;

        // 2. **A LINHA QUE FALTAVA**: Alimenta o BoardManager estático para a lógica interna do jogo
        SetStaticBoardManagerTracks(listaDeTrilhosDoCenario);

        // Configura o resto do cenário para a IA
        var bilhete = ScriptableObject.CreateInstance<Bilhete>();
        bilhete.Rota = new[] { "Lisboa", "Paris" };
        aiPlayer.MaoBilhetes.Add(bilhete);

        for (int i = 0; i < 3; i++)
        {
            var carta = ScriptableObject.CreateInstance<Carta>();
            carta.Cor = "preto";
            aiPlayer.MaoCartas.Add(carta);
        }
        aiPlayer.UpdateNumeroCartasDict();

        int trensAntes = aiPlayer.Trens;
        int cartasAntes = aiPlayer.MaoCartas.Count;

        // --- ACT ---
        var playerAdapter = new PlayerDataAdapter(aiPlayer);
        var aiLogic = new AIController_Refactored(playerAdapter, mockBoard);
        AIAction acaoDecidida = aiLogic.DecideAcaoPrincipal();

        Assert.AreEqual(AIAction.ActionType.ClaimRoute, acaoDecidida.Type);
        controle.ExecutarLogicaConquistaRota(acaoDecidida.Data as TrackController, aiPlayer);

        // --- ASSERT ---
        Assert.IsTrue(rotaAlvoController.isClaimed);
        Assert.AreEqual(aiPlayer.Nome, rotaAlvoController.ownerPlayerName);
        Assert.AreEqual(trensAntes - rotaAlvoController.trackData.length, aiPlayer.Trens);
        Assert.AreEqual(cartasAntes - rotaAlvoController.trackData.length, aiPlayer.MaoCartas.Count);

        yield return null;
    }

    [UnityTest]
    public IEnumerator IA_DecideComprarBilhetes_QuandoTemPoucosObjetivosEMuitosTrens()
    {
        // --- ARRANGE ---
        // Condição: < 2 bilhetes, > 20 trens
        aiPlayer.MaoBilhetes.Clear(); // Garante 0 bilhetes
        aiPlayer.Trens = 30;

        // Preparar o baralho de bilhetes no Controle
        var bilhete1 = ScriptableObject.CreateInstance<Bilhete>();
        var bilhete2 = ScriptableObject.CreateInstance<Bilhete>();
        var bilhete3 = ScriptableObject.CreateInstance<Bilhete>();
        controle.DeckBilhetes.Deck = new List<Bilhete> { bilhete1, bilhete2, bilhete3 };

        int bilhetesAntes = aiPlayer.MaoBilhetes.Count;

        // --- ACT ---
        var playerAdapter = new PlayerDataAdapter(aiPlayer);
        var aiLogic = new AIController_Refactored(playerAdapter, mockBoard); // mockBoard pode estar vazio aqui

        // 1. A IA decide a ação principal
        AIAction acaoDecidida = aiLogic.DecideAcaoPrincipal();
        Assert.AreEqual(AIAction.ActionType.DrawTickets, acaoDecidida.Type, "A IA deveria ter decidido comprar bilhetes.");

        // 2. O Controle executa a lógica de comprar 3 bilhetes
        List<Bilhete> bilhetesDisponiveis = controle.ExecutarLogicaCompraBilhete(3);
        Assert.AreEqual(3, bilhetesDisponiveis.Count, "O controle deveria ter comprado 3 bilhetes.");

        // 3. A IA decide quais bilhetes manter (lógica de decisão separada)
        // A regra é manter no mínimo 1
        AIAction escolhaDeBilhetes = aiLogic.DecideQuaisBilhetesManter(bilhetesDisponiveis, 1);
        var bilhetesEscolhidos = escolhaDeBilhetes.Data as List<Bilhete>;

        // 4. Adicionamos os bilhetes escolhidos à mão do jogador
        foreach (var bilhete in bilhetesEscolhidos)
        {
            aiPlayer.MaoBilhetes.Add(bilhete);
        }

        // --- ASSERT ---
        Assert.IsTrue(aiPlayer.MaoBilhetes.Count > bilhetesAntes, "O número de bilhetes na mão da IA deveria ter aumentado.");
        Assert.AreEqual(1, aiPlayer.MaoBilhetes.Count, "A IA deveria ter mantido o número mínimo de bilhetes.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator IA_DecideComprarCartas_QuandoOutrasAcoesNaoSaoVantajosas()
    {
        // --- ARRANGE ---
        // Condição: A IA não pode conquistar rotas e não precisa de bilhetes.
        aiPlayer.Trens = 15; // Menos de 20 para não querer bilhetes
        aiPlayer.MaoBilhetes.Add(ScriptableObject.CreateInstance<Bilhete>());
        aiPlayer.MaoBilhetes.Add(ScriptableObject.CreateInstance<Bilhete>()); // Garante >= 2 bilhetes

        // O mockBoard está vazio, então não há rotas para conquistar.

        // Preparar o baralho de cartas no Controle
        var carta1 = ScriptableObject.CreateInstance<Carta>();
        var carta2 = ScriptableObject.CreateInstance<Carta>();
        controle.DeckCartas.Deck = new List<Carta> { carta1, carta2 };

        int cartasAntes = aiPlayer.MaoCartas.Count;

        // --- ACT ---
        var playerAdapter = new PlayerDataAdapter(aiPlayer);
        var aiLogic = new AIController_Refactored(playerAdapter, mockBoard);

        // 1. A IA decide a ação principal
        AIAction acaoDecidida = aiLogic.DecideAcaoPrincipal();
        Assert.AreEqual(AIAction.ActionType.DrawCards, acaoDecidida.Type, "A IA deveria ter decidido comprar cartas.");

        // 2. O Controle executa a lógica de comprar 2 cartas do deck
        List<Carta> cartasCompradas = controle.ExecutarLogicaCompraCartaDoDeck(2);
        Assert.AreEqual(2, cartasCompradas.Count);

        // 3. Adicionamos as cartas à mão do jogador
        foreach (var carta in cartasCompradas)
        {
            aiPlayer.MaoCartas.Add(carta);
        }

        // --- ASSERT ---
        Assert.AreEqual(cartasAntes + 2, aiPlayer.MaoCartas.Count, "O número de cartas na mão da IA deveria ter aumentado em 2.");

        yield return null;
    }
}