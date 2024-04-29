using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUIManager : MonoBehaviour //provides functions for all button presses in combat scene
{
    public static CombatUIManager instance;
    private CombatSystem combatSystem;

    [SerializeField] private HealthBar playerHealth;

    [SerializeField] private GameObject defaultPanel;
    [SerializeField] private GameObject selectWeaponPanel;
    [SerializeField] private GameObject attackPanel;

    [SerializeField] private AbilityButton abilityButton1;
    [SerializeField] private AbilityButton abilityButton2;

    public int numOfPanels;
    private GameObject[] panels;
    private int currentPanelIndex;

    private void Awake()
    {
        if (instance != null && instance != this) //singleton
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        panels = new GameObject[numOfPanels];
        panels[0] = defaultPanel;
        panels[1] = selectWeaponPanel;
        panels[2] = attackPanel;
        currentPanelIndex = 0;
    }

    private void Start()
    {
        combatSystem = CombatSystem.instance;
        ShowOnly(defaultPanel);
    }

    public void ShowOnly(GameObject panel)
    {
        for (int i = 0; i < numOfPanels; i++)
        {
            if (panels[i] == panel)
            {
                panels[i].SetActive(true);
                continue;
            }
            panels[i].SetActive(false);
        }
    }

    public void GoBack() 
    {
        if (currentPanelIndex == 0) return;
        ShowOnly(panels[currentPanelIndex - 1]);
        currentPanelIndex--;
    }

    public void ShowSelectWeaponPanel()
    {
        ShowOnly(selectWeaponPanel);
        currentPanelIndex = 1;
    }

    public void SelectLeftWeapon()
    {
        PlayerWeaponManager.instance.RotateLeft();
        combatSystem.SwapWeapon(0); //performs the actual game system rotation
    }

    public void SelectRightWeapon()
    {
        PlayerWeaponManager.instance.RotateRight();
        combatSystem.SwapWeapon(1); //performs the actual game system rotation
    }

    public void SelectWeapon() //called when player chooses a weapon
    {
        combatSystem.EnterSelectEnemy(); //allow player to select enemies; turns on enemyselect buttons
        abilityButton1.setAbility(combatSystem.currentWeapon.GetComponent<WeaponObject>().weapon.abilityList[0]); //initialize ability buttons
        abilityButton2.setAbility(combatSystem.currentWeapon.GetComponent<WeaponObject>().weapon.abilityList[1]);
        currentPanelIndex = 2; //allow player to choose ability to use
        ShowOnly(attackPanel); 
    }

    public void Attack(int id) //called when player confirms attack to use via abilityButton press
    {
        //id is 0 or 1; which attack to use
        if (combatSystem.selectedEnemy == null) return;
        if (!combatSystem.Attack(id)) return; //attempt to perform the attack

        combatSystem.EndSelectEnemy(); //unallow player to select enemies; turns off enemyselect buttons

        abilityButton1.unsetAbility(); //reset ability buttons
        abilityButton2.unsetAbility();

        currentPanelIndex = 0; //return to default panel
        ShowOnly(defaultPanel);
    }

    public void ShowBag()
    {
        Debug.Log("pressed the bag button");
    }


    public void Flee()
    {
        Debug.Log("pressed the flee button");
    }

    public HealthBar GetPlayerHealthbar()
    {
        return this.playerHealth;
    }

}
