using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SelectUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _upgradeCostText;
    [SerializeField]
    private Button _upgradeButton;

    [SerializeField]
    private TMP_Text _sellCostText;

    // the ui object in this panel
    private GameObject _ui;

    private Turret _target;

    private float _originalTextSize;
    private Color _originalButtonColor;
    private bool _buttonColorChanged = false;

    private static Color orange = new Color(1.0f, 0.64f, 0.0f);

    /// <summary>
    /// called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        // initialize variables
        _ui = transform.GetChild(0).gameObject;
        _ui.SetActive(false);
        _upgradeCostText = _upgradeCostText ?? transform.FindDeepChild("Upgrade")?.Find("Cost Panel")?.Find("Text").GetComponent<TMP_Text>();
        _upgradeButton = _upgradeButton ?? transform.FindDeepChild("Upgrade").GetComponent<Button>();
        _sellCostText = _sellCostText ?? transform.FindDeepChild("Sell")?.Find("Cost Panel")?.Find("Text").GetComponent<TMP_Text>();
        _originalTextSize = _upgradeCostText.fontSize;
        _originalButtonColor = _upgradeButton.colors.normalColor;
    }

    /// <summary>
    /// Update phase in the native player loop
    /// </summary>
    private void Update()
    {
        // do nothing before panel is open
        if (_target == null)
        {
            return;
        }

        // if the target turret has an upgrade check if there is gold for the upgrade
        if (_target.turretUpgrade != null)
        {
            // change color to orange if no gold to pay for upgrade
            if (!_buttonColorChanged && _target.upgradeCost > GameManager.gameManager.playerStats.currentMoney)
            {
                // TO DO: give option to save color every time it changes so changing of color during game is possible
                ColorBlock cb = _upgradeButton.colors;
                cb.normalColor = orange;
                cb.highlightedColor = orange;
                _upgradeButton.colors = cb;
                _buttonColorChanged = true;
            }
            // change color back if gold becomes available
            else if (_buttonColorChanged && _target.upgradeCost <= GameManager.gameManager.playerStats.currentMoney)
            {
                ColorBlock cb = _upgradeButton.colors;
                cb.normalColor = _originalButtonColor;
                _upgradeButton.colors = cb;
                _buttonColorChanged = false;
            }
        }
    }

    /// <summary>
    /// set the target turret to display the panel above
    /// </summary>
    public void SetTarget(Turret t)
    {
        this._target = t;
        
        // move panel to target's position
        transform.position = _target.transform.position;
        // change upgrade and sell prices according to the target's info
        _sellCostText.text = GameManager.gameManager.buildManager.SellCost(_target).ToString();

        // if no upgrade available, set text of upgrade to 'done'
        if (_target.turretUpgrade == null)
        {
            _originalTextSize = _upgradeCostText.fontSize;
            _upgradeCostText.SetText("DONE");
            _upgradeCostText.enableAutoSizing = true;
            _upgradeButton.interactable = false;
        }
        // if upgrade available, set cost accordingly
        else
        {
            _upgradeCostText.SetText(_target.upgradeCost.ToString());
            _upgradeCostText.enableAutoSizing = false;
            _upgradeCostText.fontSize = _originalTextSize;
            _upgradeButton.interactable = true;
        }

        // show the panel
        _ui.SetActive(true);
    }

    /// <summary>
    /// hide the panel from game view
    /// </summary>
    public void Hide()
    {
        _ui.SetActive(false);
    }

    /// <summary>
    /// upgrade the target turret
    /// </summary>
    public void Upgrade()
    {
        GameManager.gameManager.buildManager.UpgradeTurret(_target);
    }

    /// <summary>
    /// sell the target turret
    /// </summary>
    public void Sell()
    {
        GameManager.gameManager.buildManager.SellTurret(_target);
    }
}
