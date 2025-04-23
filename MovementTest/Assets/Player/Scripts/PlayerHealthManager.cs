using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthManager : MonoBehaviour, IDamageable
{
    [field: SerializeField]
    public float Health { get; set; }

    [field: SerializeField]
    public float MaxHealth { get; set; }
    public UnityEvent OnHealthChanged { get; set; }

    private void Awake()
    {
        Health = MaxHealth;
    }

    public void TakeDamage(float value)
    {
        value = -value;
    }

    public void IncreaseHealth(float value)
    {

    }

}
