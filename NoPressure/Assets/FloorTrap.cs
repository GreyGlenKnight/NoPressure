using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTrap : MonoBehaviour
{

    public enum EffectType
    {
        None,
        Damage,
        GivePressure,
        BrokenPressure,
    }

    public Material enabledMateral;
    public Material disabledMateral;
    public LayerMask pCollisionLayer;
    public EffectType pEffectType;
    public int pMagnitude;

    public float TimesinceLastTrigger = 0f;
    public float TriggerDelay = 1f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        TimesinceLastTrigger += Time.deltaTime;
    }

    public void OnTriggerStay(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (TimesinceLastTrigger < TriggerDelay)
            return;

        if (player != null)
        {
            switch (pEffectType)
            {
                case EffectType.Damage:
                    if (player.mHasDisableTrapSkill == false)
                    {
                        player.TakeDamage(pMagnitude);
                        TimesinceLastTrigger = 0f;
                    }
                    else
                    {
                        GetComponent<Renderer>().material = disabledMateral;
                        pEffectType = EffectType.None;
                    }
                    break;

                case EffectType.GivePressure:
                    player.mPressure += pMagnitude;
                    TimesinceLastTrigger = 0f;
                    break;

                case EffectType.BrokenPressure:
                    if(player.mHasFixPressureSkill)
                    {
                        GetComponent<Renderer>().material = enabledMateral;
                        pEffectType = EffectType.GivePressure;
                    }
                    break;
            }
        }
    }
}