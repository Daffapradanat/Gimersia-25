using System.Collections;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float lifetime = 0.1f;
    void Start()
    {

    }

    void OnEnable()
    {
        animator.Play("start_shockwave", 0, 0);
        StartCoroutine(SetFalseInTime());
    }
    
    IEnumerator SetFalseInTime()
    {
        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
    }
}
