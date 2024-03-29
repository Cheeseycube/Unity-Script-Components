using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_Projectile : MonoBehaviour
{
    [Header("Customizable Stats")]
    [SerializeField] int damagePerHit = 25;
    [SerializeField] float lifeTime = 3f;
    [SerializeField] float moveSpeed = 0.4f;
    [SerializeField] bool destroyOnHit = true;
    
    private void Start() {
        StartCoroutine(lifetimer());
        //transform.Rotate(new Vector3(0f, 0f, RotationNum));  Useful sometimes
    }

    private void FixedUpdate()
    {
        move();
    }
    
    private void move()
    {
        transform.Translate(Vector2.right * moveSpeed);
    }
    
    IEnumerator lifetimer()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if touching something damageable other than the player, deal damage
        if ((collision.gameObject.TryGetComponent<DamageableComponent>(out DamageableComponent target)) && (!collision.CompareTag("Player") && (!collision.CompareTag("Machine"))) )
        {
            target.TakeDamage(damagePerHit);
        }
        
        // if touching anything other than the player
        if (!collision.CompareTag("Player") && destroyOnHit)
        {
            Destroy(this.gameObject);
        }

    }
    
   
}
