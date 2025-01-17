using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatEnemy : CombatEntity
{
    public Button button;

    [SerializeField]
    private SpriteRenderer render;
    private Color originalColor;

    //animation stuff
    [SerializeField]
    protected Animator anim;
    private bool inAnim;

    public new string name;

    [SerializeField]
    private Sprite mark;
    [SerializeField]
    private SpriteRenderer markRender;
    public Dictionary<string, int> statuses; // keeps track of names and duration of marks, buffs, debuffs

    public float damage;

    public bool turnTaken;

    public bool isDead;

    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
        originalColor = render.color;

        statuses = new Dictionary<string, int>();
        turnTaken = false;

        isDead = false;
    }

    public virtual void StartTurn()
    {
        if (isDead) return;
        Attack();
    }

    protected void Attack() //triggers attack animation
    {
        string output = name + " is attacking for " + damage + " damage!";
        CombatUIManager.instance.PlayDialogue(output);
        anim.SetTrigger("Attack");
    }

    protected void DoDamage() //called in animation
    {
        CombatSystem.instance.playerCombat.TakeDamage(damage);
    }

    private void EndTurn()
    {
        anim.SetTrigger("Idle");
        turnTaken = true;
        foreach (KeyValuePair<string, int> status in statuses) //decrement status effects
        {
            statuses[status.Key]--;
            if (statuses[status.Key] == 0)
            {
                statuses.Remove(status.Key);
            }
        }
    }

    public void AddEffect(string effectName, int duration)
    {
        statuses.Add(effectName, duration);
        markRender.sprite = mark;
        Debug.Log("mark added");
    }

    public bool CheckEffect(string effectName)
    {
        return statuses.ContainsKey(effectName);
    }

    public void RemoveEffect(string effectName)
    {
        CombatSystem.instance.selectedEnemy.statuses.Remove(effectName);
        markRender.sprite = null;
        Debug.Log("mark removed");
    }

    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth/maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        //Debug.Log("enemy died");
        anim.SetBool("isDead", true);
        isDead = true;
    }

    #region Selecting

    public void Select()
    {
        if (!CombatSystem.instance.inSelect)
        {
            Debug.Log("not in select mode!");
            return;
        }
        if (CombatSystem.instance.selectedEnemy == gameObject) //if this enemy is already selected, then user wants to deselect
        {
            Deselect();
            return;
        }

        if (CombatSystem.instance.selectedEnemy != null) //if there is another enemy already selected, then deselect that first
        {
            CombatSystem.instance.selectedEnemy.Deselect();
        }

        render.color = Color.red;
        Debug.Log("selected");
        CombatSystem.instance.setEnemy(this);
    }

    public void Deselect()
    {
        //Debug.Log("unselected");
        render.color = originalColor;
        CombatSystem.instance.unsetEnemy();
    }

    #endregion

}
