import axios from 'axios'

const api = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' }
})

// ── Products ──────────────────────────────────────────
export const productApi = {
  getAll:  ()         => api.get('/products'),
  getById: id         => api.get(`/products/${id}`),
  create:  data       => api.post('/products', data),
  update:  (id, data) => api.put(`/products/${id}`, data),
  delete:  id         => api.delete(`/products/${id}`)
  // Không có /categories và /low-stock
}

// ── Orders ────────────────────────────────────────────
export const orderApi = {
  getAll:       ()              => api.get('/orders'),
  getById:      id              => api.get(`/orders/${id}`),
  create:       data            => api.post('/orders', data),
  // ✅ PUT thay vì PATCH — khớp với backend của bạn
  updateStatus: (id, newStatus) => api.put(`/orders/${id}/status`, { newStatus })
}

// ── Reports ───────────────────────────────────────────
export const reportApi = {
  getRevenue:     (from, to)  => api.get('/reports/revenue', { params: { from, to } }),
  getTopProducts: (limit = 5) => api.get('/reports/top-products', { params: { limit } })
  // Không có /dashboard
}

export default api