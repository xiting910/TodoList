import vue from '@vitejs/plugin-vue'
import { defineConfig } from 'vitest/config'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    // 开发环境下将 /api 请求代理到后端, 避免跨域问题
    proxy: {
      '/api': {
        target: 'http://localhost:5273',
        changeOrigin: true,
      },
    },
  },
  test: {
    environment: 'jsdom',
    passWithNoTests: true,
  },
})
