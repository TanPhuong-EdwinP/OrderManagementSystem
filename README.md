Order Management System 
Hệ thống quản lý đơn hàng nước khoáng full-stack, xây dựng với ASP.NET Core Web API và Vue 3, tích hợp JWT authentication, phân quyền Admin/User, và quản lý vòng đời đơn hàng hoàn chỉnh.
Tech Stack
LayerTechnologyBackendASP.NET Core Web API (.NET 8)FrontendVue 3 (SPA, Composition API)DatabaseMySQL + Entity Framework CoreAuthJWT Bearer Token + BCryptHTTP ClientAxios (với interceptor tự động attach token)RoutingVue Router (role-based guard)ContainerDocker
*Tính năng
 Authentication
Đăng nhập / Đăng ký
Mật khẩu hash bằng BCrypt
JWT tự động đính kèm vào mọi request qua Axios interceptor
Tự động redirect về /login khi token hết hạn (401)
Route guard theo role: Admin → /admin, User → /home
*Sản phẩm
Xem danh sách sản phẩm, lọc theo tên và danh mục
Cảnh báo tồn kho thấp (≤ 20 sản phẩm)
Admin: thêm, sửa, xóa sản phẩm
*Giỏ hàng (User)
Thêm sản phẩm với số lượng tuỳ chọn
Cập nhật số lượng, xóa từng item hoặc xóa toàn bộ
Tự động tính tổng tiền
*Đơn hàng
Đặt hàng từ giỏ hàng, kèm thông tin giao hàng
Vòng đời: Pending → Confirmed → Shipped → Delivered / Cancelled
User tự hủy đơn khi còn Pending hoặc Confirmed
Tự động hoàn tồn kho khi hủy
Admin xem tất cả đơn; User chỉ thấy đơn của mình
Lọc đơn hàng theo trạng thái
*Báo cáo (Admin)
Tổng doanh thu (đơn Shipped + Delivered)
Top 5 sản phẩm bán chạy
Dashboard: tổng sản phẩm, đơn hàng, doanh thu hôm nay, cảnh báo tồn kho thấp
*Hồ sơ người dùng
Xem và cập nhật thông tin cá nhân (tên, SĐT, địa chỉ)

Cấu trúc dự án
OrderManagementSystem/
├── backend/                            # ASP.NET Core Web API
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── CartController.cs
│   │   ├── OrdersController.cs
│   │   ├── ProductsController.cs
│   │   ├── ReportsController.cs
│   │   └── UserController.cs
│   ├── Services/
│   │   ├── Interfaces/                 # IAuthService, ICartService, ...
│   │   ├── AuthService.cs
│   │   ├── CartService.cs
│   │   ├── OrderService.cs
│   │   ├── ProductService.cs
│   │   ├── ReportService.cs
│   │   └── UserService.cs
│   ├── DTOs/                           # Auth / Cart / Order / Product / Report / User
│   ├── Models/                         # User, Product, Order, OrderItem, CartItem, Category
│   ├── Data/                           # AppDbContext
│   ├── Migrations/
│   └── Dockerfile
│
└── frontend/                           # Vue 3 SPA
    └── src/
        ├── views/
        │   ├── Login.vue               # Đăng nhập / Đăng ký
        │   ├── Dashboard.vue           # Báo cáo doanh thu
        │   ├── admin/
        │   │   ├── AdminHome.vue       # Dashboard Admin (stats + menu)
        │   │   ├── Products.vue        # Quản lý sản phẩm
        │   │   ├── Orders.vue          # Quản lý & cập nhật trạng thái đơn hàng
        │   │   ├── Users.vue           # Quản lý khách hàng
        │   │   ├── Admins.vue          # Quản lý tài khoản Admin
        │   │   └── Promotions.vue      # Khuyến mãi & mã giảm giá
        │   └── user/
        │       ├── UserHome.vue
        │       ├── UserShop.vue        # Cửa hàng (filter + thêm giỏ)
        │       ├── UserCart.vue        # Giỏ hàng + form thanh toán
        │       ├── UserOrders.vue      # Lịch sử đơn hàng (filter theo status)
        │       └── UserProfile.vue     # Hồ sơ cá nhân
        ├── router/index.js             # Route guard theo role
        └── services/api.js             # Axios instance + authApi, cartApi, orderApi, ...
