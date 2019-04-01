using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob_Cluster : Mob
{

    public GameObject clusterMinions;
    public int minionsCount;
    public float ejectionForce = 1f;

    private void Start()
    {
      
    }

    public override void Death(bool loot)
    {

        for(int i = 0; i < minionsCount; i++)
        {
            GameObject newMinion = Instantiate(clusterMinions, transform.position, transform.rotation, null);
            newMinion.GetComponent<Mob>().scalingRatio = scalingRatio;
            newMinion.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle.normalized * ejectionForce);
        }

        base.Death(loot);
    }
}
