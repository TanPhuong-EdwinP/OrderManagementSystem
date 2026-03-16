<template>
  <div class="container">
    <div class="page-header">
      <h1>Đơn hàng</h1>
      <button class="btn btn-primary" @click="openCreate">+ Tạo đơn hàng</button>
    </div>

    <div class="card">
      <div v-if="loading" class="loading-text">Đang tải…</div>
      <div v-else class="table-wrap">
        <table>
          <thead>
            <tr>
              <th style="width:60px">ID</th>
              <th>Khách hàng</th>
              <th>Tổng tiền</th>
              <th>Trạng thái</th>
              <th>Ngày tạo</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="o in orders" :key="o.id || o.orderId">
              <td style="color:#9aa0a6">#{{ o.id || o.orderId || '—' }}</td>
              <td>
                <div style="font-weight:500">{{ o.customerName || o.userName || o.fullName || 'Khách lẻ' }}</div>
                <div style="font-size:12px;color:#9aa0a6">{{ o.customerEmail || o.userEmail || '' }}</div>
              </td>
              <td style="font-weight:600;color:#1a73e8">
                {{ fmt(o.totalAmount || o.totalPrice || o.total || 0) }}
              </td>
              <td>
                <span :class="`badge badge-s${o.status}`">
                  {{ STATUS_LABEL[o.status] ?? (o.statusName || o.status) }}
                </span>
              </td>
              <td style="color:#9aa0a6;font-size:13px">
                {{ fmtDate(o.createdAt || o.orderDate || o.updatedAt) }}
              </td>
              <td>
                <div style="display:flex;gap:6px;flex-wrap:wrap">
                  <button class="btn btn-secondary btn-sm" @click="detail = o">Chi tiết</button>
                  <button
                    v-for="next in (NEXT_STATUS[o.status] || [])" :key="next"
                    :class="`btn btn-sm btn-${NEXT_COLOR[next] || 'secondary'}`"
                    @click="changeStatus(o.id || o.orderId, next)">
                    {{ STATUS_LABEL[next] }}
                  </button>
                </div>
              </td>
            </tr>
            <tr v-if="!orders.length">
              <td colspan="6" class="loading-text">Chưa có đơn hàng nào.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div v-if="detail" class="modal-overlay" @click.self="detail = null">
      <div class="modal" style="width:560px">
        <div class="modal-title">Đơn hàng #{{ detail.id || detail.orderId }}</div>
        <div class="grid-2" style="margin-bottom:16px;font-size:13.5px">
          <div><span style="color:#9aa0a6">Khách hàng:</span> <strong>{{ detail.customerName || detail.userName || '—' }}</strong></div>
          <div><span style="color:#9aa0a6">Trạng thái:</span> <span :class="`badge badge-s${detail.status}`">{{ STATUS_LABEL[detail.status] }}</span></div>
        </div>
        <div class="table-wrap">
          <table>
            <thead>
              <tr><th>Sản phẩm</th><th>SL</th><th>Đơn giá</th><th>Thành tiền</th></tr>
            </thead>
            <tbody>
              <tr v-for="item in (detail.orderItems || detail.items || [])" :key="item.id">
                <td>{{ item.productName || (item.product ? item.product.name : 'Sản phẩm') }}</td>
                <td>{{ item.quantity }}</td>
                <td>{{ fmt(item.price || item.unitPrice || 0) }}</td>
                <td>{{ fmt((item.price || item.unitPrice || 0) * item.quantity) }}</td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="modal-footer"><button class="btn btn-secondary" @click="detail = null">Đóng</button></div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { orderApi, productApi } from '../services/api.js'

const orders      = ref([])
const loading     = ref(true)
const detail      = ref(null)

const STATUS_LABEL = { 0: 'Chờ xác nhận', 1: 'Đã xác nhận', 2: 'Đang giao', 3: 'Đã giao', 4: 'Đã hủy' }
const NEXT_STATUS  = { 0: [1, 4], 1: [2], 2: [3] }
const NEXT_COLOR   = { 1: 'primary', 2: 'success', 3: 'success', 4: 'danger' }

async function load() {
  loading.value = true
  try {
    const { data } = await orderApi.getAll()
    console.log("Dữ liệu thực tế từ Backend:", data) // Kiểm tra tên biến tại đây
    orders.value = data
  } catch (e) {
    console.error("Lỗi tải đơn hàng:", e)
  } finally { loading.value = false }
}

async function changeStatus(id, status) {
  if (!confirm(`Chuyển sang "${STATUS_LABEL[status]}"?`)) return
  try {
    await orderApi.updateStatus(id, status)
    await load()
  } catch (e) { alert('Lỗi cập nhật trạng thái.') }
}

const fmt = v => {
  if (v === undefined || v === null || isNaN(v)) return '0 ₫';
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v)
}

const fmtDate = d => {
  if (!d) return '—';
  const date = new Date(d);
  return isNaN(date.getTime()) ? 'Ngày lỗi' : date.toLocaleDateString('vi-VN');
}

onMounted(load)
</script>