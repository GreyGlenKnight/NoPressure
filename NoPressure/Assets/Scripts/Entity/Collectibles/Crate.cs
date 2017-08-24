using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crate : PersistentEntity {

    public static List<IInventoryItem> RandomItemSpawn = new List<IInventoryItem>();
    public static void addRandomItemToSpawn(IInventoryItem addItem)
    {
        RandomItemSpawn.Add(addItem);
    }

    // The kinds of LivingEntity objects that can utilize this pack.
    List<string> permissibleTags = new List<string>()
    {
        "Player"
    };

    public Transform GuiHoverButton;
    private Transform ButtonClone;
    public IInventoryItem contents = null;
    public bool isLocked = false;//TODO

    public void SetButtonActive()
    {
        ButtonClone.gameObject.SetActive(true);
    }

    public void SetButtonInactive()
    {
        ButtonClone.gameObject.SetActive(false);
    }

    private void CreateGuiButton()
    {
        Vector3 pos = new Vector3(transform.position.x, 3, transform.position.z);

        ButtonClone = Instantiate(GuiHoverButton);
        ButtonClone.transform.SetParent(GameObject.Find("CanvasWorld").transform);
        ButtonClone.transform.localScale = new Vector3(1f, 1f, 1f);
        ButtonClone.transform.position = pos;
        ButtonClone.gameObject.SetActive(false);

        //Add triger to onClick that adds item to player inventory and then deletes itself

        //ButtonClone.GetComponent<Button>().onClick.AddListener(Pickup);
    }

    private void Awake()
    {
    }

    public void Pickup()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (contents == null)
            Debug.LogError("contents are null on pickup");
        if (player == null)
            Debug.LogError("can not find player");
        if (ButtonClone == null)
            Debug.LogError("ButtonClone not set");

        if (player.GetComponent<Player>().AddInventoryItem(this) == true)
        {
            ButtonClone.gameObject.SetActive(false);
            Destroy(gameObject);
        }

        Die();
        //SectorSpawn.RemoveObjectAt(TileLocationInSector);

    }

    public void Remove()
    {
        Debug.Log("Reminder for TODO");
        //TODO BoardManager.getBoardManager().RemoveCrateFromList(transform);
        Destroy(gameObject);
    }

    //Display Gui when close enough to trigger collision
    public void OnTriggerEnter(Collider collider)
    {
        if (permissibleTags.Contains(collider.tag))
        {
            ButtonClone.gameObject.SetActive(true);
            Player player = collider.GetComponent<Player>();
            //Add to nearby Items list
            player.AddNearbyCrate(this);

            return;
        }
    }

    public void SetButtonActive(bool lActive)
    {
        ButtonClone.gameObject.SetActive(lActive);
    }

    public void SetCrateItem(IInventoryItem lContents)
    {
        if (ButtonClone == null)
            CreateGuiButton();

        if (lContents == null)
        {
            Debug.Log("Trying to set a null contents crate");
            return;
        }

       // Debug.Log("Set contents: " + contents.getItemClass());
        contents = lContents;

        if (ButtonClone == null)
            Debug.LogError("Button Clone not created");

        if (ButtonClone.GetChild(0) == null)
            Debug.LogError("Child Sprite not found");

        if (ButtonClone.GetChild(0).GetComponent<Image>() == null)
        {
            Debug.LogError("Image component not found in Child Sprite");
        }

        ButtonClone.GetChild(0).GetComponent<Image>().sprite = contents.mDisplaySprite;

    }

}
