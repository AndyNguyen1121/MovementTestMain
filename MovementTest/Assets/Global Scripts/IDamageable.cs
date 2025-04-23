using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IDamageable
{
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public UnityEvent OnHealthChanged { get; set; }

    public void TakeDamage(float value);
    public void IncreaseHealth(float value);
}
