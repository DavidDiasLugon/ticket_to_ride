using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsController : MonoBehaviour
{
    public List<GameObject> playerList;
    public Color onColor;
    public Color offColor;
    public GameObject botaoIniciar;

    void Awake()
    {
        botaoIniciar.SetActive(false);
        foreach (var player in playerList)
        {
            Toggle toggle = player.GetComponentInChildren<Toggle>();
            Image targetImage = toggle.targetGraphic as Image;
            SetColor(toggle, targetImage, toggle.isOn);

            TMP_Dropdown dropdown = player.GetComponentInChildren<TMP_Dropdown>();
            if (dropdown != null)
            {
                dropdown.ClearOptions();
                dropdown.AddOptions(new List<string> { "IA", "REAL" });
                dropdown.SetValueWithoutNotify(1);
                dropdown.gameObject.SetActive(toggle.isOn);

                dropdown.onValueChanged.AddListener((int index) =>
                {
                    // Lógica para tratar a mudança no Dropdown
                    string selected = index == 0 ? "IA" : "REAL";
                    Debug.Log("Seleção: " + selected);
                });
            }

            toggle.onValueChanged.AddListener((bool isOn) =>
            {
                FindAnyObjectByType<AudioManager>().Play("Click");
                SetColor(toggle, targetImage, isOn);
                UpdateStartButton();

                if (dropdown != null)
                {
                    dropdown.gameObject.SetActive(isOn);
                }
            });
        }
        
    }

    public void SetColor(Toggle toggle, Image image, bool isOn)
    {
        if (image != null)
        {
            image.color = isOn ? onColor : offColor;
        }
    }

    public void UpdateStartButton()
    {
        bool anyToggleOn = playerList.Exists(player =>
        {
            Toggle toggle = player.GetComponentInChildren<Toggle>();
            return toggle.isOn;
        });
        botaoIniciar.SetActive(anyToggleOn);
    }

    public void StartGame()
    {
        // Lógica para iniciar o jogo
        Debug.Log("Iniciando o jogo...");
    }

}
