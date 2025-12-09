# 🚀 คู่มือติดตั้งระบบ Equipment แบบอัตโนมัติ

## 📋 ภาพรวม
`EquipmentAutoGenerator` ช่วยให้คุณติดตั้งระบบ Equipment ได้ในคลิกเดียว! ไม่ต้องสร้าง UI เอง ไม่ต้องตั้งค่า Manual ให้ยุ่งยาก

### 🎯 สิ่งที่จะได้รับ
- ✅ **Equipment Panel** - หน้าต่างสวมใส่อุปกรณ์พร้อม UI สวยงาม
- ✅ **Equipment Manager** - จัดการการสวมใส่/ถอดอุปกรณ์
- ✅ **Player Stats Manager** - คำนวณและแสดงสถานะตัวละคร
- ✅ **Equipment Toggle** - ปุ่มเปิด/ปิดหน้าต่าง Equipment
- ✅ **Sample Equipment Items** - ไอเท็มตัวอย่างพร้อมใช้งาน

---

## 🛠️ การติดตั้ง (3 ขั้นตอนง่ายๆ)

### ขั้นที่ 1: เปิด Unity Editor
1. เปิดโปรเจค Unity ของคุณ
2. ตรวจสอบว่ามี Canvas ใน Scene (ถ้าไม่มีจะสร้างให้อัตโนมัติ)

### ขั้นที่ 2: รัน Auto Generator
1. ไปที่เมนู **Tools → Equipment System**
2. คลิก **"Setup Equipment System"**
3. รอสักครู่... 🎉

### ขั้นที่ 3: สร้างไอเท็มตัวอย่าง (Optional)
1. ไปที่เมนู **Tools → Equipment System**
2. คลิก **"Create Sample Equipment Items"**
3. ไอเท็มตัวอย่างจะถูกสร้างในโฟลเดอร์ `Assets/Items`

---

## 🎨 ผลลัพธ์ที่ได้

### UI ที่สร้างอัตโนมัติ
```
┌─────────────────────────┐
│      EQUIPMENT         │
├─────────────────────────┤
│      [HELMET]          │
│                         │
│  [WEAPON]   [ACCESSORY] │
│                         │
│      [ARMOR]           │
│                         │
│      [BOOTS]           │
├─────────────────────────┤
│  STATS:                │
│  HP: 100/100           │
│  ATK: 10               │
│  DEF: 5                │
│  SPD: 3                │
└─────────────────────────┘
```

### GameObjects ที่สร้าง
- **EquipmentPanel** - หน้าต่างหลักของระบบ Equipment
- **EquipmentGenerator** - Component สำหรับจัดการ UI
- **EquipmentManager** - Manager หลักของระบบ
- **PlayerStatsManager** - จัดการสถานะตัวละคร
- **EquipmentToggle** - ปุ่มเปิด/ปิดหน้าต่าง

### ไอเท็มตัวอย่าง
- **Iron Sword** - อาวุธ (ATK +15)
- **Iron Helmet** - หมวก (DEF +5)
- **Iron Armor** - เกราะ (DEF +10, SPD -1)
- **Iron Boots** - รองเท้า (DEF +3, SPD +2)
- **Magic Ring** - แหวน (ATK +5, DEF +2, SPD +1)

---

## 🎮 การใช้งานหลังติดตั้ง

### เปิด/ปิดหน้าต่าง Equipment
- คลิกปุ่ม **"Equipment"** ที่มุมขวาบนของหน้าจอ
- หรือค้นหา GameObject **"EquipmentToggle"** ใน Hierarchy

### สวมใส่อุปกรณ์
1. เปิดหน้าต่าง Inventory (ถ้ามี)
2. ลากไอเท็ม Equipment จาก Inventory
3. วางที่ช่องที่ตรงกับประเภท
4. สถานะจะอัปเดตอัตโนมัติ!

### ถอดอุปกรณ์
1. ลากไอเท็มจากช่อง Equipment
2. วางกลับไปในช่อง Inventory ว่าง
3. ไอเท็มจะกลับไปอยู่ใน Inventory

---

## 🔧 การปรับแต่ง

### ปรับขนาดและสี UI
1. ค้นหา **EquipmentGenerator** ใน Hierarchy
2. แก้ไขค่าใน Inspector:
   - `panelWidth/panelHeight` - ขนาดหน้าต่าง
   - `slotSize` - ขนาดช่อง
   - `panelColor/slotColor` - สีพื้นหลัง

### ปรับสถานะตัวละคร
1. ค้นหา **PlayerStatsManager** ใน Hierarchy
2. แก้ไขค่าใน Inspector:
   - `maxHealth` - HP สูงสุด
   - `baseAttack` - พลังโจมตีพื้นฐาน
   - `baseDefense` - ป้องกันพื้นฐาน
   - `baseSpeed` - ความเร็วพื้นฐาน

### ปรับตำแหน่ง UI
1. คลิกที่ **EquipmentPanel**
2. แก้ไข RectTransform ใน Inspector
3. ปรับ Anchor และ Position ตามต้องการ

---

## 📁 โครงสร้างไฟล์ที่สร้าง

```
Assets/
├── Items/                          # ไอเท็มตัวอย่าง
│   ├── IronSword.asset
│   ├── IronHelmet.asset
│   ├── IronArmor.asset
│   ├── IronBoots.asset
│   └── MagicRing.asset
├── Scripts/
│   ├── Inventory/
│   │   ├── EquipmentSlot.cs        # สคริปต์ช่องสวมใส่
│   │   ├── EquipmentManager.cs     # จัดการ Equipment
│   │   ├── PlayerStats.cs          # คลาสสถานะ
│   │   └── ItemData.cs             # ข้อมูลไอเท็ม (อัปเดต)
│   └── UIGenarate/
│       ├── EquipmentGenerator.cs    # สร้าง UI
│       └── EquipmentAutoGenerator.cs # ติดตั้งอัตโนมัติ
└── README_Equipment_*.md           # คู่มือ
```

---

## 🔄 เมนู Tools ที่เพิ่ม

หลังจากติดตั้ง จะมีเมนูใหม่ใน Unity:

### **Tools → Equipment System**
- **Setup Equipment System** - ติดตั้งระบบทั้งหมด
- **Destroy Equipment System** - ลบระบบทั้งหมด
- **Create Sample Equipment Items** - สร้างไอเท็มตัวอย่าง

---

## 🐛 การแก้ไขปัญหา

### ปัญหาที่อาจเกิดขึ้น

**❌ ไม่เจอเมนู Tools → Equipment System**
- ตรวจสอบว่ามีไฟล์ `EquipmentAutoGenerator.cs` ในโปรเจค
- รอสักครู่ให้ Unity คอมไพล์สคริปต์ใหม่
- Restart Unity Editor

**❌ Equipment Panel ไม่แสดง**
- ตรวจสอบว่ามี Canvas ใน Scene
- ตรวจสอบว่า Equipment Panel ไม่ถูกซ่อนอยู่
- ตรวจสอบ Canvas Group Alpha

**❌ ลากไอเท็มไม่ได้**
- ตรวจสอบว่ามี DragDropHandler ใน Inventory
- ตรวจสอบว่าไอเท็มมี `isEquippable = true`
- ตรวจสอบว่ามี Event System ใน Scene

**❌ สถานะไม่อัปเดต**
- ตรวจสอบว่ามี PlayerStatsManager
- ตรวจสอบการผูก Components ใน Inspector
- ดู Console สำหรับ Error Messages

---

## 🎯 การทดสอบระบบ

### เช็คลิสต์หลังติดตั้ง
- [ ] มีเมนู **Tools → Equipment System**
- [ ] มี **EquipmentPanel** ใน Hierarchy
- [ ] มี **EquipmentManager** ใน Scene
- [ ] มี **PlayerStatsManager** ใน Scene
- [ ] มีปุ่ม **Equipment Toggle** ใน Canvas
- [ ] ลากไอเท็มได้ (ถ้ามีไอเท็ม)
- [ ] สถานะอัปเดตตอนสวมใส่
- [ ] Tooltip แสดงข้อมูลไอเท็ม

### การทดสอบการทำงาน
1. รัน Auto Generator → **Setup Equipment System**
2. รัน Auto Generator → **Create Sample Equipment Items**
3. เพิ่มไอเท็มตัวอย่างใน Inventory (ถ้ามีระบบ)
4. ลากไอเท็มไปยัง Equipment Slots
5. ตรวจสอบสถานะใน Stats Panel
6. ลองถอดไอเท็มกลับไป Inventory

---

## 🚀 ขั้นตอนถัดไป

### หลังจากติดตั้งเสร็จ:
1. **สร้างไอเท็มของคุณเอง** - ใช้ ItemData ScriptableObject
2. **ปรับแต่ง UI** - เปลี่ยนสี ขนาด ตำแหน่ง
3. **เชื่อมต่อกับระบบอื่น** - Save/Load, Combat, etc.
4. **เพิ่มฟีเจอร์เสริม** - Durability, Upgrade, Set Bonus

### ศึกษาเพิ่มเติม:
- 📖 `README_Equipment_Setup.md` - คู่มือ Manual Setup
- 🔧 `README_Inventory_Setup.md` - คู่มือระบบ Inventory
- 💡 `README_Tooltip_Setup.md` - คู่มือระบบ Tooltip

---

## ✅ สรุป

**EquipmentAutoGenerator** ทำให้การติดตั้งระบบ Equipment เป็นเรื่องง่าย:

1. **คลิกเดียวติดตั้ง** - ไม่ต้องสร้าง UI เอง
2. **พร้อมใช้งาน** - ทุก Component ผูกกันอัตโนมัติ
3. **ปรับแต่งง่าย** - แก้ไขใน Inspector ได้ทันที
4. **มีตัวอย่าง** - ไอเท็มพร้อมทดสอบ
5. **เอกสารครบ** - คู่มือและเมนู Tools ครบถ้วน

**🎉 ติดตั้งเสร็จแล้ว! ตอนนี้คุณมีระบบ Equipment ที่สมบูรณ์แล้ว!**

**หากต้องการลบระบบ ใช้เมนู **Tools → Equipment System → Destroy Equipment System** ได้เลย! 🗑️**
