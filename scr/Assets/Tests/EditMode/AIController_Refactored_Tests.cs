using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine; // Adicionado para usar GameObject e ScriptableObject


// ----- INÍCIO DAS CLASSES MOCK -----
// Classes "falsas" que simulam o jogo para que possamos controlar o cenário de teste.

public class MockPlayerData : IPlayerData
{
    // CORREÇÃO: Instancia o Jogador (um ScriptableObject) da maneira correta.
    private Jogador data = ScriptableObject.CreateInstance<Jogador>();

    // Implementação das propriedades da interface
    public int Trens => data.Trens;
    public IReadOnlyDictionary<string, int> CartasNaMao => data.CartaNmr;
    public IEnumerable<Bilhete> BilhetesNaMao => data.MaoBilhetes;

    // Métodos de Ajuda para Testes (Helpers)
    #region Métodos de Ajuda para Testes
    public void SetTrens(int count)
    {
        data.Trens = count;
    }

    public void AddCartas(string cor, int count)
    {
        if (data.CartaNmr.ContainsKey(cor))
        {
            data.CartaNmr[cor] = count;
        }
        else
        {
            data.CartaNmr.Add(cor, count);
        }
    }

    public void AddBilhete(Bilhete b)
    {
        data.MaoBilhetes.Add(b);
    }

    public void ClearBilhetes()
    {
        data.MaoBilhetes.Clear();
    }
    #endregion
}

public class MockBoardState : IBoardStateProvider
{
    public List<TrackController> MockTracks = new List<TrackController>();

    public IEnumerable<TrackController> GetAvailableTracks()
    {
        return MockTracks.Where(t => t != null && !t.isClaimed);
    }
}
// ----- FIM DAS CLASSES MOCK -----


public class AIController_Refactored_Tests
{
    private MockPlayerData mockPlayer;
    private MockBoardState mockBoard;
    private AIController_Refactored aiController;

    [SetUp]
    public void Setup()
    {
        mockPlayer = new MockPlayerData();
        mockBoard = new MockBoardState();
        aiController = new AIController_Refactored(mockPlayer, mockBoard);
    }

    [Test]
    public void PodeConquistar_ComCartasExatas_RetornaTrue()
    {
        // ARRANGE
        // CORREÇÃO: Instancia TrackData (um ScriptableObject) da maneira correta.
        var rota = ScriptableObject.CreateInstance<TrackData>();
        rota.length = 3;
        rota.color = TrackColor.Blue;

        mockPlayer.AddCartas("azul", 3);

        // ACT
        bool resultado = aiController.PodeConquistar(rota);

        // ASSERT
        Assert.IsTrue(resultado);
    }

    [Test]
    public void PodeConquistar_ComCartasInsuficientes_RetornaFalse()
    {
        // ARRANGE
        // CORREÇÃO: Instancia TrackData (um ScriptableObject) da maneira correta.
        var rota = ScriptableObject.CreateInstance<TrackData>();
        rota.length = 3;
        rota.color = TrackColor.Blue;

        mockPlayer.AddCartas("azul", 2);

        // ACT
        bool resultado = aiController.PodeConquistar(rota);

        // ASSERT
        Assert.IsFalse(resultado);
    }

    [Test]
    public void DecideAcaoPrincipal_QuandoConquistarRotaEhMelhor_RetornaAcaoClaimRoute()
    {
        // ARRANGE
        // CORREÇÃO: Instancia Bilhete e TrackData (ScriptableObjects) e TrackController (MonoBehaviour) corretamente.

        var bilhete = ScriptableObject.CreateInstance<Bilhete>();
        bilhete.Rota = new string[] { "A", "B" };
        bilhete.Pontos = 10;
        mockPlayer.AddBilhete(bilhete);

        // Para criar um MonoBehaviour, primeiro crie um GameObject e depois adicione o componente.
        var rotaGameObject = new GameObject();
        var rotaUtil = rotaGameObject.AddComponent<TrackController>();

        // Crie e configure o TrackData separadamente.
        var trackDataParaRota = ScriptableObject.CreateInstance<TrackData>();
        trackDataParaRota.city1Name = "A";
        trackDataParaRota.city2Name = "C";
        trackDataParaRota.length = 2;
        trackDataParaRota.color = TrackColor.Red;

        // Atribua o TrackData ao TrackController.
        rotaUtil.trackData = trackDataParaRota;

        mockBoard.MockTracks.Add(rotaUtil);
        mockPlayer.AddCartas("vermelho", 2);

        // ACT
        AIAction decisao = aiController.DecideAcaoPrincipal();

        // ASSERT
        Assert.AreEqual(AIAction.ActionType.ClaimRoute, decisao.Type);
        Assert.AreEqual(rotaUtil, decisao.Data);

        // Limpeza (opcional, mas boa prática em testes com GameObjects)
        Object.DestroyImmediate(rotaGameObject);
    }

    [Test]
    public void DecideAcaoPrincipal_QuandoComprarBilhetesEhMelhor_RetornaAcaoDrawTickets()
    {
        // ARRANGE
        mockPlayer.ClearBilhetes();
        mockPlayer.SetTrens(30);

        // ACT
        AIAction decisao = aiController.DecideAcaoPrincipal();

        // ASSERT
        Assert.AreEqual(AIAction.ActionType.DrawTickets, decisao.Type);
    }

    [Test]
    public void DecideAcaoPrincipal_QuandoComprarCartasEhMelhor_RetornaAcaoDrawCards()
    {
        // ARRANGE
        mockPlayer.SetTrens(10);
        mockBoard.MockTracks.Clear();

        // Adiciona um bilhete para que a IA não escolha comprar mais bilhetes
        var bilhete = ScriptableObject.CreateInstance<Bilhete>();
        bilhete.Rota = new string[] { "X", "Y" };
        bilhete.Pontos = 5;
        mockPlayer.AddBilhete(bilhete);


        // ACT
        AIAction decisao = aiController.DecideAcaoPrincipal();

        // ASSERT
        Assert.AreEqual(AIAction.ActionType.DrawCards, decisao.Type);
    }
}