//luiz
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private bool isDragging = false;
    private int isOverDropZone = -1;
    private bool isDraggable = true;
    private GameObject dropZone;
    private GameObject startParent;
    private Vector2 startPosition;

    //    private Controle controle;

    //    private void Start(){
    //        controle = GameObject.Find("GameManager").GetComponent<GameManager>().controle;
    //    }

    void Update()
    {
        if (isDraggable && isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.transform.name == "AreaCombate")
        {
            isOverDropZone = 0;

        }
        else if (collision.transform.tag == "JogadorPrincipal")
        {
            isOverDropZone = 1;

        }
        else if (collision.transform.tag == "JogadorAdversario")
        {
            isOverDropZone = 2;

        }
        else if (collision.transform.tag == "AreaDelete")
        {
            isOverDropZone = 3;

        }
        else
        {
            isOverDropZone = -1;
        }

        dropZone = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverDropZone = -1;
        dropZone = null;
    }

    public void StartDrag()
    {
        startParent = transform.parent.gameObject;
        startPosition = transform.position;
        isDragging = true;
    }

    public void EndDrag()
    {
        isDragging = false;

        if (isOverDropZone == 0)
        {
            isDraggable = false;
        }
        else
        {
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
        }
    }



}