using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Enemy : MonoBehaviour
{
    private float Difficulty = 1f;  //for now there is no other difficulty planned.
    [HideInInspector]
    public float speed;
    [Header("Enemy stats")]
    public float startSpeed = 10f;
    public float baseHealth = 100;
    private float health;
    private float barHealth;
    public int worth = 50;
    public int healthDamage = 1;
    public bool immunity = false;
    public bool regeneration = false;
    public float regen_rate = 0f;
    public float weakenMult = 0.25f;
    public float incWeakenMultSkillBonus = 0.05f;
    public float invincibleTime = 0f;
    bool invincible = false;

    [Header("Debuffs")]
    public float froze = 0;
    //public bool burning = false;
    public float BurningDoT = 0f;
    public bool weaken = false;
    public GameObject burningEffect;
    public GameObject weakenEffect;
    GameObject burningFX;
    GameObject weakenFX;

    [Header("Unity Stuff")]
    public GameObject deathEffect;
    public Image healthBar;
    private bool isDead = false;


    void Start()
    {
        speed = startSpeed;
        health = baseHealth * (float)Math.Pow(WaveSpawner.roundMultiplier, WaveSpawner.waveIndex) * Difficulty; //math.pow returns a double. Need to cast it to float.
        barHealth = health;
        weakenMult += (incWeakenMultSkillBonus * PlayerPrefs.GetInt("Increased Weaken", 0));
        if (regeneration)
            InvokeRepeating("Regen", 0f, 0.5f);
    }

    public void TakeDamage(float amount)    //void -> we dont want it to return anything
    {
        if (!invincible)
        {
            //Debug.Log("ToInt32(weaken): " + Convert.ToInt32(weaken));
            health -= (amount + (Convert.ToInt32(weaken) * amount * weakenMult));
            //Debug.Log("health: " + health);
            healthBar.fillAmount = health / barHealth;
        }

        if (health < barHealth / 2)         //hard coded so that invincibility starts at half health.
        {
            if (invincibleTime > 0f)
                StartCoroutine(invincibleON());
        }

        if (health < 1 && !isDead)
        {
            Die();
        }
    }

    public void Slow(float pct)
    {
        if (!immunity)
            speed = startSpeed * (1f - pct);
    }

    void Die()
    {
        isDead = true;
        PlayerStats.Money += (worth + (int)(worth * 0.05f * PlayerPrefs.GetInt("Extra Bounty Bonus", 0)));

        GameObject effect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5f);

        WaveSpawner.EnemiesAlive--;

        Destroy(gameObject);

    }

    public void frozen(float seconds)
    {
        if (!immunity && seconds > 0f)  //make sure that hitting them wont remove the frozen status. However can be overwritten with shorter slow.
            froze = seconds;
    }

    void Regen()
    {
        if (health < (barHealth - (barHealth * regen_rate * 0.5f)))  //make sure it doesnt overheal. The missing health must be greater than the healing tick.
        {
            health += (barHealth * regen_rate * 0.5f - (Convert.ToInt32(weaken) * barHealth * regen_rate * 0.5f * weakenMult));    //since this function is being called 2 per second.
            healthBar.fillAmount = health / barHealth;
        }
        else if (health < barHealth)
        {
            health = barHealth;
            healthBar.fillAmount = 1;
        }
    }

    IEnumerator invincibleON()
    {
        this.transform.Find("Halo").GetComponent<Renderer>().material.EnableKeyword("_EMISSION");   //turns on emission, so we know that enemy is invincible
        invincible = true;
        yield return new WaitForSeconds(invincibleTime);
        invincible = false;


        invincibleTime = 0f;        //this makes sure they can only be invincible once. 
        this.transform.Find("Halo").GetComponent<MeshRenderer>().enabled = false;
        //this.transform.Find("Halo").GetComponent<Renderer>().material.DisableKeyword("_EMISSION");    //this disables the emission when invincibility period is gone
    }

    public void CallingBurnFromEnemy()  //Ineumerator stops when the gameobject that calls it is destroyed. So as the bullets call the function, as soon as the bullets are destroyed, the coroutine also stops
    {
        if (!immunity)
        {
            StopCoroutine("burn");          //somehow the name of the coroutine must be a string for this "refresh" to work. Otherwise StopCoroutine only "pauses" it, instead of stopping it.
            Destroy(burningFX, 0.5f);
            StartCoroutine("burn");
        }
    }

    public IEnumerator burn()
    {
        //Debug.Log("Burning: " + BurningDoT);
        burningFX = (GameObject)Instantiate(burningEffect, transform.position, Quaternion.identity);
        burningFX.transform.SetParent(transform);
        for (int i = 0; i < 6; i++)
        {
            TakeDamage(BurningDoT * 0.5f);              //half the burning DoT every half second
            //Debug.Log("int i = 0; i < 6; i++" + i);
            yield return new WaitForSeconds(0.5f);
            //Debug.Log("burning. health: " + health);
        }
        BurningDoT = 0f;
        Destroy(burningFX, 0.5f);
    }

    public void CallingWeakFromEnemy()  //Ineumerator stops when the gameobject that calls it is destroyed. So as the bullets call the function, as soon as the bullets are destroyed, the coroutine also stops
    {
        //if (!immunity)                //turn off immunity to weaken. Otherwise immune_regen enemies are too difficult.
        //{
        StopCoroutine("weak");          //somehow the name of the coroutine must be a string for this "refresh" to work. Otherwise StopCoroutine only "pauses" it, instead of stopping it.
        Destroy(weakenFX, 0.5f);
        StartCoroutine("weak");
        //}
    }
    public IEnumerator weak()
    {
        //Debug.Log("weaken");
        weaken = true;
        weakenFX = (GameObject)Instantiate(weakenEffect, transform.position, Quaternion.identity);
        weakenFX.transform.SetParent(transform);
        yield return new WaitForSeconds(2f);
        weaken = false;
        Destroy(weakenFX, 0.5f);
    }
}
