# PRM Car Rental - Hướng Dẫn Phát Triển Android (Phiên Bản Rút Gọn)

> **Timeline**: 5 ngày | **Team**: 3 developers | **Luồng Core**: `[Đăng ký → Đặt xe → Nhận xe → Trả xe → Thanh toán]`

---

## 📱 Tổng Quan

**PRM Car Rental** - Hệ thống cho thuê xe điện (EV)

**3 vai trò**:
- **EV Renter (Người thuê)**: ⭐ **Flow chính** - Ưu tiên phát triển
- **Station Staff (Nhân viên trạm)**: ⚠️ Simplified - Chỉ core features
- **Admin**: ❌ Skip trong phase này

---

## 🔑 Luồng Nghiệp Vụ Chính

```
Đăng ký → Đặt xe → Nhận xe (Staff) → Sử dụng → Trả xe (Staff) → Thanh toán → Lịch sử
```

---

## 🔧 Technical Specifications

### Base URL
- **Development**: `http://localhost:5000/api`
- **Production**: `https://api.prm-carrental.com/api`

### Authentication
- **Type**: JWT Bearer Token
- **Header**: `Authorization: Bearer {token}`
- **Storage**: SharedPreferences (không cần Encrypted trong 5 ngày)
- ❌ Bỏ refresh token (không cần)

### API Response Format
```json
{
  "isSuccess": true,
  "value": { /* data */ },
  "errors": [
    {
      "code": "Error.Code",
      "message": "Error message"
    }
  ]
}
```

### Error Codes
- **200 OK**: Success
- **201 Created**: Created resource
- **400 Bad Request**: Validation errors
- **401 Unauthorized**: Token invalid → redirect to Login
- **403 Forbidden**: Không có quyền
- **404 Not Found**: Resource not found
- **500**: Server error

### Pagination
Format: `?pageNumber=1&pageSize=20`

Response:
```json
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 100,
  "totalPages": 5
}
```

---

## 👤 EV RENTER - Core Features

### 1. Đăng ký & Đăng nhập

#### 1.1. Đăng ký
**Màn hình**: `RegisterActivity`

**API**: `POST /api/auth/register`

**Request**:
```json
{
  "fullName": "Nguyễn Văn A",
  "email": "nguyenvana@example.com",
  "password": "SecurePass123",
  "driverLicenseNumber": "DL123456",
  "idCardNumber": "123456789012"
}
```

**Response (201)**:
```json
{
  "isSuccess": true,
  "value": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "nguyenvana@example.com",
    "fullName": "Nguyễn Văn A"
  }
}
```

**Note**: Không upload ảnh, chỉ nhập text.

---

#### 1.2. Đăng nhập
**Màn hình**: `LoginActivity`

**API**: `POST /api/auth/login`

**Request**:
```json
{
  "email": "nguyenvana@example.com",
  "password": "SecurePass123"
}
```

**Response (200)**:
```json
{
  "isSuccess": true,
  "value": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "...",
      "fullName": "Nguyễn Văn A",
      "email": "...",
      "role": "Renter",
      "status": "Active",
      "isVerified": true
    }
  }
}
```

**Xử lý**: 
- Lưu token vào SharedPreferences
- Lưu user info
- Nếu `isVerified = false` → hiển thị banner "Chờ xác thực"
- Navigate đến HomeActivity

---

#### 1.3. Xem Profile
**Màn hình**: `ProfileFragment`

**API**: `GET /api/users/{userId}`

**Headers**: `Authorization: Bearer {token}`

**Response**:
```json
{
  "isSuccess": true,
  "value": {
    "id": "...",
    "fullName": "Nguyễn Văn A",
    "email": "...",
    "status": "Active",
    "isVerified": true,
    "driverLicenseNumber": "DL123456",
    "idCardNumber": "123456789012"
  }
}
```

**UI**: Nếu `isVerified = false` → hiển thị badge "Chờ xác thực", disable nút "Đặt xe"

---

### 2. Tìm Trạm & Xe

#### 2.1. Xem danh sách trạm
**Màn hình**: `StationListFragment`

**API**: `GET /api/stations?pageNumber=1&pageSize=50`

**Response**:
```json
{
  "isSuccess": true,
  "value": {
    "items": [
      {
        "id": "station-id-1",
        "name": "Trạm Quận 1",
        "address": "123 Nguyễn Huệ, Quận 1, TP.HCM",
        "latitude": 10.7769,
        "longitude": 106.7009,
        "availableVehiclesCount": 5
      }
    ],
    "pageNumber": 1,
    "pageSize": 50,
    "totalCount": 2
  }
}
```

**UI**: List view đơn giản (không Map). Click → `StationDetailActivity`

---

#### 2.2. Xem chi tiết trạm và xe
**Màn hình**: `StationDetailActivity` + `VehicleListFragment`

**API 1**: `GET /api/stations/{stationId}`

**Response**:
```json
{
  "isSuccess": true,
  "value": {
    "id": "station-id-1",
    "name": "Trạm Quận 1",
    "address": "123 Nguyễn Huệ, Quận 1",
    "latitude": 10.7769,
    "longitude": 106.7009,
    "availableVehiclesCount": 5
  }
}
```

**API 2**: `GET /api/stations/{stationId}/vehicles?pageNumber=1&pageSize=20`

**Response**:
```json
{
  "isSuccess": true,
  "value": {
    "items": [
      {
        "id": "vehicle-id-1",
        "plateNumber": "30A-12345",
        "type": "Car",
        "status": "Available",
        "batteryLevel": 85,
        "stationId": "station-id-1",
        "stationName": "Trạm Quận 1"
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 5
  }
}
```

**UI**: List view, hiển thị: Biển số, Loại xe, Pin (%), Status. Click → `VehicleDetailActivity`

---

#### 2.3. Đặt xe
**Màn hình**: `VehicleDetailActivity`

**API 1**: `GET /api/vehicles/{vehicleId}` (để hiển thị thông tin)

**API 2**: `POST /api/rentals/start`

**Headers**: `Authorization: Bearer {token}`

**Request**:
```json
{
  "vehicleId": "vehicle-id-1",
  "renterId": "user-id-from-token",
  "stationId": "station-id-1",
  "staffId": "staff-id"
}
```

**Response (201)**:
```json
{
  "isSuccess": true,
  "value": {
    "rentalId": "rental-id-123",
    "vehicleId": "vehicle-id-1",
    "plateNumber": "30A-12345",
    "startTime": "2024-10-28T10:00:00Z",
    "status": "Active"
  }
}
```

**Error Cases**:
- `Vehicle.NotAvailable` - Xe không còn trống
- `Vehicle.LowBattery` - Pin xe quá thấp (< 10%)
- `User.NotVerified` - Tài khoản chưa được xác thực

**Flow**: Hiển thị thông tin → Confirm dialog → Call API → Success → `ActiveRentalActivity`

---

### 3. Sử dụng & Trả Xe

#### 3.1. Xem rental đang active
**Màn hình**: `ActiveRentalActivity`

**API**: `GET /api/rentals/{rentalId}`

**Headers**: `Authorization: Bearer {token}`

**Response**:
```json
{
  "isSuccess": true,
  "value": {
    "id": "rental-id-123",
    "vehicleId": "vehicle-id-1",
    "plateNumber": "30A-12345",
    "renterName": "Nguyễn Văn A",
    "stationName": "Trạm Quận 1",
    "startTime": "2024-10-28T10:00:00Z",
    "endTime": null,
    "totalCost": 0,
    "status": "Active"
  }
}
```

**UI**: Hiển thị thông tin xe, timer đếm thời gian, nút "Trả xe" (chỉ hiển thị thông báo)

---

#### 3.2. Xem lịch sử thuê
**Màn hình**: `RentalHistoryFragment`

**API**: `GET /api/rentals?userId={userId}&status={status}&pageNumber=1&pageSize=20`

**Headers**: `Authorization: Bearer {token}`

**Query Parameters**:
- `userId` (optional - lấy từ token nếu không có)
- `status` (optional) - `Ongoing`, `Completed`, `Cancelled`
- `pageNumber`, `pageSize`

**Response**:
```json
{
  "isSuccess": true,
  "value": {
    "items": [
      {
        "id": "rental-id-123",
        "plateNumber": "30A-12345",
        "stationName": "Trạm Quận 1",
        "startTime": "2024-10-28T10:00:00Z",
        "endTime": "2024-10-28T15:30:00Z",
        "totalCost": 150000,
        "status": "Completed"
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 1
  }
}
```

**UI**: List đơn giản. Click → `RentalDetailActivity`

---

### 4. Thanh Toán

#### 4.1. Thanh toán
**Màn hình**: `PaymentActivity`

**API**: `POST /api/payments`

**Headers**: `Authorization: Bearer {token}`

**Request**:
```json
{
  "rentalId": "rental-id-123",
  "amount": 150000,
  "paymentMethod": "Cash"
}
```

**PaymentMethod enum**: `Cash`, `Card`, `MobileMoney`, `Bank`

**Response (201)**:
```json
{
  "isSuccess": true,
  "value": {
    "id": "payment-id-456",
    "rentalId": "rental-id-123",
    "amount": 150000,
    "paymentMethod": "Cash",
    "paidTime": "2024-10-28T15:30:00Z"
  }
}
```

**Flow**: Hiển thị hóa đơn → Chọn phương thức → Xác nhận → Success → `PaymentSuccessActivity` → Back to History

---

## 👨‍💼 STATION STAFF - Simplified

### 1. Xác thực Người Thuê

#### 1.1. Xem users chờ xác thực
**Màn hình**: `PendingVerificationListActivity`

**API**: `GET /api/users?pageNumber=1&pageSize=20`

**Headers**: `Authorization: Bearer {token}` (Staff role)

**Response**: List users, filter client-side `isVerified=false`

**UI**: List đơn giản. Click → `UserVerificationDetailActivity`

---

#### 1.2. Xác thực user
**Màn hình**: `UserVerificationDetailActivity`

**API**: `PUT /api/users/{userId}`

**Headers**: `Authorization: Bearer {token}` (Staff role)

**Request**:
```json
{
  "fullName": "Nguyễn Văn A",
  "isVerified": true
}
```

**Response**: UserDto với `isVerified = true`

**UI**: Xem thông tin → Nút "Chấp nhận" → Call API

---

### 2. Quản Lý Xe

#### 2.1. Xem xe tại trạm
**Màn hình**: `StationVehiclesActivity`

**API**: `GET /api/stations/{stationId}/vehicles?pageNumber=1&pageSize=50`

**Response**: Tương tự Use Case 2.2

**UI**: List đơn giản, chỉ xem

---

#### 2.2. Cập nhật pin
**Màn hình**: `VehicleManagementActivity`

**API**: `PATCH /api/vehicles/{vehicleId}/battery`

**Headers**: `Authorization: Bearer {token}` (Staff role)

**Request**:
```json
{
  "batteryLevel": 85
}
```

**Response**:
```json
{
  "isSuccess": true,
  "value": {
    "id": "vehicle-id-1",
    "plateNumber": "30A-12345",
    "type": "Car",
    "status": "Available",
    "batteryLevel": 85,
    "stationId": "station-id-1",
    "stationName": "Trạm Quận 1"
  }
}
```

**Note**: Tự động mark Maintenance nếu battery < 10%

---

#### 2.3. Thay đổi trạng thái xe
**Màn hình**: `VehicleManagementActivity`

**API**: `PATCH /api/vehicles/{vehicleId}/status`

**Headers**: `Authorization: Bearer {token}` (Staff role)

**Request**:
```json
{
  "status": "Available"
}
```

**Status enum**: `Available`, `Booked`, `InUse`, `Maintenance`

**Response**: Tương tự UpdateBattery

**Note**: Không cho chuyển InUse → Available nếu có active rental

---

### 3. Nhận Xe Trả Về

#### 3.1. Nhận xe trả về
**Màn hình**: `ReceiveVehicleActivity`

**API**: `POST /api/rentals/{rentalId}/complete`

**Headers**: `Authorization: Bearer {token}` (Staff role)

**Request**:
```json
{
  "endStationId": "station-id-1",
  "finalBatteryLevel": 45,
  "notes": "Xe không có thiệt hại"
}
```

**Response**:
```json
{
  "isSuccess": true,
  "value": {
    "rentalId": "rental-id-123",
    "endTime": "2024-10-28T15:30:00Z",
    "totalCost": 150000,
    "status": "Completed"
  }
}
```

**Business Logic**:
- Tính cost tự động: Car 50k/h, Scooter 30k/h, Other 20k/h
- Update vehicle status to Available
- Update vehicle battery
- Move vehicle to endStationId
- Set rental status = Completed

**UI**: Input Rental ID → Input pin → Textarea ghi chú → Xác nhận

---

## 📋 PHÂN CHIA CÔNG VIỆC

### Module 1: Authentication & Discovery

**Tasks**:
- [ ] `LoginActivity`, `RegisterActivity`
- [ ] `ProfileFragment` (chỉ xem)
- [ ] Token management (SharedPreferences)
- [ ] `StationListFragment`, `StationDetailActivity`
- [ ] `VehicleListFragment`, `VehicleDetailActivity`
- [ ] API Services: `AuthService`, `UserService`, `StationService`, `VehicleService`

**APIs sử dụng**:
- `POST /api/auth/login`
- `POST /api/auth/register`
- `GET /api/users/{id}`
- `GET /api/stations`
- `GET /api/stations/{id}`
- `GET /api/stations/{id}/vehicles`
- `GET /api/vehicles/{id}`

**Deliverable**: User có thể đăng ký, đăng nhập, xem trạm, xem xe, đặt xe

---

### Module 2: Booking & Payment

**Tasks**:
- [ ] `ActiveRentalActivity`
- [ ] `RentalHistoryFragment`, `RentalDetailActivity`
- [ ] `PaymentActivity`, `PaymentSuccessActivity`
- [ ] API Services: `RentalService`, `PaymentService`

**APIs sử dụng**:
- `POST /api/rentals/start`
- `GET /api/rentals/{id}`
- `GET /api/rentals?userId={id}`
- `POST /api/payments`
- `GET /api/payments/{id}`

**Deliverable**: User có thể đặt xe, xem rental, xem lịch sử, thanh toán

---

### Module 3: Staff Features

**Tasks**:
- [ ] `PendingVerificationListActivity`, `UserVerificationDetailActivity`
- [ ] `StationVehiclesActivity`, `VehicleManagementActivity`
- [ ] `ReceiveVehicleActivity`
- [ ] API Service: `StaffService`

**APIs sử dụng**:
- `GET /api/users` (filter client-side)
- `PUT /api/users/{id}` (verify với `isVerified=true`)
- `GET /api/stations/{id}/vehicles`
- `PATCH /api/vehicles/{id}/battery`
- `PATCH /api/vehicles/{id}/status`
- `POST /api/rentals/{id}/complete`

**Deliverable**: Staff có thể verify user, quản lý xe, nhận xe trả về

---

## 📱 Màn Hình Cần Có

### Renter (12 màn hình):
1. `LoginActivity`
2. `RegisterActivity`
3. `HomeActivity` (Bottom Nav: Stations, Rentals, Profile)
4. `StationListFragment`
5. `StationDetailActivity` + `VehicleListFragment`
6. `VehicleDetailActivity`
7. `ActiveRentalActivity`
8. `RentalHistoryFragment`
9. `RentalDetailActivity`
10. `PaymentActivity`
11. `PaymentSuccessActivity`
12. `ProfileFragment`

### Staff (6 màn hình):
1. `StaffHomeActivity` (Bottom Nav)
2. `PendingVerificationListActivity`
3. `UserVerificationDetailActivity`
4. `StationVehiclesActivity`
5. `VehicleManagementActivity`
6. `ReceiveVehicleActivity`

**Tổng**: 18 màn hình

---

## 📦 Dependencies (Minimal)

```gradle
// Networking
implementation 'com.squareup.retrofit2:retrofit:2.9.0'
implementation 'com.squareup.retrofit2:converter-gson:2.9.0'
implementation 'com.squareup.okhttp3:logging-interceptor:4.11.0'

// Image Loading (optional)
implementation 'com.github.bumptech.glide:glide:4.16.0'

// Coroutines
implementation 'org.jetbrains.kotlinx:kotlinx-coroutines-android:1.7.3'

// ViewModel
implementation 'androidx.lifecycle:lifecycle-viewmodel-ktx:2.6.2'
```

**Bỏ**: Maps, QR Scanner, Hilt/Dagger, DataStore (dùng SharedPreferences)

---

## ✅ Checklist Hoàn Thành

### Day 1-2 (Developer 1)
- [ ] Login/Register hoạt động
- [ ] Hiển thị list stations
- [ ] Hiển thị vehicles tại station
- [ ] Có thể đặt xe thành công

### Day 3-4 (Developer 2)
- [ ] Xem rental đang active
- [ ] Xem lịch sử rentals
- [ ] Thanh toán hoạt động

### Day 5 (Developer 3 + Testing)
- [ ] Staff verify user
- [ ] Staff quản lý xe (pin/status)
- [ ] Staff nhận xe trả về
- [ ] Test toàn bộ flow

---

## 🎯 Mục Tiêu Cuối Cùng

**Sau 5 ngày, app phải có thể**:
1. ✅ User đăng ký/đăng nhập
2. ✅ User xem trạm và xe
3. ✅ User đặt xe
4. ✅ User xem rental đang active
5. ✅ User xem lịch sử và thanh toán
6. ✅ Staff verify user
7. ✅ Staff quản lý xe (pin/status)
8. ✅ Staff nhận xe trả về

**Flow hoàn chỉnh**: Đăng ký → Đặt xe → Nhận xe (Staff) → Trả xe (Staff) → Thanh toán

---

## 💡 Tips

1. **Reuse Components**: Base Activity/Fragment để reuse
2. **Simple UI**: Material Design cơ bản, không fancy animations
3. **Error Handling**: Toast đơn giản
4. **Testing**: Test trên 1 device, 1 user flow đầy đủ
5. **Communication**: Daily standup để sync

---

## 🚫 Đã Bỏ Qua (Có thể làm sau)

1. Map View
2. QR Code Scanner
3. Upload ảnh
4. Admin Features
5. Charts/Reports
6. Real-time updates
7. VnPay Integration
8. Chụp ảnh xe
9. Filter/Search phức tạp
10. Notification

---

**Document Version**: 3.0 (Optimized - Self-contained)  
**Last Updated**: October 2024  
**Timeline**: 5 ngày, 3 developers
