using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Player : MovingEntity
{
    public delegate PersistentEntity AttemptDropItem(Transform lTransform, IInventoryItem lItem);
    public AttemptDropItem mAttemptDropItem;

    public delegate void ChangeCurser(ControlsManager.TargetingMode setTargetMode);
    public ChangeCurser mChangeCurser;

    MapTile TileAtCurserLocation;
    PowerSource ObjectAtCurserLocation;
    Crate ClickedOnCrate;

    Rigidbody rb;
    // Player Condition
    Vector3 mVelocity;
    bool mIsDashing;

    // Player Attributes
    public float mMoveSpeed = 5f;
    public float mRotateSpeed = 5f;
    float mDashSpeed;

    // Nearby items the player can see
    List<Crate> mNearbyCrates;

    // Stuff for GUI
    public Transform HUDText;
    public List<Transform> pInventoryDisplay;
    public Sprite pEmptyInventorySprite = null;
    public Color pUnselectedColor = new Color(208f / 255f, 199f / 255f, 174f / 255f, 1);
    public Color pSelectedColor = new Color(200 / 255f, 133f / 255f, 117f / 255f, 1);
    ColorBlock mUnselectedColorBlock = new ColorBlock();
    ColorBlock mSelectedColorBlock;

    // Cheats
    public bool mGodMode = false;

    private void Awake()
    {
    }

    protected override void Start()
    {
        // Sanity Check
        if (pInventoryDisplay.Count != 4)
            Debug.LogError("!Assign the Inventory GUI Buttons to the player");

        if (HUDText == null)
            Debug.LogError("!Assign Player HUD text in editor");

        // Initialize variables
        base.Start();
        //UpdateStats(GameManager.instance.playerHealth, GameManager.instance.playerXP);
        InitColorBlocks();
        mNearbyCrates = new List<Crate>();
        mInventory.mMassDriverAmmo = Instantiate(mInventory.mMassDriverAmmo);
        mInventory.mEnergyCells = Instantiate(mInventory.mEnergyCells);
        mInventory.mExplosives = Instantiate(mInventory.mExplosives);
        mInventory.mParts = Instantiate(mInventory.mParts);
        rb = GetComponent<Rigidbody>();
        SetSelection(0);
        UpdateResourceInGUI();

        // The player is leaking oxygen at a rate of 1 unit / second
        mPressureLeakRate = 1;

        // Set Starting inventory
        //mInventory.UpdateResource(new ResourcePool(ResourceType.MassDriver, 240, 24));
        //mInventory.UpdateResource(new ResourcePool(ResourceType.EnergyCell, 20, 1));
        //mInventory.UpdateResource(new ResourcePool(ResourceType.Explosive, 20, 2));
        //mInventory.UpdateResource(new ResourcePool(ResourceType.Parts, 100, 10));
    }

    protected override void Update()
    {
        base.Update();

        // Check Location of nearby crates, 
        // if they are far away remove them from nearby list
        for (int i = 0; i < mNearbyCrates.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, mNearbyCrates[i].transform.position);
            if (distance > 3)
            {
                //set button inactive and remove the item from list
                mNearbyCrates[i].SetButtonActive(false);
                mNearbyCrates.Remove(mNearbyCrates[i]);
            }
        }
        // God mode for debugging
        if (mGodMode == true)
        {
            mShield += 10;
            mHealth += 10;
            mPressure += 10;
        }

        UpdateGUIText();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    private void FixedUpdate()
    {

        //TODO: readd dashing
        if (mIsDashing)
        {
            rb.MovePosition(transform.position + transform.forward * mDashSpeed * Time.deltaTime);
            //mIsDashing = false;
        }
        else
        {
            // Move the Player to a position specified by the input received
            rb.MovePosition(rb.position + mVelocity * Time.deltaTime);
        }
    }

    public void DestroyCurrentItem()
    {
        mInventory.RemoveSelectedItem();
        pInventoryDisplay[mInventory.getSelectionIndex()].Find("Image").GetComponent<Image>().sprite = pEmptyInventorySprite;

        UpdateResourceInGUI();
    }

    public void UpdateGUIText()
    {
        string HUDstring = "";

        HUDstring = "Player HUD \n";
        HUDstring += mPressure + "\n";
        HUDstring += mShield + "\n";
        HUDstring += mHealth + "\n";
        HUDstring += transform.position.x +"," + transform.position.y +"," + transform.position.z + "\n";

        HUDText.GetComponent<Text>().text = HUDstring;
    }

    // Try to drop the currently selected item on the ground.
    // return true if we sucessfully moved an item in the inventory and
    // spawned an item crate in the world nearby.
    public bool DropCurrentItem()
    {
        // Don't do anything if we are not holding an item
        if (mInventory.GetSelectionItem() == null)
            return false;

        // Try to spawn the item on the ground, is sucessfull
        // remove the item from our inventory

        if (mAttemptDropItem != null)
        {
            if (mAttemptDropItem(transform, mInventory.GetSelectionItem()) != null)
            {
                //if (PrefabSpawner.GetPrefabSpawner().CreateCrate(
                //    (int)Math.Round(transform.position.x,0), 
                //    (int)Math.Round(transform.position.z,0),
                //    mInventory.GetSelectionItem()) == true)
                //{
                // Remove item from inventory
                mInventory.RemoveSelectedItem();

                // Reflect the loss of item in the GUI
                pInventoryDisplay[mInventory.getSelectionIndex()].Find("Image").GetComponent<Image>().sprite = pEmptyInventorySprite;
                UpdateResourceInGUI();
                return true;
            }
        }

        // Failed to drop item on the ground, there may be too many 
        // objects nearby that are preventing the item spawn.
        return false;
    }

    public bool AddInventoryItem(Crate CrateToAdd)
    {
        IInventoryItem itemToAdd = CrateToAdd.contents;

        //Cant add an item that does not exist
        if (itemToAdd == null)
        {
            Debug.Log("Cant add an item that does not exist");
            return false;
        }

        int addIndex = mInventory.AddItemToInventory(itemToAdd);

        if (addIndex == -1)
        {
            Debug.Log("Failed Adding item to inventory");
            return false;//Not added to inventory
        }
        else
        {
            if (pInventoryDisplay.Count < addIndex)
            {
                Debug.LogError("More space in inventory then set up for display");
            }

            if (pInventoryDisplay[addIndex] == null)
            {
                Debug.LogError("Display item can not be null");
            }

            pInventoryDisplay[addIndex].Find("Image").GetComponent<Image>().sprite = itemToAdd.mDisplaySprite;

            Debug.Log(mInventory.getSelectionIndex() + "," + addIndex);

            if (mInventory.getSelectionIndex() == addIndex)
            {
                mInventory.GetSelectionItem().Select(transform);
                SetSelection(addIndex);
            }


            UpdateResourceInGUI();
            return true;
        }
    }

    // Use to scroll up and down in the inventory items list
    public void CycleInventory(int Distance)
    {
        int newSelection = mInventory.getSelectionIndex() + Distance;
        if (newSelection >= mInventory.getInventoryCap()-1)
        {
            newSelection = mInventory.getInventoryCap() -1;
        }

        if (newSelection < 0)
            newSelection = 0;

        SetSelection(newSelection);
    }

    public void SetSelection(int selection)
    {
        // Not sure what to do if selection is out of bounds. 
        if (selection > pInventoryDisplay.Count || selection < 0)
            return;

        for (int i = 0; i < pInventoryDisplay.Count; i++)
        {
            // Set all Buttons to unselected color
            pInventoryDisplay[i].GetComponent<Button>().colors = mUnselectedColorBlock;

            // Tell all items in inventory that that are not being used right now
            IInventoryItem item = mInventory.getItemAt(i);
            if (item != null)
                item.UnSelect();
            
        }
        // Set the selected button color to be the selected color 
        pInventoryDisplay[selection].GetComponent<Button>().colors = mSelectedColorBlock;

        // Tell inventory what item is selected 
        mInventory.setSelection(selection);

        //Tell the Item that it is selected
        IInventoryItem currentItem = mInventory.GetSelectionItem();
        if (currentItem != null)
            currentItem.Select(transform);

        if (mChangeCurser != null)
        {

            if (currentItem == null)
                mChangeCurser(ControlsManager.TargetingMode.TileSelect);

            else if (currentItem is GunItem)
            {
                mChangeCurser(ControlsManager.TargetingMode.Crosshair);
            }
            else
            {
                mChangeCurser(ControlsManager.TargetingMode.TileSelect);
            }
        }
    }

    public void updateMassDriverAmmo(int amount)
    {
        mInventory. mMassDriverAmmo += amount;
        //update resources display
        UpdateResourceInGUI();
    }

    private void UpdateResourceInGUI()
    {
        for (int i = 0; i < pInventoryDisplay.Count; i++)
        {
            string displayChargesText = "";
            displayChargesText += mInventory.DisplayChargesForItem(i);

            
            //IInventoryItem inventoryItem = mInventory.getItemAt(i);

            //if (inventoryItem != null)
            //{
            //    ResourcePool charges = mInventory.getItemAt(i).mCharges;
                
            //}
            pInventoryDisplay[i].Find("TxtUses").GetComponent<Text>().text = displayChargesText;
        }
    }

    // This method allows the player to carry experience and health over
    // levels. The Player's stats are stored when a new level is to be loaded.
    //void UpdateStats(float newHP, float newXP)
    //{
    //    mHealth = newHP;
    //    mExperience = newXP;
    //}

    public void Move(Vector3 _velocity)
    {
        //Debug.Log("Move Command");
        mVelocity = _velocity * mMoveSpeed;
    }

    public void LookAt(Vector3 lookPoint)
    {
        if (transform == null)
            return;
        // Set the point to the same height as the Player
        Vector3 newPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        
        transform.LookAt(newPoint);
    }

    public void setResource(ResourceType resourceType, int amount)
    {
        switch (resourceType)
        {
            case ResourceType.EnergyCell:
                mInventory.mEnergyCells.SetValue(amount);
                break;
            case ResourceType.Explosive:
                mInventory.mExplosives.SetValue(amount);
                break;
            case ResourceType.MassDriver:
                mInventory.mMassDriverAmmo.SetValue(amount);
                break;
            case ResourceType.Parts:
                mInventory.mParts.SetValue(amount);
                break;
            default:
                Debug.Log("Player does not know Resource: " + resourceType);
                break;
        }
    }

    public ResourcePool getResource(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.EnergyCell:
                return mInventory.mEnergyCells;
            case ResourceType.Explosive:
                return mInventory.mExplosives;
            case ResourceType.MassDriver:
                return mInventory.mMassDriverAmmo;
            case ResourceType.Parts:
                return mInventory.mParts;
            default:
                Debug.Log("Player does not know Resource: " + resourceType);
                return null;
        }
    }

    //public int getResource(ResourceType resourceType)
    //{
    //    switch(resourceType)
    //    {
    //        case ResourceType.Charges:
    //            return 0;
    //        case ResourceType.EnergyCell:
    //            return (int) mInventory.mEnergyCells;
    //        case ResourceType.Explosive:
    //            return (int) mInventory.mExplosives;
    //        case ResourceType.MassDriver:
    //            return (int)mInventory.mMassDriverAmmo;
    //        case ResourceType.Parts:
    //            return (int)mInventory.mParts;
    //        default:
    //            Debug.Log("Player does not know Resource: " + resourceType);
    //            return 0;
    //    }
    //}

    // Press/Hold the reload button
    public void Reload()
    {
        mInventory.ReloadEquipedWeapon();

        //setResource(reloadWithResource,resourceRemain);
        UpdateResourceInGUI();
    }

    // Release the reload button
    public void AbortReload()
    {
        IInventoryItem currentItem = mInventory.GetSelectionItem();
        if (currentItem == null)
            return;

        currentItem.AbortReload();
    }

    public void OnWallClick(DestructibleObstacle wall)
    {
        //Do nothing
    }

    public void OnItemClickDown(Crate pickup)
    {
        ClickedOnCrate = pickup;
    }

    public bool OnItemClickUp(Crate pickup)
    {
        if (pickup == null)
        {
            ClickedOnCrate = null;
            return false;
        }
        if(pickup != ClickedOnCrate)
        {
            ClickedOnCrate = null;
            return false;
        }
        ClickedOnCrate = null;
        return AddInventoryItem(pickup);
    }

    public void OnSelectDown(MapTile TileClicked, PowerSource objectClicked)
    {
        TileAtCurserLocation = TileClicked;
        ObjectAtCurserLocation = objectClicked; 
    }

    public void OnSelectUp(MapTile TileClicked, PowerSource objectClicked)
    {
        if (TileClicked != null && TileAtCurserLocation!= null)
        {
            if (TileClicked == TileAtCurserLocation)
            {
                IInventoryItem currentItem = mInventory.GetSelectionItem();
                if(currentItem != null)
                    currentItem.ClickTile(TileClicked);
            }
        }

        if (objectClicked != null && ObjectAtCurserLocation != null)
        {
            if (objectClicked == ObjectAtCurserLocation)
            {
                IInventoryItem currentItem = mInventory.GetSelectionItem();
                if (currentItem != null)
                    currentItem.ClickObject(objectClicked.transform);
                objectClicked.OnClick();
            }
        }

        TileAtCurserLocation = null;
        ObjectAtCurserLocation = null;

    }

    // Press/Hold the action trigger
    public void OnTriggerHold()
    {
        IInventoryItem currentItem = mInventory.GetSelectionItem();
        if (currentItem != null)
        {
            currentItem.Select(transform);
            currentItem.Use();
            UpdateResourceInGUI();
        }
    }

    // Release the action trigger
    public void OnTriggerRelease()
    {
        IInventoryItem currentItem = mInventory.GetSelectionItem();
        if (currentItem != null)
        {
            currentItem.AbortUse();
            UpdateResourceInGUI();
        }
    }
    // Add a crate to the list of crates we can see the items in
    public void AddNearbyCrate(Crate NearbyCrate)
    {
        if (!mNearbyCrates.Contains(NearbyCrate))
            mNearbyCrates.Add(NearbyCrate);
    }

    // Init helper function
    private void InitColorBlocks()
    {
        mUnselectedColorBlock = new ColorBlock();
        mSelectedColorBlock = new ColorBlock();

        mUnselectedColorBlock.normalColor = pUnselectedColor;
        mUnselectedColorBlock.highlightedColor = pUnselectedColor;
        mUnselectedColorBlock.colorMultiplier = 1;

        mSelectedColorBlock.normalColor = pSelectedColor;
        mSelectedColorBlock.highlightedColor = pSelectedColor;
        mSelectedColorBlock.colorMultiplier = 1;
    }
}
