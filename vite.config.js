import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        // ✅ Đổi thành đúng port backend của bạn
        target: 'https://localhost:7252',
        changeOrigin: true,
        secure: false    // ← bắt buộc khi dùng HTTPS localhost (cert tự ký)
      }
    }
  }
})