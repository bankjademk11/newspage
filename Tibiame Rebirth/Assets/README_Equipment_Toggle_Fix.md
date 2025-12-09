# Equipment Toggle Fix - แก้ไขปัญหา Equipment Panel เปิดตอนเริ่มเกม

## ปัญหาที่พบ
- Equipment Panel แสดงผลตอนเริ่มเกม แม้ว่าจะตั้งค่า CanvasGroup.alpha = 0 แล้ว
- ปุ่ม Equipment Toggle ไม่สามารถควบคุมการเปิด/ปิดได้อย่างถูกต้อง

## สาเหตุ
- GameObject ของ Equipment Panel ยังคงเปิดอยู่ (SetActive(true)) แม้ว่า CanvasGroup.alpha = 0
- การควบคุม UI ผ่าน CanvasGroup อย่างเดียวไม่เพียงพอ

## วิธีแก้ไข

### 1. แก้ไข EquipmentGenerator.cs
- เพิ่มการบังคับปิด GameObject ด้วย `SetActive(false)` ตอนสร้าง UI
- ตั้งค่า CanvasGroup ให้ปิดสนิท (alpha = 0, interactable = false, blocksRaycasts = false)

### 2. สร้าง EquipmentToggle.cs ใหม่
- สร้าง Component สำหรับควบคุมการเปิด/ปิด Equipment Panel โดยเฉพาะ
- มีฟังก์ชันครบถ้วน: Toggle, Open, Close, ForceClose
- บังคับปิด UI ตอน Start เพื่อให้แน่ใจ

### 3. แก้ไข EquipmentAutoGenerator.cs
- เปลี่ยนการสร้างปุ่ม Toggle ให้ใช้ EquipmentToggle Component
- ผูก Event กับฟังก์ชัน ToggleEquipmentPanel() ของ EquipmentToggle

### 4. แก้ไข EquipmentManager.cs
- เพิ่มการบังคับปิด Equipment Panel อีกครั้งใน Start()
- ให้แน่ใจว่า UI ปิดสนิทหลังจากตั้งค่าทั้งหมดเรียบร้อย

## การใช้งาน

### ติดตั้งระบบ Equipment (พร้อม Fix)
1. เปิด Unity Editor
2. ไปที่เมนู `Tools > Equipment System > Setup Equipment System`
3. ระบบจะสร้าง Equipment Panel ที่ปิดสนิทตอนเริ่มเกม
4. ปุ่ม Equipment Toggle จะควบคุมการเปิด/ปิดได้อย่างถูกต้อง

### ทดสอบการทำงาน
1. กดปุ่ม Equipment (มุมขวาบน) เพื่อเปิดหน้าต่าง Equipment
2. กดปุ่มอีกครั้งเพื่อปิดหน้าต่าง
3. ตรวจสอบ Console Log ว่ามีข้อความแสดงสถานะการเปิด/ปิด

## ฟีเจอร์ที่เพิ่มเข้ามา

### EquipmentToggle Component
- **ToggleEquipmentPanel()**: เปิด/ปิดสลับกัน
- **OpenEquipmentPanel()**: เปิดหน้าต่าง
- **CloseEquipmentPanel()**: ปิดหน้าต่าง
- **ForceCloseEquipmentPanel()**: บังคับปิดให้แน่นอน
- **SetEquipmentPanel()**: ตั้งค่า Equipment Panel จากภายนอก

### การบังคับปิดแบบหลายชั้น
1. **GameObject.SetActive(false)** - ปิด GameObject ทั้งหมด
2. **CanvasGroup.alpha = 0** - ทำให้มองไม่เห็น
3. **CanvasGroup.interactable = false** - ปิดการโต้ตอบ
4. **CanvasGroup.blocksRaycasts = false** - ไม่รับ Raycast

## ไฟล์ที่แก้ไข
- ✅ `Scripts/UIGenarate/EquipmentGenerator.cs` - บังคับปิด UI ตอนสร้าง
- ✅ `Scripts/Inventory/EquipmentToggle.cs` - Component ควบคุมใหม่
- ✅ `Scripts/UIGenarate/EquipmentAutoGenerator.cs` - ใช้ EquipmentToggle Component
- ✅ `Scripts/Inventory/EquipmentManager.cs` - บังคับปิด UI ตอน Start

## ผลลัพธ์
- ✅ Equipment Panel ปิดสนิทตอนเริ่มเกม
- ✅ ปุ่ม Equipment Toggle ทำงานได้อย่างถูกต้อง
- ✅ ไม่มีปัญหา UI แสดงผลผิดพลาด
- ✅ มี Log แสดงสถานะการทำงานชัดเจน

## ถ้ายังมีปัญหา
1. ตรวจสอบว่ามี EquipmentToggle Component อยู่บนปุ่มหรือไม่
2. ตรวจสอบว่า Equipment Panel มี CanvasGroup Component หรือไม่
3. ดู Console Log สำหรับข้อความแสดงสถานะ
4. ลองลบระบบ Equipment แล้วติดตั้งใหม่

---
**หมายเหตุ**: การแก้ไขนี้มั่นใจว่า Equipment Panel จะปิดสนิทตอนเริ่มเกมและสามารถควบคุมได้อย่างถูกต้องผ่านปุ่ม Toggle
