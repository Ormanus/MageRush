using UnityEngine;

public class Shield : MonoBehaviour
{
    public int hitpoints = 5;

    private void Update()
    {
        transform.position = GetComponent<Projectile>().owner.transform.position;
    }
}
