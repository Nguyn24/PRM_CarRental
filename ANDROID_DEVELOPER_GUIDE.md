# PRM Car Rental - Hướng Dẫn Phát Triển Android

## 📱 Tổng Quan Hệ Thống

**PRM Car Rental** là hệ thống cho thuê xe điện (EV) với 3 vai trò chính:
- **EV Renter (Người thuê)**: Khách hàng thuê xe
- **Station Staff (Nhân viên trạm)**: Nhân viên quản lý tại trạm thuê xe
- **Admin (Quản trị viên)**: Quản lý toàn hệ thống

---

## 🔑 Luồng Nghiệp Vụ Chính

### Flow Tổng Quan:
```
[Đăng ký → Xác thực → Đặt xe → Nhận xe → Sử dụng → Trả xe → Thanh toán → Lịch sử]
```

---

## 👤 1. EV RENTER (Người Thuê)

### 1.1. Đăng ký & Xác thực

#### Use Case 1.1.1: Đăng ký tài khoản
**Màn hình**: `RegisterActivity` hoặc `RegisterFragment`

**API Endpoint**: `POST /api/auth/register`

**Request Body**:
```json
{
  "fullName": "Nguyễn Văn A",
  "email": "nguyenvana@example.com",
  "password": "SecurePass123",
  "driverLicenseNumber": "DL123456",  // Optional
  "idCardNumber": "123456789012"      // Optional
}
```

**Response (201 Created)**:
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

**Error Response (400 Bad Request)**:
```json
{
  "isSuccess": false,
  "errors": [
    {
      "code": "User.EmailExists",
      "message": "Email đã tồn tại trong hệ thống"
    }
  ]
}
```

**Xử lý UI**:
- Hiển thị form đăng ký với validation
- Upload ảnh giấy phép lái xe và CMND (nếu có)
- Lưu ảnh và gửi URL trong request (hoặc upload riêng qua API khác)
- Sau khi đăng ký thành công → chuyển đến màn hình "Chờ xác thực"

---

#### Use Case 1.1.2: Đăng nhập
**Màn hình**: `LoginActivity`

**API Endpoint**: `POST /api/auth/login`

**Request Body**:
```json
{
  "email": "nguyenvana@example.com",
  "password": "SecurePass123"
}
```

**Response (200 OK)**:
```json
{
  "isSuccess": true,
  "value": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "refresh_token_string",
    "expiresIn": 3600,
    "user": {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "fullName": "Nguyễn Văn A",
      "email": "nguyenvana@example.com",
      "role": "Renter",
      "status": "Active",
      "isVerified": true,
      "avatarUrl": "https://..."
    }
  }
}
```

**Xử lý**:
- Lưu token vào SharedPreferences hoặc DataStore
- Lưu user info để hiển thị trên UI
- Nếu `isVerified = false` → hiển thị banner "Tài khoản đang chờ xác thực"
- Navigate đến HomeActivity nếu đăng nhập thành công

---

#### Use Case 1.1.3: Upload giấy tờ (Giấy phép lái xe/CMND)
**Màn hình**: `VerificationActivity`

**API Endpoint**: `PUT /api/users/{userId}`

**Request Body**:
```json
{
  "fullName": "Nguyễn Văn A",
  "avatarUrl": "https://storage.example.com/avatars/user123.jpg",
  "driverLicenseNumber": "DL123456",
  "idCardNumber": "123456789012"
}
```

**Headers**: `Authorization: Bearer {token}`

**Xử lý**:
- Cho phép chụp ảnh hoặc chọn từ gallery
- Upload ảnh lên storage (có thể cần API riêng)
- Lưu URL vào `avatarUrl`, `driverLicenseNumber`, `idCardNumber`
- Hiển thị trạng thái: "Đã gửi, chờ nhân viên xác thực"

---

#### Use Case 1.1.4: Kiểm tra trạng thái xác thực
**Màn hình**: `ProfileFragment`

**API Endpoint**: `GET /api/users/{userId}`

**Response**:
```json
{
  "isSuccess": true,
  "value": {
    "id": "...",
    "fullName": "Nguyễn Văn A",
    "email": "...",
    "status": "Active",
    "isVerified": true,  // true = đã được xác thực
    "driverLicenseNumber": "DL123456",
    "idCardNumber": "123456789012"
  }
}
```

**UI Logic**:
- Nếu `isVerified = false`: Hiển thị badge "Chờ xác thực", disable nút "Đặt xe"
- Nếu `isVerified = true`: Cho phép đặt xe

---

### 1.2. Đặt & Nhận Xe

#### Use Case 1.2.1: Xem danh sách trạm (Map/List View)
**Màn hình**: `StationMapActivity` hoặc `StationListFragment`

**API Endpoint**: `GET /api/stations?pageNumber=1&pageSize=50`

**Response (200 OK)**:
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
      },
      {
        "id": "station-id-2",
        "name": "Trạm Quận 3",
        "address": "456 Lê Văn Sỹ, Quận 3, TP.HCM",
        "latitude": 10.7820,
        "longitude": 106.6932,
        "availableVehiclesCount": 3
      }
    ],
    "pageNumber": 1,
    "pageSize": 50,
    "totalCount": 2,
    "totalPages": 1
  }
}
```

**UI Implementation**:
- **Map View**: Hiển thị markers trên Google Maps tại `latitude`, `longitude`
- **List View**: Hiển thị danh sách với `availableVehiclesCount`
- Click vào station → navigate đến `StationDetailActivity`

---

#### Use Case 1.2.2: Xem chi tiết trạm và xe có sẵn
**Màn hình**: `StationDetailActivity`

**API Endpoint**: `GET /api/stations/{stationId}`

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

**API Endpoint**: `GET /api/stations/{stationId}/vehicles?pageNumber=1&pageSize=20`

**Response**:
```json
{
  "isSuccess": true,
  "value": {
    "items": [
      {
        "id": "vehicle-id-1",
        "plateNumber": "30A-12345",
        "type": "Car",  // Car, Motorcycle, Bicycle
        "status": "Available",
        "batteryLevel": 85,
        "stationId": "station-id-1",
        "stationName": "Trạm Quận 1"
      },
      {
        "id": "vehicle-id-2",
        "plateNumber": "30B-67890",
        "type": "Motorcycle",
        "status": "Available",
        "batteryLevel": 92,
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

**UI Display**:
- Hiển thị thông tin trạm
- List hoặc Grid các xe có sẵn (chỉ hiển thị `status = "Available"`)
- Mỗi xe hiển thị: Biển số, Loại xe, Pin (%), Icon loại xe
- Click vào xe → `VehicleDetailActivity`

---

#### Use Case 1.2.3: Chọn xe và đặt xe
**Màn hình**: `VehicleDetailActivity` → `ConfirmBookingActivity`

**API Endpoint**: `GET /api/vehicles/{vehicleId}`

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

**Đặt xe**: `POST /api/rentals/start`

**Request Body**:
```json
{
  "vehicleId": "vehicle-id-1",
  "renterId": "user-id-from-token",  // Lấy từ token đã lưu
  "stationId": "station-id-1",
  "staffId": "staff-id"  // Tạm thời có thể để null hoặc lấy từ station
}
```

**Headers**: `Authorization: Bearer {token}`

**Response (201 Created)**:
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
```json
{
  "isSuccess": false,
  "errors": [
    {
      "code": "Vehicle.NotAvailable",
      "message": "Xe không còn trống"
    },
    {
      "code": "Vehicle.LowBattery",
      "message": "Pin xe quá thấp (< 10%)"
    },
    {
      "code": "User.NotVerified",
      "message": "Tài khoản chưa được xác thực"
    }
  ]
}
```

**UI Flow**:
1. Hiển thị thông tin xe (biển số, loại, pin, trạm)
2. Nút "Đặt xe" → Confirm dialog
3. Sau khi confirm → gọi API
4. Nếu thành công → navigate đến `ActiveRentalActivity`
5. Nếu lỗi → hiển thị error message

---

#### Use Case 1.2.4: Đến trạm và nhận xe (Phối hợp với Staff)
**Màn hình**: `ActiveRentalActivity` hoặc `RentalQRCodeActivity`

**Luồng**:
1. Renter đặt xe thành công → có `rentalId`
2. Đến trạm → Hiển thị QR Code hoặc Rental ID cho Staff quét
3. Staff kiểm tra giấy tờ và bàn giao xe (xem phần Staff Use Case)
4. Renter nhận xe → `RentalInProgressActivity`

**API Endpoint**: `GET /api/rentals/{rentalId}` (để check status)

**Response**:
```json
{
  "isSuccess": true,
  "value": {
    "id": "rental-id-123",
    "vehicleId": "vehicle-id-1",
    "plateNumber": "30A-12345",
    "renterId": "user-id",
    "renterName": "Nguyễn Văn A",
    "stationId": "station-id-1",
    "stationName": "Trạm Quận 1",
    "startTime": "2024-10-28T10:00:00Z",
    "endTime": null,
    "totalCost": 0,
    "status": "Active"  // Active = đã nhận xe và đang sử dụng
  }
}
```

**UI Display**:
- Hiển thị thông tin xe đang thuê
- Timer đếm thời gian đã sử dụng
- Nút "Xem vị trí trạm trả xe"
- Nút "Trả xe" (chỉ hiện khi đã nhận xe)

---

### 1.3. Sử dụng & Trả Xe

#### Use Case 1.3.1: Sử dụng xe (Tracking)
**Màn hình**: `RentalInProgressActivity`

**Features**:
- Hiển thị timer từ `startTime` đến hiện tại
- Hiển thị thông tin xe: Biển số, loại, pin hiện tại (có thể poll API)
- Map view với vị trí trạm trả xe
- Nút "Báo cáo sự cố"
- Nút "Trả xe"

**Polling**: Có thể gọi `GET /api/rentals/{rentalId}` mỗi 30 giây để update thông tin

---

#### Use Case 1.3.2: Trả xe tại trạm
**Màn hình**: `ReturnVehicleActivity`

**Luồng**:
1. Renter đến trạm → Quét QR hoặc nhập mã rental
2. Staff kiểm tra tình trạng xe (xem phần Staff)
3. Hệ thống tính phí tự động khi Complete Rental
4. Renter thanh toán (xem Use Case 1.4)

**Note**: Việc trả xe thường do Staff thực hiện qua `CompleteRentalCommand` (chưa có API này, cần implement)

---

### 1.4. Thanh Toán

#### Use Case 1.4.1: Thanh toán sau khi trả xe
**Màn hình**: `PaymentActivity`

**API Endpoint**: `POST /api/payments`

**Request Body**:
```json
{
  "rentalId": "rental-id-123",
  "amount": 150000,  // Từ TotalCost trong Rental
  "paymentMethod": "Cash"  // Cash, Card, MobileMoney, Bank
}
```

**Headers**: `Authorization: Bearer {token}`

**Response (201 Created)**:
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

**UI Flow**:
1. Hiển thị thông tin hóa đơn: Tổng tiền, Thời gian thuê, Loại xe
2. Chọn phương thức thanh toán
3. Xác nhận thanh toán
4. Sau khi thành công → `PaymentSuccessActivity` → `RentalHistoryActivity`

---

#### Use Case 1.4.2: Xem lịch sử thuê
**Màn hình**: `RentalHistoryFragment`

**API Endpoint**: (Chưa có, cần implement `GetUserRentalsQuery`)
- Tạm thời có thể dùng: `GET /api/rentals/{rentalId}` cho từng rental

**Response mong đợi**:
```json
{
  "isSuccess": true,
  "value": {
    "items": [
      {
        "id": "rental-id-123",
        "vehicleId": "vehicle-id-1",
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

**UI Display**:
- List các rental đã hoàn thành
- Hiển thị: Ngày, Trạm, Biển số xe, Tổng tiền, Trạng thái
- Click vào item → `RentalDetailActivity`

---

## 👨‍💼 2. STATION STAFF (Nhân Viên Trạm)

### 2.1. Xác thực Người Thuê

#### Use Case 2.1.1: Xem danh sách người thuê chờ xác thực
**Màn hình**: `PendingVerificationListActivity`

**API Endpoint**: (Chưa có, cần implement `GetUsersByStatusQuery`)
- Tạm thời: `GET /api/users?pageNumber=1&pageSize=20`

**Filter**: Lọc những user có `isVerified = false`

**UI**:
- List users với `isVerified = false`
- Hiển thị: Tên, Email, Driver License, ID Card (nếu đã upload)
- Click vào → `UserVerificationDetailActivity`

---

#### Use Case 2.1.2: Xác thực giấy tờ người thuê
**Màn hình**: `UserVerificationDetailActivity`

**Actions**:
1. Xem thông tin user: FullName, Email, Avatar, Driver License, ID Card
2. Nút "Chấp nhận" → Gọi API update `isVerified = true`
3. Nút "Từ chối" → Gửi thông báo lý do

**API Endpoint**: (Chưa có, cần implement `VerifyUserCommand` hoặc `UpdateUserCommand` với `isVerified`)

**Tạm thời**: Có thể dùng `PUT /api/users/{userId}` với thêm field `isVerified`

---

### 2.2. Quản Lý Xe tại Trạm

#### Use Case 2.2.1: Xem danh sách xe tại trạm
**Màn hình**: `StationVehiclesActivity` (dành cho Staff)

**API Endpoint**: `GET /api/stations/{stationId}/vehicles?pageNumber=1&pageSize=50`

**Response**: Xem Use Case 1.2.2

**UI Features**:
- Hiển thị tất cả xe (Available, InUse, Maintenance, Reserved)
- Filter theo Status và Type
- Mỗi xe hiển thị: Biển số, Loại, Pin, Trạng thái
- Click vào xe → `VehicleManagementActivity`

---

#### Use Case 2.2.2: Cập nhật pin và trạng thái xe
**Màn hình**: `VehicleManagementActivity`

**API Endpoint**: (Chưa có, cần implement)
- `PATCH /api/vehicles/{vehicleId}/battery` - Cập nhật pin
- `PATCH /api/vehicles/{vehicleId}/status` - Thay đổi trạng thái

**Request Body (Battery)**:
```json
{
  "batteryLevel": 85  // 0-100
}
```

**Request Body (Status)**:
```json
{
  "status": "Available"  // Available, InUse, Maintenance, Reserved
}
```

**UI**:
- Input số pin, slider 0-100
- Dropdown chọn trạng thái
- Lưu → gọi API update

---

### 2.3. Bàn Giao & Nhận Xe

#### Use Case 2.3.1: Bàn giao xe cho người thuê
**Màn hình**: `HandoverVehicleActivity`

**Luồng**:
1. Renter quét QR code hoặc nhập Rental ID
2. Staff xem thông tin rental: `GET /api/rentals/{rentalId}`
3. Staff kiểm tra giấy tờ (Driver License, ID Card)
4. Staff chụp ảnh xe trước khi bàn giao
5. Staff xác nhận bàn giao → Rental status = "Active" (đã được set khi StartRental)

**UI**:
- Input field: Rental ID hoặc QR Scanner
- Hiển thị thông tin: Renter, Vehicle, Station
- Nút "Xác nhận bàn giao"
- Camera để chụp ảnh xe

**Note**: Có thể cần thêm API để Staff confirm handover (tạo event hoặc update rental)

---

#### Use Case 2.3.2: Nhận xe trả về
**Màn hình**: `ReceiveVehicleActivity`

**Luồng**:
1. Renter đến trạm trả xe
2. Staff quét QR code rental
3. Staff kiểm tra tình trạng xe (thiệt hại, pin còn lại)
4. Staff chụp ảnh xe sau khi trả
5. Staff xác nhận trả xe → Gọi API CompleteRental (chưa có, cần implement)

**API Endpoint mong đợi**: `POST /api/rentals/{rentalId}/complete`

**Request Body**:
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

**UI**:
- Input Rental ID / QR Scanner
- Hiển thị thông tin rental
- Input pin còn lại (0-100)
- Checkbox thiệt hại (nếu có)
- Textarea ghi chú
- Camera chụp ảnh
- Nút "Xác nhận trả xe"

---

## 👑 3. ADMIN (Quản Trị Viên)

### 3.1. Quản Lý Người Dùng

#### Use Case 3.1.1: Xem danh sách tất cả người dùng
**Màn hình**: `AdminUserListActivity`

**API Endpoint**: `GET /api/users?pageNumber=1&pageSize=20&sortBy=fullName`

**UI Features**:
- Search bar
- Filter theo Role (Renter, Staff, Admin)
- Filter theo Status (Active, Inactive, Suspended)
- List view với pagination
- Click vào user → `AdminUserDetailActivity`

---

#### Use Case 3.1.2: Quản lý trạng thái người dùng
**Màn hình**: `AdminUserDetailActivity`

**Actions**:
- Xem chi tiết user
- Thay đổi Status (Active/Inactive/Suspended)
- Xóa user (soft delete)

**API Endpoint**: `DELETE /api/users/{userId}` (soft delete)

---

### 3.2. Quản Lý Trạm

#### Use Case 3.2.1: Tạo trạm mới
**Màn hình**: `CreateStationActivity`

**API Endpoint**: `POST /api/stations`

**Request Body**:
```json
{
  "name": "Trạm Quận 7",
  "address": "789 Nguyễn Thị Thập, Quận 7, TP.HCM",
  "latitude": 10.7300,
  "longitude": 106.7200
}
```

**Headers**: `Authorization: Bearer {token}` (Admin role)

---

#### Use Case 3.2.2: Xem danh sách trạm
**Màn hình**: `AdminStationListActivity`

**API Endpoint**: `GET /api/stations?pageNumber=1&pageSize=20`

**UI**:
- Map view với tất cả trạm
- List view
- Click vào trạm → `AdminStationDetailActivity`

---

### 3.3. Quản Lý Xe

#### Use Case 3.3.1: Thêm xe mới
**Màn hình**: `AddVehicleActivity`

**API Endpoint**: `POST /api/vehicles`

**Request Body**:
```json
{
  "plateNumber": "30C-99999",
  "type": "Car",  // Car, Motorcycle, Bicycle
  "stationId": "station-id-1",
  "batteryLevel": 100
}
```

---

#### Use Case 3.3.2: Xem tất cả xe
**Màn hình**: `AdminVehicleListActivity`

**API Endpoint**: `GET /api/vehicles?pageNumber=1&pageSize=20&status=Available&type=Car`

**Filters**:
- Status: Available, InUse, Maintenance, Reserved
- Type: Car, Motorcycle, Bicycle
- Station (nếu có filter)

---

### 3.4. Báo Cáo & Thống Kê

#### Use Case 3.4.1: Xem báo cáo doanh thu
**Màn hình**: `RevenueReportActivity`

**API Endpoint**: (Chưa có, cần implement `GetRentalRevenueReportQuery`)

**Response mong đợi**:
```json
{
  "isSuccess": true,
  "value": [
    {
      "date": "2024-10-28",
      "totalRevenue": 5000000,
      "rentalCount": 15,
      "averageCost": 333333
    }
  ]
}
```

**UI**:
- Chart (Line/Bar) hiển thị doanh thu theo ngày/tuần/tháng
- Filter: Start Date, End Date, Group By (Daily/Weekly/Monthly)
- Summary: Tổng doanh thu, Số lượng thuê, Trung bình

---

#### Use Case 3.4.2: Xem thống kê sử dụng
**Màn hình**: `UsageStatisticsActivity`

**Metrics cần hiển thị**:
- Tổng số xe
- Số xe đang sử dụng
- Số xe có sẵn
- Số xe bảo trì
- Tổng số người dùng
- Tổng số rental trong tháng

**API Endpoints**:
- `GET /api/vehicles` → count theo status
- `GET /api/users` → count
- `GET /api/rentals` (khi có API) → count

---

#### Use Case 3.4.3: Xem lịch sử thuê chi tiết
**Màn hình**: `AdminRentalListActivity`

**API Endpoint**: (Chưa có, cần implement `GetAllRentalsQuery`)

**UI**:
- List tất cả rentals
- Filter: Status, Date range, Station, User
- Click vào → `AdminRentalDetailActivity`

---

## 📋 PHÂN CHIA CÔNG VIỆC CHO ANDROID TEAM

### Module 1: Authentication & User Management


**Tasks**:
- [ ] `LoginActivity` + `LoginFragment`
- [ ] `RegisterActivity` + `RegisterFragment`
- [ ] `ProfileFragment` (Xem/Edit profile)
- [ ] `VerificationActivity` (Upload giấy tờ)
- [ ] Token management (SharedPreferences/DataStore)
- [ ] API Service: `AuthService`, `UserService`

**APIs cần dùng**:
- `POST /api/auth/login`
- `POST /api/auth/register`
- `GET /api/users/{id}`
- `PUT /api/users/{id}`


---

### Module 2: Station & Vehicle Discovery

**Tasks**:
- [ ] `StationMapActivity` (Google Maps)(nếu còn thời gian)
- [ ] `StationListFragment`
- [ ] `StationDetailActivity`
- [ ] `VehicleListFragment` (Grid/List)
- [ ] `VehicleDetailActivity`
- [ ] API Service: `StationService`, `VehicleService`

**APIs cần dùng**:
- `GET /api/stations`
- `GET /api/stations/{id}`
- `GET /api/stations/{id}/vehicles`
- `GET /api/vehicles/{id}`


---

### Module 3: Booking & Rental Management

**Tasks**:
- [ ] `ConfirmBookingActivity`
- [ ] `ActiveRentalActivity`
- [ ] `RentalInProgressActivity`
- [ ] `ReturnVehicleActivity`
- [ ] `RentalHistoryFragment`
- [ ] QR Code Scanner (ZXing)
- [ ] API Service: `RentalService`

**APIs cần dùng**:
- `POST /api/rentals/start`
- `GET /api/rentals/{id}`
- `GET /api/rentals` (user's rentals - khi có API)


---

### Module 4: Payment

**Tasks**:
- [ ] `PaymentActivity`
- [ ] `PaymentSuccessActivity`
- [ ] Payment method selection UI
- [ ] Integration với VnPay (nếu có)
- [ ] API Service: `PaymentService`

**APIs cần dùng**:
- `POST /api/payments`
- `GET /api/payments/{id}`


---

### Module 5: Staff Features

**Tasks**:
- [ ] `PendingVerificationListActivity`
- [ ] `UserVerificationDetailActivity`
- [ ] `StationVehiclesActivity`
- [ ] `VehicleManagementActivity`
- [ ] `HandoverVehicleActivity`
- [ ] `ReceiveVehicleActivity`
- [ ] Camera integration (chụp ảnh xe)

**APIs cần dùng**:
- `GET /api/users` (filter unverified)
- `PUT /api/users/{id}` (verify)
- `GET /api/stations/{id}/vehicles`
- `PATCH /api/vehicles/{id}/battery` (khi có)
- `PATCH /api/vehicles/{id}/status` (khi có)
- `POST /api/rentals/{id}/complete` (khi có)


---

### Module 6: Admin Features

**Tasks**:
- [ ] `AdminUserListActivity`
- [ ] `AdminUserDetailActivity`
- [ ] `AdminStationListActivity`
- [ ] `CreateStationActivity`
- [ ] `AdminVehicleListActivity`
- [ ] `AddVehicleActivity`
- [ ] `RevenueReportActivity` (với charts)
- [ ] `UsageStatisticsActivity`
- [ ] `AdminRentalListActivity`

**APIs cần dùng**:
- Tất cả APIs trên + admin-specific APIs


---

## 🔧 TECHNICAL SPECIFICATIONS

### Base URL
```
Development: http://localhost:5000/api
Production: https://api.prm-carrental.com/api
```

### Authentication
- **Type**: JWT Bearer Token
- **Header**: `Authorization: Bearer {token}`
- **Token Storage**: SharedPreferences hoặc EncryptedSharedPreferences
- **Refresh Token**: Lưu riêng, tự động refresh khi token hết hạn

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

### Error Handling
- **200 OK**: Success với data
- **201 Created**: Created resource
- **400 Bad Request**: Validation errors
- **401 Unauthorized**: Token invalid/expired → redirect to Login
- **403 Forbidden**: Không có quyền
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server error

### Pagination
Tất cả list APIs sử dụng pagination:
```
?pageNumber=1&pageSize=20
```

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

## 📱 UI/UX RECOMMENDATIONS

### Color Scheme
- **Primary**: Xanh lá (EV theme)
- **Secondary**: Xanh dương
- **Success**: Xanh lá đậm
- **Warning**: Vàng
- **Error**: Đỏ
- **Neutral**: Xám

### Icons
- Material Design Icons
- Google Maps Icons cho stations
- Custom icons cho vehicle types

### Navigation
- Bottom Navigation Bar cho Renter/Staff
- Drawer Navigation cho Admin
- Deep linking cho notifications

### Loading States
- Shimmer loading cho lists
- Progress bar cho forms
- Skeleton screens

### Error States
- Toast messages cho lỗi nhỏ
- Snackbar với action button
- Full-screen error với retry button

---

## 🔐 SECURITY CONSIDERATIONS

1. **Token Storage**: Sử dụng EncryptedSharedPreferences
2. **HTTPS Only**: Không gọi API qua HTTP
3. **Certificate Pinning**: Pin SSL certificate cho production
4. **Input Validation**: Validate trên client trước khi gửi
5. **Biometric Auth**: Optional cho login (Face ID/Fingerprint)

---

## 🧪 TESTING CHECKLIST

### Unit Tests
- API Service mocks
- ViewModel tests
- Repository tests

### Integration Tests
- API integration tests
- Database tests

### UI Tests
- Critical flows (Login → Book → Return)
- Error handling
- Edge cases

---

## 📦 DEPENDENCIES RECOMMENDED

```gradle
// Networking
implementation 'com.squareup.retrofit2:retrofit:2.9.0'
implementation 'com.squareup.retrofit2:converter-gson:2.9.0'
implementation 'com.squareup.okhttp3:logging-interceptor:4.11.0'

// Image Loading
implementation 'com.github.bumptech.glide:glide:4.16.0'

// Maps
implementation 'com.google.android.gms:play-services-maps:18.2.0'

// QR Code
implementation 'com.journeyapps:zxing-android-embedded:4.3.0'

// Dependency Injection
implementation 'com.google.dagger:hilt-android:2.48'

// Coroutines
implementation 'org.jetbrains.kotlinx:kotlinx-coroutines-android:1.7.3'

// ViewModel
implementation 'androidx.lifecycle:lifecycle-viewmodel-ktx:2.6.2'

// DataStore
implementation 'androidx.datastore:datastore-preferences:1.0.0'
```

---

## 🚀 NEXT STEPS

1. **Backend**: Implement các APIs còn thiếu:
   - `CompleteRentalCommand`
   - `GetUserRentalsQuery`
   - `GetAllRentalsQuery`
   - `GetRentalRevenueReportQuery`
   - `UpdateVehicleBatteryCommand`
   - `ChangeVehicleStatusCommand`
   - `VerifyUserCommand`

2. **Android**:
   - Setup project structure (MVVM/Clean Architecture)
   - Create API services
   - Implement authentication flow
   - Build core screens

3. **Integration**:
   - Test APIs với Postman
   - Integrate Android app với backend
   - Handle errors và edge cases

---


**Document Version**: 1.0  
**Last Updated**: October 2024  
**Maintained By**: Backend Team

