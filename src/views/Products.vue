<template>
  <div class="container">

    <div class="page-header">
      <h1>Sản phẩm</h1>
      <button class="btn btn-primary" @click="openCreate">+ Thêm sản phẩm</button>
    </div>

    <!-- Filter -->
    <div class="card" style="margin-bottom:16px;padding:14px 20px;display:flex;gap:12px;align-items:center">
      <input class="form-control" v-model="search"
        placeholder="Tìm tên sản phẩm…" style="max-width:300px" />
      <span style="margin-left:auto;font-size:13px;color:#9aa0a6">
        {{ filtered.length }} sản phẩm
      </span>
    </div>

    <!-- Bảng -->
    <div class="card">
      <div v-if="loading" class="loading-text">Đang tải…</div>
      <div v-else class="table-wrap">
        <table>
          <thead>
            <tr>
              <th style="width:50px">ID</th>
              <th>Tên sản phẩm</th>
              <th>Mô tả</th>
              <th>Giá bán</th>
              <th>Tồn kho</th>
              <th>Trạng thái</th>
              <th style="width:140px">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="p in filtered" :key="p.id">
              <td style="color:#9aa0a6">#{{ p.id }}</td>
              <td style="font-weight:500">{{ p.name }}</td>
              <td style="color:#9aa0a6;font-size:13px">{{ p.description || '—' }}</td>
              <td style="font-weight:500;color:#1a73e8">{{ fmt(p.price) }}</td>
              <td>
                <span :style="(p.stockQuantity <= p.lowStockThreshold)
                  ? 'color:#ea4335;font-weight:600' : ''">
                  {{ p.stockQuantity }}
                </span>
                <span v-if="p.stockQuantity <= p.lowStockThreshold"
                  class="badge badge-warning" style="margin-left:6px">Thấp</span>
              </td>
              <td>
                <span :class="p.isActive ? 'badge badge-ok' : 'badge badge-danger'">
                  {{ p.isActive ? 'Đang bán' : 'Ngừng bán' }}
                </span>
              </td>
              <td>
                <button class="btn btn-secondary btn-sm"
                  style="margin-right:6px" @click="openEdit(p)">Sửa</button>
                <button class="btn btn-danger btn-sm"
                  @click="handleDelete(p.id)">Xóa</button>
              </td>
            </tr>
            <tr v-if="filtered.length === 0">
              <td colspan="7" class="loading-text">Không có sản phẩm nào.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal thêm/sửa -->
    <div v-if="showModal" class="modal-overlay" @click.self="showModal = false">
      <div class="modal">
        <div class="modal-title">
          {{ editingId ? 'Cập nhật sản phẩm' : 'Thêm sản phẩm mới' }}
        </div>
        <form @submit.prevent="handleSubmit">
          <div class="form-group">
            <label>Tên sản phẩm *</label>
            <input class="form-control" v-model="form.name" required />
          </div>
          <div class="form-group">
            <label>Mô tả</label>
            <input class="form-control" v-model="form.description"
              placeholder="Mô tả ngắn…" />
          </div>
          <div class="grid-2">
            <div class="form-group">
              <label>Giá bán (VNĐ) *</label>
              <input class="form-control" v-model.number="form.price"
                type="number" min="0" required />
            </div>
            <div class="form-group">
              <label>Tồn kho *</label>
              <input class="form-control" v-model.number="form.stockQuantity"
                type="number" min="0" required />
            </div>
          </div>
          <div class="grid-2">
            <div class="form-group">
              <label>Ngưỡng cảnh báo tồn kho</label>
              <input class="form-control" v-model.number="form.lowStockThreshold"
                type="number" min="1" />
            </div>
            <div class="form-group">
              <label>Danh mục ID</label>
              <input class="form-control" v-model.number="form.categoryId"
                type="number" min="1"
                placeholder="1=Khoáng, 2=Tinh khiết, 3=Có gas" />
            </div>
          </div>
          <div v-if="formError" class="alert alert-error">{{ formError }}</div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary"
              @click="showModal = false">Hủy</button>
            <button type="submit" class="btn btn-primary" :disabled="submitting">
              {{ submitting ? 'Đang lưu…' : (editingId ? 'Cập nhật' : 'Thêm mới') }}
            </button>
          </div>
        </form>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { productApi } from '../services/api.js'

const products   = ref([])
const loading    = ref(true)
const search     = ref('')
const showModal  = ref(false)
const editingId  = ref(null)
const submitting = ref(false)
const formError  = ref('')

const blank = () => ({
  name: '',
  price: 0,
  stock: 0,        // ← stock thay vì stockQuantity
  categoryId: 1
})
const form = ref(blank())

const filtered = computed(() =>
  products.value.filter(p =>
    p.name.toLowerCase().includes(search.value.toLowerCase())
  )
)

async function load() {
  loading.value = true
  try {
    const { data } = await productApi.getAll()
    products.value = data
  } finally { loading.value = false }
}

function openCreate() {
  editingId.value = null
  form.value      = blank()
  formError.value = ''
  showModal.value = true
}

function openEdit(p) {
  editingId.value = p.id
  form.value = {
    name:       p.name,
    price:      p.price,
    stock:      p.stockQuantity ?? p.stock ?? 0,  // ← dùng stock
    categoryId: p.categoryId
  }
  formError.value = ''
  showModal.value = true
}

async function handleSubmit() {
  formError.value  = ''
  submitting.value = true
  try {
    editingId.value
      ? await productApi.update(editingId.value, form.value)
      : await productApi.create(form.value)
    showModal.value = false
    await load()
  } catch (e) {
    formError.value = e.response?.data?.message || 'Có lỗi xảy ra.'
  } finally { submitting.value = false }
}

async function handleDelete(id) {
  if (!confirm('Xác nhận xóa sản phẩm này?')) return
  try { await productApi.delete(id); await load() }
  catch (e) { alert(e.response?.data?.message || 'Xóa thất bại.') }
}

const fmt = v =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v)

onMounted(load)
</script>