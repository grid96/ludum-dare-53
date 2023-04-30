using UnityEngine;

public class IntroManager : MonoBehaviour
{
    public static IntroManager Instance { get; private set; }

    public IntroManager() => Instance = this;

    private void Start() => Init();

    private async void Init()
    {
        MapManager.Instance.Load(0);
        await GridGamesIntroManager.Instance.Play();
#if !UNITY_EDITOR
        await DialogManager.Instance.WelcomeDialog();
#endif
    }
}