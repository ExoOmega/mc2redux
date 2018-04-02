using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public CharacterStats selectedUnit;
    public int playerTeam;
    //public GameObject unitControls;
    public bool doubleClick;
    public bool overUIElement;
    public GameObject CameraMover;
    public float cameraSpeed = 0.3f;

    bool isSelecting = false;
    Vector3 mousePosition1;
    List<CharacterStats> selectedObjects = new List<CharacterStats>();

    // Update is called once per frame
    void Update()
    {
        if (!overUIElement)
            HandleSelection();

        //bool hasUnit = selectedUnit;
        //unitControls.SetActive(hasUnit);

        HandleCameraMovement();
    }
    void OnGUI()
    {
        if (isSelecting)
        {
            // Create a rect from both mouse positions
            var rect = UnitSelectionBox.GetScreenRect(mousePosition1, Input.mousePosition);
            UnitSelectionBox.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            UnitSelectionBox.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    void HandleSelection()
    {
        // If we press the left mouse button, save mouse location and begin selection
        if (Input.GetButtonDown("RightClick"))
        {
            selectedObjects = new List<CharacterStats>();
            isSelecting = true;
            mousePosition1 = Input.mousePosition;

            foreach (var selectableObject in FindObjectsOfType<CharacterStats>())
            {
                if (selectableObject.team == playerTeam)
                {
                    selectableObject.selected = false;
                }
            }
        }
        // If we let go of the left mouse button, end selection
        if (Input.GetButtonUp("RightClick"))
        {
            foreach (var selectableObject in FindObjectsOfType<CharacterStats>())
            {
                if (IsWithinSelectionBounds(selectableObject.gameObject))
                {
                    selectedObjects.Add(selectableObject);
                    selectableObject.selected = true;
                }
            }

            isSelecting = false;
        }
        if (Input.GetButtonUp("LeftClick"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                foreach (var obj in selectedObjects)
                {
                    obj.transform.GetComponent<CharacterStats>().MoveToPosition(hit.point);
                }
            }
        }
    }

    void HandleCameraMovement()
    {
        float hor = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        float zoom = Input.GetAxis("Mouse ScrollWheel") * 30;

        Vector3 newPos = new Vector3(hor, zoom, vert) * cameraSpeed;
        CameraMover.transform.position += newPos;
    }

    void CheckHit(RaycastHit hit)
    {
        if (hit.transform.GetComponent<CharacterStats>())
        {
            CharacterStats hitStats = hit.transform.GetComponent<CharacterStats>();

            if(hitStats.team == playerTeam)
            {
                if(selectedUnit == null)
                {
                    selectedUnit = hitStats;
                    selectedUnit.selected = true;
                }
                else
                {
                    selectedUnit.selected = false;
                    selectedUnit = hitStats;
                    selectedUnit.selected = true;
                }
            }
            else
            {
                if(selectedUnit == null)
                {
                    //Add enemy team logic here.
                }
            }
        }
        else
        {
            if(selectedUnit)
            {
                if(doubleClick)
                {
                    selectedUnit.run = true;
                }
                else
                {
                    doubleClick = true;
                    StartCoroutine("closeDoubleClick");
                }

                selectedUnit.MoveToPosition(hit.point);
            }
        }
    }

    IEnumerator closeDoubleClick()
    {
        yield return new WaitForSeconds(1);
        doubleClick = false;
    }

    public void EnterUIElement()
    {
        overUIElement = true;
    }

    public void ExitUIElement()
    {
        overUIElement = false;
    }
       
    public bool IsWithinSelectionBounds(GameObject gameObject)
    {
        if (!isSelecting)
            return false;

        var camera = Camera.main;
        var viewportBounds =
            UnitSelectionBox.GetViewportBounds(camera, mousePosition1, Input.mousePosition);

        return viewportBounds.Contains(
            camera.WorldToViewportPoint(gameObject.transform.position));
    }
}
