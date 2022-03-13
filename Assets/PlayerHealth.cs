using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar(hook = "OnChangeHealth")]
    [SerializeField] int currentHealth = 100;

    [SerializeField] bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnChangeHealth(int oldHealth, int newHealth)
    {
        currentHealth = newHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int amount)
    {
        //if (!isServer) return;

        currentHealth -= amount;
    }

    void Die()
    {
        isDead = true;

        //Disable player
        gameObject.SetActive(false);
    }


}
