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
            // �J�[�h�̌��� => �ύX�͂ł��Ȃ�(����{�^���������Ȃ�/�J�[�h�̌����͂ł��Ȃ�)
            IsSubmitted = true;
            // GameMaster�ɒʒm
            OnSubmitAction?.Invoke();
            submitButton?.SetActive(false);
        }
    }

    public  void RandomSubmit()
    {
        // ��D���烉���_���ŃJ�[�h�𔲂����
        Card card = hand.RandomRemove();
        // ��o�p��Set
        submitPosition.Set(card);
        // ��o��GameMaster�ɒʒm����
        IsSubmitted = true;
        OnSubmitAction?.Invoke();
        hand.ResetPosition();
    }
    public void SetSubmitCard(int number)
    {
        // ��D���烉���_���ŃJ�[�h�𔲂����
        Card card = hand.Remove(number);
        // ��o�p��Set
        submitPosition.Set(card);
        // ��o��GameMaster�ɒʒm����
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
