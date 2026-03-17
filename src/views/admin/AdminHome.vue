<template>
  <div class="container">
    <div class="page-header">
      <h1>🛡️ Tổng quan Admin</h1>
    </div>

    <div class="stats-row">
      <div class="stat-card blue">
        <div class="stat-label">Tổng sản phẩm</div>
        <div class="stat-value">{{ stats.products }}</div>
      </div>
      <div class="stat-card green">
        <div class="stat-label">Tổng đơn hàng</div>
        <div class="stat-value">{{ stats.orders }}</div>
      </div>
      <div class="stat-card yellow">
        <div class="stat-label">Đơn chờ xử lý</div>
        <div class="stat-value">{{ stats.pending }}</div>
      </div>
      <div class="stat-card red">
        <div class="stat-label">Tổng doanh thu</div>
        <div class="stat-value sm">{{ fmt(stats.revenue) }}</div>
      </div>
    </div>

    <div class="menu-grid" style="margin-top:24px">
      <div class="menu-card" @click="$router.push('/admin/products')">
        <div class="menu-icon" style="background:#e8f0fe;font-size:28px">📦</div>
        <div class="menu-info">
          <div class="menu-title">Quản lý sản phẩm</div>
          <div class="menu-desc">Thêm, sửa, xóa sản phẩm</div>
        </div>
        <span>→</span>
      </div>
      <div class="menu-card" @click="$router.push('/admin/orders')">
        <div class="menu-icon" style="background:#e6f4ea;font-size:28px">📋</div>
        <div class="menu-info">
          <div class="menu-title">Quản lý đơn hàng</div>
          <div class="menu-desc">Xem và cập nhật trạng thái đơn</div>
        </div>
        <span>→</span>
      </div>
      <div class="menu-card" @click="$router.push('/admin/dashboard')">
        <div class="menu-icon" style="background:#fce8e6;font-size:28px">📊</div>
        <div class="menu-info">
          <div class="menu-title">Báo cáo doanh thu</div>
          <div class="menu-desc">Thống kê và top sản phẩm</div>
        </div>
        <span>→</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { productApi, orderApi, reportApi } from '../../services/api.js'

const stats = ref({ products: 0, orders: 0, pending: 0, revenue: 0 })

onMounted(async () => {
  try {
    const [p, o, r] = await Promise.all([
      productApi.getAll(), orderApi.getAll(), reportApi.getRevenue()
    ])
    stats.value.products = p.data.length
    stats.value.orders   = o.data.length
    stats.value.pending  = o.data.filter(x => x.status === 'Pending').length
    stats.value.revenue  = r.data.totalRevenue ?? 0
  } catch (e) { console.error(e) }
})

const fmt = v => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v)
</script>

<style scoped>
.stats-row { display: grid; grid-template-columns: repeat(4,1fr); gap: 14px; }
.stat-card {
  border-radius: 10px; padding: 20px;
  border-left: 4px solid transparent;
}
.stat-card.blue   { background:#e8f0fe; border-color:#1a73e8; }
.stat-card.green  { background:#e6f4ea; border-color:#34a853; }
.stat-card.yellow { background:#fef9e7; border-color:#fbbc04; }
.stat-card.red    { background:#fce8e6; border-color:#ea4335; }
.stat-label { font-size:12.5px; color:#5f6368; margin-bottom:6px; }
.stat-value { font-size:28px; font-weight:700; color:#1e2329; }
.stat-value.sm { font-size:18px; }
.menu-grid { display:grid; grid-template-columns:repeat(3,1fr); gap:14px; }
.menu-card {
  background:#fff; border:1px solid #e8eaed; border-radius:12px;
  padding:20px; display:flex; align-items:center; gap:16px;
  cursor:pointer; transition:border-color .2s;
}
.menu-card:hover { border-color:#1a73e8; }
.menu-icon { width:52px; height:52px; border-radius:12px; display:flex; align-items:center; justify-content:center; }
.menu-info { flex:1; }
.menu-title { font-size:15px; font-weight:600; margin-bottom:4px; }
.menu-desc  { font-size:13px; color:#9aa0a6; }
</style>