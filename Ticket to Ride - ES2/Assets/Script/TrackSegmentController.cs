using UnityEngine;

public class TrackSegmentController : MonoBehaviour
{
    public TrackController parentTrackController;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private Material segmentMaterialInstance;
    private static readonly int PlayerColorProp = Shader.PropertyToID("_PlayerColor");
    private static readonly int ClaimedIntensityProp = Shader.PropertyToID("_ClaimedIntensity");

    // --- NOVOS IDs PARA AS PROPRIEDADES DE HIGHLIGHT ---
    private static readonly int HighlightColorProp = Shader.PropertyToID("_HighlightColor");
    private static readonly int HighlightIntensityProp = Shader.PropertyToID("_HighlightIntensity");

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError("TrackSegmentController precisa de um SpriteRenderer!", this);

        segmentMaterialInstance = spriteRenderer.material;
    }

    public void SetVisuals(Color color, int sortOrder, bool isClaiming)
    {
        if (spriteRenderer == null || segmentMaterialInstance == null) return;

        originalColor = color;
        spriteRenderer.sortingOrder = sortOrder;

        // A cor do jogador é passada diretamente para o shader
        segmentMaterialInstance.SetColor(PlayerColorProp, originalColor);

        // Ativa ou desativa o efeito de listras no shader
        segmentMaterialInstance.SetFloat(ClaimedIntensityProp, isClaiming ? 1.0f : 0.0f);
        // *** A LINHA DA CORREÇÃO ESTÁ AQUI: ***
        // Garante que qualquer highlight ativo seja removido ao definir um estado visual permanente.
        segmentMaterialInstance.SetFloat(HighlightIntensityProp, 0.0f);
    }

    // --- MÉTODO SETHIGHLIGHT TOTALMENTE REFEITO ---
    // Aplica/remove um destaque temporário controlando as novas propriedades do shader
    public void SetHighlight(bool highlight, Color highlightColor)
    {
        if (segmentMaterialInstance != null)
        {
            if (highlight)
            {
                // Quando destacado, define a cor e a intensidade do highlight no shader
                segmentMaterialInstance.SetColor(HighlightColorProp, highlightColor);
                segmentMaterialInstance.SetFloat(HighlightIntensityProp, 0.6f); // 60% de mistura. Ajuste este valor para a força desejada!
            }
            else
            {
                // Para remover o destaque, basta zerar a intensidade. A cor não importa.
                segmentMaterialInstance.SetFloat(HighlightIntensityProp, 0.0f);
            }
        }
    }

    public Color GetOriginalColor()
    {
        return originalColor;
    }

    // --- Eventos do Mouse (sem alterações) ---
    void OnMouseEnter()
    {
        parentTrackController?.HighlightTrack(true);
    }

    void OnMouseExit()
    {
        parentTrackController?.HighlightTrack(false);
    }

    void OnMouseDown()
    {
        parentTrackController?.HandleSelectionAttempt();
    }
}