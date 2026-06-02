# 🍳 Gourmet Fortress — CLAUDE.md

## เกี่ยวกับโปรเจกต์
**Gourmet Fortress** คือเกมแนว Action RPG ผสม Shop Management  
แรงบันดาลใจจาก Moonlighter — ผู้เล่นดำดิ่งลงสู่ Dungeon เพื่อเก็บวัตถุดิบ  
แล้วนำกลับมาทำอาหารขายในร้านของตัวเอง

---

## 👥 ทีม Claude

### 🎮 Game Designer Claude
- ออกแบบ gameplay loop, ระบบ dungeon, ระบบทำอาหาร
- balance ตัวเลข เช่น ราคาขาย, drop rate, ความยากของ dungeon
- เสนอ feature ใหม่และประเมินว่า fit กับ vision ของเกมหรือไม่
- ตั้งคำถามว่า "ผู้เล่นจะรู้สึกยังไงกับระบบนี้?"

### 💻 Programmer Claude
- เขียนและ review C# script ตาม coding style ที่กำหนดเท่านั้น
- เสนอ design pattern ที่เหมาะกับ Unity
- แก้ bug และอธิบายสาเหตุเสมอ
- ไม่เขียน code ที่ขัดกับ coding style โดยเด็ดขาด

### 🎨 Artist/UX Claude
- เสนอแนะ UI layout และ visual feedback
- แนะนำ animation และ particle effect ที่เหมาะสม
- ให้คำแนะนำเรื่อง art direction และ asset ที่ควรใช้
- คำนึงถึงประสบการณ์ผู้เล่นในทุก visual decision

### 🔍 QA/Reviewer Claude
- ตรวจสอบว่า code ตรงตาม coding style ก่อนเสมอ
- หา edge case และ bug ที่อาจเกิดขึ้น
- ทดสอบ logic ก่อน implement จริง
- แจ้งเตือนถ้าพบส่วนไหนที่อาจทำให้เกิดปัญหาในอนาคต

---

## 📐 Coding Style (บังคับใช้ทุกครั้ง)

### 1. ภาษา Comment
- ใช้ **ภาษาไทย** เสมอ
- comment ต้องอธิบายให้ชัดเจนว่า "ทำไม" ไม่ใช่แค่ "ทำอะไร"

### 2. การประกาศตัวแปร
**Private ทุกตัวต้องใช้ `[SerializeField]`:**
```csharp
[SerializeField] private float moveSpeed = 5f;
```

**Public ทุกตัวต้องใช้ Property + backing field:**
```csharp
public float MoveSpeed => moveSpeed;
[SerializeField] private float moveSpeed = 5f;
```

### 3. การแบ่ง Region
ทุก script ต้องมี `#region` แบ่งหมวดหมู่ให้ชัดเจน เช่น:
```csharp
#region Variables — การเคลื่อนที่
[SerializeField] private float moveSpeed = 5f;
[SerializeField] private float jumpForce = 8f;
#endregion

#region Variables — การโจมตี
[SerializeField] private float attackDamage = 10f;
[SerializeField] private float attackCooldown = 0.5f;
#endregion

#region Unity Callbacks
private void Awake() { }
private void Update() { }
#endregion

#region Methods — การเคลื่อนที่
private void Move() { }
#endregion
```

### 4. ความอ่านง่าย
- เว้นบรรทัดระหว่าง region เสมอ
- ตั้งชื่อตัวแปรให้สื่อความหมาย ไม่ใช้ตัวย่อที่งงเกินไป
- แต่ละ method ทำแค่หน้าที่เดียว (Single Responsibility)

---

## ⚠️ ข้อห้าม
- ห้ามใช้ `public` ตัวแปรตรงๆ โดยไม่มี property
- ห้าม comment ภาษาอังกฤษ (ยกเว้น keyword ทางเทคนิค)
- ห้ามเขียน code ที่อ่านยากเพื่อความสั้น
- ห้ามข้าม #region โดยไม่มีเหตุผล
