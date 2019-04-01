using System.Collections.Generic;
using UnityEngine;

public class SwipeWeapon : MonoBehaviour
{

    public LayerMask layerMask;
    private Vector3 _weaponPosition;
    private Vector3 _weaponScreenPosition;

    public float swipeMaxDuration;
    private bool _isSwipeWeaponActive = false;
    
    private float swipeTimer;

    public GameObject selectionFX;

    public int mobsAmountToTrigger = 4;

    // List of mobs striked during the swipe time window
  
    private Collider2D _myCollider;
    private TrailRenderer _myTrailRenderer;

    private List<Mob> _mobsStriked = new List<Mob>();

    public static SwipeWeapon swipeWeaponInstance;


    void Awake()
    {
        swipeWeaponInstance = this;

        _myCollider = GetComponent<Collider2D>();
        _myTrailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            if(Input.touchCount > 0)
            {
                _weaponScreenPosition = Input.GetTouch(0).position;
                _weaponPosition = Camera.main.ScreenToWorldPoint(_weaponScreenPosition);
            }
            else if(Input.GetMouseButton(0))
            {
                _weaponScreenPosition = Input.mousePosition;
                _weaponPosition = Camera.main.ScreenToWorldPoint(_weaponScreenPosition);
            }

            if (!_isSwipeWeaponActive)
            {
                _isSwipeWeaponActive = true;
            }
        }
        else
        {
            SwipeEffect();

        }    


        // Raycast in order to know if the player is aiming outside of the bottom of the screen (it's the "player zone", where the upgrades and turrets are)
        RaycastHit2D hit = Physics2D.Raycast(_weaponPosition, Vector3.forward, layerMask);
        Debug.DrawRay(transform.position, Vector3.forward * 15f);
        if(hit.collider)
        {
            if (hit.collider.tag != "PlayerZone")
            {
                transform.position = new Vector3(_weaponPosition.x, _weaponPosition.y, 0f);
            }
            else
            {
                SwipeEffect();
            }
        }
        else
        {
            transform.position = new Vector3(_weaponPosition.x, _weaponPosition.y, 0f);
        }

        SwipeWeaponTimer();

    }

   private void SwipeWeaponTimer()
    {
        if (_isSwipeWeaponActive)
        {

            if(!_myCollider.isActiveAndEnabled)
            {
                _myCollider.enabled = true;
                _myTrailRenderer.emitting = true;
                _myTrailRenderer.Clear();
            }


            swipeTimer += Time.deltaTime;

            if (swipeTimer >= swipeMaxDuration)
            {              
                SwipeEffect();
            }
        }
        else
        {
            _mobsStriked = new List<Mob>();
        }
    }

   private void OnTriggerEnter2D(Collider2D other)
    {    
        if(other.tag == "Enemy")
        {
            bool isEnemyValid = true;

            for(int i = 0; i < _mobsStriked.Count; i++)
            {
                // The player can only strike a single enemy once in a swipe
                if (other.transform == _mobsStriked[i].transform)
                {
                    isEnemyValid = false;
                    break;
                }
            
                // All the enemies in the swipe must be of the same type, determined by the first one touched
                if (other.GetComponent<Mob>().MobType != _mobsStriked[0].MobType)
                {
                    isEnemyValid = false;
                    break;
                }
            }

            if(isEnemyValid)
            {
                _mobsStriked.Add(other.GetComponent<Mob>());
                Instantiate(selectionFX, other.transform, false);
            }
                
        }
    }

    private void SwipeEffect()
    {            
     
        if (_isSwipeWeaponActive)
        {
            _isSwipeWeaponActive = false;
            _myCollider.enabled = false;
            _myTrailRenderer.emitting = false;

            swipeTimer = 0;

            if (_mobsStriked.Count >= mobsAmountToTrigger)
            {
                // TOOD : Call turret Special attacks
                OnSpecialAttackTrigger(_mobsStriked.Count);



                _mobsStriked = new List<Mob>();
            }
        }
    }

    public delegate void SpecialAttackEvent(int enemyCount);
    public event SpecialAttackEvent OnSpecialAttackTrigger;

}
