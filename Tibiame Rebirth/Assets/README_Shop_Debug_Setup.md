# Shop Debug Setup Guide

## การแก้ไขปัญหาการซื้อขายในร้านค้า

### ปัญหาที่พบ:
- กดปุ่มซื้อแล้วไม่มี Debug Message ปรากฏใน Console
- ไอเท็มไม่เข้า Inventory ของผู้เล่น

### การแก้ไขที่ทำไป:

#### 1. เพิ่ม Debug Log ใน ShopSlot.cs
```csharp
public void OnBuyClicked()
{
    Debug.Log("=== OnBuyClicked() เริ่มทำงาน ===");
    
    if (itemData == null) 
    {
        Debug.LogError("itemData เป็น null!");
        return;
    }
    
    Debug.Log($"พยายามซื้อ: {itemData.itemName} ราคา {itemData.price}");
    
    // ... ตรวจสอบ ShopManager และทำการซื้อ
}
```

#### 2. เพิ่ม Debug Log ใน ShopGenerator.cs
```csharp
// hook buy event
btn.onClick.AddListener(slotScript.OnBuyClicked);

Debug.Log($"สร้าง ShopSlot {i + 1} เรียบร้อย และเชื่อมต่อปุ่มซื้อแล้ว");
```

#### 3. เพิ่ม Debug Log ใน ShopManager.cs
```csharp
public bool TryPurchaseItem(ShopItemData item)
{
    Debug.Log("=== TryPurchaseItem() เริ่มทำงาน ===");
    Debug.Log($"ตรวจสอบการซื้อ: {item.itemName} ราคา {item.price} เงินปัจจุบัน {playerGold}");
    
    // ... ตรวจสอบเงินและทำการซื้อ
}
```

#### 4. เพิ่ม Debug Log ใน InventoryManager.cs
```csharp
public bool AddItem(ItemData item, int amount = 1)
{
    Debug.Log("=== InventoryManager.AddItem() เริ่มทำงาน ===");
    Debug.Log($"พยายามเพิ่ม: {item?.itemName} x{amount}");
    
    // ... ตรวจสอบช่องว่างและเพิ่มไอเท็ม
}
```

## วิธีการตรวจสอบปัญหา:

### ขั้นตอนที่ 1: ตรวจสอบ Console
1. เปิด Unity Console (Window → General → Console)
2. กดปุ่มซื้อในร้านค้า
3. ดูว่ามี Debug Message ปรากฏหรือไม่

### ขั้นตอนที่ 2: ตรวจสอบ Component ใน Inspector
1. **ตรวจสอบ ShopSlot:**
   - เลือกช่องไอเท็มในร้านค้า
   - ตรวจสอบว่ามี ShopSlot component หรือไม่
   - ตรวจสอบว่ามี Button component หรือไม่
   - ตรวจสอบว่า Button มี OnClick event หรือไม่

2. **ตรวจสอบ ShopManager:**
   - ตรวจสอบว่ามี ShopManager ในฉากหรือไม่
   - ตรวจสอบว่า `autoLoadItems` เป็น `true` หรือไม่
   - ตรวจสอบว่ามีไอเท็มใน `shopItems` list หรือไม่

3. **ตรวจสอบ InventoryManager:**
   - ตรวจสอบว่ามี InventoryManager ในฉากหรือไม่
   - ตรวจสอบว่า `slots` array ไม่ว่าง
   - ตรวจสอบว่าแต่ละช่องมี InventorySlot component หรือไม่

### ขั้นตอนที่ 3: ทดสอบการเชื่อมต่อ
1. **ทดสอบปุ่ม:**
   - เพิ่ม Debug Log ง่ายๆ ใน OnBuyClicked()
   - กดปุ่มดูว่ามีข้อความปรากฏหรือไม่

2. **ทดสอบ ShopManager:**
   - ตรวจสอบว่า FindObjectOfType<ShopManager>() หาเจอหรือไม่
   - ตรวจสอบว่า TryPurchaseItem() ถูกเรียกหรือไม่

3. **ทดสอบ InventoryManager:**
   - ตรวจสอบว่า FindObjectOfType<InventoryManager>() หาเจอหรือไม่
   - ตรวจสอบว่า AddItem() ถูกเรียกหรือไม่

## สิ่งที่ต้องตรวจสอบ:

### ✅ ที่ต้องมีในฉาก:
1. **ShopManager** GameObject พร้อม ShopManager component
2. **InventoryManager** GameObject พร้อม InventoryManager component  
3. **Shop UI** ที่สร้างจาก ShopGenerator
4. **Inventory UI** ที่สร้างจาก MainInventoryGenerator

### ✅ ที่ต้องตั้งค่าใน Inspector:
1. **ShopManager:**
   - `autoLoadItems = true` (หรือใส่ไอเท็มเอง)
   - `startingGold` > 0

2. **InventoryManager:**
   - `slots` array ไม่ว่าง
   - แต่ละช่องมี InventorySlot component

### ✅ ที่ต้องเชื่อมต่อ:
1. **Button OnClick** → `ShopSlot.OnBuyClicked()`
2. **ShopSlot** → `ShopManager.TryPurchaseItem()`
3. **ShopManager** → `InventoryManager.AddItem()`

## การแก้ไขปัญหาทั่วไป:

### ปัญหา: ไม่มี Debug Message
- **สาเหตุ:** Button ไม่ได้เชื่อมต่อกับ OnBuyClicked
- **แก้ไข:** ตรวจสอบ Button OnClick ใน Inspector

### ปัญหา: หา ShopManager ไม่เจอ
- **สาเหตุ:** ไม่มี ShopManager ในฉาก
- **แก้ไข:** เพิ่ม ShopManager GameObject ในฉาก

### ปัญหา: หา InventoryManager ไม่เจอ
- **สาเหตุ:** ไม่มี InventoryManager ในฉาก
- **แก้ไข:** เพิ่ม InventoryManager GameObject ในฉาก

### ปัญหา: กระเป๋าเต็ม
- **สาเหตุ:** ไม่มีช่องว่างใน Inventory
- **แก้ไข:** ลบไอเท็มบางอย่างออกก่อน

### ปัญหา: เงินไม่พอ
- **สาเหตุ:** playerGold < item.price
- **แก้ไข:** เพิ่มเงินให้ผู้เล่น

## วิธีการทดสอบ:

1. **เปิด Console** และ **Clear Log**
2. **เปิดร้านค้า** และ **กดซื้อไอเท็ม**
3. **ดู Console** ว่ามี Debug Message อะไรบ้าง
4. **ตาม Debug Message** เพื่อหาสาเหตุของปัญหา

ถ้ายังไม่ได้ ให้ส่ง Debug Log มาให้ตรวจสอบครับ!
