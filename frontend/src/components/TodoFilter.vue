<script setup lang="ts">
import type { TodoFilter } from "../types/todo";

/**
 * 筛选栏组件
 */
defineProps<{
  /** 当前选中的筛选条件 */
  modelValue: TodoFilter;
}>();

const emit = defineEmits<{
  /** 筛选条件变化 */
  (e: "update:modelValue", value: TodoFilter): void;
}>();

/** 可选的筛选条件列表 */
const filters: Array<{ value: TodoFilter; label: string }> = [
  { value: "all", label: "全部" },
  { value: "active", label: "进行中" },
  { value: "completed", label: "已完成" },
];
</script>

<template>
  <div class="todo-filter" role="group" aria-label="筛选待办事项">
    <button
      v-for="filter in filters"
      :key="filter.value"
      class="todo-filter__item"
      :class="{ 'todo-filter__item--active': modelValue === filter.value }"
      type="button"
      :aria-pressed="modelValue === filter.value"
      :data-test="`filter-${filter.value}`"
      @click="emit('update:modelValue', filter.value)"
    >
      {{ filter.label }}
    </button>
  </div>
</template>

<style scoped>
.todo-filter {
  display: inline-flex;
  gap: var(--space-xs);
  padding: 4px;
  background: var(--color-bg);
  border-radius: var(--radius-md);
}

.todo-filter__item {
  padding: 6px 16px;
  font-size: 14px;
  color: var(--color-text-secondary);
  background: transparent;
  border: none;
  border-radius: var(--radius-sm);
  transition:
    color 0.2s ease,
    background-color 0.2s ease,
    box-shadow 0.2s ease;
}

.todo-filter__item:hover {
  color: var(--color-text);
}

.todo-filter__item--active {
  color: var(--color-primary);
  font-weight: 600;
  background: var(--color-surface);
  box-shadow: 0 1px 4px rgba(44, 51, 64, 0.1);
}
</style>
