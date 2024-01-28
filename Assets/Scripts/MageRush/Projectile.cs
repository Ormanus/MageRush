using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Netcode;
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
        BigAir,
        Lava,
        Mud,
        Barrier,
    }

    public SpriteAnimationSet sprites;
    public GameObject explosion;
    public AnimationController animationController;
    public float speed;
    public Vector3 direction;
    public Element element;
    public CharacterController owner;

    public float duration = 1;
    float startTime;

    void Awake()
    {
        animationController.ChangeAnimation(sprites.spriteSets.FirstOrDefault(x => x.name.Contains(name, System.StringComparison.InvariantCultureIgnoreCase)));
        startTime = Time.time;
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI);
        if (duration == 0)
        {
            return;
        }
        transform.position += speed * Time.deltaTime * direction;
        if (Time.time > startTime + duration)
        {
            if (explosion != null)
                Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    bool IsBig
    {
        get
        {
            return element == Element.BigFire
                || element == Element.BigWater
                || element == Element.BigEarth
                || element == Element.BigAir;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<CharacterController>(out var character))
        {
            if (character == owner)
            {
                return;
            }

            Destroy(this.gameObject);
            // TODO: all the different effects
            if (element == Element.Water)
                character.TakeDamage(-1);
            else
                character.TakeDamage(1);
        }
        else if (collision.gameObject.TryGetComponent<Projectile>(out var projectile))
        {
            Vector2 point = collision.contacts[0].point;
            if (IsBig && projectile.IsBig)
            {
                CharacterController.RequestEffect(point, "BOOM", Vector2.zero);

                for (int i = 0; i < 5; i++)
                {
                    Vector2 offset = new Vector2(Random.value * 2f - 1f, Random.value * 2f - 1f);
                    CharacterController.RequestEffect(point + offset, "BOOM", Vector2.zero);
                }
                Destroy(this.gameObject);
                Destroy(projectile.gameObject);
            }
            else if (element == Element.BigFire)
            {
                if (projectile.element == Element.Water)
                {
                    Debug.Log("Fusion!");
                    Fusion(projectile, "fog");
                }
                else if (projectile.element == Element.Air)
                {
                    Fusion(projectile, "BOOM");
                }
                else if (projectile.element == Element.Earth)
                {
                    Fusion(projectile, "lava");
                }
            }
            else if (element == Element.BigWater)
            {
                if (projectile.element == Element.Fire)
                {
                    Destroy(projectile.gameObject);
                }
                else if (projectile.element == Element.Air)
                {
                    speed += 2f;
                    CharacterController.RequestEffect(point, "wave", direction);
                }
                else if (projectile.element == Element.Water)
                {
                    Fusion(projectile, "mud");
                }
            }
            else if (element == Element.BigEarth)
            {
                if (projectile.element == Element.Fire)
                {
                    Fusion(projectile, "barrier");
                }
                else if (projectile.element == Element.Water)
                {
                    Fusion(projectile, "mud");
                }
                else if (projectile.element == Element.Air)
                {
                    Destroy(projectile.gameObject);
                }
            }
            else if (element == Element.BigAir)
            {
                Destroy(this.gameObject);
                Vector2 pos = transform.position;
                if (projectile.element == Element.Fire)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        float dir = Random.value * Mathf.PI * 2f;
                        Vector2 offset = new Vector2(Mathf.Cos(dir), Mathf.Sin(dir));
                        CharacterController.RequestEffect(pos + offset, "firewhip", offset);
                    }
                }
                else if (projectile.element == Element.Water)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        float dir = Random.value * Mathf.PI * 2f;
                        Vector2 offset = new Vector2(Mathf.Cos(dir), Mathf.Sin(dir));
                        CharacterController.RequestEffect(pos + offset, "bubble", offset);
                    }
                }
                else if (projectile.element == Element.Earth)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        float dir = Random.value * Mathf.PI * 2f;
                        Vector2 offset = new Vector2(Mathf.Cos(dir), Mathf.Sin(dir));
                        CharacterController.RequestEffect(pos + offset, "earthball", offset);
                    }
                }
            }
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


    void Fusion(Projectile p2, string next) 
    {
        Destroy(this.gameObject);
        Destroy(p2.gameObject);
        CharacterController.RequestEffect(transform.position, next, Vector2.zero);
    }
}
