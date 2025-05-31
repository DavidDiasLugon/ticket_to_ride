using UnityEngine;

public class _GameManager : MonoBehaviour
{
    public static _GameManager Instance;
    public Controle controle;
    public UIMao maoCartas;
    public UICartasAbertas cartasAbertas;

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
