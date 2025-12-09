# 🎨 Combat Effects Setup Guide

## ภาพรวมระบบ Effects ใหม่

ระบบ Combat Effects ใหม่ถูกสร้างขึ้นเพื่อแยกส่วนแสดงผลทางภาพออกจาก logic การต่อสู้ ทำให้ระบบมีความยืดหยุ่นและบำรุงรักษาง่ายขึ้น

## 📁 ไฟล์ที่เพิ่ม/แก้ไข

### ✅ ไฟล์ใหม่
- `Scripts/Combat/CombatEffectManager.cs` - จัดการแสดง Effects ทั้งหมด
- `Prefabs/CombatEffects/` - โฟลเดอร์สำหรับเก็บ Prefabs ของ Effects

### ✅ ไฟล์ที่แก้ไข
- `Scripts/Combat/DamageCalculator.cs` - เอา Speed ออกจากการคำนวณ
- `Scripts/Combat/CombatManager.cs` - เพิ่มการใช้ Effects
- `Scripts/Enemies/EnemyController.cs` - เพิ่มการใช้ Effects

## 🎯 คุณสมบัติหลักของระบบใหม่

### 1. **ไม่ใช้ Speed ในการคำนวณ**
- โจมตีโดน 100% ถ้าอยู่ในระยะ
- ไม่มีการพลาด (miss)
- ทำให้การต่อสู้น่าเชื่อถือมากขึ้น

### 2. **แยกส่วนแสดงผล**
- `CombatEffectManager` จัดการ Effects ทั้งหมด
- สนับสนุนหลายประเภทของ Effects:
  - Player Attack Effects (Normal, Double Strike, Power)
  - Enemy Attack Effects
  - Hit Effects (Normal, Critical, Enemy)
  - Skill Effects

### 3. **Auto-detection Components**
- หา Components อัตโนมัติถ้าไม่มี
- ลดความซับซ้อนในการตั้งค่า

## 🔧 วิธีการตั้งค่า

### 1. **สร้าง Prefabs สำหรับ Effects**
```csharp
// ตัวอย่าง Prefabs ที่ต้องสร้าง
Prefabs/CombatEffects/
├── PlayerAttackEffect.prefab
├── DoubleStrikeEffect.prefab
├── PowerAttackEffect.prefab
├── EnemyAttackEffect.prefab
├── HitEffect.prefab
├── CriticalHitEffect.prefab
└── EnemyHitEffect.prefab
```

### 2. **ตั้งค่า CombatEffectManager**
```csharp
// ใน Inspector ของ CombatEffectManager
public GameObject playerAttackEffectPrefab;      // PlayerAttackEffect.prefab
public GameObject doubleStrikeEffectPrefab;       // DoubleStrikeEffect.prefab
public GameObject powerAttackEffectPrefab;         // PowerAttackEffect.prefab
public GameObject enemyAttackEffectPrefab;         // EnemyAttackEffect.prefab
public GameObject hitEffectPrefab;                // HitEffect.prefab
public GameObject criticalHitEffectPrefab;         // CriticalHitEffect.prefab
public GameObject enemyHitEffectPrefab;            // EnemyHitEffect.prefab
```

### 3. **เชื่อมต่อกับระบบอื่น**
- `CombatManager` จะหา `CombatEffectManager` อัตโนมัติ
- `EnemyController` จะหา `CombatEffectManager` อัตโนมัติ
- ไม่ต้องลิงก์ manually ถ้าใช้ auto-detection

## 🎮 การใช้งาน

### Player Attack Effects
```csharp
// Normal Attack
effectManager.ShowPlayerAttackEffect(PlayerAttackType.Normal, playerPos, enemyPos);

// Double Strike
effectManager.ShowPlayerAttackEffect(PlayerAttackType.DoubleStrike, playerPos, enemyPos);

// Power Attack
effectManager.ShowPlayerAttackEffect(PlayerAttackType.Power, playerPos, enemyPos);
```

### Enemy Attack Effects
```csharp
// Enemy Attack
effectManager.ShowEnemyAttackEffect(enemyPos, playerPos);
```

### Hit Effects
```csharp
// Normal Hit
effectManager.ShowHitEffect(target, isCritical: false, isEnemy: false);

// Critical Hit
effectManager.ShowHitEffect(target, isCritical: true, isEnemy: false);

// Enemy Hit
effectManager.ShowHitEffect(target, isCritical: false, isEnemy: true);
```

## 🔄 การทำงานร่วมกับระบบเดิม

### กับ CombatManager
- เรียก Effects ตอนโจมตี
- แสดง Effect การโจมตีและ Effect ตอนโดนโจมตี
- ไม่กระทบ logic การคำนวณความเสียหาย

### กับ EnemyController
- เรียก Effects ตอน Enemy โจมตี Player
- แสดง Effect การโจมตีของ Enemy
- แสดง Effect ตอน Player โดนโจมตี

### กับ DamageCalculator
- ไม่ใช้ Speed ในการคำนวณอีกต่อไป
- โจมตีโดนเสมอถ้าอยู่ในระยะ
- เหลือฟังก์ชันเก่าไว้สำหรับ backward compatibility

## 🎨 ประเภทของ Effects

### PlayerAttackType Enum
```csharp
public enum PlayerAttackType
{
    Normal,        // โจมตีปกติ
    DoubleStrike,   // สกิล Double Strike
    Power          // สกิล Power Attack
}
```

### HitEffectType Enum
```csharp
public enum HitEffectType
{
    Normal,        // โดนโจมตีปกติ
    Critical,      // โดนคริติคอล
    Enemy          // โดน Enemy โจมตี
}
```

## 🚀 ขั้นตอนถัดไป

### 1. **สร้าง Prefabs จริง**
- สร้าง Prefabs สำหรับแต่ละประเภทของ Effect
- อาจใช้ Particle Systems หรือ Sprite Animations

### 2. **ทดสอบระบบ**
- ทดสอบการโจมตีปกติ
- ทดสอบสกิลต่างๆ
- ทดสอบ Effects ที่แสดง

### 3. **ปรับแต่ง Performance**
- ตรวจสอบว่า Effects ไม่ทำให้เกม lag
- ใช้ Object Pooling ถ้าจำเป็น

## 🔍 การ Debug

### Log Messages
- ทุกการเรียก Effect จะมี Debug Log
- แสดงชนิดของ Effect และตำแหน่ง
- ช่วยในการตรวจสอบว่า Effect ทำงานถูกต้อง

### Gizmos
- `CombatManager` แสดงระยะโจมตี
- `EnemyController` แสดงระยะตรวจจับและโจมตี
- ช่วยในการปรับแต่ง balance

## 📝 บันทึกการเปลี่ยนแปลง

### ✅ เสร็จสิ้น
- [x] สร้าง CombatEffectManager.cs
- [x] แก้ไข DamageCalculator.cs ไม่ใช้ Speed
- [x] แก้ไข CombatManager.cs เพิ่ม Effects
- [x] แก้ไข EnemyController.cs เพิ่ม Effects
- [x] สร้างโฟลเดอร์ Prefabs/CombatEffects

### 🔄 รอดำเนินการ
- [ ] สร้าง Prefabs สำหรับ Effects
- [ ] ทดสอบระบบใหม่
- [ ] ปรับแต่ง Performance ถ้าจำเป็น

## 🎯 ประโยชน์ของระบบใหม่

1. **ความยืดหยุ่น** - แยกส่วนแสดงผลออกจาก logic
2. **บำรุงรักษาง่าย** - แก้ไข Effects ไม่กระทบการต่อสู้
3. **Performance ดีขึ้น** - ไม่ต้องคำนวณ Speed ที่ซับซ้อน
4. **น่าเชื่อถือ** - โจมตีโดนเสมอถ้าอยู่ในระยะ
5. **ขยายง่าย** - เพิ่มประเภท Effects ใหม่ได้ง่าย

---

**📅 อัปเดตล่าสุด: 12/9/2025**
**👨‍💻 พัฒนาโดย: Combat System Team**
