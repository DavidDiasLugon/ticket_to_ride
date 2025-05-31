using System.Collections.Generic;
using UnityEngine;

public class EstadoCompraBilhete : EstadoJogo
{
    public override void IniciarEstado(Controle controle)
    {
        List<Bilhete> bilhetesComprados = new List<Bilhete>();
        List<Bilhete> bilhetesEscolhidos = new List<Bilhete>();
        Bilhete b;
        for (int i = 0; i < 3; i++)
        {
            b = controle.DeckBilhetes.CompraBilhete();
            bilhetesComprados.Add(b);
        }
    }

    public override void RunEstado(Controle controle)
    {

    }

    public override void ProcessarSelecao(Controle controle, int indice, Carta cartaSelecionada)
    {
    }
}
