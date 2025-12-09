using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;
    public float pickupRange = 2f;
    public GameObject player;
    public InventoryManager inventoryManager;
    
    private bool canPickup = false;
    
    void Start()
    {
        // หา Player ถ้ายังไม่ได้กำหนด
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        // หา InventoryManager ถ้ายังไม่ได้กำหนด
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }
    }
    
    void Update()
    {
        // ตรวจสอบระยะห่างจาก Player
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            canPickup = distance <= pickupRange;
            
            // ตรวจสอบการกดปุ่ม E
            if (Input.GetKeyDown(KeyCode.E) && canPickup)
            {
                PickupItem();
            }
        }
    }
    
    void PickupItem()
    {
        if (itemData != null && inventoryManager != null)
        {
            // พยายามเพิ่มไอเท็มเข้า Inventory
            bool success = inventoryManager.AddItem(itemData);
            
            if (success)
            {
                Debug.Log("เก็บไอเท็ม: " + itemData.itemName);
                
                // ทำลายไอเท็มใน Scene
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("กระเป๋าเต็ม! ไม่สามารถเก็บ " + itemData.itemName);
            }
        }
    }
    
    // แสดงข้อความแจ้งเตือน (ถ้าต้องการใช้ UI)
    public bool CanPickup()
    {
        return canPickup;
    }
}
