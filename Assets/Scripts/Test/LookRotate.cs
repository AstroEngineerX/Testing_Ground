using UnityEngine;

public class LookRotate : MonoBehaviour
{
    public Transform target;
    public bool look = true;
    void Update()
    {
        if (target == null) return;
        if (look)
        {
            Vector3 relativePos = target.position - transform.position;
            transform.rotation = Quaternion.LookRotation(relativePos);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
