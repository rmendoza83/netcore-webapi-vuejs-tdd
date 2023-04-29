import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import * as sass from 'sass'

// https://vitejs.dev/config/
export default defineConfig({
  css: {
    preprocessorOptions: {
      sass: {
        implementation: sass
      }
    }
  },
  plugins: [vue()],
  resolve: {
    alias: [
      {
        find: '@',
        replacement: fileURLToPath(new URL('./src', import.meta.url))
      },
      {
        find: /~(.+)/,
        replacement: `${process.cwd()}/node_modules/$1`,
      }
    ]
  },
  server: {
    port: 3000,
  },
})
