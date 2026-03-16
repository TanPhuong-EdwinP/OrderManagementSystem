<template>
  <div class="container">

    <div class="page-header"><h1>Báo cáo</h1></div>

    <div class="grid-2" style="gap:20px">

      <!-- Doanh thu theo ngày -->
      <div class="card">
        <strong style="font-size:15px">Doanh thu theo ngày</strong>
        <div style="display:flex;gap:8px;align-items:center;margin:16px 0;flex-wrap:wrap">
          <input class="form-control" type="date" v-model="dateFrom" style="width:145px" />
          <span style="color:#9aa0a6">đến</span>
          <input class="form-control" type="date" v-model="dateTo" style="width:145px" />
          <button class="btn btn-primary btn-sm" @click="loadRevenue">Xem</button>
        </div>
        <template v-if="revenue">
          <div style="display:flex;gap:20px;margin-bottom:14px;font-size:13.5px">
            <div>Tổng: <strong style="color:#1a73e8">{{ fmt(revenue.totalRevenue) }}</strong></div>
            <div>Số đơn: <strong>{{ revenue.totalOrders }}</strong></div>
          </div>
          <div class="table-wrap">
            <table>
              <thead>
                <tr><th>Ngày</th><th>Doanh thu</th><th>Đơn</th></tr>
              </thead>
              <tbody>
                <tr v-for="d in revenue.dailyData || []" :key="d.date">
                  <td>{{ fmtDate(d.date) }}</td>
                  <td style="color:#1a73e8;font-weight:500">{{ fmt(d.revenue) }}</td>
                  <td>{{ d.orderCount }}</td>
                </tr>
                <tr v-if="!(revenue.dailyData || []).length">
                  <td colspan="3" class="loading-text">Chưa có dữ liệu.</td>
                </tr>
              </tbody>
            </table>
          </div>
        </template>
        <div v-else class="loading-text">Nhấn "Xem" để tải dữ liệu.</div>
      </div>

      <!-- Top sản phẩm bán chạy -->
      <div class="card">
        <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:16px">
          <strong style="font-size:15px">Top sản phẩm bán chạy</strong>
          <select class="form-control" v-model.number="topLimit"
            @change="loadTop" style="width:90px">
            <option :value="5">Top 5</option>
            <option :value="10">Top 10</option>
          </select>
        </div>
        <div class="table-wrap">
          <table v-if="topProducts.length">
            <thead>
              <tr><th>#</th><th>Sản phẩm</th><th>Đã bán</th><th>Doanh thu</th></tr>
            </thead>
            <tbody>
              <tr v-for="(p, i) in topProducts" :key="p.productId">
                <td>
                  <span :class="i < 3 ? 'badge badge-warning' : 'badge badge-info'"
                    style="min-width:24px;text-align:center">{{ i + 1 }}</span>
                </td>
                <td style="font-weight:500">{{ p.productName }}</td>
                <td><strong>{{ p.totalSold }}</strong></td>
                <td style="color:#1a73e8">{{ fmt(p.totalRevenue) }}</td>
              </tr>
            </tbody>
          </table>
          <div v-else class="loading-text">Đang tải…</div>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { reportApi } from '../services/api.js'

const revenue     = ref(null)
const topProducts = ref([])
const topLimit    = ref(5)

const today     = new Date().toISOString().split('T')[0]
const thirtyAgo = new Date(Date.now() - 30 * 86400000).toISOString().split('T')[0]
const dateFrom  = ref(thirtyAgo)
const dateTo    = ref(today)

async function loadRevenue() {
  try {
    const { data } = await reportApi.getRevenue(dateFrom.value, dateTo.value)
    revenue.value = data
  } catch (e) { console.error('Revenue error:', e) }
}

async function loadTop() {
  try {
    const { data } = await reportApi.getTopProducts(topLimit.value)
    topProducts.value = data
  } catch (e) { console.error('Top products error:', e) }
}

const fmt     = v => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v)
const fmtDate = d => new Date(d).toLocaleDateString('vi-VN')

onMounted(() => { loadRevenue(); loadTop() })
</script>