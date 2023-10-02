using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void OnStartButton()
    {
        GameDataManager.Instance.IsOnlineBattle = false;
        SceneManager.LoadScene("Game");
    }

    public void OnOnlineStartButton()
    {
        GameDataManager.Instance.IsOnlineBattle = true;
        SceneManager.LoadScene("Online");
    }
}
