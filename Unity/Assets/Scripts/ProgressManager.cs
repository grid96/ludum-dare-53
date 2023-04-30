using TMPro;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [SerializeField] private TMP_Text progressText;
    [SerializeField] private Material parcelMaterial;
    [SerializeField] private GameObject cooldownIndicator;
    [SerializeField] private RectTransform cooldownBar;

    public bool OnCooldown => cooldown > 0;

    private const float cooldownAmount = 3;
    private float cooldown;

    public ProgressManager() => Instance = this;
    
    public int progress { get; private set; }
    public int total { get; private set; }
    
    public void Init(int total)
    {
        this.total = total;
        progress = -1;
        MakeProgress();
    }

    public void MakeProgress()
    {
        progress++;
        progressText.text = $"<cspace=-0.4em><voffset=.4em>{progress}<size=120%><voffset=0em>/<size=100%><voffset=-.4em>{total}";
        ResetCooldown();
        if (progress == total)
            ScoringManager.Instance.Show(Time.timeSinceLevelLoad);
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
        parcelMaterial.color = Color.white * (OnCooldown ? 0.5f : 0.75f);
        if (!OnCooldown)
            return;
        cooldown -= Time.deltaTime;
        cooldownBar.localScale = new Vector3(cooldown / cooldownAmount, 1, 1);
        if (!OnCooldown)
            ResetCooldown();
    }
}