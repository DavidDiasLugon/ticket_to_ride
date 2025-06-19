using UnityEngine;
using UnityEngine;
using UnityEngine.UI; // <<< ADICIONE ESTA LINHA no topo
using System.Collections;
public class _GameManager : MonoBehaviour
{
    public static _GameManager Instance;
    public Controle controle;
    public UIMao maoCartas;
    public UICartasAbertas cartasAbertas;
    public UIHud uiHud;
    public Button botaoCompraCarta;
    public Button botaoCompraBilhete;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        controle = Controle.CreateInstance<Controle>();

        controle.CriaCartas();
        controle.AtualizarCartasAbertas();
        controle.AtribuiJogadores();
        Debug.Log("Jogadores atribuídos: " + controle.Jogadores.Count);
        controle.Preparacao();
        cartasAbertas.AtualizaExibicaoCartasAbertas(controle.CartasAbertas);
        controle.JogadorAtual = controle.Jogadores[0];
        controle.JogadorAtual.UpdateNumeroCartasDict();
        Debug.Log(controle.CartasAbertas.Count);
        controle.TrocaEstado(EstadoEspera.CreateInstance<EstadoEspera>());
    }

    void Update()
    {
        controle.RunEstadoAtual();
    }
}
