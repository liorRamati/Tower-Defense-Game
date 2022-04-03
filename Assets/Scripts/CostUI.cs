using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CostUI : MonoBehaviour
{
    public Turret turretItem;

    private Text costText;

    /// <summary>
    /// called on the frame when a script is enabled just before any of the Update methods are called the first time
    /// </summary>
    private void Start()
    {
        // set text according to associated turret
        costText = GetComponent<Text>();
        costText.text = turretItem.cost.ToString();
    }
}
