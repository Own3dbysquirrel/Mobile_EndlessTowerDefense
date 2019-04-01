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

    [Tooltip("This value is used in order to determine the composition of the waves. If a wave as a difficulty of 15, it can spawn up to 5 ennemies with a difficulty score of 3")]
    public float difficultyScore = 1f;

    [Tooltip("The amount of gold the enemy will drop on his death. This value is multiplied by the scaling ratio.")]
    public int goldDrop = 1;

    [Tooltip("The life and goldDrop of the enemy will be multiplied by this value, which is determined by the difficulty of the wave")]
    public float scalingRatio = 1;
  
    public Text lifeDisplay;

    private int _baseLife;
    private int _baseGoldDrop;

    public int MobType = 0;

    private Animator _myAnimator;
  
    void Awake()
    {
        _baseLife = _life;
        _baseGoldDrop = goldDrop;

        _myAnimator = GetComponent<Animator>();
       
    }

    // Start is called before the first frame update
    void Start()
    {
        _life = (int) Mathf.Round(_baseLife * scalingRatio);
        goldDrop = (int) Mathf.Round(_baseGoldDrop * scalingRatio);              
        lifeDisplay.text = _life.ToString();      
    }

    void OnEnable()
    {
        _life = (int) Mathf.Round(_baseLife * scalingRatio);
        goldDrop = (int)Mathf.Round(_baseGoldDrop * scalingRatio);
        lifeDisplay.text = _life.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    public virtual void Movement()
    {
        float mov = Time.deltaTime * MovementSpeed;
        gameObject.transform.position -= Vector3.up * mov;
        gameObject.transform.rotation = Quaternion.identity;


        // if the mob is near the border of the screen, move toward center
        if (Camera.main.WorldToViewportPoint(transform.position).x < 0.1f)
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

        _myAnimator.SetTrigger("TakeDamage");
        if (_life <= 0)
        {
            Death(true);
        }
    }

    public virtual void Death(bool loot)
    {
        // This events informs the Player Turrets that the enemy died
        OnTargetDeath?.Invoke(this);
        gameObject.SetActive(false);


        // The enemy may not drop gold on death (ex : killed by the player zone)
        if(loot)
        {
            CurrencyManager.currencyManagerInstance.AddGold(goldDrop);
        }
    }


    public delegate void DeathEvent(Mob target);
    public event DeathEvent OnTargetDeath;


}
