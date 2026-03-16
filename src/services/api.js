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
}

// ── Orders ────────────────────────────────────────────
export const orderApi = {
  getAll:       ()           => api.get('/orders'),
  getById:      id           => api.get(`/orders/${id}`),
  create:       data         => api.post('/orders', data),
  // ✅ PUT /{id}/status nhận { status: string }
  updateStatus: (id, status) => api.put(`/orders/${id}/status`, { status })
}

// ── Reports ───────────────────────────────────────────
// ✅ GetRevenue() và GetTopProducts() không nhận param
export const reportApi = {
  getRevenue:     () => api.get('/reports/revenue'),
  getTopProducts: () => api.get('/reports/top-products')
}

export default api