using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    [HideInInspector]
    public float _movementSpeed = 1f;

    [HideInInspector]
    public int _damage = 1;

    public Transform target;


    // Update is called once per frame
    void Update()
    {
        Movement();

        CheckIfOutOfBounds();
    }

    private void Movement()
    {

        // Homing at target
        if(target != null && target.gameObject.activeSelf)
        {
            transform.up = target.position - transform.position;
        }
        else
        {
            target = null;
        }
        
        // Forward movement
        float mov = Time.deltaTime * _movementSpeed;
        gameObject.transform.position += transform.up * mov;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Mob mobScript = other.GetComponent<Mob>();
            if (mobScript)
            {               
                mobScript.TakeDamage(_damage);
            }
            target = null;
            gameObject.SetActive(false);
        }
    }


    // Deactivate the bullet if it isn't visible by the camera anymore
    private void CheckIfOutOfBounds()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < 0f || viewPos.x > 1f || viewPos.y < 0f || viewPos.y > 1f)
        {      
            target = null;
            gameObject.SetActive(false);
        }

    }
}
