import { createRouter, createWebHistory } from 'vue-router'
import Login     from '@/views/Login.vue'
import Products  from '@/views/Products.vue'
import Orders    from '@/views/Orders.vue'
import Dashboard from '@/views/Dashboard.vue'

const routes = [
  { path: '/',          redirect: '/products' },
  { path: '/login',     component: Login },
  { path: '/products',  component: Products,  meta: { requiresAuth: true } },
  { path: '/orders',    component: Orders,    meta: { requiresAuth: true } },
  { path: '/dashboard', component: Dashboard, meta: { requiresAuth: true, adminOnly: true } }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

// Route guard: kiểm tra đăng nhập trước khi vào trang
router.beforeEach((to) => {
  const token = localStorage.getItem('token')
  const user  = JSON.parse(localStorage.getItem('user') || '{}')
  if (to.meta.requiresAuth && !token)           return '/login'
  if (to.meta.adminOnly && user.role !== 'Admin') return '/products'
})

export default router