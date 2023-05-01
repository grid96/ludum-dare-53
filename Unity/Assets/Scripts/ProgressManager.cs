using CarterGames.Assets.AudioManager;
using TMPro;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private Material parcelMaterial;
    [SerializeField] private GameObject cooldownIndicator;
    [SerializeField] private RectTransform cooldownBar;

    public bool OnCooldown => cooldown > 0;

    private const float cooldownAmount = 2;
    private float cooldown;

    public ProgressManager() => Instance = this;

    private float timer;
    private int progress;
    private int total;

    public int Progress => progress;

    public void Init(int total)
    {
        this.total = total;
        timer = 0;
        progress = -1;
        MakeProgress();
    }

    public void MakeProgress()
    {
        progress++;
        progressText.text = $"<cspace=-0.4em><voffset=.4em>{progress}<size=120%><voffset=0em>/<size=100%><voffset=-.4em><cspace=0em>{total}";
        ResetCooldown();
        if (progress == total)
            ScoringManager.Instance.Show(timer);
    }

    public void StartCooldown()
    {
        cooldown = cooldownAmount;
        cooldownIndicator.SetActive(true);
    }

    private void ResetCooldown()
    {
        cooldown = 0;
        cooldownIndicator.SetActive(false);
    }

    private void Update()
    {
        if (!ScoringManager.Instance.IsShown)
        {
            if (timer < 300 && timer + Time.deltaTime >= 300)
                _ = DialogManager.Instance.LongTimeDialog();
            timer += Time.deltaTime;
        }

        timerText.text = $"<mspace=0.5em>{(int)timer:00}</mspace>:<mspace=0.5em>{(int)(timer % 1 * 100):00}";
        parcelMaterial.color = Color.white * (OnCooldown ? 0.5f : 0.75f);
        if (!OnCooldown)
            return;
        cooldown -= Time.deltaTime;
        cooldownBar.localScale = new Vector3(cooldown / cooldownAmount, 1, 1);
        if (!OnCooldown)
        {
            AudioManager.instance.Play("Cooldown", 0.125f);
            ResetCooldown();
        }
    }
}