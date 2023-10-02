using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    // ÉJÅ[ÉhUI
    // ÉQÅ[ÉÄì‡ÇÃèàóù
    [SerializeField] Text nameText;
    [SerializeField] Text numberText;
    [SerializeField] Image icon;
    [SerializeField] Text descriptionText;
    [SerializeField] GameObject hidePanel;
    public CardBase Base { get; private set; }
    // ä÷êîÇìoò^Ç≈Ç´ÇÈ
    public UnityAction<Card> OnClickCard;

    public void Set(CardBase cardBase, bool isEnemy)
    {
        Base = cardBase;
        nameText.text = cardBase.Name;
        numberText.text = cardBase.Number.ToString();
        icon.sprite = cardBase.Icon;
        descriptionText.text = cardBase.Desctiption;
        hidePanel.SetActive(isEnemy);
    }

    public void OnClick()
    {
        OnClickCard?.Invoke(this);
    }

    public void OnPointerEnter()
    {
        transform.position += Vector3.up * 0.3f;
        transform.localScale = Vector3.one * 1.1f;
        GetComponentInChildren<Canvas>().sortingLayerName = "Overlay";
    }
    public void OnPointerExit()
    {
        transform.position -= Vector3.up * 0.3f;
        transform.localScale = Vector3.one;
        GetComponentInChildren<Canvas>().sortingLayerName = "Default";
    }

    public void Open()
    {
        hidePanel.SetActive(false);
    }
}
