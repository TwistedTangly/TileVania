using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D myRigidbody2D;
    PlayerMovement player;
    float xSpeed;

    [SerializeField] float projectileSpeed = 10f;

    void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>();
        xSpeed = player.transform.localScale.x * projectileSpeed;
    }

    void Update()
    {
        myRigidbody2D.velocity = new Vector2(xSpeed, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Enemy")
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        StartCoroutine(DestroyProjectile());

    }

    IEnumerator DestroyProjectile()
    {
        myRigidbody2D.velocity = new Vector2(0f, 0f);
        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }
}
