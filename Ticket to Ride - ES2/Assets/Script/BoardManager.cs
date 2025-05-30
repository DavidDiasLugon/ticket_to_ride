using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Usado para .Any() - opcional

public class BoardManager : MonoBehaviour
{
    [Header("Referências do Editor")]
    [Tooltip("Arraste aqui TODOS os assets ScriptableObject de TrackData do seu projeto.")]
    public List<TrackData> allTrackData;

    [Tooltip("Arraste aqui o Prefab do segmento de trilha (que tem TrackSegmentController).")]
    public GameObject trackSegmentPrefab;

    [Tooltip("Opcional: Arraste um GameObject da cena para ser o pai de todos os trilhos criados (para organização).")]
    public Transform tracksParent;

    // --- Referências Internas ---
    // Dicionário para acesso rápido às cidades pelo nome
    private Dictionary<string, CityController> cities = new Dictionary<string, CityController>();

    // Lista para guardar referências a todos os TrackControllers criados (se precisar acessá-los depois)
    private List<TrackController> allTrackControllers = new List<TrackController>();

    // ========================================================================
    // Ciclo de Vida Unity
    // ========================================================================

    void Start()
    {
        // 1. Encontrar todas as cidades e colocá-las no dicionário
        PopulateCityDictionary();

        // 2. Criar os trilhos visuais com base nos TrackData
        CreateAllTracks();

        Debug.Log("BoardManager concluído. Cidades encontradas: " + cities.Count + ", Trilhos criados: " + allTrackControllers.Count);
    }

    // ========================================================================
    // Métodos de Inicialização
    // ========================================================================

    /// <summary>
    /// Encontra todos os CityController na cena e os armazena em um dicionário para acesso rápido.
    /// </summary>
    void PopulateCityDictionary()
    {
        cities.Clear();
        // Encontra todos os componentes CityController ativos na cena
        CityController[] foundCities = FindObjectsByType<CityController>(FindObjectsSortMode.None);

        foreach (CityController city in foundCities)
        {
            // Verifica se o nome da cidade não é vazio e se já não existe no dicionário
            if (!string.IsNullOrEmpty(city.cityName) && !cities.ContainsKey(city.cityName))
            {
                cities.Add(city.cityName, city);
            }
            else
            {
                Debug.LogWarning($"Cidade com nome inválido ou duplicado encontrada: '{city.cityName}' no GameObject '{city.gameObject.name}'. Ignorando.", city.gameObject);
            }
        }

        if (cities.Count == 0)
        {
            Debug.LogError("Nenhuma cidade (CityController) encontrada na cena! Verifique se os GameObjects das cidades têm o script CityController e nomes definidos.");
        }
    }

    /// <summary>
    /// Itera sobre a lista allTrackData e instancia os trilhos visuais na cena.
    /// </summary>
    void CreateAllTracks()
    {
        // Verificações iniciais essenciais
        if (trackSegmentPrefab == null)
        {
            Debug.LogError("Prefab do Segmento de Trilha (trackSegmentPrefab) não foi atribuído no Inspector do BoardManager!", this.gameObject);
            return;
        }
        // Verifica se o prefab tem o script necessário
        if (trackSegmentPrefab.GetComponent<TrackSegmentController>() == null)
        {
            Debug.LogError($"O prefab '{trackSegmentPrefab.name}' não tem o componente TrackSegmentController anexado!", trackSegmentPrefab);
            return;
        }
        if (allTrackData == null || allTrackData.Count == 0 || allTrackData.All(item => item == null)) // .All é do Linq, verifica se todos são nulos
        {
            Debug.LogError("A lista 'All Track Data' no BoardManager está vazia ou contém apenas itens nulos. Arraste seus assets TrackData para a lista no Inspector.", this.gameObject);
            return;
        }
        if (cities.Count == 0)
        {
            Debug.LogError("Não é possível criar trilhos pois nenhuma cidade foi encontrada. Verifique o método PopulateCityDictionary e os GameObjects das cidades.");
            return;
        }


        // Loop principal para criar cada trilho
        foreach (TrackData data in allTrackData)
        {
            // Pula itens nulos na lista (caso arraste algo errado)
            if (data == null)
            {
                Debug.LogWarning("Item nulo encontrado na lista 'All Track Data'. Pulando.", this.gameObject);
                continue;
            }

            // Valida os nomes das cidades no TrackData
            if (string.IsNullOrEmpty(data.city1Name) || string.IsNullOrEmpty(data.city2Name))
            {
                Debug.LogError($"TrackData '{data.name}' tem um nome de cidade vazio (City1: '{data.city1Name}', City2: '{data.city2Name}'). Pulando este trilho.", data);
                continue;
            }

            if (data.city1Name.Equals(data.city2Name, System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogError($"TrackData '{data.name}' conecta uma cidade a ela mesma ('{data.city1Name}'). Pulando este trilho.", data);
                continue;
            }


            // Encontrar os GameObjects / posições das cidades de início e fim
            if (!cities.TryGetValue(data.city1Name, out CityController city1))
            {
                Debug.LogError($"Não foi possível encontrar a cidade '{data.city1Name}' definida no TrackData '{data.name}'. Verifique se o nome está correto e se a cidade existe na cena com CityController.", data);
                continue; // Pula este trilho se não encontrar a cidade
            }
            if (!cities.TryGetValue(data.city2Name, out CityController city2))
            {
                Debug.LogError($"Não foi possível encontrar a cidade '{data.city2Name}' definida no TrackData '{data.name}'. Verifique se o nome está correto e se a cidade existe na cena com CityController.", data);
                continue; // Pula este trilho
            }

            Vector3 startPos = city1.transform.position;
            Vector3 endPos = city2.transform.position;

            Vector3 trackOffset = Vector3.zero; // Deslocamento padrão é zero
            if (data.isDoubleTrack && data.twinTrack != null)
            {
                // Calcula a direção do trilho
                Vector3 direction = (endPos - startPos).normalized;
                // Calcula um vetor perpendicular (para o deslocamento lateral)
                Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0) * 0.2f; // O valor 0.2f é a distância do deslocamento, ajuste conforme necessário

                // Para garantir que os gêmeos se desloquem em direções opostas de forma consistente,
                // usamos o nome dos objetos para decidir a direção do deslocamento.
                // Isso evita que ambos se desloquem para o mesmo lado.
                if (string.Compare(data.name, data.twinTrack.name) > 0)
                {
                    trackOffset = perpendicular;
                }
                else
                {
                    trackOffset = -perpendicular;
                }
            }
            // --- Criação do GameObject Pai para o Trilho ---
            GameObject trackParentObj = new GameObject($"Track_{data.city1Name}_{data.city2Name}_{data.color}");
            trackParentObj.transform.position = (startPos + endPos) / 2f; // Posição central (útil para gizmos, etc)

            // Organizar na hierarquia sob o pai opcional 'tracksParent'
            if (tracksParent != null)
            {
                trackParentObj.transform.SetParent(tracksParent);
            }

            // Adicionar e configurar o TrackController
            TrackController trackController = trackParentObj.AddComponent<TrackController>();
            List<TrackSegmentController> currentSegments = new List<TrackSegmentController>(); // Lista temporária para os segmentos deste trilho

            // --- Instanciação dos Segmentos Visuais ---
            if (data.length <= 0)
            {
                Debug.LogWarning($"TrackData '{data.name}' tem comprimento inválido ({data.length}). Definindo para 1.", data);
                data.length = 1; // Correção simples
            }

            for (int i = 0; i < data.length; i++)
            {
                // Calcular a posição do segmento ao longo da linha (interpolação linear)
                float t = (float)(i + 0.5f) / data.length; // Ponto central de cada passo do segmento
                Vector3 segmentPosition = Vector3.Lerp(startPos, endPos, t) + trackOffset;

                // Calcular a rotação para alinhar o segmento com a direção do trilho
                Vector3 direction = (endPos - startPos); // Não normalizar ainda se precisar para Atan2
                if (direction == Vector3.zero) direction = Vector3.right; // Evitar divisão por zero se cidades estiverem no mesmo lugar

                // Opção 1: Usando LookRotation (bom se o 'up' do sprite for o topo do vagão)
                Quaternion segmentRotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);

                // Opção 2: Usando Atan2 (bom se o 'right' (eixo X) do sprite for a direção do vagão)
                // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                // Quaternion segmentRotation = Quaternion.Euler(0, 0, angle); // Ajuste o -90f se necessário dependendo da orientação original do sprite

                // Instanciar o prefab do segmento
                GameObject segmentObj = Instantiate(trackSegmentPrefab, segmentPosition, segmentRotation);
                segmentObj.name = $"Segment_{i}";

                // Tornar o segmento filho do GameObject pai do trilho
                segmentObj.transform.SetParent(trackParentObj.transform);

                // Obter o controller do segmento e adicionar à lista
                TrackSegmentController segmentController = segmentObj.GetComponent<TrackSegmentController>();
                if (segmentController != null)
                {
                    currentSegments.Add(segmentController);
                    // Opcional: passar o índice, se precisar dele no segmento
                    // segmentController.segmentIndex = i;
                }
                else
                {
                    // Este erro não deveria acontecer se a verificação inicial do prefab passou
                    Debug.LogError($"Instância do prefab {trackSegmentPrefab.name} não contém o script TrackSegmentController!", segmentObj);
                }
            } // Fim do loop de segmentos

            // --- Finalização do TrackController ---
            // Inicializa o TrackController com seus dados e a lista de segmentos que acabamos de criar
            trackController.Initialize(data, currentSegments);

            // Adiciona o controller do trilho à lista principal (se precisar referenciá-los depois)
            allTrackControllers.Add(trackController);

        } // Fim do loop de TrackData
    }

    // ========================================================================
    // Métodos Públicos (Exemplos - Adicionar conforme necessário)
    // ========================================================================

    /// <summary>
    /// Encontra um TrackController específico (ex: para lógica de gêmeos).
    /// Pode ser ineficiente se chamado frequentemente; considere um dicionário se necessário.
    /// </summary>
    public TrackController FindTrackControllerForData(TrackData dataToFind)
    {
        foreach (TrackController tc in allTrackControllers)
        {
            if (tc.trackData == dataToFind)
            {
                return tc;
            }
        }
        return null; // Não encontrado
    }

}