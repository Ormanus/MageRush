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
    public Vector2 direction;

    // Start is called before the first frame update
    void Start()
    {
        animationController.ChangeAnimation(sprites);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
