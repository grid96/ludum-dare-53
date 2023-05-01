using UnityEngine;

public class IntroManager : MonoBehaviour
{
    public static IntroManager Instance { get; private set; }
    
    public bool IsShown { get; private set; } = true;

    public IntroManager() => Instance = this;

    private void Start() => Init();

    private async void Init()
    {
        MapManager.Instance.Load(0);
        await GridGamesIntroManager.Instance.Play();
        IsShown = false;
// #if !UNITY_EDITOR
        await DialogManager.Instance.WelcomeDialog();
// #endif
    }
}