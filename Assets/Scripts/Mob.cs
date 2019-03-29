using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Mob : MonoBehaviour
{
    public int life;
    public float MovementSpeed;

  
    public Text lifeDisplay;
    // Start is called before the first frame update
    void Start()
    {
        lifeDisplay.text = life.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        float mov = Time.deltaTime * MovementSpeed;
        gameObject.transform.position -= Vector3.up * mov;
    }

    public void TakeDamage(int dmg)
    {
        life -= dmg;
        lifeDisplay.text = life.ToString();
        if (life <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        OnTargetDeath(this);
        Destroy(gameObject);   
    }

    public delegate void DeathEvent(Mob target);
    public event DeathEvent OnTargetDeath;


}
