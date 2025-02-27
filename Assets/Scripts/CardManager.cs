using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }
    [SerializeField] public GameObject cardContainer;
    [SerializeField] public GameObject cardPrefab;
    [SerializeField] public List<GameObject> cards = new();
    [SerializeField] public List<Sprite> cardSprites = new();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        DontDestroyOnLoad(gameObject); // Keeps it across scenes
    }
    public void GameStart(int difficulity)
    {
        CleaningCards();
        GeneratingCards(difficulity);
        SettingSprite();
    }
    private void GeneratingCards(int difficulity)
    {
        int cardsCount = 0;
        switch (difficulity)
        {
            case 1:
                cardsCount = 10;
                break;
            case 2:
                cardsCount = 30;
                break;
            case 3:
                cardsCount = 50;
                break;
            case 4:
                cardsCount = 80;
                break;
        }
        for (int i = 0; i < cardsCount; i++)
        {
            GameObject card = Instantiate(cardPrefab, cardContainer.transform);
            cards.Add(card);
        }
    }
    public void CleaningCards()
    {
        foreach (GameObject card in cards)
        {
            Destroy(card);
        }
        cards.Clear();
    }
    private void SettingSprite()
    {
        List<Sprite> spritesList = new List<Sprite>(cardSprites);
        List<GameObject> cardsList = new List<GameObject>(cards);
        while (cardsList.Count > 0)
        {
            GameObject cardOne = cardsList[Random.Range(0, cardsList.Count)];
            cardsList.Remove(cardOne);
            GameObject cardTwo = cardsList[Random.Range(0, cardsList.Count)];
            cardsList.Remove(cardTwo);
            Sprite _sprite = spritesList[Random.Range(0, spritesList.Count)];
            spritesList.Remove(_sprite);

            GameObject cardOneBack = cardOne.transform.Find("Back").gameObject;
            cardOneBack.GetComponent<Image>().sprite = _sprite;
            GameObject cardTwoBack = cardTwo.transform.Find("Back").gameObject;
            cardTwoBack.GetComponent<Image>().sprite = _sprite;
        }
    }
    private GameObject cardOne = null;
    private GameObject cardTwo = null;
    public bool Flippable = true;
    public void GetCard(GameObject card)
    {
        if (cardOne == null)
        {
            cardOne = card;
        }
        else if (cardTwo == null)
        {
            cardTwo = card;
            Flippable = false;
            GameManager.Instance.tries++;
            StartCoroutine(CheckCards());
        }
    }
    IEnumerator CheckCards()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject cardOneBack = cardOne.transform.Find("Back").gameObject;
        GameObject cardTwoBack = cardTwo.transform.Find("Back").gameObject;
        if (cardOneBack.GetComponent<Image>().sprite == cardTwoBack.GetComponent<Image>().sprite)
        {
            cardOne.GetComponent<Card>().SetSolved();
            cardTwo.GetComponent<Card>().SetSolved();
            GameManager.Instance.progression++;
        }
        else
        {
            cardOne.GetComponent<Card>().FlipTheCard();
            cardTwo.GetComponent<Card>().FlipTheCard();
        }
        cardOne = null;
        cardTwo = null;
        Flippable = true;
    }
}
