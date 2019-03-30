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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        CheckIfOutOfBounds();
    }

    private void Movement()
    {
        if(target != null && target.gameObject.activeSelf)
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
            if(other.GetComponent<Mob>())
                other.GetComponent<Mob>().TakeDamage(_damage);

            gameObject.SetActive(false);
        }
    }


    // Deactivate the bullet if it isn't visible by the camera anymore
    private void CheckIfOutOfBounds()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < 0f || viewPos.x > 1f || viewPos.y < 0f || viewPos.y > 1f)
        {
            gameObject.SetActive(false);
        }

    }
}
