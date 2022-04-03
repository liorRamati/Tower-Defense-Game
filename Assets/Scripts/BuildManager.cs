using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    // blueprints for the different turrets
    public BlueprintItem StandartTurretBlueprint;
    public BlueprintItem MissileLauncherBlueprint;
    public BlueprintItem LaserBeamerBlueprint;

    // the ui pnael for the upgrade and sell buttons
    public SelectUI selectUI;

    [Range(0, 100)]
    [Tooltip("The percent of the turret cost returned when selling")]
    public int SellReturnPercent = 50;

    [HideInInspector]
    public bool BuildingToBuildSelected = false;

    [HideInInspector]
    public Turret BuildingToUpgrade;

    /// <summary>
    /// create standart turret blueprint
    /// </summary>
    public void SpawnStandartTurret()
    {
        SpawnTurretBlueprint(StandartTurretBlueprint);
    }

    /// <summary>
    /// create missile launcher blueprint
    /// </summary>
    public void SpawnMissileLauncher()
    {
        SpawnTurretBlueprint(MissileLauncherBlueprint);
    }

    /// <summary>
    /// create laser beamer blueprint
    /// </summary>
    public void SpawnLaserBeamer()
    {
        SpawnTurretBlueprint(LaserBeamerBlueprint);
    }

    /// <summary>
    /// create a turret blueprint from the given prefab
    /// </summary>
    /// <param name="turretBlueprint">the prefab</param>
    public void SpawnTurretBlueprint(BlueprintItem turretBlueprint)
    {
        // if there is a blueprint in the scene do nothing
        if (!BuildingToBuildSelected)
        {
            Instantiate(turretBlueprint.gameObject);
            BuildingToBuildSelected = true;
            // if upgrade panel is visible hide it
            DeselectUpgrade();
        }
    }

    /// <summary>
    /// Build the given turret in the location given
    /// </summary>
    /// <param name="turretPreFab">a prefab for the turret to build</param>
    /// <param name="loc">location to build</param>
    /// <param name="rot">rotation of the turret</param>
    /// <param name="checkLoc">if true check the location is valid for positioninga turret</param>
    /// <returns>0 if the turret was built, 1 if the location given is illegal and 2 if the location is legal but there a turret nearby, 
    /// and 3 if the location is legal but there is no gold to build the turret</returns>
    public int BuildTurrent(Turret turretPreFab, Vector3 loc, Quaternion rot, bool checkLoc)
    {
        // if needed check turret is on a flat area and does not collide with another turret
        if (checkLoc)
        {
            if (GameManager.gameManager.Utils.IsInFlatCircle(loc, turretPreFab.baseRadius))
            {
                float radius = turretPreFab.GetComponent<SphereCollider>().radius;
                foreach (Collider c in Physics.OverlapSphere(loc, radius))
                {
                    if (c.gameObject.tag == "placement")
                    {
                        return 2;
                    }
                }
            }
            else
            {
                return 1;
            }
        }

        // check if there is enough money
        if (GameManager.gameManager.playerStats.currentMoney < turretPreFab.cost)
        {
            //print("no enough money");
            return 3;
        }
        
        // create turret
        Turret temp = Instantiate(turretPreFab.gameObject, loc, rot).GetComponent<Turret>();
        // reduce cost from current money
        GameManager.gameManager.playerStats.ReduceMoney(turretPreFab.cost);
        // add turret to list of turrets
        GameManager.gameManager.Turrets.Add(temp);
        return 0;
    }

    /// <summary>
    /// upgrade the given turret if possible
    /// </summary>
    /// <returns>0 if upgraded successfully, 2 if there is no upgrade for the turret or 3 if there is not enough money for upgrade</returns>
    public int UpgradeTurret(Turret turret)
    {
        // check if there is mpney for upgrade
        if (GameManager.gameManager.playerStats.currentMoney < turret.upgradeCost)
        {
            //print("no enough money");
            return 3;
        }

        // check if turret has upgrade
        if (turret.turretUpgrade == null)
        {
            print("no upgrade for the turret");
            return 2;
        }

        // switch turret with upgraded turret
        turret.gameObject.SetActive(false);
        Instantiate(turret.turretUpgrade, turret.transform.position, turret.transform.rotation);
        Destroy(turret.gameObject);

        // remove upgrade cost from current gold
        GameManager.gameManager.playerStats.ReduceMoney(turret.upgradeCost);
        // close upgrade ui panel
        DeselectUpgrade();
        return 0;
    }

    /// <summary>
    /// sell the given turret
    /// </summary>
    /// <param name="turret"></param>
    public void SellTurret(Turret turret)
    {
        // add sell price to current gold
        GameManager.gameManager.playerStats.IncreaseMoney(SellCost(turret));

        // remove turret
        Destroy(turret.gameObject);

        // close upgrade ui panel
        DeselectUpgrade();
    }

    /// <summary>
    /// calculate sell price of the given turret
    /// </summary>
    public int SellCost(Turret turret)
    {
        return (int)Mathf.Round(turret.cost * SellReturnPercent / 100);
    }

    /// <summary>
    /// open or closes upgrade ui panel, depending or the turret selected. the panel will appear above the given turret
    /// </summary>
    public void ToggleSelectUI(Turret target)
    {
        // if no turret is select do nothing
        if (target == null)
        {
            return;
        }

        // dont open the ui if there is a button above the turret
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // dont open panel if there is a blueprint in the scene
        if (!BuildingToBuildSelected)
        {
            // if panel is closed or opened over another turret open it above the given turret
            if (!BuildingToUpgrade || BuildingToUpgrade != target)
            {
                SelectUpgrade(target);
            }
            // if panel open above the given turret close it
            else
            {
                DeselectUpgrade();
            }
        }
    }

    /// <summary>
    /// set the turret to (potentialy) upgrade
    /// </summary>
    public void SelectUpgrade(Turret turret)
    {
        BuildingToUpgrade = turret;
        selectUI.SetTarget(turret);
    }

    /// <summary>
    /// unset the turret to upgrade
    /// </summary>
    public void DeselectUpgrade()
    {
        BuildingToUpgrade = null;
        selectUI.Hide();
    }
}
