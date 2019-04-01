using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class Turret : MonoBehaviour
  
{
    public GameObject bulletPrefab;
   
    private List<Mob> _mobsInRange = new List<Mob>();

    private Transform _target;
    private float _targetDistance;

    
    private float _reloadTimer = 0f;

    public int damage = 1;
    public float reloadTime = 1f;
    public float bulletSpeed = 3f;

    [Tooltip("The attack value of the turret is multiplied by this variable each time the player buys an Attack upgrade")]
    public float attackUpgradeMultiplier = 1.1f;


    [Tooltip("The bullet's movement speed is multiplied by this variable each time the player buys a Speed upgrade")]
    public float bulletSpeedUpgradeMultiplier = 1.01f;



    [Tooltip("The rate of fire of the turret is divided by this variable each time the player buys a Speed upgrade")]
    public float fireSpeedUpgradeDivider = 1.1f;

    [Tooltip("The cost of the next upgrade in relation to the last one")]
    public float costUpgradeMultiplier = 1.1f;

    
    public int atkUpgradeCost = 10;
    public int speedUpgradeCost = 10;

    public Button atkUpgradebutton;
    public TextMeshProUGUI atkUpgradeCostText;

    public Button speedUpgradebutton;
    public TextMeshProUGUI speedUpgradeCostText;

    private string _goldStringDisplay;


    private List<GameObject> _bulletsPool = new List<GameObject>();

    private CurrencyManager _currencyManager;

    public GameObject specialBulletPrefab;

    private void Start()
    {
        _currencyManager = CurrencyManager.currencyManagerInstance;

        _currencyManager.OnGold += LockUpgrade;

        atkUpgradeCostText.text = atkUpgradeCost.ToString();
        speedUpgradeCostText.text = speedUpgradeCost.ToString();


        //subscribe to the Special Bullet event
        SwipeWeapon.swipeWeaponInstance.OnSpecialAttackTrigger += SpecialAttack;

    }

    // Update is called once per frame
    void Update()
    {
        if(_mobsInRange.Count > 0)
        {
            TargetNearestEnemy();
            RotateTowardTarget();
            AutoAttack();
        } 
        else
        {
            ResetTurretRotation();
        }

    }

    // Update the list of ennemies each time another comes in range
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Mob mob = other.gameObject.GetComponent<Mob>();

            _mobsInRange.Add(mob);

            mob.OnTargetDeath += RemoveTargetFromList;

        }
    }

  
    private void TargetNearestEnemy()
    {
        _target = _mobsInRange[0].transform;
        _targetDistance = Vector2.Distance(transform.position, _target.transform.position);
       
        // Select the nearest enemy of the list as the target of the turret
        for (int i = 0; i < _mobsInRange.Count; i++)
        {
            if (Vector2.Distance(transform.position, _mobsInRange[i].transform.position) < _targetDistance)
            {
                _target = _mobsInRange[i].transform;
                _targetDistance = Vector2.Distance(transform.position, _mobsInRange[i].transform.position);
            }
        }         
    }

  

    void RemoveTargetFromList(Mob target)
    {
        _mobsInRange.Remove(target);
        target.OnTargetDeath -= RemoveTargetFromList;
    }


    private void RotateTowardTarget()
    {
        // Rotate toward the enemy target
        if (_target != null)
            transform.up = _target.position - transform.position;
        else
            transform.up =  Vector3.up - transform.position;


    }


    private void AutoAttack()
    {
        if(_reloadTimer >= reloadTime)
        {

            // Spawn a new bullet
            GameObject newBullet = GetPooledBullet();

            newBullet.transform.position = transform.position;
            newBullet.transform.rotation = transform.rotation;

            TurretBullet bulletScript = newBullet.GetComponent<TurretBullet>();

            bulletScript.target = _target;
            bulletScript.damage = damage;
            bulletScript._movementSpeed = bulletSpeed;

            // Reset the reloadTimer after the turret fired;
            _reloadTimer = 0;
        }
        else
        {
            // Increment reload timer between shots
            _reloadTimer += Time.deltaTime;
        }

    }

   
    private GameObject GetPooledBullet()
    {
        for (int i = 0; i < _bulletsPool.Count; i++)
        {
            // Look for an available bullet in the pool
            if (!_bulletsPool[i].activeInHierarchy)
            {
                _bulletsPool[i].SetActive(true);
                return _bulletsPool[i];
            }
        }

        // Otherwise Instantiate a new one
        GameObject newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation, null);

        _bulletsPool.Add(newBullet);

        return newBullet;
    }

  
    private void ResetTurretRotation()
    {
        if (_target != null)
            _target = null;

        if (gameObject.transform.rotation != Quaternion.identity)
            gameObject.transform.rotation = Quaternion.identity;
    }

    public void UpgradeAttack()
    {

        _currencyManager.AddGold(-atkUpgradeCost);

       if((Mathf.Round((float)damage * attackUpgradeMultiplier) == damage))
        {
            damage++;
        }
       else
        {
            damage = (int) Mathf.Round((float)damage * attackUpgradeMultiplier);
        }

      atkUpgradeCost = (int)Mathf.Round(atkUpgradeCost * costUpgradeMultiplier);
      atkUpgradeCostText.text = UpgradeCostFormat(atkUpgradeCost);


    }

    public void UpgradeSpeed()
    {
        _currencyManager.AddGold(-speedUpgradeCost);

        bulletSpeed *= bulletSpeedUpgradeMultiplier;
        reloadTime /= fireSpeedUpgradeDivider;

        speedUpgradeCost = (int)Mathf.Round(speedUpgradeCost * costUpgradeMultiplier);
        speedUpgradeCostText.text = UpgradeCostFormat(speedUpgradeCost);
    }

    private string UpgradeCostFormat(int amount)
    {
        _goldStringDisplay = amount.ToString();
        // If the gold amount is too big, display it with K, M ,B symbols (thousand, million, billion)
        if (amount >= 1000)
        {
            _goldStringDisplay = ((float)amount / 1000f).ToString("F1") + " K";
        }
        if (amount >= 1000000)
        {
            _goldStringDisplay = ((float)amount / 1000000f).ToString("F1") + " M";
        }
        if (amount >= 1000000000)
        {
            _goldStringDisplay = ((float)amount / 1000000000f).ToString("F1") + " B";
        }

        return _goldStringDisplay;
    }

    public void SpecialAttack(int enemyCount)
    {
        GameObject specialBullet = Instantiate(specialBulletPrefab, null);

        specialBullet.transform.position = transform.position;
        specialBullet.transform.rotation = transform.rotation;

        TurretSpecialBullet specialScript = specialBullet.GetComponent<TurretSpecialBullet>();
        specialScript.damage =  damage * 5;
        specialScript.movementSpeed = bulletSpeed * 5;


    }


    // If one of the upgrade costs more than the player gold, disable the button
    public void LockUpgrade()
    {
        atkUpgradebutton.interactable = atkUpgradeCost <= _currencyManager.gold;
        speedUpgradebutton.interactable = speedUpgradeCost <= _currencyManager.gold;
    }
}
