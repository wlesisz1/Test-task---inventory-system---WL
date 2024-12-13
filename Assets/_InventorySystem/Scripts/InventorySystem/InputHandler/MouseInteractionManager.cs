using UnityEngine;

namespace InventorySystem
{
    public class MouseInteractionManager : MonoBehaviour
    {
        private IItem currentlyHeldItem;
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private Backpack backpack;
        [SerializeField] private GameObject inventoryUI; // Reference to the backpack UI
        [SerializeField] private float groundHoverDistance;

        private bool isBackpackUIOpen = false;

        void Update()
        {
            if (Input.GetMouseButtonDown(0)) // Left Mouse Button pressed
            {
                HandlePickupOrBackpackInteraction();
            }
            else if (Input.GetMouseButton(0)) // Holding LMB
            {
                if (currentlyHeldItem != null)
                {
                    HandleDrag();
                }
            }
            else if (Input.GetMouseButtonUp(0)) // Releasing LMB
            {
                if (isBackpackUIOpen)
                {
                    CloseBackpackUI();
                }
                else if (currentlyHeldItem != null)
                {
                    HandleDrop();
                }
            }
        }

        private void HandlePickupOrBackpackInteraction()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // If clicking on a backpack
                if (hit.collider.CompareTag("Backpack"))
                {
                    OpenBackpackUI();
                    return;
                }

                // If clicking on an item
                IItem item = hit.collider.GetComponent<IItem>();
                if (item != null)
                {
                    currentlyHeldItem = item;
                    item.OnPickup(hit.point);
                }
            }
        }

        private void HandleDrag()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
            {
                Vector3 hoverPosition = hit.point + Vector3.up * groundHoverDistance; // Hover above ground
                currentlyHeldItem.OnDrag(hoverPosition);
            }
        }

        private void HandleDrop()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
            {
                if (hit.collider.CompareTag("Backpack"))
                {
                    if (backpack.AddItem(currentlyHeldItem))
                    {
                        currentlyHeldItem = null; // Successfully added to backpack
                    }
                }
                else
                {
                    currentlyHeldItem.OnDrop(); // Drop on ground
                    currentlyHeldItem = null;
                }
            }
        }

        private void OpenBackpackUI()
        {
            inventoryUI.SetActive(true); // Show the UI
            isBackpackUIOpen = true;
        }

        private void CloseBackpackUI()
        {
            inventoryUI.SetActive(false); // Hide the UI
            isBackpackUIOpen = false;
        }


    }
}