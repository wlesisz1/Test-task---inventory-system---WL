using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace InventorySystem
{
    public class Backpack : MonoBehaviour
    {
        [Header("Slots")]
        [SerializeField] private Transform[] typeSpecificSlots; // Assign these in the Inspector.


        public Dictionary<ItemType, IItem> itemsInBackpack = new Dictionary<ItemType, IItem>(); // Stores items by Item Type
        public UnityEvent<IItem> OnItemAdded { get; internal set; } = new();
        public UnityEvent<IItem> OnItemRemoved { get; internal set; } = new();
        public bool AddItem(IItem item)
        {
            // Add item to the backpack
            if (itemsInBackpack.ContainsKey(item.ItemType))
            {
                Debug.Log("Item already in backpack!");
                return false;
            }

            itemsInBackpack.Add(item.ItemType, item);
            item.OnPlaceInBackpack(typeSpecificSlots[(int)item.ItemType]); // Attach the item to the backpack
            OnItemAdded.Invoke(item);
            return true;
        }

        public bool RemoveItem(ItemType itemType)
        {
            // Remove item from the backpack
            if (itemsInBackpack.TryGetValue(itemType, out IItem item))
            {
                itemsInBackpack.Remove(itemType);
                item.OnRemoveFromBackpack(); // Call item's OnRemove function
                OnItemRemoved.Invoke(item);
                return true;
            }

            Debug.Log("Item not found in backpack!");
            return false;
        }
    }
}