using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    public float _movementSpeed = 1f;

    public int _damage = 1;

    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if(target)
        {
            transform.up = target.position - transform.position;
        }
        
        float mov = Time.deltaTime * _movementSpeed;
        gameObject.transform.position += transform.up * mov;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<Mob>().TakeDamage(_damage);
            Destroy(gameObject);
        }
    }


    // NB : Will need pooling system in order to prevent performance drops
}
