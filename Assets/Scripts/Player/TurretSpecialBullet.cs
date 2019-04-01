using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpecialBullet : MonoBehaviour
{
    public float movementSpeed = 1f;
    public int damage = 1;

    // Update is called once per frame
    void Update()
    {
        Movement();

        CheckIfOutOfBounds();
    }

    private void Movement()
    {
        // Forward movement
        float mov = Time.deltaTime * movementSpeed;
        gameObject.transform.position += transform.up * mov;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Mob mobScript = other.GetComponent<Mob>();
            if (mobScript)
            {
                mobScript.TakeDamage(damage);
            }
        }
    }


    // Deactivate the bullet if it isn't visible by the camera anymore
    private void CheckIfOutOfBounds()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < 0f || viewPos.x > 1f || viewPos.y < 0f || viewPos.y > 1f)
        {
            Destroy(gameObject, 2f);
        }

    }

}
