import axios from 'axios'

const api = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' }
})

api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

api.interceptors.response.use(
  res => res,
  err => {
    if (err.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      window.location.href = '/login'
    }
    return Promise.reject(err)
  }
)

export const authApi = {
  login:    data => api.post('/auth/login', data),
  register: data => api.post('/auth/register', data)
}

export const productApi = {
  getAll:  ()         => api.get('/products'),
  getById: id         => api.get(`/products/${id}`),
  create:  data       => api.post('/products', data),
  update:  (id, data) => api.put(`/products/${id}`, data),
  delete:  id         => api.delete(`/products/${id}`)
}

export const cartApi = {
  get:    ()         => api.get('/cart'),
  add:    data       => api.post('/cart', data),
  update: (id, data) => api.put(`/cart/${id}`, data),
  remove: id         => api.delete(`/cart/${id}`),
  clear:  ()         => api.delete('/cart/clear')
}

export const orderApi = {
  getAll:       ()           => api.get('/orders'),
  getById:      id           => api.get(`/orders/${id}`),
  create:       data         => api.post('/orders', data),
  updateStatus: (id, status) => api.put(`/orders/${id}/status`, { status }),
  cancel:       id           => api.put(`/orders/${id}/cancel`)
}

export const userApi = {
  getProfile:    ()    => api.get('/users/profile'),
  updateProfile: data  => api.put('/users/profile', data)
}

export const reportApi = {
  getRevenue:     () => api.get('/reports/revenue'),
  getTopProducts: () => api.get('/reports/top-products')
}

export default api