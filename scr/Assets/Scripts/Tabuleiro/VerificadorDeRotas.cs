using System.Collections.Generic;
using System.Linq;

public static class VerificadorDeRotas
{
    public static bool ExisteCaminho(string origem, string destino, List<TrackController> todasAsRotasDoTabuleiro, string nomeJogador)
    {
        var grafoDoJogador = ConstruirGrafoDoJogador(todasAsRotasDoTabuleiro, nomeJogador);
        if (!grafoDoJogador.ContainsKey(origem) || !grafoDoJogador.ContainsKey(destino))
        {
            return false;
        }
        var filaDeCidadesParaVisitar = new Queue<string>();
        var cidadesJaVisitadas = new HashSet<string>();
        filaDeCidadesParaVisitar.Enqueue(origem);
        cidadesJaVisitadas.Add(origem);
        
        while (filaDeCidadesParaVisitar.Count > 0)
        {
            string cidadeAtual = filaDeCidadesParaVisitar.Dequeue();
            if (cidadeAtual == destino)
            {
                return true;
            }
            foreach (string vizinho in grafoDoJogador[cidadeAtual])
            {
                if (!cidadesJaVisitadas.Contains(vizinho))
                {
                    cidadesJaVisitadas.Add(vizinho);
                    filaDeCidadesParaVisitar.Enqueue(vizinho);
                }
            }
        }
        return false;
    }

    private static Dictionary<string, List<string>> ConstruirGrafoDoJogador(List<TrackController> todasAsRotas, string nome)
    {
        var grafo = new Dictionary<string, List<string>>();
        var rotasDoJogador = todasAsRotas.Where(r => r.ownerPlayerName == nome);

        foreach (var rota in rotasDoJogador)
        {
            string cidade1 = rota.trackData.city1Name;
            string cidade2 = rota.trackData.city2Name;
            if (!grafo.ContainsKey(cidade1)) grafo[cidade1] = new List<string>();
            if (!grafo.ContainsKey(cidade2)) grafo[cidade2] = new List<string>();
            grafo[cidade1].Add(cidade2);
            grafo[cidade2].Add(cidade1);
        }
        return grafo;
    }
}