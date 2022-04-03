using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public void OnMouseDown()
    {
        GameManager.gameManager.buildManager.ToggleSelectUI(GetComponent<Turret>());
    }
}
