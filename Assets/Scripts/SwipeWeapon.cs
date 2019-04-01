using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeWeapon : MonoBehaviour
{

    private Vector3 _weaponPosition;
    private Vector3 _weaponScreenPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            _weaponScreenPosition = Input.GetTouch(0).position;
            _weaponPosition = Camera.main.ScreenToWorldPoint(_weaponScreenPosition);
            
        }

#endif


#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            _weaponScreenPosition = Input.mousePosition;
            _weaponPosition = Camera.main.ScreenToWorldPoint(_weaponScreenPosition);            
        }
#endif
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.forward);
        Debug.DrawRay(transform.position, Vector3.forward * 15f);

        if (hit.collider.tag != "PlayerZone")
        {
         //   transform.position = new Vector3(_weaponPosition.x, _weaponPosition.y, 0f);
           
        }
        Debug.Log(hit.collider.name);
    }
            
}
