# Enemy Animation Setup Guide

## 🎯 ภาพรวม
การตั้งค่า Animation สำหรับศัตรูให้เล่น Walk animation เมื่อเคลื่อนที่

## ✅ ที่ทำไปแล้ว

### 1. อัปเดต EnemyController.cs
- ✅ เพิ่ม `bool isWalking` logic
- ✅ ตั้งค่า Animator parameters: `IsWalking`, `IsAttacking`, `IsDead`
- ✅ เพิ่ม debug log สำหรับตรวจสอบ animation

### 2. สร้าง Animator Controller
- ✅ สร้าง `Animation/Enemy/Enemy.controller`
- ✅ ตั้งค่า Parameters ที่จำเป็น
- ✅ สร้าง States: Idle, Walk

## 🔧 ที่ต้องทำต่อใน Unity Editor

### 1. ตั้งค่า Animator Controller
1. เปิด `Animation/Enemy/Enemy.controller`
2. ลาก `walk.anim` เข้าไปใน Walk state
3. ลาก `Idle.anim` เข้าไปใน Idle state
4. สร้าง Transitions:
   - **Idle → Walk**: `IsWalking = true`
   - **Walk → Idle**: `IsWalking = false`
   - **Any State → Attack**: `IsAttacking = true`
   - **Any State → Dead**: `IsDead = true`

### 2. ตั้งค่า Enemy Prefab
1. สร้าง GameObject สำหรับศัตรู
2. เพิ่ม Components:
   - `SpriteRenderer`
   - `Rigidbody2D` (Gravity Scale = 0, Freeze Rotation)
   - `Animator` (Controller = Enemy.controller)
   - `EnemyController`
   - `EnemyStats`
3. กำหนดค่าใน Inspector

### 3. ตั้งค่า Animation Transitions

#### Idle → Walk
- Condition: `IsWalking` = `true`
- Transition Duration: 0.1
- Has Exit Time: ❌

#### Walk → Idle  
- Condition: `IsWalking` = `false`
- Transition Duration: 0.1
- Has Exit Time: ❌

#### Any State → Attack
- Condition: `IsAttacking` = `true`
- Transition Duration: 0.0
- Has Exit Time: ❌

#### Any State → Dead
- Condition: `IsDead` = `true`
- Transition Duration: 0.0
- Has Exit Time: ❌

## 🎮 การทดสอบ

### 1. ทดสอบ Animation
1. วาง Enemy prefab ในฉาก
2. กด Play
3. ตรวจสอบ Console log:
   ```
   🤖 [EnemyName] AI initialized in Aggressive mode
   🎬 [EnemyName] Animation: Walking=True/False, Dead=False, Attacking=False
   ```

### 2. ทดสอบ AI States
- **Idle**: ศัตรูยืนนิ่ง → เล่น Idle animation
- **Patrol**: ศัตรูเดินสุ่ม → เล่น Walk animation  
- **Chase**: ศัตรูไล่ตาม Player → เล่น Walk animation
- **Attack**: ศัตรูโจมตี → เล่น Attack animation
- **Dead**: ศัตรูตาย → เล่น Death animation

## 🐛 การแก้ไขปัญหา

### Animation ไม่เปลี่ยน
1. ตรวจสอบว่า Animator Controller ถูกกำหนดหรือไม่
2. ตรวจสอบ Parameters ใน Animator
3. ตรวจสอบ Transitions
4. ดู Console log สำหรับ animation states

### Walk animation ไม่เล่น
1. ตรวจสอบ `rb.velocity.magnitude` > 0.1
2. ตรวจสอบ AI state (Patrol, Chase, Retreat)
3. ตรวจสอบ `IsWalking` parameter ใน Animator

## 📝 Notes

### Animation Logic ใน EnemyController
```csharp
bool isWalking = rb.velocity.magnitude > 0.1f && 
                (currentState == AIState.Patrol || 
                 currentState == AIState.Chase || 
                 currentState == AIState.Retreat);
```

### Parameters ที่ใช้
- `IsWalking` (Bool) - เดินหรือไม่
- `Attack` (Trigger) - ทริกเกอร์โจมตี

## ✅ Checklist การตั้งค่า
- [ ] ตั้งค่า Animator Controller transitions
- [ ] กำหนด animation clips ให้แต่ละ state
- [ ] สร้าง Enemy prefab
- [ ] ทดสอบการทำงาน
- [ ] แก้ไขปัญหา (ถ้ามี)

เมื่อทำตามขั้นตอนนี้เสร็จ ศัตรูจะเล่น Walk animation เมื่อเคลื่อนที่แล้ว! 🎮✨
