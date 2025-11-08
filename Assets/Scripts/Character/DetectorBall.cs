using System.Xml.Serialization;
using UnityEngine;

public class DetectorBall : MonoBehaviour
{
    
    public Rigidbody2D rbBall;
  
    void Start()
    {
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("ball"))
        {
            rbBall = collider.GetComponent<Rigidbody2D>();
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("ball"))
        {
            rbBall = null;
        }
    }
}
