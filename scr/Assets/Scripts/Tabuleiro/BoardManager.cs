using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Usado para .Any() - opcional

public class BoardManager : MonoBehaviour
{
    [Header("Refer�ncias do Editor")]
    [Tooltip("Arraste aqui TODOS os assets ScriptableObject de TrackData do seu projeto.")]
    public List<TrackData> allTrackData;

    public static List<TrackController> AllTrackControllers { get; private set; }

    [Tooltip("Arraste aqui o Prefab do segmento de trilha (que tem TrackSegmentController).")]
    public GameObject trackSegmentPrefab;

    [Tooltip("Opcional: Arraste um GameObject da cena para ser o pai de todos os trilhos criados (para organiza��o).")]
    public Transform tracksParent;

    // --- Refer�ncias Internas ---
    // Dicion�rio para acesso r�pido �s cidades pelo nome
    public static Dictionary<string, CityController> Cities = new Dictionary<string, CityController>();

    // Lista para guardar refer�ncias a todos os TrackControllers criados (se precisar acess�-los depois)
    private List<TrackController> allTrackControllers = new List<TrackController>();

    // ========================================================================
    // Ciclo de Vida Unity
    // ========================================================================

    void Start()
    {
        // 1. Encontrar todas as cidades e coloc�-las no dicion�rio
        PopulateCityDictionary();

        // 2. Criar os trilhos visuais com base nos TrackData
        CreateAllTracks();

        Debug.Log("BoardManager conclu�do. Cidades encontradas: " + Cities.Count + ", Trilhos criados: " + allTrackControllers.Count);
    }

    // ========================================================================
    // M�todos de Inicializa��o
    // ========================================================================

    /// <summary>
    /// Encontra todos os CityController na cena e os armazena em um dicion�rio para acesso r�pido.
    /// </summary>
    void PopulateCityDictionary()
    {
        Cities.Clear();
        // Encontra todos os componentes CityController ativos na cena
        CityController[] foundCities = FindObjectsByType<CityController>(FindObjectsSortMode.None);

        foreach (CityController city in foundCities)
        {
            // Verifica se o nome da cidade n�o � vazio e se j� n�o existe no dicion�rio
            if (!string.IsNullOrEmpty(city.cityName) && !Cities.ContainsKey(city.cityName))
            {
                Cities.Add(city.cityName, city);
            }
            else
            {
                Debug.LogWarning($"Cidade com nome inv�lido ou duplicado encontrada: '{city.cityName}' no GameObject '{city.gameObject.name}'. Ignorando.", city.gameObject);
            }
        }

        if (Cities.Count == 0)
        {
            Debug.LogError("Nenhuma cidade (CityController) encontrada na cena! Verifique se os GameObjects das cidades t�m o script CityController e nomes definidos.");
        }
    }

    /// <summary>
    /// Itera sobre a lista allTrackData e instancia os trilhos visuais na cena.
    /// </summary>
    void CreateAllTracks()
    {
        // Verifica��es iniciais essenciais
        if (trackSegmentPrefab == null)
        {
            Debug.LogError("Prefab do Segmento de Trilha (trackSegmentPrefab) n�o foi atribu�do no Inspector do BoardManager!", this.gameObject);
            return;
        }
        // Verifica se o prefab tem o script necess�rio
        if (trackSegmentPrefab.GetComponent<TrackSegmentController>() == null)
        {
            Debug.LogError($"O prefab '{trackSegmentPrefab.name}' n�o tem o componente TrackSegmentController anexado!", trackSegmentPrefab);
            return;
        }
        if (allTrackData == null || allTrackData.Count == 0 || allTrackData.All(item => item == null)) // .All � do Linq, verifica se todos s�o nulos
        {
            Debug.LogError("A lista 'All Track Data' no BoardManager est� vazia ou cont�m apenas itens nulos. Arraste seus assets TrackData para a lista no Inspector.", this.gameObject);
            return;
        }
        if (Cities.Count == 0)
        {
            Debug.LogError("N�o � poss�vel criar trilhos pois nenhuma cidade foi encontrada. Verifique o m�todo PopulateCityDictionary e os GameObjects das cidades.");
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


            // Encontrar os GameObjects / posi��es das cidades de in�cio e fim
            if (!Cities.TryGetValue(data.city1Name, out CityController city1))
            {
                Debug.LogError($"N�o foi poss�vel encontrar a cidade '{data.city1Name}' definida no TrackData '{data.name}'. Verifique se o nome est� correto e se a cidade existe na cena com CityController.", data);
                continue; // Pula este trilho se n�o encontrar a cidade
            }
            if (!Cities.TryGetValue(data.city2Name, out CityController city2))
            {
                Debug.LogError($"N�o foi poss�vel encontrar a cidade '{data.city2Name}' definida no TrackData '{data.name}'. Verifique se o nome est� correto e se a cidade existe na cena com CityController.", data);
                continue; // Pula este trilho
            }

            Vector3 startPos = city1.transform.position;
            Vector3 endPos = city2.transform.position;

            Vector3 trackOffset = Vector3.zero; // Deslocamento padr�o � zero
            if (data.isDoubleTrack && data.twinTrack != null)
            {
                // Calcula a dire��o do trilho
                Vector3 direction = (endPos - startPos).normalized;
                // Calcula um vetor perpendicular (para o deslocamento lateral)
                Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0) * 0.1f; // O valor 0.2f � a dist�ncia do deslocamento, ajuste conforme necess�rio

                // Para garantir que os g�meos se desloquem em dire��es opostas de forma consistente,
                // usamos o nome dos objetos para decidir a dire��o do deslocamento.
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
            // --- Cria��o do GameObject Pai para o Trilho ---
            GameObject trackParentObj = new GameObject($"Track_{data.city1Name}_{data.city2Name}_{data.color}");
            trackParentObj.transform.position = (startPos + endPos) / 2f; // Posi��o central (�til para gizmos, etc)

            // Organizar na hierarquia sob o pai opcional 'tracksParent'
            if (tracksParent != null)
            {
                trackParentObj.transform.SetParent(tracksParent);
            }

            // Adicionar e configurar o TrackController
            TrackController trackController = trackParentObj.AddComponent<TrackController>();
            List<TrackSegmentController> currentSegments = new List<TrackSegmentController>(); // Lista tempor�ria para os segmentos deste trilho

            // --- Instancia��o dos Segmentos Visuais ---
            if (data.length <= 0)
            {
                Debug.LogWarning($"TrackData '{data.name}' tem comprimento inv�lido ({data.length}). Definindo para 1.", data);
                data.length = 1; // Corre��o simples
            }

            for (int i = 0; i < data.length; i++)
            {
                // Calcular a posi��o do segmento ao longo da linha (interpola��o linear)
                float t = (float)(i + 0.5f) / data.length; // Ponto central de cada passo do segmento
                Vector3 segmentPosition = Vector3.Lerp(startPos, endPos, t) + trackOffset;

                // Calcular a rota��o para alinhar o segmento com a dire��o do trilho
                Vector3 direction = (endPos - startPos); // N�o normalizar ainda se precisar para Atan2
                if (direction == Vector3.zero) direction = Vector3.right; // Evitar divis�o por zero se cidades estiverem no mesmo lugar

                // Op��o 1: Usando LookRotation (bom se o 'up' do sprite for o topo do vag�o)
                Quaternion segmentRotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);

                // Op��o 2: Usando Atan2 (bom se o 'right' (eixo X) do sprite for a dire��o do vag�o)
                // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                // Quaternion segmentRotation = Quaternion.Euler(0, 0, angle); // Ajuste o -90f se necess�rio dependendo da orienta��o original do sprite

                // Instanciar o prefab do segmento
                GameObject segmentObj = Instantiate(trackSegmentPrefab, segmentPosition, segmentRotation);
                segmentObj.name = $"Segment_{i}";

                // Tornar o segmento filho do GameObject pai do trilho
                segmentObj.transform.SetParent(trackParentObj.transform);

                // Obter o controller do segmento e adicionar � lista
                TrackSegmentController segmentController = segmentObj.GetComponent<TrackSegmentController>();
                if (segmentController != null)
                {
                    currentSegments.Add(segmentController);
                    // Opcional: passar o �ndice, se precisar dele no segmento
                    // segmentController.segmentIndex = i;
                }
                else
                {
                    // Este erro n�o deveria acontecer se a verifica��o inicial do prefab passou
                    Debug.LogError($"Inst�ncia do prefab {trackSegmentPrefab.name} n�o cont�m o script TrackSegmentController!", segmentObj);
                }
            } // Fim do loop de segmentos

            // --- Finaliza��o do TrackController ---
            // Inicializa o TrackController com seus dados e a lista de segmentos que acabamos de criar
            trackController.Initialize(data, currentSegments);

            // Adiciona o controller do trilho � lista principal (se precisar referenci�-los depois)
            allTrackControllers.Add(trackController);

        } // Fim do loop de TrackData
        AllTrackControllers = this.allTrackControllers;
    }

    // ========================================================================
    // M�todos P�blicos (Exemplos - Adicionar conforme necess�rio)
    // ========================================================================

    /// <summary>
    /// Encontra um TrackController espec�fico (ex: para l�gica de g�meos).
    /// Pode ser ineficiente se chamado frequentemente; considere um dicion�rio se necess�rio.
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
        return null; // N�o encontrado
    }

}