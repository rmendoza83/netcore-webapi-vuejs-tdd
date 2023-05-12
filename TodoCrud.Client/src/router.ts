import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'

import TodoComponent from '@/components/Todo.vue';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'todo',
    component: TodoComponent
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
