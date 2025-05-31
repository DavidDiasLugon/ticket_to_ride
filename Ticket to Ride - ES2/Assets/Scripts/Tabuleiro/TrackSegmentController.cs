using UnityEngine;

public class TrackSegmentController : MonoBehaviour
{
    public TrackController parentTrackController;
    private SpriteRenderer spriteRenderer;
    // private Color originalColor; // Não é mais necessário aqui se o shader gerencia a cor base e o highlight separadamente

    private Material segmentMaterialInstance;
    private static readonly int PlayerColorProp = Shader.PropertyToID("_PlayerColor");
    private static readonly int ClaimedIntensityProp = Shader.PropertyToID("_ClaimedIntensity");
    private static readonly int HighlightColorProp = Shader.PropertyToID("_HighlightColor");
    private static readonly int HighlightIntensityProp = Shader.PropertyToID("_HighlightIntensity");

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError("TrackSegmentController precisa de um SpriteRenderer!", this);

        // É crucial que cada segmento tenha sua própria instância do material
        // para que as alterações de highlight em um não afetem os outros diretamente (a menos que desejado via controller)
        if (spriteRenderer != null)
        {
            segmentMaterialInstance = spriteRenderer.material; // Cria uma instância do material
        }
        if (parentTrackController == null)
        {
            Debug.LogError($"TrackSegment {gameObject.name}: parentTrackController é NULO no Awake!");
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            segmentMaterialInstance = spriteRenderer.material;
        }
    }

    public void SetVisuals(Color basePlayerColor, int sortOrder, bool isClaiming)
    {
        if (spriteRenderer == null || segmentMaterialInstance == null)
        {
            Debug.LogWarning($"TrackSegment {gameObject.name}: Renderer ou Material NULO em SetVisuals.");
            return;
        }
        Debug.Log($"TrackSegment {gameObject.name}: SetVisuals - Claiming: {isClaiming}, HighlightIntensity definido para 0.");

        // originalColor = basePlayerColor; // Se você ainda precisar da cor base original por algum motivo
        spriteRenderer.sortingOrder = sortOrder;

        segmentMaterialInstance.SetColor(PlayerColorProp, basePlayerColor);
        segmentMaterialInstance.SetFloat(ClaimedIntensityProp, isClaiming ? 1.0f : 0.0f);

        // Garante que qualquer highlight ativo (hover/seleção) seja removido ao definir um estado visual permanente (claim)
        // ou ao resetar para o estado base.
        segmentMaterialInstance.SetFloat(HighlightIntensityProp, 0.0f); //
    }

    public void SetHighlight(bool highlight, Color highlightColorParam)
    {
        if (segmentMaterialInstance != null)
        {
            // Adicione um log aqui para ver quando o highlight está sendo efetivamente alterado
            Debug.Log($"TrackSegment {gameObject.name}: SetHighlight({highlight}, {highlightColorParam}) | Intensidade: {(highlight ? 0.6f : 0.0f)}");

            if (highlight)
            {
                segmentMaterialInstance.SetColor(HighlightColorProp, highlightColorParam);
                segmentMaterialInstance.SetFloat(HighlightIntensityProp, 0.6f); // Ajuste conforme necessário
            }
            else
            {
                segmentMaterialInstance.SetFloat(HighlightIntensityProp, 0.0f);
            }
        }
        else
        {
            Debug.LogWarning($"TrackSegment {gameObject.name}: segmentMaterialInstance é NULO ao tentar SetHighlight.");
        }
    }

    // GetOriginalColor não é mais usado diretamente pela lógica de highlight do controller,
    // mas pode ser útil para outras coisas. O shader agora tem a PlayerColor.
    public Color GetOriginalColor()
    {
        // Se você quer a cor base definida no shader:
        if (segmentMaterialInstance != null) return segmentMaterialInstance.GetColor(PlayerColorProp);
        return Color.clear; // Ou um valor padrão
    }

    void OnMouseEnter()
    {
        if (parentTrackController == null)
        {
            Debug.LogError($"TrackSegment {gameObject.name}: parentTrackController é NULO no OnMouseEnter!");
            return;
        }
        Debug.Log($"<color=green>MOUSE ENTER:</color> {gameObject.name} (Track: {parentTrackController.gameObject.name})");
        parentTrackController?.NotifySegmentMouseEnter(); // Modificado
    }

    void OnMouseExit()
    {
        if (parentTrackController == null)
        {
            Debug.LogError($"TrackSegment {gameObject.name}: parentTrackController é NULO no OnMouseExit!");
            return;
        }
        Debug.Log($"<color=red>MOUSE EXIT:</color> {gameObject.name} (Track: {parentTrackController.gameObject.name})");
        parentTrackController?.NotifySegmentMouseExit(); // Modificado
    }

    void OnMouseDown()
    {
        parentTrackController?.HandleSelectionAttempt();
    }
}