using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(CanvasGroup))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string corCarta;
    private CanvasGroup canvasGroup;
    private Vector3 posicaoInicial;
    private Transform paiInicial;
    public float draggingScale = 0.02f;
    private Vector3 originalScale;
    private CardHoverEffect cardHoverEffect;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        posicaoInicial = transform.position;
        paiInicial = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
        originalScale = transform.localScale;
        cardHoverEffect = GetComponent<CardHoverEffect>();
        cardHoverEffect.enabled = false; // Desativa o efeito de hover durante o arrasto
        transform.localScale = originalScale * draggingScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 screenSpot = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        screenSpot.z = 100f;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenSpot);
        transform.position = worldPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            TrackSegmentController segmentHit = hit.collider.GetComponent<TrackSegmentController>();
            if (segmentHit != null)
            {
                Debug.Log("Carta Solta sobre trilho:");
                _GameManager.Instance.controle.ConquistarRota(this, segmentHit.parentTrackController);
                return;
            }
        }
        RetornarParaMao();
    }

    public void RetornarParaMao()
    {
        transform.SetParent(paiInicial);
        transform.position = posicaoInicial;
        transform.localScale = originalScale;
        cardHoverEffect.enabled = true;
    }
}
