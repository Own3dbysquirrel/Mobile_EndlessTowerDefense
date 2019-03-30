using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Mob : MonoBehaviour
{
    [SerializeField]
    private int _life = 1;
    public float MovementSpeed;

    private bool _isAlive = true;

    [Tooltip("This value is used in order to determine the composition of the waves. If a wave as a difficulty of 15, it can spawn up to 5 ennemies with a difficulty score of 3")]
    public float difficultyScore = 1f;

    [Tooltip("The amount of gold the enemy will drop on his death. This value is multiplied by the scaling ratio.")]
    public RangeInt goldDropRange;
    private int _goldDrop;

    [Tooltip("The life and goldDrop of the enemy will be multiplied by this value, which is determined by the difficulty of the wave")]
    public float scalingRatio = 1;
  
    public Text lifeDisplay;
    // Start is called before the first frame update
    void Start()
    {
        _life = (int) Mathf.Round(_life * scalingRatio);

        
        _goldDrop = (int) Mathf.Round(Random.Range(goldDropRange.start, goldDropRange.end + 1) * scalingRatio);
                
        lifeDisplay.text = _life.ToString();

      
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

        // if the mob is near the border of the screen, move toward center
        if(Camera.main.WorldToViewportPoint(transform.position).x < 0.1f)
        {
            gameObject.transform.position += Vector3.right * mov;
        }

        if (Camera.main.WorldToViewportPoint(transform.position).x > 0.9f)
        {
            gameObject.transform.position += Vector3.left * mov;
        }    
    }

    public void TakeDamage(int dmg)
    {
        _life -= dmg;
        lifeDisplay.text = _life.ToString();
        if (_life <= 0 && _isAlive)
        {
            _isAlive = false;
            Death();
        }
    }

    private void Death()
    {  
        if(OnTargetDeath != null)
             OnTargetDeath(this);
              
        //  Destroy(gameObject); 
        gameObject.SetActive(false);
    }

    public delegate void DeathEvent(Mob target);
    public event DeathEvent OnTargetDeath;


}
