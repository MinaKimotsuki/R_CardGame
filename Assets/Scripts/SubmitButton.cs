using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SubmitButton : MonoBehaviour
{
    void Start()
    {
        transform.DOScale(0.1f, 1f)
            .SetRelative(true)
            .SetEase(Ease.OutQuart)
            .SetLoops(-1, LoopType.Restart);
    }
}
