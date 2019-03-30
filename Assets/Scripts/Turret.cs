using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

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
    
    public float attackUpgradeMultiplier = 1.1f;
    public float bulletSpeedUpgradeMultiplier = 1.01f;
    public float fireSpeedUpgradeDivider = 1.1f;

    private List<GameObject> _bulletsPool = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
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
            bulletScript._damage = damage;
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
       if((Mathf.Round((float)damage * attackUpgradeMultiplier) == damage))
        {
            damage++;
        }
       else
        {
            damage = (int) Mathf.Round((float)damage * attackUpgradeMultiplier);
        }
    }

    public void UpgradeSpeed()
    {
        bulletSpeed *= bulletSpeedUpgradeMultiplier;
        reloadTime /= fireSpeedUpgradeDivider;
    }

    private void SpecialAttack()
    {

    }




}
