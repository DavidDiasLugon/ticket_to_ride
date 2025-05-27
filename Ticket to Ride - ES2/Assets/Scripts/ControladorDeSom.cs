using UnityEngine;
using UnityEngine.UI;

public class ControladorDeSom : MonoBehaviour
{
    private bool estadoSom = true;
    [SerializeField] private AudioSource musicaFundo;

    [SerializeField] private Sprite volumeOn;
    [SerializeField] private Sprite volumeOff;
    [SerializeField] private Image buttonImage;
    public void LigarDesligarSom()
    {
        estadoSom = !estadoSom;
        musicaFundo.enabled = estadoSom;

        if (estadoSom)
        {
            buttonImage.sprite = volumeOn;
        }
        else
        {
            buttonImage.sprite = volumeOff;
        }
    }

    public void Volume(float value)
    {
        musicaFundo.volume = value;
    }
}
