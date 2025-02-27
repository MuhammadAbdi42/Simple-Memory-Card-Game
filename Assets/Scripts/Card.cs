using DG.Tweening;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] GameObject cardFace;
    [SerializeField] GameObject cardBack;
    private bool solved = false;
    public bool flipped = false;
    public void OnClick()
    {
        if (!solved && CardManager.Instance.Flippable && !flipped)
        {
            FlipTheCard();
            SendCard();
        }
    }
    public void FlipTheCard()
    {
        transform.DORotate(new Vector3(0, flipped ? 0 : 180, 0), 0.25f);
        flipped = !flipped;
    }
    private void SendCard()
    {
        CardManager.Instance.GetCard(this.gameObject);
    }
    public void SetSolved()
    {
        solved = true;
    }
}
