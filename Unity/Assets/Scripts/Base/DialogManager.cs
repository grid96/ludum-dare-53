using System;
using System.Collections.Generic;
using CarterGames.Assets.AudioManager;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [SerializeField] private bool disableInEditor;
    [SerializeField] private Animator avatarAnimator;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Vector2 shownPosition;
    [SerializeField] private Vector2 hiddenPosition;

    private const float typewriterDelay = 0.025f;
    private const string beepSoundName = "Beep";
    private const int beepStep = 3;
    private const float beepMinPitch = 1;
    private const float beepMaxPitch = 1.5f;
    private readonly Queue<(RuntimeAnimatorController avatar, string message)> messageQueue = new();
    private bool isTyping;
    private Tween positionTween;
    private event EventHandler clicked;
    public bool IsShown { get; private set; }

    public DialogManager() => Instance = this;

    private async UniTask ShowMessage(RuntimeAnimatorController avatar, string message)
    {
        if (!IsShown)
            await Show();
        messageQueue.Enqueue((avatar, message));
        if (isTyping)
            return;
        isTyping = true;
        while (messageQueue.Count > 0)
        {
            (RuntimeAnimatorController currentAvatar, string currentMessage) = messageQueue.Dequeue();
            avatarAnimator.runtimeAnimatorController = currentAvatar;
            await TypewriterEffect(currentMessage);
        }

        isTyping = false;
    }

    private async UniTask TypewriterEffect(string message)
    {
        messageText.text = "";
        foreach (var c in message)
        {
            if (messageText.text.Length % beepStep == 0)
                PlayBeep();
            messageText.text += c;
            await UniTask.Delay((int)(typewriterDelay * 1000));
        }
    }

    private void PlayBeep()
    {
        AudioManager.instance.Play(beepSoundName, 0.5f, Random.Range(beepMinPitch, beepMaxPitch));
    }

    private void Awake()
    {
        transform.ToRect().anchoredPosition = hiddenPosition;
        messageText.text = "";
    }

    private async UniTask Show() => await AnimateShown(true);
    public async UniTask Hide() => await AnimateShown(false);

    private async UniTask AnimateShown(bool shown)
    {
#if UNITY_EDITOR
        if (disableInEditor)
            return;
#endif
        if (shown == IsShown)
            return;
        IsShown = shown;
        messageText.text = "";
        positionTween?.Kill(true);
        var tcs = new UniTaskCompletionSource<bool>();
        positionTween = transform.ToRect().DOAnchorPos(shown ? shownPosition : hiddenPosition, 0.5f).OnComplete(() => tcs.TrySetResult(true));
        await tcs.Task;
    }

    private async UniTask ClickToContinue()
    {
#if UNITY_EDITOR
        if (disableInEditor)
            return;
#endif
        var tcs = new UniTaskCompletionSource<bool>();
        void Handler(object sender, EventArgs e) => tcs.TrySetResult(true);
        clicked += Handler;
        await tcs.Task;
        clicked -= Handler;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
            clicked?.Invoke(this, EventArgs.Empty);
    }
}