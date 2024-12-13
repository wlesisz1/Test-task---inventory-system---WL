using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace InventorySystem
{
    public class BackpackUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image[] backpackSlots; // Array to hold the 3 backpack slot images
        [SerializeField] private Sprite emptySlotSprite; // Sprite for empty slot

        private Backpack backpack; // Reference to the Backpack component
        private bool isItemHovered; // Flag to track if an item is being hovered
        private ItemType currentHoveredItem; // The currently hovered item type

        private void Start()
        {
            backpack = FindObjectOfType<Backpack>();
            UpdateBackpackUI();
        }

        private void OnEnable()
        {
            backpack = FindObjectOfType<Backpack>();
            UpdateBackpackUI();
        }

        // Update the Backpack UI based on the Backpack dictionary
        public void UpdateBackpackUI()
        {
            for (int i = 0; i < backpackSlots.Length; i++)
            {
                // Try to get the item in the current slot (by ItemType index)
                if (backpack.itemsInBackpack.TryGetValue((ItemType)i, out var item))
                {
                    backpackSlots[i].sprite = item.GetItemImage(); // Set the slot's image to the item's image
                    EnableEventTriggersForSlot(backpackSlots[i], item.ItemType);
                }
                else
                {
                    backpackSlots[i].sprite = emptySlotSprite; // Empty slot
                    DisableEventTriggersForSlot(backpackSlots[i]);
                }
            }
        }

        // Enable event triggers for the slot with an item
        private void EnableEventTriggersForSlot(Image slot, ItemType itemId)
        {
            var trigger = slot.gameObject.GetComponent<EventTrigger>();
            trigger.enabled = true;
            trigger.triggers.Clear();

            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            pointerEnterEntry.callback.AddListener((data) => OnPointerEnter(itemId));
            trigger.triggers.Add(pointerEnterEntry);

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            pointerExitEntry.callback.AddListener((data) => OnPointerExit());
            trigger.triggers.Add(pointerExitEntry);
        }

        // Disable event triggers for the slot when empty
        private void DisableEventTriggersForSlot(Image slot)
        {
            var trigger = slot.gameObject.GetComponent<EventTrigger>();
            trigger.enabled = false;
        }

        // Called when pointer enters a slot (item is hovered)
        private void OnPointerEnter(ItemType itemType)
        {
            isItemHovered = true;
            currentHoveredItem = itemType;
        }

        // Called when pointer exits a slot (no item is hovered)
        private void OnPointerExit()
        {
            isItemHovered = false;
        }

        // Called when the mouse LMB is up
        private void OnDisable()
        {
            if (isItemHovered)
            {
                isItemHovered = false;
                backpack.RemoveItem(currentHoveredItem); // Remove the item from the backpack
                UpdateBackpackUI(); // Update the UI after removal
            }
        }
    }
}