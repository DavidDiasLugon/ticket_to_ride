using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("Simula��o")]
    public int simulatedCurrentPlayerId = 0;
    public Color simulatedPlayer0Color = Color.red;
    public Color simulatedPlayer1Color = Color.blue;

    [Header("Estado de Sele��o")]
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
        // --- Input para TENTAR REIVINDICAR o trilho SELECIONADO (Ex: Tecla Espa�o) ---
        if (Input.GetKeyDown(KeyCode.Space)) // Usa Espa�o em vez de P
        {
            if (selectedTrack != null)
            {
                Debug.Log($"Espa�o pressionado - Tentando reivindicar trilho selecionado: {selectedTrack.name} para Jogador Simulado {simulatedCurrentPlayerId}");
                ClaimResult result = AttemptClaimTrack(selectedTrack);

                // Desseleciona ap�s a tentativa (bem-sucedida ou n�o)
                DeselectCurrentTrack(); // Chama m�todo auxiliar

            }
            else
            {
                Debug.Log("Espa�o pressionado - Nenhum trilho selecionado para reivindicar.");
                // Poderia dar um feedback sonoro/visual de "nada selecionado"
            }
        }

        // --- Input para DESELECIONAR (Ex: Clique com Bot�o Direito ou Clique no Fundo) ---
        if (Input.GetMouseButtonDown(1)) // Bot�o direito do mouse
        {
            if (selectedTrack != null)
            {
                Debug.Log("Bot�o direito - Deselecionando trilho.");
                DeselectCurrentTrack();
            }
        }
        // TODO (Opcional): Implementar clique no fundo para deselecionar (requer Raycast)
    }

    /// <summary>
    /// Chamado pelo TrackController quando um de seus segmentos � clicado.
    /// Gerencia a l�gica de sele��o/dessele��o.
    /// </summary>
    public void SelectTrack(TrackController trackToSelect)
    {
        // N�o pode selecionar um trilho j� reivindicado
        if (trackToSelect.isClaimed)
        {
            Debug.Log($"N�o pode selecionar trilho '{trackToSelect.name}', j� reivindicado.");
            // Se algo estava selecionado antes, deseleciona
            DeselectCurrentTrack();
            return;
        }

        // Se j� estava selecionado, deseleciona (clicar de novo no mesmo)
        if (selectedTrack == trackToSelect)
        {
            Debug.Log($"Deselecionando '{trackToSelect.name}' por clique repetido.");
            DeselectCurrentTrack();
        }
        // Se � um trilho diferente do j� selecionado (ou se nada estava selecionado)
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
    /// M�todo auxiliar para garantir que qualquer trilho selecionado seja deselecionado.
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
        // Adiciona verifica��o inicial
        if (trackToClaim == null)
        {
            Debug.LogError("AttemptClaimTrack chamado com trackToClaim nulo.");
            return ClaimResult.Fail_Other; // Ou Fail_NoTrackSelected se apropriado
        }

        Debug.Log($"GameManager processando pedido para: {trackToClaim.gameObject.name}");

        // ... (Restante da l�gica de valida��o: isClaimed, DoubleTrackRule, simula��o de cartas) ...
        // ... (COMO NO C�DIGO ANTERIOR) ...

        // 3. Verificar se o trilho j� foi reivindicado (j� feito na sele��o, mas bom verificar de novo)
        if (trackToClaim.isClaimed)
        {
            Debug.Log($"Falha: Trilha '{trackToClaim.name}' j� reivindicada por Player {trackToClaim.ownerPlayerName}.");
            // Garante desele��o se o estado mudou inesperadamente
            DeselectCurrentTrack();
            return ClaimResult.Fail_AlreadyClaimed;
        }

        // 4. Verificar regras de Trilha Dupla (COMO NO C�DIGO ANTERIOR)
        // ... (c�digo da valida��o de trilha dupla) ...
        if (trackToClaim.trackData.isDoubleTrack)
        {
            // ... (l�gica para encontrar g�meo e verificar) ...
            // if (falhou_regra_dupla) { DeselectCurrentTrack(); return ClaimResult.Fail_DoubleTrackRule; }
        }

        // 5. Simula��o de valida��o de cartas/pe�as (COMO NO C�DIGO ANTERIOR)
        bool hasEnoughResources = true; // Simula��o
        if (!hasEnoughResources)
        {
            DeselectCurrentTrack();
            return ClaimResult.Fail_InsufficientCards; // Ou outro erro
        }


        // === Tudo OK - Processar Reivindica��o ===
        // ... (Obter playerIdToClaim e playerColorToClaim da simula��o, como antes) ...
        //string playerIdToClaim = simulatedCurrentPlayerId;
        Color playerColorToClaim = (simulatedCurrentPlayerId == 0) ? simulatedPlayer0Color : simulatedPlayer1Color;


        // --- A��es Reais Comentadas (COMO NO C�DIGO ANTERIOR) ---
        /*
         * Remover Cartas
         * Deduzir Pe�as
         * Adicionar Pontos
         * Atualizar UI
        */

        // Chamar o Claim no TrackController (L�gica Ativa)
        //trackToClaim.Claim(playerIdToClaim, playerColorToClaim);
        //Debug.Log($"Trilha {trackToClaim.name} reivindicada com sucesso por Jogador Simulado {playerIdToClaim}.");

        // Passar o turno (Simulado)
        simulatedCurrentPlayerId = (simulatedCurrentPlayerId + 1) % 2; // Alterna 0 e 1
        Debug.Log($"Pr�ximo jogador simulado �: {simulatedCurrentPlayerId}");

        // Retorna sucesso (a desele��o j� foi chamada no Update ap�s esta chamada)
        return ClaimResult.Success;
    }

    // --- Restante do GameManager (Start, etc) ---
}