using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI sliderText;

    public void ValueChanged()
    {
        sliderText.text = slider.value.ToString("0");
        GameSettings.qtdJogadores = Mathf.RoundToInt(slider.value);
    }
}
