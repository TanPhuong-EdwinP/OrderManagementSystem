# 🛒 Order Management System

Hệ thống quản lý đơn hàng full-stack với phân quyền Admin/User, xây dựng bằng ASP.NET Core Web API và Vue 3.

---

Tính năng

### User
- Đăng ký, đăng nhập (JWT Authentication)
- Xem danh sách sản phẩm
- Thêm, cập nhật, xóa sản phẩm trong giỏ hàng
- Đặt hàng, theo dõi trạng thái đơn hàng
- Hủy đơn hàng (chỉ khi Pending hoặc Confirmed)
- Xem và cập nhật thông tin cá nhân

### Admin
- Quản lý sản phẩm (thêm, sửa, xóa, theo dõi tồn kho)
- Quản lý đơn hàng (cập nhật trạng thái)
- Xem báo cáo doanh thu và top sản phẩm bán chạy
- Dashboard thống kê tổng quan

---

## Vòng đời đơn hàng

```
Pending → Confirmed → Shipped → Delivered
                    ↘
                    Cancelled (hoàn tồn kho tự động)
```

## Công nghệ sử dụng

| Phần | Công nghệ |
|---|---|
| Frontend | JavaScript, Vue 3, Vue Router, Axios, Vite |
| Backend | C#, ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | MySQL |
| Auth | JWT Bearer Token |
| Password | BCrypt |

---

## Cấu trúc project

```
OrderManagementSystem/
├── Controllers/        # API endpoints
├── Services/           # Business logic
├── Models/             # Database entities
├── DTOs/               # Data transfer objects
└── frontend/
    ├── views/
    │   ├── admin/      # Trang Admin
    │   └── user/       # Trang User
    ├── services/       # API calls
    └── router/         # Vue Router
```

## Cài đặt & Chạy

### Yêu cầu
- .NET 8 SDK
- Node.js 18+
- MySQL 8+

### Backend
```bash
# Clone project
git clone https://github.com/TanPhuong-EdwinP/OrderManagementSystem.git

# Cấu hình connection string trong appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;database=OrderDB;user=root;password=yourpassword"
}

# Chạy migration
dotnet ef database update

# Chạy server
dotnet run
```

### Frontend
```bash
cd frontend
npm install
npm run dev
```

---

## 📡 API Endpoints

| Method | Endpoint | Mô tả | Auth |
|---|---|---|---|
| POST | /api/auth/register | Đăng ký | ❌ |
| POST | /api/auth/login | Đăng nhập | ❌ |
| GET | /api/products | Danh sách sản phẩm | ❌ |
| POST | /api/products | Thêm sản phẩm | Admin |
| GET | /api/cart | Xem giỏ hàng | User |
| POST | /api/cart | Thêm vào giỏ | User |
| GET | /api/orders | Danh sách đơn hàng | User/Admin |
| POST | /api/orders | Tạo đơn hàng | User |
| PUT | /api/orders/{id}/status | Cập nhật trạng thái | Admin |
| PUT | /api/orders/{id}/cancel | Hủy đơn hàng | User |
| GET | /api/reports/dashboard | Thống kê tổng quan | Admin |
| GET | /api/reports/revenue | Báo cáo doanh thu | Admin |
| GET | /api/reports/top-products | Top sản phẩm | Admin |

---

**Nguyễn Võ Tấn Phương**  
Email: tanphuongnv@gmail.com  
GitHub: [TanPhuong-EdwinP](https://github.com/TanPhuong-EdwinP)
