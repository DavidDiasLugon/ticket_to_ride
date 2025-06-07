using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("Simulação")]
    public int simulatedCurrentPlayerId = 0;
    public Color simulatedPlayer0Color = Color.red;
    public Color simulatedPlayer1Color = Color.blue;

    [Header("Estado de Seleção")]
    private TrackController selectedTrack = null; // Guarda o trilho atualmente selecionado

    private BoardManager boardManager;
    // public List<Player> players; // Comentado
    // public int realCurrentPlayerId; // Comentado
    // private int numberOfPlayers; // Comentado

    public enum ClaimResult { Success, Fail_AlreadyClaimed, Fail_DoubleTrackRule, Fail_InsufficientCards, Fail_NotPlayerTurn, Fail_NoTrackPieces, Fail_Other, Fail_NoTrackSelected }

    void Start()
    {
        Debug.Log("GameManager Iniciado!");
        boardManager = FindFirstObjectByType<BoardManager>();
    }

    void Update()
    {
        // --- Input para TENTAR REIVINDICAR o trilho SELECIONADO (Ex: Tecla Espaço) ---
        if (Input.GetKeyDown(KeyCode.Space)) // Usa Espaço em vez de P
        {
            if (selectedTrack != null)
            {
                Debug.Log($"Espaço pressionado - Tentando reivindicar trilho selecionado: {selectedTrack.name} para Jogador Simulado {simulatedCurrentPlayerId}");
                ClaimResult result = AttemptClaimTrack(selectedTrack);

                // Desseleciona após a tentativa (bem-sucedida ou não)
                DeselectCurrentTrack(); // Chama método auxiliar

            }
            else
            {
                Debug.Log("Espaço pressionado - Nenhum trilho selecionado para reivindicar.");
                // Poderia dar um feedback sonoro/visual de "nada selecionado"
            }
        }

        // --- Input para DESELECIONAR (Ex: Clique com Botão Direito ou Clique no Fundo) ---
        if (Input.GetMouseButtonDown(1)) // Botão direito do mouse
        {
            if (selectedTrack != null)
            {
                Debug.Log("Botão direito - Deselecionando trilho.");
                DeselectCurrentTrack();
            }
        }
        // TODO (Opcional): Implementar clique no fundo para deselecionar (requer Raycast)
    }

    /// <summary>
    /// Chamado pelo TrackController quando um de seus segmentos é clicado.
    /// Gerencia a lógica de seleção/desseleção.
    /// </summary>
    public void SelectTrack(TrackController trackToSelect)
    {
        // Não pode selecionar um trilho já reivindicado
        if (trackToSelect.isClaimed)
        {
            Debug.Log($"Não pode selecionar trilho '{trackToSelect.name}', já reivindicado.");
            // Se algo estava selecionado antes, deseleciona
            DeselectCurrentTrack();
            return;
        }

        // Se já estava selecionado, deseleciona (clicar de novo no mesmo)
        if (selectedTrack == trackToSelect)
        {
            Debug.Log($"Deselecionando '{trackToSelect.name}' por clique repetido.");
            DeselectCurrentTrack();
        }
        // Se é um trilho diferente do já selecionado (ou se nada estava selecionado)
        else
        {
            Debug.Log($"Selecionando trilho '{trackToSelect.name}'.");
            // Desseleciona o anterior (se houver)
            DeselectCurrentTrack();

            // Seleciona o novo
            selectedTrack = trackToSelect;
            selectedTrack.SetSelected(true); // Diz ao trilho para mostrar o visual de selecionado
        }
    }

    /// <summary>
    /// Método auxiliar para garantir que qualquer trilho selecionado seja deselecionado.
    /// </summary>
    private void DeselectCurrentTrack()
    {
        if (selectedTrack != null)
        {
            selectedTrack.SetSelected(false); // Diz ao trilho para remover o visual de selecionado
            selectedTrack = null;
        }
    }


    /// <summary>
    /// Tenta reivindicar a trilha FORNECIDA (geralmente a selecionada).
    /// </summary>
    public ClaimResult AttemptClaimTrack(TrackController trackToClaim)
    {
        // Adiciona verificação inicial
        if (trackToClaim == null)
        {
            Debug.LogError("AttemptClaimTrack chamado com trackToClaim nulo.");
            return ClaimResult.Fail_Other; // Ou Fail_NoTrackSelected se apropriado
        }

        Debug.Log($"GameManager processando pedido para: {trackToClaim.gameObject.name}");

        // ... (Restante da lógica de validação: isClaimed, DoubleTrackRule, simulação de cartas) ...
        // ... (COMO NO CÓDIGO ANTERIOR) ...

        // 3. Verificar se o trilho já foi reivindicado (já feito na seleção, mas bom verificar de novo)
        if (trackToClaim.isClaimed)
        {
            Debug.Log($"Falha: Trilha '{trackToClaim.name}' já reivindicada por Player {trackToClaim.ownerPlayerId}.");
            // Garante deseleção se o estado mudou inesperadamente
            DeselectCurrentTrack();
            return ClaimResult.Fail_AlreadyClaimed;
        }

        // 4. Verificar regras de Trilha Dupla (COMO NO CÓDIGO ANTERIOR)
        // ... (código da validação de trilha dupla) ...
        if (trackToClaim.trackData.isDoubleTrack)
        {
            // ... (lógica para encontrar gêmeo e verificar) ...
            // if (falhou_regra_dupla) { DeselectCurrentTrack(); return ClaimResult.Fail_DoubleTrackRule; }
        }

        // 5. Simulação de validação de cartas/peças (COMO NO CÓDIGO ANTERIOR)
        bool hasEnoughResources = true; // Simulação
        if (!hasEnoughResources)
        {
            DeselectCurrentTrack();
            return ClaimResult.Fail_InsufficientCards; // Ou outro erro
        }


        // === Tudo OK - Processar Reivindicação ===
        // ... (Obter playerIdToClaim e playerColorToClaim da simulação, como antes) ...
        int playerIdToClaim = simulatedCurrentPlayerId;
        Color playerColorToClaim = (simulatedCurrentPlayerId == 0) ? simulatedPlayer0Color : simulatedPlayer1Color;


        // --- Ações Reais Comentadas (COMO NO CÓDIGO ANTERIOR) ---
        /*
         * Remover Cartas
         * Deduzir Peças
         * Adicionar Pontos
         * Atualizar UI
        */

        // Chamar o Claim no TrackController (Lógica Ativa)
        trackToClaim.Claim(playerIdToClaim, playerColorToClaim);
        Debug.Log($"Trilha {trackToClaim.name} reivindicada com sucesso por Jogador Simulado {playerIdToClaim}.");

        // Passar o turno (Simulado)
        simulatedCurrentPlayerId = (simulatedCurrentPlayerId + 1) % 2; // Alterna 0 e 1
        Debug.Log($"Próximo jogador simulado é: {simulatedCurrentPlayerId}");

        // Retorna sucesso (a deseleção já foi chamada no Update após esta chamada)
        return ClaimResult.Success;
    }

    // --- Restante do GameManager (Start, etc) ---
}