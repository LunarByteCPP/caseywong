using UnityEngine;

public class ResetPosition : MonoBehaviour
{

    public KeyCode resetKey = KeyCode.R;
    public Rigidbody rb;


    void FixedUpdate()
    {

        if (Input.GetKeyDown(resetKey))
        {
            ResetToOrigin();
        }
    }


    private void ResetToOrigin()
    {
        transform.position = new Vector3(0,0,0);
    }
}