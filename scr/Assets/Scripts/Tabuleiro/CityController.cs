using TMPro;
using UnityEngine; // Provavelmente já existe

public class CityController : MonoBehaviour
{
    public string cityName = "Nome Padrão da Cidade"; // O nome da sua cidade

    // Referência para o componente TextMeshPro no objeto filho (ou no mesmo objeto, dependendo de onde adicionou)
    [SerializeField] private TextMeshPro textMeshProComponent;

    void Start()
    {
        // Tenta encontrar o componente automaticamente no filho se não for atribuído
        if (textMeshProComponent == null)
        {
            // Procura no mesmo objeto E nos filhos. Ajuste se necessário.
            textMeshProComponent = GetComponentInChildren<TextMeshPro>();
        }

        // Define o texto quando o jogo inicia
        UpdateCityNameDisplay();
    }

    // Função para atualizar o texto (pode ser chamada se o nome mudar)
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

    // Opcional: Atualizar no editor quando o nome muda para visualização
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