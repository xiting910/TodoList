<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { todoApi } from "./api/todoApi";
import TodoInput from "./components/TodoInput.vue";
import TodoFilter from "./components/TodoFilter.vue";
import TodoList from "./components/TodoList.vue";
import type { TodoFilter as FilterValue, TodoItem } from "./types/todo";

/** 全部待办事项 */
const items = ref<TodoItem[]>([]);
/** 是否正在加载列表 */
const loading = ref(true);
/** 当前筛选条件 */
const filter = ref<FilterValue>("all");
/** 是否正在执行写操作 */
const submitting = ref(false);
/** 操作失败时的错误信息 */
const error = ref("");

/**
 * 按筛选条件过滤后的待办事项
 */
const filteredItems = computed(() => {
  switch (filter.value) {
    case "active":
      return items.value.filter((item) => !item.isCompleted);
    case "completed":
      return items.value.filter((item) => item.isCompleted);
    default:
      return items.value;
  }
});

/**
 * 未完成的待办数量
 */
const activeCount = computed(
  () => items.value.filter((item) => !item.isCompleted).length,
);

/**
 * 加载全部待办事项
 */
async function loadItems(): Promise<void> {
  loading.value = true;
  error.value = "";
  try {
    items.value = await todoApi.list();
  } catch {
    error.value = "加载待办事项失败, 请确认后端服务已启动";
  } finally {
    loading.value = false;
  }
}

/**
 * 新增待办事项
 * @param description 描述内容
 */
async function addTodo(description: string): Promise<void> {
  submitting.value = true;
  error.value = "";
  try {
    const created = await todoApi.create({ description });
    items.value.push(created);
  } catch {
    error.value = "新增待办事项失败, 请稍后重试";
  } finally {
    submitting.value = false;
  }
}

/**
 * 切换待办事项的完成状态
 * @param id 待办事项 ID
 */
async function toggleTodo(id: number): Promise<void> {
  const target = items.value.find((item) => item.id === id);
  if (target === undefined) {
    return;
  }
  // 乐观更新, 失败时回滚
  const previous = target.isCompleted;
  target.isCompleted = !previous;
  error.value = "";
  try {
    await todoApi.update(id, { isCompleted: target.isCompleted });
  } catch {
    target.isCompleted = previous;
    error.value = "更新待办事项失败, 请稍后重试";
  }
}

/**
 * 更新待办事项的描述
 * @param id 待办事项 ID
 * @param description 新描述
 */
async function updateTodo(id: number, description: string): Promise<void> {
  const target = items.value.find((item) => item.id === id);
  if (target === undefined) {
    return;
  }
  const previous = target.description;
  // 乐观更新, 失败时回滚
  target.description = description;
  error.value = "";
  try {
    await todoApi.update(id, { description });
  } catch {
    target.description = previous;
    error.value = "更新待办事项失败, 请稍后重试";
  }
}

/**
 * 删除待办事项
 * @param id 待办事项 ID
 */
async function removeTodo(id: number): Promise<void> {
  const index = items.value.findIndex((item) => item.id === id);
  if (index === -1) {
    return;
  }
  // 乐观更新, 失败时回滚
  const [removed] = items.value.splice(index, 1);
  error.value = "";
  try {
    await todoApi.remove(id);
  } catch {
    if (removed !== undefined) {
      items.value.splice(index, 0, removed);
    }
    error.value = "删除待办事项失败, 请稍后重试";
  }
}

onMounted(loadItems);
</script>

<template>
  <main class="app">
    <section class="app__card">
      <header class="app__header">
        <h1 class="app__title">待办清单</h1>
        <p class="app__subtitle">今天也要加油哦</p>
      </header>

      <p v-if="error" class="app__error" role="alert" data-test="error">
        {{ error }}
      </p>

      <TodoInput :disabled="submitting" @submit="addTodo" />

      <div class="app__toolbar">
        <TodoFilter v-model="filter" />
        <p class="app__count" data-test="active-count">
          还剩 {{ activeCount }} 项未完成
        </p>
      </div>

      <TodoList
        :items="filteredItems"
        :loading="loading"
        @toggle="toggleTodo"
        @update="updateTodo"
        @remove="removeTodo"
      />
    </section>
  </main>
</template>

<style scoped>
.app {
  display: flex;
  justify-content: center;
  padding: 48px 16px 64px;
}

.app__card {
  display: flex;
  flex-direction: column;
  gap: var(--space-md);
  width: 100%;
  max-width: 640px;
  padding: var(--space-lg);
  background: var(--color-surface);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-card);
}

.app__header {
  margin-bottom: var(--space-xs);
}

.app__title {
  font-size: 28px;
  font-weight: 700;
  letter-spacing: 0.5px;
}

.app__subtitle {
  margin-top: var(--space-xs);
  font-size: 14px;
  color: var(--color-text-secondary);
}

.app__error {
  padding: 10px 14px;
  font-size: 14px;
  color: var(--color-danger);
  background: var(--color-danger-light);
  border-radius: var(--radius-sm);
}

.app__toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--space-md);
  flex-wrap: wrap;
}

.app__count {
  font-size: 13px;
  color: var(--color-text-secondary);
}
</style>
