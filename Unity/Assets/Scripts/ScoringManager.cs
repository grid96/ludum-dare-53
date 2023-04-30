using CarterGames.Assets.AudioManager;
using TMPro;
using UnityEngine;

public class ScoringManager : MonoBehaviour
{
    public static ScoringManager Instance { get; private set; }

    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text personalBestText;
    [SerializeField] private GameObject[] letters;
    [SerializeField] private TMP_Text[] targetTimeTexts;

    public bool IsShown { get; private set; }

    public ScoringManager() => Instance = this;
    
    private float time;

    public void Show(float time)
    {
        this.time = time;
        IsShown = true;
        timeText.text = ToTimeString(time);
        float personalBest = PlayerPrefs.GetFloat($"PersonalBestLevel{MapManager.Instance.Level}", float.MaxValue);
        if (time < personalBest)
        {
            personalBestText.text = "<size=50%>New Personal Best!";
            PlayerPrefs.SetFloat($"PersonalBestLevel{MapManager.Instance.Level}", time);
        }
        else
            personalBestText.text = $"<size=50%>Personal Best<size=100%>\n{ToTimeString(personalBest)}";

        for (int i = 0; i < letters.Length; i++)
            letters[i].SetActive(i == 0 || time <= MapManager.Instance.Level.TargetTimes[i - 1]);

        for (int i = 0; i < targetTimeTexts.Length; i++)
        {
            targetTimeTexts[i].text = $"< {ToTimeString(MapManager.Instance.Level.TargetTimes[i])}";
            targetTimeTexts[i].gameObject.SetActive(i == 0 || time > MapManager.Instance.Level.TargetTimes[i]);
        }

        animator.Play("Scoring");
    }

    public void Hide()
    {
        IsShown = false;
        animator.Play("HideScoring");
    }

    public void TryAgain()
    {
        Hide();
        MapManager.Instance.Load(MapManager.Instance.Level.Index);
    }

    public void NextLevel()
    {
        Hide();
        MapManager.Instance.Load(MapManager.Instance.Level.Index + 1);
    }

    public void PlaySound(int index)
    {
        if (index == 0 || time <= MapManager.Instance.Level.TargetTimes[index - 1])
            AudioManager.instance.Play("Scoring", 0.75f, Random.Range(0.9f, 1.1f));
    }

    public static string ToTimeString(float seconds) => $"{(int)seconds:00}:{(int)(seconds % 1 * 100):00}";
}