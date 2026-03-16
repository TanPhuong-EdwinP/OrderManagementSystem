<template>
  <div class="container">

    <div class="page-header">
      <h1>Sản phẩm</h1>
      <button class="btn btn-primary" @click="openCreate">+ Thêm sản phẩm</button>
    </div>

    <!-- Filter -->
    <div class="card" style="margin-bottom:16px;padding:14px 20px;
         display:flex;gap:12px;align-items:center">
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
              <th style="width:60px">ID</th>
              <th>Tên sản phẩm</th>
              <th>Giá bán</th>
              <th style="width:160px">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="p in filtered" :key="p.id">
              <td style="color:#9aa0a6">#{{ p.id }}</td>
              <td style="font-weight:500">{{ p.name }}</td>
              <td style="font-weight:500;color:#1a73e8">{{ fmt(p.price) }}</td>
              <td>
                <button class="btn btn-secondary btn-sm"
                  style="margin-right:6px" @click="openEdit(p)">Sửa</button>
                <button class="btn btn-danger btn-sm"
                  @click="handleDelete(p.id)">Xóa</button>
              </td>
            </tr>
            <tr v-if="filtered.length === 0">
              <td colspan="4" class="loading-text">Không có sản phẩm nào.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal thêm / sửa -->
    <div v-if="showModal" class="modal-overlay" @click.self="showModal = false">
      <div class="modal">
        <div class="modal-title">
          {{ editingId ? 'Cập nhật sản phẩm' : 'Thêm sản phẩm mới' }}
        </div>
        <form @submit.prevent="handleSubmit">
          <div class="form-group">
            <label>Tên sản phẩm *</label>
            <input class="form-control" v-model="form.name"
              required placeholder="Nhập tên sản phẩm" />
          </div>
          <div class="grid-2">
            <div class="form-group">
              <label>Giá bán (VNĐ) *</label>
              <input class="form-control" v-model.number="form.price"
                type="number" min="0" required />
            </div>
            <div class="form-group">
              <label>Tồn kho *</label>
              <input class="form-control" v-model.number="form.stock"
                type="number" min="0" required />
            </div>
          </div>
          <div class="form-group">
            <label>Danh mục *</label>
            <select class="form-control" v-model.number="form.categoryId" required>
              <option value="">-- Chọn danh mục --</option>
              <option :value="1">Nước khoáng</option>
              <option :value="2">Nước tinh khiết</option>
              <option :value="3">Nước có gas</option>
            </select>
          </div>

          <!-- Hiển thị lỗi chi tiết từ backend -->
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

// ✅ Khớp đúng CreateProductDto: { name, price, stock, categoryId }
const blank = () => ({ name: '', price: 0, stock: 0, categoryId: '' })
const form  = ref(blank())

const filtered = computed(() =>
  products.value.filter(p =>
    p.name.toLowerCase().includes(search.value.toLowerCase())
  )
)

async function load() {
  loading.value = true
  try {
    const { data } = await productApi.getAll()
    products.value = Array.isArray(data) ? data : []
  } catch (e) {
    console.error('Load error:', e)
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
    stock:      0,
    categoryId: 1
  }
  formError.value = ''
  showModal.value = true
}

async function handleSubmit() {
  formError.value  = ''
  submitting.value = true
  try {
    const payload = {
      name:       form.value.name,
      price:      form.value.price,
      stock:      form.value.stock,
      categoryId: form.value.categoryId
    }
    if (editingId.value) {
      await productApi.update(editingId.value, payload)
    } else {
      await productApi.create(payload)
    }
    showModal.value = false
    await load()
  } catch (e) {
    // Hiển thị lỗi chi tiết nhất có thể
    const d = e.response?.data
    formError.value = d?.message
      || d?.title
      || (d?.errors ? JSON.stringify(d.errors) : null)
      || `Lỗi ${e.response?.status}: Không thêm được sản phẩm`
  } finally { submitting.value = false }
}

async function handleDelete(id) {
  if (!confirm('Xác nhận xóa sản phẩm này?')) return
  try {
    await productApi.delete(id)
    await load()
  } catch (e) {
    alert('Xóa thất bại: ' + (e.response?.data?.message || e.message))
  }
}

const fmt = v =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v)

onMounted(load)
</script>