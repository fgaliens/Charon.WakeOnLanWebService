import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  build: {
    // lightningcss can't parse "@media (not(hover))" in 98.css; 98.css is already minified.
    cssMinify: false,
  },
})
