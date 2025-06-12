using TMPro;
using UnityEngine; // Provavelmente j� existe

public class CityController : MonoBehaviour
{
    public string cityName = "Nome Padr�o da Cidade"; // O nome da sua cidade

    // Refer�ncia para o componente TextMeshPro no objeto filho (ou no mesmo objeto, dependendo de onde adicionou)
    [SerializeField] private TextMeshPro textMeshProComponent;

    public SpriteRenderer shapeSpriteRenderer; // Referencia para o SpriteRenderer do shape da cidade
    private Color originalColor;

    void Awake()
    {
        if (shapeSpriteRenderer != null)
        {
            originalColor = shapeSpriteRenderer.color; // Guarda a cor original do shape
        }
        else
        {
            Debug.LogWarning("Shape SpriteRenderer not assigned in CityController.", this.gameObject);
        }
    }

    void Start()
    {
        // Tenta encontrar o componente automaticamente no filho se n�o for atribu�do
        if (textMeshProComponent == null)
        {
            // Procura no mesmo objeto E nos filhos. Ajuste se necess�rio.
            textMeshProComponent = GetComponentInChildren<TextMeshPro>();
        }

        // Define o texto quando o jogo inicia
        UpdateCityNameDisplay();
    }

    // Fun��o para atualizar o texto (pode ser chamada se o nome mudar)
    public void UpdateCityNameDisplay()
    {
        if (textMeshProComponent != null)
        {
            textMeshProComponent.text = cityName;
        }
        else
        {
            Debug.LogWarning($"TextMeshPro component not found for city: {cityName}", this.gameObject);
        }
    }

    public void Destacar(bool destacar, Color corDestaque)
    {
        if (shapeSpriteRenderer == null) return;
        if (destacar)
        {
            shapeSpriteRenderer.color = corDestaque;
        }
        else
        {
            shapeSpriteRenderer.color = originalColor;
        }
    }

    // Opcional: Atualizar no editor quando o nome muda para visualiza��o
#if UNITY_EDITOR
    void OnValidate()
    {
        // Garante que o componente seja pego mesmo no editor
        if (textMeshProComponent == null)
        {
            textMeshProComponent = GetComponentInChildren<TextMeshPro>();
        }
        // Atualiza imediatamente no editor ao mudar o nome no Inspector
        UpdateCityNameDisplay();
    }
#endif
}