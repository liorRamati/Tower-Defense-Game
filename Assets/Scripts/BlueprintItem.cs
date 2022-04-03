using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlueprintItem : MonoBehaviour
{
    // the turret for to create from the blueprint
    public Turret itemPrefab;

    private Color initialColor;
    private static Color orange = new Color(1.0f, 0.64f, 0.0f);

    // keep a count of the collider in the blueprint area
    private int collisionCount = 0;

    private Vector3 currentMousePosition;

    private bool isInPlacablePosition = false;

    // meaningful name for the different colors the blueprint can be
    enum BlueprintColor
    {
        Placable,
        NotPlacable,
        NoMoney
    }
    private BlueprintColor currentColor = BlueprintColor.Placable;

    // reference to components in child objects for color changes
    private Renderer[] childrenRenderers;
    private List<Color> childrenColors;

    /// <summary>
    /// called on the frame when a script is enabled just before any of the Update methods are called the first time
    /// </summary>
    private void Start()
    {
        // get all renderers in the child objects
        childrenRenderers = GetComponentsInChildren<Renderer>();
        childrenColors = new List<Color>();
        foreach (Renderer r in childrenRenderers)
        {
            foreach (Material m in r.materials)
            {
                childrenColors.Add(m.color);
            }
        }

        // initial update the blueprint according to the mouse position
        CheckPositionAndMove(Input.mousePosition);
    }

    /// <summary>
    /// Update phase in the native player loop
    /// </summary>
    private void Update()
    {
        // if game ended destroy object
        if (GameManager.gameManager.GameEnded)
        {
            GameManager.gameManager.buildManager.BuildingToBuildSelected = false;
            Destroy(gameObject);
            return;
        }

        // if game pause do nothing
        if (GameManager.gameManager.GamePaused)
        {
            return;
        }

        // if mouse didnt move do nothing
        Vector3 tempMousePos = Input.mousePosition;
        if (tempMousePos != currentMousePosition)
        {
            CheckPositionAndMove(tempMousePos);
        }

        currentMousePosition = tempMousePos;

        // Change color if no money for turrent
        if (GameManager.gameManager.playerStats.currentMoney < itemPrefab.cost)
        {
            if (currentColor == BlueprintColor.Placable)
            {
                ChangeColor(BlueprintColor.NoMoney);
            }
        }

        // change color back if got money for turrent
        else if (currentColor == BlueprintColor.NoMoney)
        {
            ChangeColor(BlueprintColor.Placable);
        }

        // Delete blueprint if backspace or rightclick button is pressed is pressed
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetMouseButtonDown(1))
        {
            GameManager.gameManager.buildManager.BuildingToBuildSelected = false;
            Destroy(gameObject);
            return;
        }

        // place turret in the current position if mouse is clicked and the position is valid
        if (Input.GetMouseButtonDown(0) && isInPlacablePosition)
        {
            GameManager.gameManager.buildManager.BuildTurrent(itemPrefab, transform.position, transform.rotation, false);
            GameManager.gameManager.buildManager.BuildingToBuildSelected = false;
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Check if the position given is valid for creating the item and move the item to that position (color the item according to the validity of the position)
    /// </summary>
    /// <param name="mousePos">the mosue position</param>
    private void CheckPositionAndMove(Vector3 mousePos)
    {
        // get the position on the ground where the mouse is
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 500000f))
        {
            transform.position = hit.point;
        }

        // check if the position is valid (there is flat space on the ground to place item).
        isInPlacablePosition = GameManager.gameManager.Utils.IsInFlatCircle(transform.position, itemPrefab.baseRadius) && collisionCount == 0 && !EventSystem.current.IsPointerOverGameObject();

        // change color if the position change from valid to not valid, or the opposite
        if (!isInPlacablePosition)
        {
            if (currentColor != BlueprintColor.NotPlacable)
            {
                ChangeColor(BlueprintColor.NotPlacable);
            }
        }
        else if (currentColor == BlueprintColor.NotPlacable)
        {
            ChangeColor(BlueprintColor.Placable);
        }
    }

    /// <summary>
    /// changing the color of the blueprint according to the state given
    /// </summary>
    private void ChangeColor(BlueprintColor blueprintColor)
    {
        // choose a opration to execute for each material in child objects
        int i = 0;
        Action<Material> l1;
        // choose what action to take inside the loop.
        switch (blueprintColor)
        {
            case BlueprintColor.NotPlacable:
                l1 = m => m.color = Color.red;
                break;
            case BlueprintColor.NoMoney:
                l1 = m => m.color = orange;
                break;
            case BlueprintColor.Placable:
            default:
                // change back to original colors
                l1 = m =>
                {
                    m.color = childrenColors[i];
                    i++;
                };
                break;
        };

        // execute operation on the materials of the child objects
        foreach (Renderer r in childrenRenderers)
        {
            foreach (Material m in r.materials)
            {
                l1(m);
            }
        }

        currentColor = blueprintColor;
    }

    /// <summary>
    /// happens on the FixedUpdate function when two GameObjects collide.
    /// </summary>
    /// <param name="other">the collider of the other object</param>
    private void OnTriggerEnter(Collider other)
    {
        // add to collider count if the other object is a placemnet object
        if (other.gameObject.tag == "placement")
        {
            collisionCount++;
        }
    }

    /// <summary>
    /// occurs on the FixedUpdate after the Colliders have stopped touching.
    /// </summary>
    /// <param name="other">the collider of the other object</param>
    private void OnTriggerExit(Collider other)
    {
        // reduce collider count if the other object is a placemnet object
        if (other.gameObject.tag == "placement")
        {
            collisionCount--;
        }
    }
}
