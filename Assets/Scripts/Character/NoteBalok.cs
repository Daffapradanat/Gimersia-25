using UnityEngine;

public class NoteBalok : MonoBehaviour
{
    public int life = 1;

    public int Life
    {
        get => life;
        set
        {
            life = value;
            UpdateBlock();
        }
    }

    public void UpdateBlock()
    {
        Debug.Log("Hitt balock note");
        if (Life <= 0)
        {
            GameManager.Instance.totalBlock--;
            Destroy(gameObject);
        }

    }
}
