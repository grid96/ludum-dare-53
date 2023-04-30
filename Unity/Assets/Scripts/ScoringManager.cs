using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoringManager : MonoBehaviour
{
    public static ScoringManager Instance { get; private set; }
    
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text personalBestText;
    [SerializeField] private GameObject[] letters;
    
    public bool IsShown { get; private set; }
    
    public ScoringManager() => Instance = this;
    
    public void Show(float time)
    {
        IsShown = true;
        // timeText.text = time.ToString("F2");
        // personalBestText.text = PlayerPrefs.GetFloat("PersonalBest", 0).ToString("F2");
        animator.Play("Scoring");
    }
    
    public void Hide()
    {
        IsShown = false;
    }
}
