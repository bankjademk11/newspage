# Lobby UI Setup Guide

## ภาพรวม
คู่มือนี้จะอธิบายวิธีการตั้งค่าหน้า Lobby ให้มีปุ่มที่สวยงามและมีมิติ พร้อม Animation และ Effects ต่างๆ

## ไฟล์ที่สร้างขึ้น
1. **Scripts/Lobby/LobbyManager.cs** - Script หลักสำหรับจัดการ Lobby
2. **Prefabs/Lobby/ButtonGlowEffect.prefab** - Prefab สำหรับ Effect การเรืองแสงของปุ่ม
3. **Animation/Lobby/ButtonGlowAnimation.anim** - Animation สำหรับการเรืองแสง
4. **Animation/Lobby/ButtonGlowController.controller** - Animation Controller

## ขั้นตอนการตั้งค่าใน Unity

### 1. สร้าง UI Canvas และ Panel
1. สร้าง Canvas ใน Scene Lobby
2. สร้าง Panel หลักสำหรับจัดวาง UI
3. ตั้งค่า Canvas Scaler เป็น Scale With Screen Size
4. ตั้งค่า Reference Resolution เป็น 1920x1080

### 2. สร้างปุ่มหลัก
สร้างปุ่ม 3 ปุ่ม:
- **Play Button** - สีเขียว
- **Settings Button** - สีฟ้า  
- **Exit Button** - สีแดง

#### การตั้งค่าแต่ละปุ่ม:
1. สร้าง Button ใต้ Panel
2. ตั้งค่าขนาด (Width: 200, Height: 60)
3. จัดวางตำแหน่ง:
   - Play Button: (0, 50)
   - Settings Button: (0, -20)
   - Exit Button: (0, -90)
4. ตั้งค่าสี Background ตามประเภทปุ่ม
5. เพิ่ม Text Component และตั้งค่า Font Size: 24

### 3. เพิ่ม LobbyManager Script
1. สร้าง GameObject ชื่อ "LobbyManager"
2. แนบ Script `LobbyManager.cs` เข้ากับ GameObject
3. ลากปุ่มทั้ง 3 มาเชื่อมต่อกับตัวแปรใน Inspector:
   - Play Button → playButton
   - Settings Button → settingsButton
   - Exit Button → exitButton

### 4. ตั้งค่า Audio
1. เพิ่ม AudioSource ให้กับ LobbyManager GameObject
2. กำหนด Audio Clip:
   - hoverSound: เสียงเมื่อ hover ปุ่ม
   - clickSound: เสียงเมื่อคลิกปุ่ม

### 5. ตั้งค่า Visual Effects
1. ลาก `ButtonGlowEffect.prefab` มาเชื่อมต่อกับตัวแปร `buttonGlowEffect`
2. ตั้งค่าสีตามปุ่ม:
   - Play Button: Green
   - Settings Button: Blue
   - Exit Button: Red

### 6. ตั้งค่า Animation
1. เปิด `ButtonGlowEffect.prefab`
2. ตั้งค่า Animator Controller เป็น `ButtonGlowController.controller`
3. ตั้งค่า Animation Clip เป็น `ButtonGlowAnimation.anim`

## ฟีเจอร์ที่ได้รับ

### 1. Hover Effects
- ปุ่มขยายขึ้นเมื่อ hover (1.1x)
- มี Effect เรืองแสงปรากฏ
- เล่นเสียง hover

### 2. Click Effects
- ปุ่มย่อขนาดลงเมื่อคลิก (0.95x)
- เล่นเสียงคลิก
- กลับสู่ขนาดปกติหลังคลิก

### 3. Idle Animations
- ปุ่มลอยขึ้นลงอย่างนุ่มนวล
- ทำงานแบบ cycle ตลอดเวลา

### 4. Button Functions
- **Play Button**: โหลด Scene "World1"
- **Settings Button**: เปิดหน้าตั้งค่า (สำหรับพัฒนาต่อ)
- **Exit Button**: ปิดเกม

## การปรับแต่งเพิ่มเติม

### การเปลี่ยนสีปุ่ม
```csharp
// ใน LobbyManager.cs
public Color playButtonColor = Color.green;
public Color settingsButtonColor = Color.blue;
public Color exitButtonColor = Color.red;
```

### การปรับความเร็ว Animation
```csharp
public float hoverDuration = 0.2f;
public float clickDuration = 0.1f;
```

### การปรับขนาด Effect
```csharp
public float hoverScale = 1.1f;
public float clickScale = 0.95f;
```

## การแก้ไขปัญหาที่อาจเกิดขึ้น

### 1. ปุ่มไม่ทำงาน
- ตรวจสอบว่าได้เชื่อมต่อปุ่มกับ LobbyManager แล้ว
- ตรวจสอบว่ามี EventSystem ใน Scene
- ตรวจสอบว่า Graphic Raycaster อยู่บน Canvas

### 2. Animation ไม่ทำงาน
- ตรวจสอบว่า Animator Controller ถูกตั้งค่าอย่างถูกต้อง
- ตรวจสอบว่า Animation Clip ถูกเชื่อมต่อกับ Controller

### 3. เสียงไม่เล่น
- ตรวจสอบว่า AudioSource ถูกเพิ่มและตั้งค่าแล้ว
- ตรวจสอบว่า Audio Clip ถูกกำหนด
- ตรวจสอบว่า Volume ไม่ใช่ 0

## ขั้นตอนถัดไป
1. เพิ่ม Background และ Decoration ให้หน้า Lobby
2. สร้าง Settings Panel
3. เพิ่ม Particle Effects หรือ Background Animation
4. ทดสอบบนอุปกรณ์ต่างๆ เพื่อให้แน่ใจว่า Responsive
