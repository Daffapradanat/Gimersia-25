using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // singletoon
    public static GameManager Instance;

    // Sc
    [SerializeField] CharacterMove characterMoveSc;
    [SerializeField] HUDManager hUDManager;
    [SerializeField] BallSc ballSc;

    // variabel
    public int health = 5, score, combo, level, totalBlock, totalHealthBoss;
    public bool isCombo = false, isPlay = false, isHaveBoss = false;

    public int bintangSatu_Menit, bintangDua_Menit, bintangTiga_Menit;

    public float totalPlaySeconds, ultimate;

    public float Ultimate
    {
        get
        {
            return ultimate;
        }
        set
        {
            ultimate = value;
            hUDManager.UpdateBarUltimate(ultimate);
        }
    }

    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            hUDManager.textScore.text = value.ToString();
        }
    }

    void Start()
    {
        Instance = this;
        characterMoveSc.isCanMove = false;
        isPlay = false;

        List<NoteBalok> noteComps = FindObjectsOfType<NoteBalok>().ToList();
        totalBlock = noteComps.Count;

        StartCoroutine(hUDManager.ShowPrePlay("Clear The Stage And Protect Our Ship", level, 3f, () =>
        {
            isPlay = true;
            characterMoveSc.isCanMove = true;
            ballSc.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }));
    }

    void Update()
    {
        totalPlaySeconds += Time.unscaledDeltaTime;

        if (IsLevelCompleted())
        {
            StageWasClear();
        }

        if(health <= 0)
        {
            characterMoveSc.isCanMove = false;
            ballSc.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            isPlay = false;

            StageWasClear();
        }
    }

    public void AddScore(int value = 1)
    {
        Score += value * ((combo * 15) + combo);
        hUDManager.textScore.text = Score.ToString();
    }

    bool IsLevelCompleted()
    {
        if (isHaveBoss)
        {
            // berdasarkan boss
            if (totalHealthBoss <= 0)
                return true;
        }
        else
        {
            // berdasarkan balok
            if (totalBlock <= 0) return true;
        }

        return false;
    }

    void StageWasClear()
    {
        hUDManager.ShowCompletedStage(GetTextGrading(totalPlaySeconds), 4f, () =>
        {
            // Show UI Scoring
            Debug.Log("Load scene scoring");
        });
    }

    public int GetCountStar(float second)
    {
        if (second / 60f <= bintangTiga_Menit)
        {
            return 3;
        }
        else if (second / 60f <= bintangDua_Menit)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }
    string GetTextGrading(float second)
    {
        if (second / 60f <= bintangTiga_Menit)
        {
            return "YOU'RE A MASTER";
        }
        else if (second / 60f <= bintangDua_Menit)
        {
            return "YOU'RE SKILLED";
        }
        else
        {
            return "YOU NEED PRACTICE";
        }
    }

}
