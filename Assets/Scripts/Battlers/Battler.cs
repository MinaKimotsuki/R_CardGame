using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Battler : MonoBehaviour
{
    [SerializeField] BattlerHand hand;
    [SerializeField] SubmitPosition submitPosition;
    [SerializeField] GameObject submitButton;
    public bool IsSubmitted { get; private set; }
    public bool IsFirstSubmit { get; set; }
    public bool IsAddNumberMode { get; set; }
    public int AddNumber { get; private set; }
    public UnityAction OnSubmitAction;

    public BattlerHand Hand { get => hand; }
    public Card SubmitCard { get => submitPosition.SubmitCard; }
    public int Life { get; set; }

    public void SetCardToHand(Card card)
    {
        hand.Add(card);
        card.OnClickCard = SelectedCard;
    }

    void SelectedCard(Card card)
    {
        if (IsSubmitted)
        {
            return;
        }
        if (submitPosition.SubmitCard)
        {
            hand.Add(submitPosition.SubmitCard);
        }
        hand.Remove(card);
        submitPosition.Set(card);
        hand.ResetPosition();
        submitButton?.SetActive(true);
    }

    public void OnSubmitButton()
    {
        if (submitPosition.SubmitCard)
        {
            // カードの決定 => 変更はできない(決定ボタンを押せない/カードの交換はできない)
            IsSubmitted = true;
            // GameMasterに通知
            OnSubmitAction?.Invoke();
            submitButton?.SetActive(false);
        }
    }

    public  void RandomSubmit()
    {
        // 手札からランダムでカードを抜き取る
        Card card = hand.RandomRemove();
        // 提出用にSet
        submitPosition.Set(card);
        // 提出をGameMasterに通知する
        IsSubmitted = true;
        OnSubmitAction?.Invoke();
        hand.ResetPosition();
    }
    public void SetSubmitCard(int number)
    {
        // 手札からランダムでカードを抜き取る
        Card card = hand.Remove(number);
        // 提出用にSet
        submitPosition.Set(card);
        // 提出をGameMasterに通知する
        IsSubmitted = true;
        OnSubmitAction?.Invoke();
        hand.ResetPosition();
    }

    public void SetupNextTurn()
    {
        IsSubmitted = false;
        submitPosition.DeleteCard();
        if (IsAddNumberMode)
        {
            IsAddNumberMode = false;
            AddNumber = 2;
        }
        else
        {
            AddNumber = 0;
        }
    }

}
