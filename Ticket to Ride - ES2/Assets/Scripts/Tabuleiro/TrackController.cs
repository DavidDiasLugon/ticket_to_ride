using System.Collections.Generic;
using UnityEngine;

public class TrackController : MonoBehaviour
{
    // --- Variáveis ---
    public TrackData trackData;
    public List<TrackSegmentController> segments = new List<TrackSegmentController>();
    public bool isClaimed { get; private set; } = false;
    public int ownerPlayerId { get; private set; } = -1;
    private GameManager gameManager;
    private bool isSelected = false;

    // --- Cores ---
    private static readonly Color HOVER_COLOR = new Color(1f, 1f, 0.7f, 1f); // Amarelo claro (Hover)
    private static readonly Color SELECTED_COLOR = new Color(0.7f, 1f, 0.7f, 1f); // Verde claro (Selecionado)

    // NOVO: Contador para saber quantos segmentos estão com o mouse sobre
    private int mouseOverSegmentCount = 0;

    // --- Inicialização ---
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null) Debug.LogError("TrackController não conseguiu encontrar o GameManager!");
        // InitializeVisuals é chamado por Initialize agora
    }

    public void Initialize(TrackData data, List<TrackSegmentController> segmentList)
    {
        trackData = data;
        segments = segmentList;
        foreach (var segment in segments)
        {
            if (segment != null) segment.parentTrackController = this;
            else Debug.LogWarning($"Segmento nulo encontrado ao inicializar TrackController para {trackData?.name}", this);
        }
        InitializeVisuals();
    }

    void InitializeVisuals()
    {
        if (trackData == null || segments == null || segments.Count == 0) return;
        Color baseColor = GetColorFromEnum(trackData.color);
        int sortOrder = Mathf.RoundToInt(-transform.position.y * 10);
        foreach (var segment in segments)
        {
            segment?.SetVisuals(baseColor, sortOrder, false);
        }
    }

    // --- Lógica de Interação ---

    // Chamado pelo Segmento ao ser clicado
    public void HandleSelectionAttempt()
    {
        if (gameManager != null)
        {
            gameManager.SelectTrack(this);
        }
    }

    // NOVO: Chamado pelo TrackSegmentController quando o mouse entra em um segmento
    public void NotifySegmentMouseEnter()
    {
        int previousCount = mouseOverSegmentCount;
        mouseOverSegmentCount++;
        Debug.Log($"TrackController {gameObject.name}: NotifySegmentMouseEnter. Count: {previousCount} -> {mouseOverSegmentCount}. IsSelected: {isSelected}, IsClaimed: {isClaimed}");
        if (mouseOverSegmentCount == 1 && !isSelected && !isClaimed)
        {
            Debug.Log($"TrackController {gameObject.name}: Aplicando HOVER highlight.");
            SetOverallHighlightState(true, HOVER_COLOR); // HOVER_COLOR aqui
        }
    }

    // NOVO: Chamado pelo TrackSegmentController quando o mouse sai de um segmento
    public void NotifySegmentMouseExit()
    {
        int previousCount = mouseOverSegmentCount;
        mouseOverSegmentCount--;
        if (mouseOverSegmentCount < 0) mouseOverSegmentCount = 0; // Segurança
        Debug.Log($"TrackController {gameObject.name}: NotifySegmentMouseExit. Count: {previousCount} -> {mouseOverSegmentCount}. IsSelected: {isSelected}, IsClaimed: {isClaimed}");
        if (mouseOverSegmentCount == 0 && !isSelected && !isClaimed)
        {
            Debug.Log($"TrackController {gameObject.name}: Removendo HOVER highlight.");
            SetOverallHighlightState(false, HOVER_COLOR); // HOVER_COLOR aqui
        }
    }

    // ANTIGO MÉTODO: HighlightTrack - REMOVER OU SUBSTITUIR PELA LÓGICA ACIMA
    // public void HighlightTrack(bool highlight)
    // {
    //     if (isSelected || isClaimed) return;
    //     foreach (var segment in segments)
    //     {
    //         segment?.SetHighlight(highlight, HOVER_COLOR);
    //     }
    // }

    // NOVO: Método auxiliar para aplicar/remover highlight em todos os segmentos
    private void SetOverallHighlightState(bool highlightActiveStateForHover, Color colorForHover) // Parâmetros renomeados para clareza
    {
        Debug.Log($"TrackController {gameObject.name}: SetOverallHighlightState - highlightActiveForHover: {highlightActiveStateForHover}, colorForHover: {colorForHover}, isClaimed: {isClaimed}, isSelected: {isSelected}");

        if (isClaimed)
        {
            Debug.Log($"TrackController {gameObject.name}: IS CLAIMED. Forçando remoção de highlight.");
            foreach (var segment in segments)
            {
                segment?.SetHighlight(false, Color.clear); // Desliga qualquer brilho
            }
            return;
        }

        if (isSelected)
        {
            Debug.Log($"TrackController {gameObject.name}: IS SELECTED. Aplicando SELECTED_COLOR.");
            foreach (var segment in segments)
            {
                segment?.SetHighlight(true, SELECTED_COLOR);
            }
        }
        else
        { // Não reivindicado e não selecionado, então lida com o hover
            Debug.Log($"TrackController {gameObject.name}: NÃO SELECIONADO/REIVINDICADO. Aplicando estado de hover: {highlightActiveStateForHover}");
            foreach (var segment in segments)
            {
                // Usa HOVER_COLOR explicitamente quando não selecionado
                segment?.SetHighlight(highlightActiveStateForHover, HOVER_COLOR);
            }
        }
    }


    public void SetSelected(bool select)
    {
        if (isClaimed)
        {
            Debug.Log($"TrackController {gameObject.name}: Tentativa de SetSelected({select}) em trilho REIVINDICADO. Ignorando.");
            return;
        }
        Debug.Log($"TrackController {gameObject.name}: SetSelected({select}). Estado anterior de isSelected: {isSelected}");
        isSelected = select;

        if (isSelected)
        {
            SetOverallHighlightState(true, SELECTED_COLOR); // O segundo parâmetro aqui é mais para consistência da chamada
        }
        else // Foi desselecionado
        {
            if (mouseOverSegmentCount > 0) // Se o mouse ainda está sobre, re-aplica hover
            {
                Debug.Log($"TrackController {gameObject.name}: Deselecionado, mouse ainda sobre ({mouseOverSegmentCount} segmentos). Reaplicando HOVER.");
                SetOverallHighlightState(true, HOVER_COLOR);
            }
            else // Mouse não está sobre, remove qualquer highlight
            {
                Debug.Log($"TrackController {gameObject.name}: Deselecionado, mouse não está sobre. Removendo highlight.");
                SetOverallHighlightState(false, HOVER_COLOR);
            }
        }
    }

    public void Claim(int playerId, Color playerColor)
    {
        if (isClaimed) return;
        Debug.Log($"TrackController {gameObject.name}: Claiming. Forçando isSelected=false.");
        isClaimed = true;
        isSelected = false; // Não pode estar selecionado e reivindicado ao mesmo tempo para highlight
        ownerPlayerId = playerId;
        Debug.Log($"Trilho {gameObject.name} reivindicado pelo Player {playerId}");

        // Remove qualquer highlight de hover/seleção antes de aplicar visual de claim
        SetOverallHighlightState(false, Color.clear); // Desliga highlights

        int sortOrder = Mathf.RoundToInt(-transform.position.y * 10) + 1;
        foreach (var segment in segments)
        {
            segment?.SetVisuals(playerColor, sortOrder, true);
        }
        SetOverallHighlightState(false, Color.clear);
    }

    public void Unclaim()
    {
        if (!isClaimed) return;
        Debug.Log($"TrackController {gameObject.name}: Unclaiming. Forçando isSelected=false.");
        isClaimed = false;
        ownerPlayerId = -1;
        isSelected = false;
        Debug.Log($"Trilho {gameObject.name} liberado.");

        // Remove visual de claim e restaura cor base. Também desliga highlights.
        Color baseColor = GetColorFromEnum(trackData.color);
        int sortOrder = Mathf.RoundToInt(-transform.position.y * 10);
        foreach (var segment in segments)
        {
            segment?.SetVisuals(baseColor, sortOrder, false); // Isso já zera o HighlightIntensityProp no shader
        }

        // Reavalia o estado de hover caso o mouse esteja sobre ele
        if (mouseOverSegmentCount > 0)
        {
            Debug.Log($"TrackController {gameObject.name}: Unclaimed, mouse ainda sobre ({mouseOverSegmentCount} segmentos). Reaplicando HOVER.");
            SetOverallHighlightState(true, HOVER_COLOR);
        }
        else
        {
            Debug.Log($"TrackController {gameObject.name}: Unclaimed, mouse não está sobre. Removendo highlight.");
            SetOverallHighlightState(false, HOVER_COLOR);
        }
    }

    // --- Feedback de Falha (mantido como no seu código) ---
    private void TriggerFailureFeedback(GameManager.ClaimResult reason) // Adicionado GameManager.ClaimResult
    {
        if (isClaimed) return;
        // A cor original é gerenciada pelo SetHighlight agora, não precisamos mais da cor original do segmento aqui
        StartCoroutine(FlashFeedbackColor(Color.red, 0.3f));
    }

    private System.Collections.IEnumerator FlashFeedbackColor(Color flashColor, float duration)
    {
        // Salva o estado atual do highlight para restaurar depois
        bool originalHighlightState = (mouseOverSegmentCount > 0 || isSelected) && !isClaimed;
        Color originalHighlightColor = isSelected ? SELECTED_COLOR : HOVER_COLOR;

        // Aplica o flash
        foreach (var segment in segments)
        {
            segment?.SetHighlight(true, flashColor);
        }

        yield return new WaitForSeconds(duration);

        // Restaura o estado de highlight anterior ao flash
        // Verifica se não foi reivindicado durante o flash
        if (!isClaimed)
        {
            if (isSelected)
            { // Se selecionado, volta para cor de seleção
                SetOverallHighlightState(true, SELECTED_COLOR);
            }
            else if (mouseOverSegmentCount > 0)
            { // Se mouse sobre, volta para cor de hover
                SetOverallHighlightState(true, HOVER_COLOR);
            }
            else
            { // Senão, desliga highlight
                SetOverallHighlightState(false, HOVER_COLOR);
            }
        }
        // Se foi reivindicado, o Claim() já cuidou do visual.
    }


    public static Color GetColorFromEnum(TrackColor trackColor)
    {
        switch (trackColor)
        {
            case TrackColor.Red: return Color.red;
            case TrackColor.Blue: return Color.blue;
            case TrackColor.Green: return Color.green;
            case TrackColor.Yellow: return Color.yellow;
            case TrackColor.Black: return Color.black;
            case TrackColor.White: return Color.white;
            case TrackColor.Orange: return new Color(1.0f, 0.64f, 0.0f);
            case TrackColor.Pink: return Color.magenta;
            case TrackColor.Gray: return Color.gray;
        }
        Debug.LogWarning($"Cor de trilha não mapeada explicitamente: {trackColor}. Retornando Gray.");
        return Color.gray;
    }
}