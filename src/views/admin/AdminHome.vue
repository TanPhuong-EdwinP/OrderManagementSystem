<template>
  <div class="admin-home">
    <!-- Header -->
    <div class="dashboard-header">
      <div class="header-left">
        <h1 class="page-title">Tổng quan</h1>
        <p class="page-subtitle">Xin chào, Admin 👋</p>
      </div>

      <button
        class="refresh-btn"
        @click="loadDashboard"
        :class="{ spinning: loading }"
        :disabled="loading"
      >
        <span v-if="loading" class="spinner-small"></span>
        <span v-else class="refresh-icon">↻</span>
        {{ loading ? 'Đang tải...' : 'Làm mới' }}
      </button>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading-state">
      <div class="loading-spinner"></div>
      <p class="loading-text">Đang tải dữ liệu hệ thống...</p>
    </div>

    <!-- Stats Grid -->
    <div v-else class="stats-grid">
      <div
        v-for="card in statCards"
        :key="card.label"
        class="stat-card"
        :style="{ '--accent': card.color }"
      >
        <div class="stat-icon-wrapper">
          <span class="stat-icon">{{ card.icon }}</span>
        </div>
        <div class="stat-info">
          <div class="stat-label">{{ card.label }}</div>
          <div class="stat-value">{{ card.value }}</div>
        </div>
      </div>
    </div>

    <!-- Low stock warning -->
    <div v-if="lowStockProducts.length && !loading" class="warning-banner">
      <span class="warning-icon">⚠️</span>
      <span>
        Có <strong>{{ lowStockProducts.length }}</strong> sản phẩm sắp hết hàng
      </span>
      <router-link to="/admin/products" class="warning-link">Xem ngay →</router-link>
    </div>

    <!-- Menu Grid -->
    <div class="section-title">Quản lý hệ thống</div>
    <div class="menu-grid">
      <div
        v-for="item in menuItems"
        :key="item.title"
        class="menu-card"
        :style="{ '--mc': item.color }"
        @click="$router.push(item.path)"
      >
        <div class="menu-icon-wrapper">
          <span class="menu-icon">{{ item.icon }}</span>
        </div>
        <div class="menu-content">
          <div class="menu-title">{{ item.title }}</div>
          <div class="menu-desc">{{ item.desc }}</div>
        </div>
        <span class="menu-arrow">→</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { reportApi, productApi, orderApi, adminApi } from '../../services/api.js'  // thêm adminApi

// Nếu bạn đã có promotionApi thì thêm vào import:
// import { reportApi, productApi, orderApi, adminApi, promotionApi } from '../../services/api.js'

const loading = ref(false)
const stats = ref({
  products: 0,
  orders: 0,
  pending: 0,
  revenue: 0,
  todayOrders: 0,
  todayRevenue: 0,
  customers: 0,
  delivered: 0,
  adminCount: 0,
  activePromotions: 0
})

const lowStockProducts = ref([])

const menuItems = ref([
  { title: 'Sản phẩm', desc: 'Quản lý danh sách sản phẩm', path: '/admin/products', icon: '📦', color: '#1a73e8' },
  { title: 'Đơn hàng', desc: 'Xem và xử lý đơn hàng', path: '/admin/orders', icon: '📋', color: '#34a853' },
  { title: 'Báo cáo', desc: 'Thống kê doanh thu & sản phẩm', path: '/admin/dashboard', icon: '📊', color: '#fbbc05' },
  { title: 'Khách hàng', desc: 'Quản lý tài khoản người dùng', path: '/admin/users', icon: '👥', color: '#ea4335' },
  { title: 'Quản trị viên', desc: 'Quản lý tài khoản Admin', path: '/admin/admins', icon: '🛡️', color: '#ea4335' },
  { title: 'Khuyến mãi', desc: 'Mã giảm giá & chương trình', path: '/admin/promotions', icon: '🎟️', color: '#ec4899' }
])

const statCards = computed(() => [
  { label: 'Tổng sản phẩm', value: stats.value.products.toLocaleString('vi-VN'), icon: '📦', color: '#1a73e8' },
  { label: 'Tổng đơn hàng', value: stats.value.orders.toLocaleString('vi-VN'), icon: '📋', color: '#34a853' },
  { label: 'Đơn chờ xử lý', value: stats.value.pending.toLocaleString('vi-VN'), icon: '⏳', color: '#fbbc05' },
  { label: 'Doanh thu', value: formatCurrency(stats.value.revenue), icon: '💰', color: '#ea4335' },
  { label: 'Đơn hôm nay', value: stats.value.todayOrders.toLocaleString('vi-VN'), icon: '📅', color: '#4285f4' },
  { label: 'Doanh thu hôm nay', value: formatCurrency(stats.value.todayRevenue), icon: '📈', color: '#34a853' },
  { label: 'Tổng khách hàng', value: stats.value.customers.toLocaleString('vi-VN'), icon: '👥', color: '#9c27b0' },
  { label: 'Tài khoản Admin', value: stats.value.adminCount.toLocaleString('vi-VN'), icon: '🛡️', color: '#ea4335' },
  { label: 'Khuyến mãi hoạt động', value: stats.value.activePromotions.toLocaleString('vi-VN'), icon: '🎟️', color: '#ec4899' }
])

function formatCurrency(value) {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(value || 0)
}

async function loadDashboard() {
  loading.value = true

  // Reset stats để tránh hiển thị số cũ khi reload
  stats.value = {
    products: 0,
    orders: 0,
    pending: 0,
    revenue: 0,
    todayOrders: 0,
    todayRevenue: 0,
    customers: 0,
    delivered: 0,
    adminCount: 0,
    activePromotions: 0
  }

  try {
    // Phần gốc của bạn - giữ nguyên Promise.all cho 3 API chính
    const [productsRes, ordersRes, revenueRes] = await Promise.all([
      productApi.getAll(),
      orderApi.getAll(),
      reportApi.getRevenue(),
    ])

    stats.value.products = Array.isArray(productsRes.data) ? productsRes.data.length : 0

    const allOrders = Array.isArray(ordersRes.data) ? ordersRes.data : []
    stats.value.orders = allOrders.length
    stats.value.pending = allOrders.filter(o => o.status === 'Pending' || o.status === 'Chờ xử lý').length
    stats.value.delivered = allOrders.filter(o => o.status === 'Delivered' || o.status === 'Đã giao').length

    stats.value.revenue = revenueRes.data?.totalRevenue ?? 0

    const today = new Date().toISOString().split('T')[0]
    const todayOrders = allOrders.filter(o => {
      const orderDate = new Date(o.createdAt).toISOString().split('T')[0]
      return orderDate === today
    })
    stats.value.todayOrders = todayOrders.length
    stats.value.todayRevenue = todayOrders.reduce((sum, o) => sum + (o.total || 0), 0)

    lowStockProducts.value = productsRes.data?.filter(p => p.stockQuantity <= 20 && p.stockQuantity > 0) || []

    // Thêm Tài khoản Admin - gọi riêng, không ảnh hưởng nếu lỗi
    try {
      const adminsRes = await adminApi.getAll()
      stats.value.adminCount = Array.isArray(adminsRes.data) ? adminsRes.data.length : 0
    } catch (adminErr) {
      console.warn('Không tải được số tài khoản Admin:', adminErr)
      stats.value.adminCount = 0  // fallback
    }

    // Thêm Khuyến mãi hoạt động - gọi riêng (nếu bạn đã có promotionApi)
    // Nếu chưa có thì comment khối này hoặc thêm promotionApi vào api.js
    /*
    try {
      const promotionsRes = await promotionApi.getAll()
      stats.value.activePromotions = Array.isArray(promotionsRes.data)
        ? promotionsRes.data.filter(p => p.isActive === true).length
        : 0
    } catch (promoErr) {
      console.warn('Không tải được khuyến mãi:', promoErr)
      stats.value.activePromotions = 0
    }
    */

  } catch (mainErr) {
    console.error('Lỗi chính khi tải dashboard:', mainErr)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadDashboard()
})
</script>

<style scoped>
.admin-home {
  padding: 32px 28px;
  background: #f8fafc;
  min-height: 100vh;
  font-family: system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 40px;
}

.page-title {
  font-size: 32px;
  font-weight: 700;
  color: #0f172a;
  letter-spacing: -0.5px;
}

.page-subtitle {
  color: #64748b;
  font-size: 15px;
  margin-top: 4px;
}

.refresh-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 11px 20px;
  border-radius: 10px;
  background: #0f172a;
  color: white;
  border: none;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

.refresh-btn:hover:not(:disabled) {
  background: #1e2937;
  transform: translateY(-1px);
}

.spinner-small {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255,255,255,0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.9s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 24px;
  margin-bottom: 48px;
}

.stat-card {
  background: white;
  border-radius: 16px;
  padding: 28px 24px;
  box-shadow: 0 10px 15px -3px rgb(0 0 0 / 0.05);
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
}

.stat-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 20px 25px -5px rgb(0 0 0 / 0.1);
}

.stat-icon-wrapper {
  width: 56px;
  height: 56px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 20px;
  font-size: 28px;
}

.stat-info {
  line-height: 1.1;
}

.stat-label {
  font-size: 13px;
  font-weight: 500;
  color: #64748b;
  letter-spacing: 0.5px;
}

.stat-value {
  font-size: 32px;
  font-weight: 700;
  color: #0f172a;
  margin-top: 4px;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 500px;
  color: #64748b;
}

.loading-spinner {
  width: 56px;
  height: 56px;
  border: 6px solid #e2e8f0;
  border-top-color: #1d4ed8;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 20px;
}

.warning-banner {
  background: #fefce8;
  border: 1px solid #facc15;
  border-radius: 12px;
  padding: 16px 20px;
  display: flex;
  align-items: center;
  gap: 14px;
  margin-bottom: 40px;
  font-size: 15px;
  color: #854d0e;
}

.section-title {
  font-size: 18px;
  font-weight: 600;
  color: #0f172a;
  margin-bottom: 20px;
}

.menu-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
}

.menu-card {
  background: white;
  border-radius: 16px;
  padding: 24px;
  display: flex;
  align-items: center;
  gap: 20px;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  border: 1px solid #e2e8f0;
}

.menu-card:hover {
  border-color: var(--mc);
  box-shadow: 0 20px 25px -5px rgb(0 0 0 / 0.1);
  transform: translateY(-6px);
}

.menu-icon-wrapper {
  width: 64px;
  height: 64px;
  border-radius: 14px;
  background: color-mix(in srgb, var(--mc) 12%, white);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 32px;
}

.menu-content { flex: 1; }

.menu-title {
  font-size: 17px;
  font-weight: 600;
  color: #0f172a;
}

.menu-desc {
  font-size: 14px;
  color: #64748b;
  margin-top: 4px;
}

.menu-arrow {
  font-size: 22px;
  color: #94a3b8;
  transition: transform 0.3s;
}

.menu-card:hover .menu-arrow {
  transform: translateX(8px);
  color: var(--mc);
}
</style>