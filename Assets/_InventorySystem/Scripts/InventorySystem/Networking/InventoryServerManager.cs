
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


namespace InventorySystem.Networking
{
    public class InventoryServerManager : MonoBehaviour
    {
        [SerializeField] private Backpack backpack;

        private const string URL = "https://wadahub.manerai.com/api/inventory/status";
        private const string TOKEN = "Bearer kPERnYcWAY46xaSy8CEzanosAgsWM84Nx7SKM4QBSqPq6c7StWfGxzhxPfDh8MaP";

        private void Awake()
        {
            backpack = FindObjectOfType<Backpack>();
            backpack.OnItemAdded.AddListener((item) => SendItemEvent(item, "Added"));
            backpack.OnItemRemoved.AddListener((item) => SendItemEvent(item, "Removed"));
        }

        private void SendItemEvent(IItem item, string eventType)
        {
            StartCoroutine(PostEvent(item, eventType));
        }

        private IEnumerator PostEvent(IItem item, string eventType)
        {
            WWWForm form = new WWWForm();
            form.AddField("itemID", item.ItemID);
            form.AddField("event", eventType);

            UnityWebRequest request = UnityWebRequest.Post(URL, form);
            request.SetRequestHeader("Authorization", TOKEN);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Server Response: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"Server Error: {request.error}");
            }
        }
    }
}