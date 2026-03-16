import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import App       from './App.vue'
import Products  from './views/Products.vue'
import Orders    from './views/Orders.vue'
import Dashboard from './views/Dashboard.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/',          redirect: '/products' },
    { path: '/products',  component: Products  },
    { path: '/orders',    component: Orders    },
    { path: '/dashboard', component: Dashboard }
  ]
})

createApp(App).use(router).mount('#app')