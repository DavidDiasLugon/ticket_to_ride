using UnityEngine;

// Esta linha permite criar assets deste tipo no menu do Unity: Assets -> Create -> TicketToRide -> Track Data
[CreateAssetMenu(fileName = "NewTrackData", menuName = "TicketToRide/Track Data")]
public class TrackData : ScriptableObject
{
    [Header("Informações Principais")] // Ajuda a organizar no Inspector

    [Tooltip("Nome exato da primeira cidade (deve corresponder ao cityName no CityController)")]
    public string city1Name;

    [Tooltip("Nome exato da segunda cidade (deve corresponder ao cityName no CityController)")]
    public string city2Name;

    [Tooltip("Número de segmentos/vagões neste trilho")]
    [Min(1)] // Garante que o comprimento seja pelo menos 1
    public int length = 1;

    [Tooltip("Cor necessária para reivindicar este trilho (Use 'Gray' para trilhos neutros)")]
    public TrackColor color; // Define um valor padrão

    [Header("Configuração de Trilha Dupla")] // Ajuda a organizar no Inspector

    [Tooltip("Marque esta caixa se este trilho faz parte de um par de trilhos paralelos entre as mesmas duas cidades.")]
    public bool isDoubleTrack = false; // Valor padrão é falso

    [Tooltip("Se 'Is Double Track' estiver marcado, arraste o asset TrackData do trilho 'gêmeo' para este campo. Deixe como 'None' (vazio) se não for duplo ou se o gêmeo ainda não foi criado.")]
    public TrackData twinTrack = null; // Referência ao outro trilho do par (deve ser preenchido manualmente no Inspector)

    // --- Validação Opcional no Editor ---
    // Este método é chamado quando um valor é alterado no Inspector do Unity Editor.
    // Ajuda a garantir que os dados sejam consistentes.
#if UNITY_EDITOR
    void OnValidate()
    {
        // Garante que um trilho duplo tenha um gêmeo definido (opcional, mas bom)
        // if (isDoubleTrack && twinTrack == null)
        // {
        //     Debug.LogWarning($"TrackData '{this.name}' está marcado como Double Track, mas Twin Track não está definido.", this);
        // }

        // Garante que um trilho não seja seu próprio gêmeo
        if (twinTrack == this)
        {
            Debug.LogError($"TrackData '{this.name}': Um trilho não pode ser seu próprio gêmeo (Twin Track). Removendo a referência.", this);
            twinTrack = null;
        }

        // Garante que se este for gêmeo de outro, o outro também aponta para este (mais complexo de validar aqui)
        // Você pode adicionar mais validações conforme necessário.

        // Garante que City1 e City2 não sejam iguais
        if (!string.IsNullOrEmpty(city1Name) && !string.IsNullOrEmpty(city2Name) && city1Name.Equals(city2Name, System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.LogError($"TrackData '{this.name}': City 1 Name e City 2 Name não podem ser iguais ('{city1Name}').", this);
        }
    }
#endif
}