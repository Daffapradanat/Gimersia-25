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
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.05f); // Real time delay, tetap jalan walaupun timescale = 0

        // Kembalikan time scale ke normal
        Time.timeScale = 0.7f;
        

    }
    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.CompareTag("wall"))
        {
            Time.timeScale = 1f;
        }


        if (collider.gameObject.CompareTag("brick"))
        {
            Debug.Log("kena brick");
            // Time.timeScale = 0.7f;
            StartCoroutine(OnTouchBrick(collider.gameObject));

        }
    }
}
