using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isCan = true;
    private RectTransform rectTransform;

    private Vector3 orPos;

    public int level = 1;

    public GameObject DragPanel;

    public Transform curParent;

    public Sprite[] levelSp;

    public Text levelText;
    // Start is called before the first frame update
    void Start()
    {
        DragPanel = GameObject.Find("DragPanel");
        rectTransform = GetComponent<RectTransform>();
    }// Find the "DragPanel" game object and get the RectTransform component of this object
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isCan)
        {
            return;
        }// Check if isCan is false; if so, exit the method
        curParent = transform.parent;
        orPos = transform.position;// Store the current parent of the object in curParent and its position in orPos
            Debug.Log("start to drag image");
        
    }
    
    public void ShowLevel()
    {
       
        
            //levelText.transform.parent.gameObject.SetActive(true);
             //levelText.text = level.ToString();
    
    }
    
    public void AddLevel() {
        level += 1;
        GameManager.Instance.Score++;
        GetComponent<Image>().sprite = levelSp[level - 1];
       ShowLevel();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // Check if the boolean variable isCan is false
        if (!isCan)
        {
            // If it is false, return and do not proceed with dragging
            return;
        }

        // Set the parent of the game object to be the "DragPanel" game object
        transform.SetParent(DragPanel.transform);

        // Convert the position of the cursor to world coordinates and store the result in the variable pos
        Vector3 pos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.enterEventCamera, out pos);

        // Set the position of the game object to the converted position
        rectTransform.position = pos;
    }
    
    // Called when the user stops dragging the object
public void OnEndDrag(PointerEventData eventData)
{
    // Set the parent of the object to be its original parent and place it at the second index of the parent's children
    transform.parent = curParent;
    transform.SetSiblingIndex(1);

    // Check if the object is overlapping another object
    Drag dr = null;
    if (triggTrans != null)
    {
        // If the overlapping object has a "Drag" component, assign it to the variable dr
        dr = triggTrans.transform.GetComponentInChildren<Drag>();
    }

    // Combine images if they have the same level and the overlapping object is not the current object
    if (dr != null && level == dr.level && dr != this && level < 6)
    {
        // Increase the level of the overlapping object, set the current cell's curDrag to null, and show the new level
        dr.AddLevel();
        transform.parent.GetComponent<Cell>().curDrag = null;
        ShowLevel();

        // Remove the current cell and destroy the current object
        GameObject.Find("Canvas").GetComponent<GameManager>().ReMoveCell(transform.parent.GetComponent<Cell>());
        Destroy(this.gameObject);
    }
    // Otherwise, if the overlapping object is empty and not the current object
    else if (triggTrans != null && dr == null && dr != this)
    {
        // Set the current cell's curDrag to null and show the level
        transform.parent.GetComponent<Cell>().curDrag = null;
        ShowLevel();

        // Remove the current cell from the freeCell list and move the object to the overlapping cell
        GameObject.Find("Canvas").GetComponent<GameManager>().ReMoveCell(transform.parent.GetComponent<Cell>());
        GameObject.Find("Canvas").GetComponent<GameManager>().freeCell.Remove(triggTrans.GetComponent<Cell>());
        transform.position = triggTrans.position;
        transform.SetParent(triggTrans.transform);
        transform.parent.GetComponent<Cell>().curDrag = this;
        transform.GetComponent<Drag>().ShowLevel();
        transform.SetSiblingIndex(1);
    }
    // If there is no overlap, move the object back to its original position
    else
    {
        transform.position = orPos;
    }
}

    // The transform of the overlapping cell
    private Transform triggTrans; 
    
    // Called when the object stays in contact with a trigger collider
    private void OnTriggerStay2D(Collider2D collision)
    {
        // If the collider has the "Cell" tag, assign its transform to triggTrans
        if (collision.tag == "Cell")
        {
            triggTrans = collision.transform;
        }
    } 
    
    // Called when the object enters a trigger collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the collider has the "Cell" tag, assign its transform to triggTrans
        if (collision.tag == "Cell")
        {
            triggTrans = collision.transform;
        }
    } 
    
    // Called when the object exits a trigger collider
    private void OnTriggerExit2D(Collider2D collision)
    {
        // If the collider has the "Cell" tag, set triggTrans to null
        if (collision.tag == "Cell")
        {
            triggTrans = null;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
