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
        Air,
        BigWater,
        BigEarth,
        BigFire,
        BigAir
    }

    public SpriteAnimationSet sprites;
    public GameObject explosion;
    public AnimationController animationController;
    public float speed;
    public Vector3 direction;
    public Element element;

    public float duration = 1;
    float startTime;

    void Start()
    {
        animationController.ChangeAnimation(sprites.spriteSets[0]);
        animationController.SetDirection(direction.x < 0 ? AnimationController.AnimationDirection.Left: AnimationController.AnimationDirection.Right);
        startTime = Time.time;
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime * direction;
        if (Time.time > startTime + duration)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<CharacterController>(out var character))
        {
            if (element == Element.Water)
                character.TakeDamage(-1);
            else
                character.TakeDamage(1);
        }
        else
        {
            if (element == Element.Air)
            {
                direction = Vector2.Reflect(direction, collision.contacts[0].normal);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
