using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CenaFinalControlador : MonoBehaviour
{
    public GameObject prefabPlacar;
    public GameObject prefabBilhete;
    public Transform painelPlacares;
    public Transform painelBilhete;
    public Button botaoVoltar;
    public TextMeshProUGUI textoAnuncioGeral;
    public TextMeshProUGUI textoVencedor;

    private Dictionary<string, TextMeshProUGUI> placarUIJogadores = new Dictionary<string, TextMeshProUGUI>();

    void Start()
    {
        botaoVoltar.gameObject.SetActive(false);
        textoAnuncioGeral.gameObject.SetActive(false);
        textoVencedor.gameObject.SetActive(false);
        painelBilhete.gameObject.SetActive(false);
        PrepararTabela();
        StartCoroutine(ExecutarSequenciaFinal());
    }

    public void PrepararTabela()
    {
        foreach (Jogador jogador in DadosFimJogo.jogadores)
        {
            GameObject linhaObj = Instantiate(prefabPlacar, painelPlacares);
            linhaObj.transform.Find("Nome").GetComponentInChildren<TextMeshProUGUI>().text = jogador.Nome;
            TextMeshProUGUI textoPontos = linhaObj.transform.Find("Pontos").GetComponentInChildren<TextMeshProUGUI>();
            textoPontos.text = jogador.Pontuacao.ToString();
            placarUIJogadores.Add(jogador.Nome, textoPontos);
            linhaObj.GetComponentInChildren<Image>().color = GetColor(jogador);
        }
    }

    IEnumerator ExecutarSequenciaFinal()
    {
        yield return new WaitForSeconds(1.5f);

        foreach (Jogador jogador in DadosFimJogo.jogadores)
        {
            textoAnuncioGeral.text = $"Verificando bilhetes de: {jogador.Nome}";
            textoAnuncioGeral.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            foreach (Bilhete bilhete in jogador.MaoBilhetes)
            {
                painelBilhete.gameObject.SetActive(true);
                GameObject bilheteObj = Instantiate(prefabBilhete, painelBilhete);
                bilheteObj.transform.Find("Origem").GetComponent<TextMeshProUGUI>().text = bilhete.Rota[0];
                bilheteObj.transform.Find("Destino").GetComponent<TextMeshProUGUI>().text = bilhete.Rota[1];
                bilheteObj.transform.Find("Pontuacao").GetComponent<TextMeshProUGUI>().text = bilhete.Pontos.ToString();
                GameObject icone = bilheteObj.transform.Find("Icone").gameObject;
                TextMeshProUGUI textoIcone = icone.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                if (bilhete.Concluido)
                {
                    textoIcone.gameObject.SetActive(true);
                }
                int pontos = bilhete.Concluido ? bilhete.Pontos : -bilhete.Pontos;
                if (bilhete.Concluido)
                {
                    FindAnyObjectByType<AudioManager>().Play("Completed");
                }
                else
                {
                    FindAnyObjectByType<AudioManager>().Play("NotCompleted");
                }
                yield return new WaitForSeconds(1.8f);

                yield return StartCoroutine(AnimarPontuacao(jogador.Nome, pontos));
                painelBilhete.gameObject.SetActive(false);
                Destroy(bilheteObj);

                yield return new WaitForSeconds(1.8f);
            }
            textoAnuncioGeral.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(2.0f);

        if (DadosFimJogo.jogadoresComMaiorCaminho != null && DadosFimJogo.jogadoresComMaiorCaminho.Count > 0)
        {
            textoAnuncioGeral.gameObject.SetActive(true);
            textoAnuncioGeral.text = "Bônus de maior caminho contínuo para...";
            yield return new WaitForSeconds(3.0f);

            var nomes = DadosFimJogo.jogadoresComMaiorCaminho.Select(j => j.Nome);
            textoAnuncioGeral.text = string.Join(" e ", nomes) + "!";
            FindAnyObjectByType<AudioManager>().Play("Whistle");

            foreach (var jogador in DadosFimJogo.jogadoresComMaiorCaminho)
            {
                yield return StartCoroutine(AnimarPontuacao(jogador.Nome, 10));
            }
            textoAnuncioGeral.gameObject.SetActive(false);
            yield return new WaitForSeconds(3.0f);
        }

        textoAnuncioGeral.gameObject.SetActive(true);
        KeyValuePair<string, TextMeshProUGUI> vencedor = placarUIJogadores.OrderByDescending(par => int.Parse(par.Value.text)).FirstOrDefault();
        textoAnuncioGeral.text = "O VENCEDOR É:";
        yield return new WaitForSeconds(2.0f);
        textoVencedor.text = vencedor.Key.ToUpper();
        textoVencedor.gameObject.SetActive(true);
        FindAnyObjectByType<AudioManager>().Play("Victory");
        botaoVoltar.gameObject.SetActive(true);
    }

    IEnumerator AnimarPontuacao(string nomeJogador, int pontos)
    {
        TextMeshProUGUI textoPontos = placarUIJogadores[nomeJogador];
        int pontuacaoInicial = int.Parse(textoPontos.text);
        int pontuacaoFinal = pontuacaoInicial + pontos;
        float duracao = 1.0f;
        float tempoDecorrido = 0;
        Color corOriginal = textoPontos.color;
        Color corDestaque = pontos >= 0 ? Color.green : Color.red;
        textoPontos.color = corDestaque;
        FindAnyObjectByType<AudioManager>().Play("Tick");
        while (tempoDecorrido < duracao)
        {
            tempoDecorrido += Time.deltaTime;
            float pontuacaoAtual = Mathf.Lerp(pontuacaoInicial, pontuacaoFinal, tempoDecorrido / duracao);
            textoPontos.text = Mathf.RoundToInt(pontuacaoAtual).ToString();
            yield return null;
        }
        textoPontos.text = pontuacaoFinal.ToString();
        yield return new WaitForSeconds(0.2f);
        textoPontos.color = corOriginal;
        FindAnyObjectByType<AudioManager>().Stop("Tick");
    }

    public void RetornarAoMenu()
    {
        FindAnyObjectByType<AudioManager>().Play("Click");
        SceneManager.LoadScene("MainMenu");
    }

    public Color GetColor(Jogador jogador)
    {
        string cor = jogador.Cor;
        switch (cor)
        {
            case "azul":
                return new Color(55f / 255f, 182f / 255f, 243f / 255f);
            case "amarelo":
                return new Color(254f / 255f, 222f / 255f, 90f / 255f, 1f);
            case "vermelho":
                return new Color(254f / 255f, 48f / 255f, 48f / 255f, 1f);
            case "rosa":
                return new Color(254f / 255f, 101f / 255f, 195f / 255f, 1f);
            case "verde":
                return new Color(0f / 255f, 192f / 255f, 99f / 255f, 1f);
            default:
                return Color.white;
        }
    }
}
