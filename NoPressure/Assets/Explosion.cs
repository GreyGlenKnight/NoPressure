using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    float mDamage;

    public void SetSize(float newSize)
    {
        transform.localScale = new Vector3(newSize,newSize,newSize);
    }

    public void SetDamage(float Damage)
    {
        mDamage = Damage;
    }

    private void OnTriggerEnter(Collider other)
    {

        IDamageable entity = other.GetComponent<IDamageable>();
        if(entity!= null)
        {
            entity.TakeDamage(mDamage);
        }
    }


}
