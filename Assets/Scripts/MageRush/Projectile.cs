using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum Element
    {
        None,
        Water,
        Earth,
        Fire,
        Air
    }

    public Sprite[] sprites;
    public GameObject explosion;
    public AnimationController animationController;
    public float speed;
    public Vector3 direction;

    public float duration = 1;
    float startTime;
    // Start is called before the first frame update
    void Start()
    {
        animationController.ChangeAnimation(sprites);
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += speed * Time.deltaTime * direction;
        if (Time.time > startTime + duration)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
