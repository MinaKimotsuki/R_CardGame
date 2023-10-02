using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    // ONLINEêÌÇ©Ç«Ç§Ç©ÅH
    public bool IsOnlineBattle { get; set; }

    // ÉVÅ[ÉìÇÇ‹ÇΩÇ¢Ç≈Ç‡îjâÛÇ≥ÇÍÇ»Ç¢
    public static GameDataManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
