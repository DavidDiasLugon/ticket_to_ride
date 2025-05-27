//Luiz
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : MonoBehaviour
{
    public GameObject Card1;
    public GameObject Hand;

    List<GameObject> cards = new List<GameObject>();

    void Start()
    {
        cards.Add(Card1);
    }
    public void OnClick()
    {
        for (var i = 0; i < 5; i++)
        {
            GameObject playercard = Instantiate(cards[Random.Range(0, cards.Count)], new Vector3(0, 0, 0), Quaternion.identity);
            playercard.transform.SetParent(Hand.transform, false);
        }
    }






}