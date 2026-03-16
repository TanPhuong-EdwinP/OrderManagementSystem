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
              <th>User ID</th>
              <th>Tổng tiền</th>
              <th>Trạng thái</th>
              <th>Ngày tạo</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="o in orders" :key="o.id">
              <td style="color:#9aa0a6">#{{ o.id }}</td>
              <td>User #{{ o.userId }}</td>
              <td style="font-weight:600;color:#1a73e8">{{ fmt(o.total) }}</td>
              <td>
                <span :class="`badge ${badgeClass(o.status)}`">
                  {{ labelOf(o.status) }}
                </span>
              </td>
              <td style="color:#9aa0a6;font-size:13px">
                {{ o.createdAt ? fmtDate(o.createdAt) : '—' }}
              </td>
              <td>
                <div style="display:flex;gap:6px;flex-wrap:wrap">
                  <button class="btn btn-secondary btn-sm"
                    @click="detail = o">Chi tiết</button>
                  <button
                    v-for="next in nextOf(o.status)" :key="next.val"
                    :class="`btn btn-sm btn-${next.color}`"
                    @click="doStatus(o.id, next.val)">
                    {{ next.label }}
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

    <!-- Modal tạo đơn -->
    <div v-if="showCreate" class="modal-overlay" @click.self="showCreate = false">
      <div class="modal">
        <div class="modal-title">Tạo đơn hàng mới</div>

        <div class="form-group">
          <label>User ID *</label>
          <input class="form-control" v-model.number="cForm.userId"
            type="number" min="1" placeholder="Nhập ID người dùng" />
        </div>
        <div class="form-group">
          <label>Tổng tiền (VNĐ) *</label>
          <input class="form-control" v-model.number="cForm.total"
            type="number" min="0" />
        </div>

        <div v-if="cError" class="alert alert-error">{{ cError }}</div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="showCreate = false">Hủy</button>
          <button class="btn btn-primary" :disabled="submitting" @click="submitOrder">
            {{ submitting ? 'Đang tạo…' : 'Tạo đơn hàng' }}
          </button>
        </div>
      </div>
    </div>

    <!-- Modal chi tiết -->
    <div v-if="detail" class="modal-overlay" @click.self="detail = null">
      <div class="modal">
        <div class="modal-title">Chi tiết đơn hàng #{{ detail.id }}</div>
        <table style="width:100%;font-size:14px;margin-bottom:16px">
          <tr>
            <td style="color:#9aa0a6;padding:7px 0;width:120px">ID</td>
            <td><strong>#{{ detail.id }}</strong></td>
          </tr>
          <tr>
            <td style="color:#9aa0a6;padding:7px 0">User ID</td>
            <td>{{ detail.userId }}</td>
          </tr>
          <tr>
            <td style="color:#9aa0a6;padding:7px 0">Tổng tiền</td>
            <td style="font-weight:600;color:#1a73e8">{{ fmt(detail.total) }}</td>
          </tr>
          <tr>
            <td style="color:#9aa0a6;padding:7px 0">Trạng thái</td>
            <td>
              <span :class="`badge ${badgeClass(detail.status)}`">
                {{ labelOf(detail.status) }}
              </span>
            </td>
          </tr>
          <tr>
            <td style="color:#9aa0a6;padding:7px 0">Ngày tạo</td>
            <td>{{ detail.createdAt ? fmtDate(detail.createdAt) : '—' }}</td>
          </tr>
        </table>

        <!-- Đổi trạng thái ngay trong modal chi tiết -->
        <div v-if="nextOf(detail.status).length > 0">
          <div style="font-size:13px;color:#5f6368;margin-bottom:8px">
            Chuyển trạng thái:
          </div>
          <div style="display:flex;gap:8px;flex-wrap:wrap">
            <button
              v-for="next in nextOf(detail.status)" :key="next.val"
              :class="`btn btn-${next.color}`"
              @click="doStatusFromDetail(detail.id, next.val)">
              {{ next.label }}
            </button>
          </div>
        </div>

        <div class="modal-footer">
          <button class="btn btn-secondary" @click="detail = null">Đóng</button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { orderApi } from '../services/api.js'

const orders     = ref([])
const loading    = ref(true)
const showCreate = ref(false)
const detail     = ref(null)
const submitting = ref(false)
const cError     = ref('')

// ✅ CreateOrderDto: { userId, total }
const cForm = ref({ userId: '', total: 0 })

// ✅ Status là string theo OrderDto.Status
const LABELS = {
  Pending:   'Chờ xác nhận',
  Confirmed: 'Đã xác nhận',
  Shipped:   'Đang giao',
  Delivered: 'Đã giao',
  Cancelled: 'Đã hủy'
}
const BADGES = {
  Pending:   'badge-warning',
  Confirmed: 'badge-info',
  Shipped:   'badge-ok',
  Delivered: 'badge-ok',
  Cancelled: 'badge-danger'
}
// State Machine: từ trạng thái hiện tại → các trạng thái tiếp theo
const NEXTS = {
  Pending:   [
    { val: 'Confirmed', label: 'Xác nhận',  color: 'primary' },
    { val: 'Cancelled', label: 'Hủy đơn',   color: 'danger'  }
  ],
  Confirmed: [{ val: 'Shipped',   label: 'Giao hàng', color: 'success' }],
  Shipped:   [{ val: 'Delivered', label: 'Đã giao',   color: 'success' }]
}

const labelOf  = s => LABELS[s] ?? s
const badgeClass = s => BADGES[s] ?? 'badge-info'
const nextOf   = s => NEXTS[s] ?? []

async function load() {
  loading.value = true
  try {
    const { data } = await orderApi.getAll()
    orders.value = Array.isArray(data) ? data : []
  } catch (e) {
    console.error('Load orders error:', e)
  } finally { loading.value = false }
}

function openCreate() {
  cForm.value  = { userId: '', total: 0 }
  cError.value = ''
  showCreate.value = true
}

async function submitOrder() {
  if (!cForm.value.userId || cForm.value.userId < 1) {
    cError.value = 'Vui lòng nhập User ID hợp lệ (số nguyên > 0).'
    return
  }
  cError.value     = ''
  submitting.value = true
  try {
    // ✅ Gửi đúng { userId, total } theo CreateOrderDto
    await orderApi.create({
      userId: Number(cForm.value.userId),
      total:  Number(cForm.value.total)
    })
    showCreate.value = false
    await load()
  } catch (e) {
    const d = e.response?.data
    cError.value = d?.message || d?.title || `Lỗi ${e.response?.status}`
  } finally { submitting.value = false }
}

async function doStatus(id, status) {
  if (!confirm(`Chuyển sang "${labelOf(status)}"?`)) return
  try {
    // ✅ Gửi { status } string theo UpdateOrderStatusDto
    await orderApi.updateStatus(id, status)
    await load()
  } catch (e) {
    alert('Lỗi: ' + (e.response?.data?.message || e.message))
  }
}

async function doStatusFromDetail(id, status) {
  if (!confirm(`Chuyển sang "${labelOf(status)}"?`)) return
  try {
    await orderApi.updateStatus(id, status)
    detail.value = null
    await load()
  } catch (e) {
    alert('Lỗi: ' + (e.response?.data?.message || e.message))
  }
}

const fmt     = v => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v)
const fmtDate = d => new Date(d).toLocaleDateString('vi-VN')

onMounted(load)
</script>