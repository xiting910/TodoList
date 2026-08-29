<script setup lang="ts">
import TodoListItem from "./TodoListItem.vue";
import type { TodoItem } from "../types/todo";

/**
 * 待办事项列表组件, 处理加载中与空状态展示
 */
defineProps<{
  /** 要展示的待办事项列表 (已按筛选条件过滤) */
  items: TodoItem[];
  /** 是否正在加载 */
  loading?: boolean;
}>();

const emit = defineEmits<{
  /** 切换完成状态 */
  (e: "toggle", id: number): void;
  /** 更新描述 */
  (e: "update", id: number, description: string): void;
  /** 删除待办事项 */
  (e: "remove", id: number): void;
}>();
</script>

<template>
  <div class="todo-list">
    <p v-if="loading" class="todo-list__status" data-test="loading">
      加载中...
    </p>
    <p
      v-else-if="items.length === 0"
      class="todo-list__status"
      data-test="empty"
    >
      暂无待办事项, 添加一条开始吧
    </p>
    <ul v-else class="todo-list__items">
      <TodoListItem
        v-for="item in items"
        :key="item.id"
        :item="item"
        @toggle="emit('toggle', $event)"
        @update="(id, description) => emit('update', id, description)"
        @remove="emit('remove', $event)"
      />
    </ul>
  </div>
</template>

<style scoped>
.todo-list__items {
  display: flex;
  flex-direction: column;
  gap: var(--space-sm);
  list-style: none;
}

.todo-list__status {
  padding: var(--space-lg) var(--space-md);
  font-size: 14px;
  color: var(--color-text-secondary);
  text-align: center;
  background: var(--color-surface);
  border: 1px dashed var(--color-border);
  border-radius: var(--radius-md);
}
</style>
