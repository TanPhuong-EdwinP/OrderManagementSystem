<template>
  <div id="app">
    <nav class="navbar">
      <span class="brand">💧 Mineral Water</span>
      <div class="nav-links">
        <router-link to="/products"  class="nav-link">Sản phẩm</router-link>
        <router-link to="/orders"    class="nav-link">Đơn hàng</router-link>
        <router-link to="/dashboard" class="nav-link">Báo cáo</router-link>
      </div>
    </nav>
    <main style="padding-top:56px">
      <router-view />
    </main>
  </div>
</template>

<script setup></script>

<style scoped>
.navbar {
  display: flex; align-items: center; gap: 24px;
  height: 56px; padding: 0 24px;
  background: #fff; border-bottom: 1px solid #e8eaed;
  position: fixed; top: 0; left: 0; right: 0; z-index: 200;
}
.brand     { font-size: 15px; font-weight: 700; color: #1a73e8; }
.nav-links { display: flex; gap: 4px; }
.nav-link  {
  padding: 7px 14px; border-radius: 7px;
  text-decoration: none; color: #5f6368;
  font-size: 13.5px; font-weight: 500;
}
.nav-link:hover              { background: #f1f3f4; }
.nav-link.router-link-active { background: #e8f0fe; color: #1a73e8; }
</style>