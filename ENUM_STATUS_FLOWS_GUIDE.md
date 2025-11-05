# Hướng Dẫn Luồng Trạng Thái Enum - Car Rental API

## 📋 Mục Lục
1. [VehicleStatus - Trạng Thái Xe](#vehiclestatus)
2. [RentalStatus - Trạng Thái Thuê Xe](#rentalstatus)
3. [VehicleType - Loại Xe](#vehicletype)
4. [UserStatus - Trạng Thái Người Dùng](#userstatus)
5. [UserRole - Vai Trò Người Dùng](#userrole)
6. [PaymentMethod - Phương Thức Thanh Toán](#paymentmethod)
7. [Luồng Nghiệp Vụ Tổng Quan](#luồng-nghiệp-vụ)
8. [Best Practices cho Android Dev](#best-practices)

---

## VehicleStatus - Trạng Thái Xe {#vehiclestatus}

### Giá Trị Enum
```json
{
  "Available": 1,      // Xe có sẵn, có thể thuê
  "Booked": 2,          // Xe đã được đặt trước (chưa implement)
  "InUse": 3,           // Xe đang được sử dụng
  "Maintenance": 4      // Xe đang bảo trì
}
```

**Lưu ý:** API trả về **string** (ví dụ: "Available", "InUse"), không phải số.

### Sơ Đồ Chuyển Đổi Trạng Thái

```
┌───────────┐
│ Available │ ◄───┐
└─────┬─────┘     │
      │           │
      │ StartRental (POST /api/rentals/start)
      │ - Vehicle phải Available
      │ - Battery ≥ 10%
      │ - User phải Active & Verified
      │
      ▼
┌──────────┐
│  InUse   │
└─────┬────┘
      │
      │ CompleteRental (POST /api/rentals/{id}/complete)
      │ - Rental.Ongoing → Completed
      │ - Battery ≥ 10% → Available
      │ - Battery < 10% → Maintenance
      │
      ▼
┌──────────────┐      ┌──────────────┐
│  Available   │      │ Maintenance  │
└──────────────┘      └──────┬───────┘
                             │
                             │ UpdateBattery ≥ 10%
                             │ (PATCH /api/vehicles/{id}/battery)
                             │
                             ▼
                       ┌──────────────┐
                       │  Available   │
                       └──────────────┘
```

### Các API Liên Quan

#### 1. Lấy danh sách xe (có filter theo status)
```
GET /api/vehicles?status=Available&type=Car&pageNumber=1&pageSize=10
```
- **Query params:** `status` (VehicleStatus?), `type` (VehicleType?), `sortBy`, `sortOrder`
- **Response:** `Page<VehicleResponse>` với `status` là string

#### 2. Lấy thông tin xe theo ID
```
GET /api/vehicles/{id}
```
- **Response:** `VehicleResponse` với `status` là string

#### 3. Thay đổi trạng thái xe (Staff/Admin only)
```
PATCH /api/vehicles/{id}/status
Body: { "status": "Maintenance" }
```
- **Validation:** Không thể chuyển từ `InUse` → `Available` nếu còn rental `Ongoing`
- **Response:** `VehicleResponse` với status mới

#### 4. Cập nhật pin xe
```
PATCH /api/vehicles/{id}/battery
Body: { "batteryLevel": 85 }
```
- **Auto status change:**
  - Nếu `batteryLevel < 10` và xe đang `Available` → tự động chuyển sang `Maintenance`
  - Nếu `batteryLevel ≥ 10` và xe đang `Maintenance` → tự động chuyển sang `Available`

### Luồng Tự Động Chuyển Trạng Thái

1. **Khi bắt đầu thuê xe:**
   - `POST /api/rentals/start` → `VehicleStatus`: `Available` → `InUse`
   - `RentalStatus`: tạo mới với `Ongoing`

2. **Khi hoàn tất thuê xe:**
   - `POST /api/rentals/{id}/complete` → `VehicleStatus`: `InUse` → `Available` (nếu battery ≥ 10%) hoặc `Maintenance` (nếu battery < 10%)
   - `RentalStatus`: `Ongoing` → `Completed`

3. **Khi cập nhật pin:**
   - `PATCH /api/vehicles/{id}/battery` → tự động chuyển status dựa trên battery level

---

## RentalStatus - Trạng Thái Thuê Xe {#rentalstatus}

### Giá Trị Enum
```json
{
  "Booked": 1,        // Đã đặt trước (chưa implement)
  "Ongoing": 2,       // Đang thuê
  "Completed": 3,     // Đã hoàn tất
  "Cancelled": 4      // Đã hủy (chưa implement)
}
```

**Lưu ý:** API trả về **string** (ví dụ: "Ongoing", "Completed").

### Sơ Đồ Chuyển Đổi Trạng Thái

```
┌─────────┐
│ Booked  │ (Chưa implement)
└────┬────┘
     │
     ▼
┌─────────┐
│ Ongoing │ ◄─── StartRental (POST /api/rentals/start)
└────┬────┘
     │
     │ CompleteRental (POST /api/rentals/{id}/complete)
     │
     ▼
┌───────────┐
│ Completed │
└───────────┘

┌───────────┐
│ Cancelled │ (Chưa implement)
└───────────┘
```

### Các API Liên Quan

#### 1. Bắt đầu thuê xe
```
POST /api/rentals/start
Body: {
  "vehicleId": "guid",
  "renterId": "guid",
  "stationId": "guid",
  "staffId": "guid"
}
```
- **Điều kiện:**
  - Vehicle phải `Available`
  - Vehicle battery ≥ 10%
  - User phải `Active` và `IsVerified = true`
- **Kết quả:**
  - Tạo `Rental` với `Status = "Ongoing"`
  - `Vehicle.Status` → `"InUse"`

#### 2. Hoàn tất thuê xe
```
POST /api/rentals/{id}/complete
Body: {
  "endStationId": "guid",
  "finalBatteryLevel": 75,
  "notes": "Optional notes"
}
```
- **Điều kiện:**
  - Rental phải `Ongoing`
  - `finalBatteryLevel` phải từ 0-100
- **Kết quả:**
  - `Rental.Status` → `"Completed"`
  - Tính toán `totalCost` dựa trên duration và vehicle type
  - `Vehicle.Status` → `"Available"` (nếu battery ≥ 10%) hoặc `"Maintenance"` (nếu battery < 10%)

#### 3. Lấy danh sách rental
```
GET /api/rentals?userId={guid}&status=Ongoing&pageNumber=1&pageSize=20
```
- **Query params:** `userId` (Guid?), `status` (RentalStatus?), `pageNumber`, `pageSize`
- **Response:** `Page<RentalDto>` với `status` là string

#### 4. Lấy rental theo ID
```
GET /api/rentals/{id}
```
- **Response:** `RentalDto` với `status` là string

### Tính Toán Chi Phí

Khi hoàn tất rental, hệ thống tự động tính `totalCost`:
- **Car:** 50,000 VND/giờ
- **Scooter:** 30,000 VND/giờ
- **Other:** 20,000 VND/giờ
- **Minimum:** 1 giờ (nếu < 1 giờ vẫn tính 1 giờ)

---

## VehicleType - Loại Xe {#vehicletype}

### Giá Trị Enum
```json
{
  "Scooter": 1,   // Xe máy
  "Car": 2,       // Ô tô
  "Other": 3      // Loại khác
}
```

**Lưu ý:** API trả về **string** (ví dụ: "Scooter", "Car").

### Sử Dụng

- **Filter khi lấy danh sách xe:**
  ```
  GET /api/vehicles?type=Car&status=Available
  ```

- **Khi tạo xe mới:**
  ```
  POST /api/vehicles
  Body: {
    "plateNumber": "30A-12345",
    "type": "Car",  // String
    "stationId": "guid",
    "batteryLevel": 100,
    "image": "file"
  }
  ```

- **Ảnh hưởng đến giá thuê:** Xem phần [RentalStatus - Tính Toán Chi Phí](#rentalstatus)

---

## UserStatus - Trạng Thái Người Dùng {#userstatus}

### Giá Trị Enum
```json
{
  "Active": 1,      // Tài khoản hoạt động
  "Inactive": 2     // Tài khoản bị vô hiệu hóa
}
```

### Sử Dụng

- **Điều kiện để thuê xe:**
  - User phải có `status = "Active"` và `isVerified = true`
  - Nếu không đủ điều kiện, API `POST /api/rentals/start` sẽ trả về lỗi:
    ```json
    {
      "error": "User.NotEligible",
      "message": "User is not eligible to rent a vehicle"
    }
    ```

---

## UserRole - Vai Trò Người Dùng {#userrole}

### Giá Trị Enum
```json
{
  "Admin": 1,      // Quản trị viên
  "Staff": 2,      // Nhân viên
  "Renter": 3      // Người thuê
}
```

### Phân Quyền (Dự Kiến)

- **Admin:** Toàn quyền quản lý hệ thống
- **Staff:** Có thể:
  - Bắt đầu thuê xe cho khách (`POST /api/rentals/start` với `staffId`)
  - Thay đổi trạng thái xe
  - Cập nhật pin xe
- **Renter:** Chỉ có thể:
  - Xem danh sách xe
  - Xem lịch sử thuê của mình

---

## PaymentMethod - Phương Thức Thanh Toán {#paymentmethod}

### Giá Trị Enum
```json
{
  "Cash": 1,        // Tiền mặt
  "Card": 2,        // Thẻ
  "EWallet": 3      // Ví điện tử
}
```

### Sử Dụng

- Được sử dụng trong các API thanh toán (Payment)
- **Lưu ý:** API trả về **string** (ví dụ: "Cash", "Card", "EWallet")

---

## Luồng Nghiệp Vụ Tổng Quan {#luồng-nghiệp-vụ}

### Luồng Thuê Xe Hoàn Chỉnh

```
1. Khách hàng xem danh sách xe
   GET /api/vehicles?status=Available&type=Car
   → Hiển thị các xe Available

2. Khách hàng chọn xe (Vehicle có status = "Available")
   → Hiển thị thông tin chi tiết: battery, station, image

3. Staff bắt đầu thuê cho khách
   POST /api/rentals/start
   {
     "vehicleId": "...",
     "renterId": "...",
     "stationId": "...",
     "staffId": "..."
   }
   → Vehicle.Status: "Available" → "InUse"
   → Tạo Rental với Status: "Ongoing"

4. Khách hàng sử dụng xe
   → Rental.Status: "Ongoing"
   → Vehicle.Status: "InUse"

5. Khách hàng trả xe
   POST /api/rentals/{id}/complete
   {
     "endStationId": "...",
     "finalBatteryLevel": 75,
     "notes": "..."
   }
   → Rental.Status: "Ongoing" → "Completed"
   → Vehicle.Status: "InUse" → "Available" (nếu battery ≥ 10%)
   → Vehicle.Status: "InUse" → "Maintenance" (nếu battery < 10%)
   → Tính toán totalCost

6. Thanh toán (nếu có)
   → Sử dụng Payment API với PaymentMethod
```

### Luồng Quản Lý Xe

```
1. Staff/Admin xem danh sách xe
   GET /api/vehicles?status=Maintenance
   → Hiển thị các xe cần bảo trì

2. Staff cập nhật pin xe
   PATCH /api/vehicles/{id}/battery
   {
     "batteryLevel": 85
   }
   → Nếu battery ≥ 10% và xe đang Maintenance → chuyển sang Available
   → Nếu battery < 10% và xe đang Available → chuyển sang Maintenance

3. Staff thay đổi trạng thái thủ công (nếu cần)
   PATCH /api/vehicles/{id}/status
   {
     "status": "Maintenance"
   }
   → Lưu ý: Không thể chuyển InUse → Available nếu còn rental Ongoing
```

---

## Best Practices cho Android Dev {#best-practices}

### 1. Xử Lý Enum Trong Android

**Kotlin Data Class:**
```kotlin
enum class VehicleStatus(val value: String) {
    AVAILABLE("Available"),
    BOOKED("Booked"),
    IN_USE("InUse"),
    MAINTENANCE("Maintenance")
}

enum class RentalStatus(val value: String) {
    BOOKED("Booked"),
    ONGOING("Ongoing"),
    COMPLETED("Completed"),
    CANCELLED("Cancelled")
}

enum class VehicleType(val value: String) {
    SCOOTER("Scooter"),
    CAR("Car"),
    OTHER("Other")
}
```

**Gson Converter (nếu dùng Gson):**
```kotlin
class VehicleStatusDeserializer : JsonDeserializer<VehicleStatus> {
    override fun deserialize(
        json: JsonElement?,
        typeOfT: Type?,
        context: JsonDeserializationContext?
    ): VehicleStatus {
        val value = json?.asString
        return VehicleStatus.values().find { it.value == value }
            ?: throw IllegalArgumentException("Unknown VehicleStatus: $value")
    }
}
```

### 2. UI/UX Recommendations

#### Danh Sách Xe
- **Available:** Hiển thị màu xanh lá, có nút "Thuê ngay"
- **InUse:** Hiển thị màu xám, disable nút thuê
- **Maintenance:** Hiển thị màu đỏ, có icon bảo trì
- **Booked:** Hiển thị màu vàng (nếu có)

#### Filter & Sort
- Luôn filter `status=Available` khi khách hàng xem danh sách
- Cho phép staff/admin xem tất cả status
- Sort theo `batteryLevel` để ưu tiên xe pin cao

#### Validation Trước Khi Gọi API
```kotlin
fun canStartRental(vehicle: VehicleResponse, user: User): Boolean {
    return vehicle.status == VehicleStatus.AVAILABLE &&
           vehicle.batteryLevel >= 10 &&
           user.status == UserStatus.ACTIVE &&
           user.isVerified
}
```

### 3. Error Handling

**Các lỗi thường gặp:**
```kotlin
// Vehicle.NotAvailable
if (error.code == "Vehicle.NotAvailable") {
    showError("Xe không khả dụng. Vui lòng chọn xe khác.")
}

// Vehicle.LowBattery
if (error.code == "Vehicle.LowBattery") {
    showError("Pin xe quá thấp. Vui lòng chọn xe khác.")
}

// User.NotEligible
if (error.code == "User.NotEligible") {
    showError("Tài khoản của bạn chưa được xác thực hoặc đã bị vô hiệu hóa.")
}

// Vehicle.HasActiveRental
if (error.code == "Vehicle.HasActiveRental") {
    showError("Không thể thay đổi trạng thái vì xe đang có rental đang hoạt động.")
}
```

### 4. Real-time Updates (Nếu cần)

- **Polling:** Refresh danh sách xe mỗi 30 giây khi ở màn hình danh sách
- **Socket.io/WebSocket:** (Nếu backend hỗ trợ) Subscribe để nhận thông báo khi status thay đổi

### 5. Local State Management

```kotlin
// Lưu trạng thái rental đang active
class RentalRepository {
    private var activeRental: Rental? = null
    
    fun getActiveRental(): Rental? = activeRental
    
    fun startRental(rental: Rental) {
        activeRental = rental
        // Update vehicle status trong local cache
        vehicleRepository.updateStatus(rental.vehicleId, VehicleStatus.IN_USE)
    }
    
    fun completeRental() {
        activeRental = null
    }
}
```

### 6. Testing Scenarios

#### Test Cases Quan Trọng:
1. ✅ Thuê xe thành công (Available → InUse)
2. ✅ Thuê xe thất bại (xe đã InUse)
3. ✅ Thuê xe thất bại (pin < 10%)
4. ✅ Hoàn tất thuê (InUse → Available)
5. ✅ Hoàn tất thuê với pin thấp (InUse → Maintenance)
6. ✅ Cập nhật pin tự động chuyển status
7. ✅ Không thể chuyển InUse → Available nếu còn rental Ongoing

---

## Tóm Tắt Nhanh

| Enum | Giá Trị | Trả Về Dạng | Luồng Chính |
|------|---------|-------------|-------------|
| **VehicleStatus** | Available, Booked, InUse, Maintenance | **String** | Available → InUse → Available/Maintenance |
| **RentalStatus** | Booked, Ongoing, Completed, Cancelled | **String** | Ongoing → Completed |
| **VehicleType** | Scooter, Car, Other | **String** | Dùng để filter và tính giá |
| **UserStatus** | Active, Inactive | - | Phải Active để thuê xe |
| **UserRole** | Admin, Staff, Renter | - | Phân quyền |
| **PaymentMethod** | Cash, Card, EWallet | **String** | Thanh toán |

---

**Lưu ý quan trọng:**
- ✅ API trả về enum dưới dạng **string**, không phải số
- ✅ `VehicleStatus.Booked` và `RentalStatus.Cancelled` hiện chưa được implement
- ✅ Status tự động chuyển đổi khi bắt đầu/hoàn tất rental
- ✅ Pin < 10% tự động chuyển xe sang Maintenance

