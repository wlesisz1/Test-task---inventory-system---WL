using System.Collections;
using UnityEngine;
namespace InventorySystem
{
    public interface IItem
    {
        string ItemName { get; }
        int ItemID { get; }
        ItemType ItemType { get; }
        float Weight { get; }

        void OnPickup(Vector3 mousePosition); // Pass mouse position for interaction
        void OnDrag(Vector3 hoverPosition);
        void OnDrop();
        void OnPlaceInBackpack(Transform slotTransform);
        void OnRemoveFromBackpack();

        Sprite GetItemImage();
    }

    public class Item : MonoBehaviour, IItem
    {
        [SerializeField] private string itemName;
        [SerializeField] private int itemID;
        [SerializeField] private ItemType itemType;
        [SerializeField] private float weight;
        [SerializeField] private Sprite itemImage;

        private Rigidbody rb;
        private Collider itemCollider;
        public string ItemName => itemName;
        public int ItemID => itemID;
        public ItemType ItemType => itemType;
        public float Weight => weight;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.mass = weight;
            itemCollider = GetComponent<Collider>();
        }

        public void OnPickup(Vector3 mousePosition)
        {
            rb.isKinematic = true; // Disable physics while dragging    
            Debug.Log($"{itemName} picked up.");
        }
        public void OnDrag(Vector3 hoverPosition)
        {
            transform.position = Vector3.Lerp(transform.position, hoverPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, 0.1f);
        }
        public void OnDrop()
        {
            rb.isKinematic = false; // Re-enable physics
            Debug.Log($"{itemName} dropped.");
        }

        public void OnPlaceInBackpack(Transform slotTransform)
        {
            transform.parent = slotTransform;
            rb.isKinematic = true; // Disable physics while in backpack
            StartCoroutine(SmoothMove(this.transform, slotTransform.position, slotTransform.rotation));
            itemCollider.enabled = false;
            Debug.Log($"{itemName} placed in backpack.");
        }

        public void OnRemoveFromBackpack()
        {
            transform.parent = null;
            rb.isKinematic = false; // Enable physics
            itemCollider.enabled = true;
            Debug.Log($"{itemName} removed from backpack.");
        }



        public Sprite GetItemImage()
        {
            return itemImage;
        }


        private IEnumerator SmoothMove(Transform obj, Vector3 targetPosition, Quaternion targetRotation)
        {
            float t = 0f;
            Vector3 startPosition = obj.position;
            Quaternion startRotation = obj.rotation;

            while (t < 1f)
            {
                t += Time.deltaTime;
                obj.position = Vector3.Lerp(startPosition, targetPosition, t);
                obj.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
                yield return null;
            }
        }
    }

    public enum ItemType
    {
        Type1,
        Type2,
        Type3
    }
}