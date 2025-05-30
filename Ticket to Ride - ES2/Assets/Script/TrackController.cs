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
    private bool isSelected = false; // Novo estado: este trilho está selecionado?

    // --- Cores ---
    private static readonly Color HOVER_COLOR = new Color(1f, 1f, 0.7f, 1f); // Amarelo claro (Hover)
    private static readonly Color SELECTED_COLOR = new Color(0.7f, 1f, 0.7f, 1f); // Verde claro (Selecionado)

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
            // A chamada inicial passa 'false' para isClaiming, garantindo que o efeito do shader está desligado.
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

    // Chamado pelo Segmento para destacar/remover destaque de HOVER
    public void HighlightTrack(bool highlight)
    {
        if (isSelected || isClaimed) return;

        foreach (var segment in segments)
        {
            // A lógica de highlight agora é tratada no TrackSegmentController
            segment?.SetHighlight(highlight, HOVER_COLOR);
        }
    }

    // Chamado pelo GameManager para definir o estado SELECIONADO
    public void SetSelected(bool select)
    {
        if (isClaimed) return; // Não pode selecionar se já reivindicado

        isSelected = select;
        foreach (var segment in segments)
        {
            segment?.SetHighlight(select, SELECTED_COLOR);
        }
        if (!select)
        {
            HighlightTrack(false);
        }
    }


    // Chamado pelo GameManager APÓS validar a reivindicação
    public void Claim(int playerId, Color playerColor)
    {
        if (isClaimed) return;
        isClaimed = true;
        isSelected = false;
        ownerPlayerId = playerId;
        Debug.Log($"Trilho {gameObject.name} reivindicado pelo Player {playerId}");

        // Aumenta a ordem para ficar na frente de trilhos não reivindicados na mesma Y
        int sortOrder = Mathf.RoundToInt(-transform.position.y * 10) + 1;
        foreach (var segment in segments)
        {
            // Esta chamada agora instrui o TrackSegmentController a definir a cor do jogador
            // e ativar o efeito de "reivindicado" (isClaiming = true) no shader.
            segment?.SetVisuals(playerColor, sortOrder, true);
        }
    }

    // Método para reverter o Claim
    public void Unclaim()
    {
        if (!isClaimed) return;
        isClaimed = false;
        ownerPlayerId = -1;
        isSelected = false;
        Debug.Log($"Trilho {gameObject.name} liberado.");

        // Volta para a cor e ordem originais
        Color baseColor = GetColorFromEnum(trackData.color);
        int sortOrder = Mathf.RoundToInt(-transform.position.y * 10);
        foreach (var segment in segments)
        {
            // Esta chamada desativa o efeito de "reivindicado" (isClaiming = false) no shader.
            segment?.SetVisuals(baseColor, sortOrder, false);
        }
    }

    // --- Feedback de Falha ---
    private void TriggerFailureFeedback(GameManager.ClaimResult reason)
    {
        if (isClaimed) return;
        StartCoroutine(FlashFeedbackColor(Color.red, 0.3f));
    }

    private System.Collections.IEnumerator FlashFeedbackColor(Color flashColor, float duration)
    {
        List<Color> originalSegmentColors = new List<Color>();
        foreach (var segment in segments)
        {
            Color segmentOriginalColor = segment != null ? segment.GetOriginalColor() : Color.clear;
            originalSegmentColors.Add(segmentOriginalColor);
            if (segment != null && segment.GetComponent<SpriteRenderer>() != null)
            {
                // Este feedback visual temporário pode ser ajustado para funcionar com o shader
                // por enquanto, ele vai sobrescrever a cor base do SpriteRenderer
                segment.GetComponent<SpriteRenderer>().color = flashColor;
            }
        }

        yield return new WaitForSeconds(duration);

        for (int i = 0; i < segments.Count; i++)
        {
            if (segments[i] != null && segments[i].GetComponent<SpriteRenderer>() != null)
            {
                if (!isClaimed)
                {
                    // Restaura a cor do highlight para branco (sem tintura)
                    segments[i].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }

        yield break;
    }


    // --- Métodos Utilitários ---
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