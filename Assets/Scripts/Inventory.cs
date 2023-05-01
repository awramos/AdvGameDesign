using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour //TODO: all of this
{
    /**
    public GameObject inventory;
    public GameObject itemPickedUp;

    private int allSlots;

    public GameObject[] slot;

    public GameObject slotHolder;

    void Start()
    {
        allSlots = 6;
        slot = new GameObject[allSlots];

        for (int i = 0; i < allSlots; i++)
        {
            slot[i] = slotHolder.transform.GetChild(i).gameObject;

            if (slot[i].GetComponent<Slot>().item == null)
            {
                slot[i].GetComponent<Slot>().empty = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "item")
        {
            itemPickedUp = other.gameObject;
            Item item = itemPickedUp.GetComponent<Item>();

            AddItem(itemPickedUp, item.ID, item.type, item.description, item.icon);
        }
    }

    //public void OnClick()
    //{
    //if(itemPickedUp.GetComponent<Item>().description == "HealthPotion")
    //{
    //potionGrab.Play();
    //GetComponent<PlayerController>().currentHealth += 15;
    //Console.WriteLine("Clicked via enabled inventory");

    //}
    //}

    void AddItem(GameObject itemObject, int itemID, string itemType, string itemDescription, Sprite itemIcon)
    {
        for (int i = 0; i < allSlots; i++)
        {
            if (slot[i].GetComponent<Slot>().empty)
            {
                itemObject.GetComponent<Item>().pickedUp = true;

                var isEmptySlot = slot[i].GetComponent<Slot>();

                isEmptySlot.item = itemObject;
                isEmptySlot.icon = itemIcon;
                isEmptySlot.type = itemType;
                isEmptySlot.ID = itemID;
                isEmptySlot.description = itemDescription;


                itemObject.transform.parent = slot[i].transform;
                itemObject.SetActive(false);

                slot[i].GetComponent<Slot>().UpdateSlot();
                slot[i].GetComponent<Slot>().empty = false;

                return;
            }
        }
    }
    */
}
