using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleBook : MonoBehaviour
{
    //*カード効果
    //・マジシャン（カード効果無効）がいれば数字対決
    //・密偵（次カードを先に出す）がいるなら追加効果
    //・道化（このターン引き分け）がいるなら引き分け
    //・暗殺者がいて、王子がいないなら数字効果反転
    //・王子と姫がいるならGameの勝利判定
    //・ここまでくれば数字対決（大臣なら2倍）
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

        // ・将軍の効果（次のターン+2）
        //  ・追加効果がつく
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
            //数字効果反転
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
