<template>
  <div class="container">

    <div class="page-header"><h1>Báo cáo</h1></div>

    <div class="grid-2" style="gap:20px">

      <!-- Doanh thu -->
      <div class="card">
        <strong style="font-size:15px;display:block;margin-bottom:16px">
          Tổng doanh thu
        </strong>
        <div v-if="revenue" style="font-size:32px;font-weight:700;color:#1a73e8;margin-bottom:8px">
          {{ fmt(revenue.totalRevenue ?? 0) }}
        </div>
        <div v-else class="loading-text">Đang tải…</div>
        <button class="btn btn-secondary btn-sm" @click="loadRevenue"
          style="margin-top:12px">Làm mới</button>
      </div>

      <!-- Top sản phẩm -->
      <div class="card">
        <strong style="font-size:15px;display:block;margin-bottom:16px">
          Top sản phẩm bán chạy
        </strong>
        <div class="table-wrap">
          <table v-if="topProducts.length">
            <thead>
              <tr>
                <th style="width:40px">#</th>
                <th>Product ID</th>
                <th>Số lượng bán</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(p, i) in topProducts" :key="p.productId">
                <td>
                  <span :class="i < 3 ? 'badge badge-warning' : 'badge badge-info'"
                    style="min-width:24px;text-align:center">{{ i + 1 }}</span>
                </td>
                <td>Product #{{ p.productId }}</td>
                <td><strong>{{ p.quantity }}</strong></td>
              </tr>
            </tbody>
          </table>
          <div v-else-if="topLoading" class="loading-text">Đang tải…</div>
          <div v-else class="loading-text">Chưa có dữ liệu đơn hàng.</div>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { reportApi } from '../services/api.js'

// ✅ RevenueReportDto: { totalRevenue }
// ✅ TopProductDto: { productId, quantity }
const revenue    = ref(null)
const topProducts = ref([])
const topLoading  = ref(true)

async function loadRevenue() {
  try {
    const { data } = await reportApi.getRevenue()
    revenue.value = data
  } catch (e) {
    console.error('Revenue error:', e)
    revenue.value = { totalRevenue: 0 }
  }
}

async function loadTop() {
  topLoading.value = true
  try {
    const { data } = await reportApi.getTopProducts()
    topProducts.value = Array.isArray(data) ? data : []
  } catch (e) {
    console.error('Top products error:', e)
    topProducts.value = []
  } finally { topLoading.value = false }
}

const fmt = v =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v)

onMounted(() => { loadRevenue(); loadTop() })
</script>