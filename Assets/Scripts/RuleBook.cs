using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleBook : MonoBehaviour
{
    //*�J�[�h����
    //�E�}�W�V�����i�J�[�h���ʖ����j������ΐ����Ό�
    //�E����i���J�[�h���ɏo���j������Ȃ�ǉ�����
    //�E�����i���̃^�[�����������j������Ȃ��������
    //�E�ÎE�҂����āA���q�����Ȃ��Ȃ琔�����ʔ��]
    //�E���q�ƕP������Ȃ�Game�̏�������
    //�E�����܂ł���ΐ����Ό��i��b�Ȃ�2�{�j
    //*/
    public Result GetResult(Battler player, Battler enemy)
    {
        if (player.SubmitCard.Base.Type == CardType.Magician || enemy.SubmitCard.Base.Type == CardType.Magician)
        {
            return NumberBattle(player, enemy, ministerEffect: false, reverseEfffect: false);
        }

        if (player.SubmitCard.Base.Type == CardType.Spy && enemy.SubmitCard.Base.Type != CardType.Spy)
        {
            enemy.IsFirstSubmit = true;
        }
        if (enemy.SubmitCard.Base.Type == CardType.Spy && player.SubmitCard.Base.Type != CardType.Spy)
        {
            player.IsFirstSubmit = true;
        }

        // �E���R�̌��ʁi���̃^�[��+2�j
        //  �E�ǉ����ʂ���
        if (player.SubmitCard.Base.Type == CardType.Shogun)
        {
            player.IsAddNumberMode = true;
        }
        if (enemy.SubmitCard.Base.Type == CardType.Shogun)
        {
            enemy.IsAddNumberMode = true;
        }

        if (player.SubmitCard.Base.Type == CardType.Clown || enemy.SubmitCard.Base.Type == CardType.Clown)
        {
            return Result.TurnDraw;
        }

        if (
            (player.SubmitCard.Base.Type == CardType.Assassin && enemy.SubmitCard.Base.Type != CardType.Prince)
            || (enemy.SubmitCard.Base.Type == CardType.Assassin && player.SubmitCard.Base.Type != CardType.Prince)
            )
        {
            //�������ʔ��]
            return NumberBattle(player, enemy, ministerEffect: true, reverseEfffect: true);
        }

        if (player.SubmitCard.Base.Type == CardType.Princess && enemy.SubmitCard.Base.Type == CardType.Prince)
        {
            return Result.GameWin;
        }
        if (enemy.SubmitCard.Base.Type == CardType.Princess && player.SubmitCard.Base.Type == CardType.Prince)
        {
            return Result.GameLose;
        }

        return NumberBattle(player, enemy, ministerEffect: true, reverseEfffect: false);
    }

    Result NumberBattle(Battler player, Battler enemy, bool ministerEffect, bool reverseEfffect)
    {
        if (ministerEffect == false)
        {
            if (player.SubmitCard.Base.Number + player.AddNumber > enemy.SubmitCard.Base.Number + enemy.AddNumber)
            {
                if (reverseEfffect)
                {
                    return Result.TurnLose;
                }
                return Result.TurnWin;
            }
            if (player.SubmitCard.Base.Number + player.AddNumber < enemy.SubmitCard.Base.Number + enemy.AddNumber)
            {
                if (reverseEfffect)
                {
                    return Result.TurnWin;
                }
                return Result.TurnLose;
            }
        }
        else
        {
            if (player.SubmitCard.Base.Number + player.AddNumber > enemy.SubmitCard.Base.Number + enemy.AddNumber)
            {
                if (reverseEfffect)
                {
                    if (enemy.SubmitCard.Base.Type == CardType.Minister)
                    {
                        return Result.TurnLose2;
                    }
                    return Result.TurnLose;
                }
                if (player.SubmitCard.Base.Type == CardType.Minister)
                {
                    return Result.TurnWin2;
                }
                return Result.TurnWin;
            }
            else if (player.SubmitCard.Base.Number + player.AddNumber < enemy.SubmitCard.Base.Number + enemy.AddNumber)
            {
                if (reverseEfffect)
                {
                    if (enemy.SubmitCard.Base.Type == CardType.Minister)
                    {
                        return Result.TurnWin2;
                    }
                    return Result.TurnWin;
                }
                if (enemy.SubmitCard.Base.Type == CardType.Minister)
                {
                    return Result.TurnLose2;
                }
                return Result.TurnLose;
            }
        }
        
        return Result.TurnDraw;
    }
}

public enum Result
{
    TurnWin,
    TurnLose,
    TurnDraw,
    TurnWin2,
    TurnLose2,
    GameWin,
    GameLose,
    GameDraw,
}
