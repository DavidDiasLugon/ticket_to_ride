using UnityEngine;

public class _GameManager : MonoBehaviour
{
    public static _GameManager Instance;
    public Controle controle;
    public UIMao maoCartas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {   
        controle = Controle.CreateInstance<Controle>();

        controle.CriaCartas();
        controle.AtribuiJogadores();
        Debug.Log("Jogadores atribuídos: " + controle.Jogadores.Count);
        controle.Preparacao();
        controle.JogadorAtual = controle.Jogadores[0];
        controle.JogadorAtual.UpdateNumeroCartasDict();

        controle.TrocaEstado(EstadoEspera.CreateInstance<EstadoEspera>());
    }

    void Update()
    {
        controle.RunEstadoAtual();
    }
}
