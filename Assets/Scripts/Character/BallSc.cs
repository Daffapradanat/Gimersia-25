using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BallSc : MonoBehaviour
{
    Rigidbody2D rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    IEnumerator OnTouchBrick(GameObject noteblock)
    {

        // Freeze time (pause effect)
        Camera.main.DOShakePosition(0.2f, 0.3f, 10, 60, true);

        noteblock.GetComponent<NoteBalok>().Life--;

        GameManager.Instance.AddScore();
        GameManager.Instance.Ultimate += 0.02f;
        GameManager.Instance.combo++;
        HUDManager.Instance.ShowCombo(GameManager.Instance.combo++);

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.05f); // Real time delay, tetap jalan walaupun timescale = 0

        // Kembalikan time scale ke normal
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(0.5f);
        Camera.main.transform.position = new Vector3(-1.05f, 0, -10);

        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
    }


    IEnumerator OnTouchWall()
    {
        Time.timeScale = 0.6f;
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1f;
    }
    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.CompareTag("wall"))
        {
            StartCoroutine(OnTouchWall());
        }


        if (collider.gameObject.CompareTag("brick"))
        {
            Debug.Log("kena brick");
            // Time.timeScale = 0.7f;
            StartCoroutine(OnTouchBrick(collider.gameObject));

        }

        if (collider.gameObject.CompareTag("shield"))
        {

        }

        if (collider.gameObject.CompareTag("ship"))
        {
            Camera.main.DOShakePosition(0.2f, 0.3f, 10, 60, true);
            GameManager.Instance.health--;
            HUDManager.Instance.UpdateBarHealth(GameManager.Instance.health / 5f);

            HUDManager.Instance.HideCombo();
            HUDManager.Instance.textCombo.text = 0.ToString();
            GameManager.Instance.combo = 0;
        }
        
    }
}
