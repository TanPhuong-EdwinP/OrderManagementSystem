import { createRouter, createWebHistory } from 'vue-router'
import Login      from '../views/Login.vue'
import AdminHome  from '../views/admin/AdminHome.vue'
import UserHome   from '../views/user/UserHome.vue'
import Products   from '../views/Products.vue'
import Orders     from '../views/Orders.vue'
import Dashboard  from '../views/Dashboard.vue'
import UserShop   from '../views/user/UserShop.vue'
import UserCart   from '../views/user/UserCart.vue'
import UserOrders from '../views/user/UserOrders.vue'
import UserProfile from '../views/user/UserProfile.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/',          redirect: '/login' },
    { path: '/login',     component: Login   },

    // ── Admin routes ──────────────────────────────
    { path: '/admin',           component: AdminHome,  meta: { requiresAuth: true, adminOnly: true } },
    { path: '/admin/products',  component: Products,   meta: { requiresAuth: true, adminOnly: true } },
    { path: '/admin/orders',    component: Orders,     meta: { requiresAuth: true, adminOnly: true } },
    { path: '/admin/dashboard', component: Dashboard,  meta: { requiresAuth: true, adminOnly: true } },

    // ── User routes ───────────────────────────────
    { path: '/home',    component: UserHome,    meta: { requiresAuth: true, userOnly: true } },
    { path: '/shop',    component: UserShop,    meta: { requiresAuth: true, userOnly: true } },
    { path: '/cart',    component: UserCart,    meta: { requiresAuth: true, userOnly: true } },
    { path: '/orders',  component: UserOrders,  meta: { requiresAuth: true, userOnly: true } },
    { path: '/profile', component: UserProfile, meta: { requiresAuth: true, userOnly: true } },
  ]
})

router.beforeEach((to) => {
  const token = localStorage.getItem('token')
  const user  = JSON.parse(localStorage.getItem('user') || '{}')
  const isAdmin = user.role === 'Admin'

  if (to.path === '/login' && token)
    return isAdmin ? '/admin' : '/home'

  if (to.meta.requiresAuth && !token) return '/login'
  if (to.meta.adminOnly && !isAdmin)  return '/home'
  if (to.meta.userOnly  &&  isAdmin)  return '/admin'
})

export default router