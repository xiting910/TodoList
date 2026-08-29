<script setup lang="ts">
import { nextTick, ref } from "vue";
import type { TodoItem } from "../types/todo";

/**
 * 单条待办事项组件, 支持勾选完成、双击编辑描述、删除
 */
const props = defineProps<{
  /** 待办事项数据 */
  item: TodoItem;
}>();

const emit = defineEmits<{
  /** 切换完成状态 */
  (e: "toggle", id: number): void;
  /** 更新描述 */
  (e: "update", id: number, description: string): void;
  /** 删除待办事项 */
  (e: "remove", id: number): void;
}>();

const editing = ref(false);
const editDraft = ref("");
const editInput = ref<HTMLInputElement | null>(null);

/**
 * 格式化创建时间为本地可读时间
 * @param iso 后端返回的 ISO 8601 时间字符串
 */
function formatCreatedAt(iso: string): string {
  const date = new Date(iso);
  if (Number.isNaN(date.getTime())) {
    return "";
  }
  return date.toLocaleString("zh-CN", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  });
}

/**
 * 进入编辑模式, 聚焦输入框并选中全部文本
 */
async function startEdit(): Promise<void> {
  editDraft.value = props.item.description;
  editing.value = true;
  await nextTick();
  editInput.value?.focus();
  editInput.value?.select();
}

/**
 * 保存编辑结果, 描述为空或未变化时不触发事件
 */
function saveEdit(): void {
  const description = editDraft.value.trim();
  if (!editing.value) {
    return;
  }
  editing.value = false;
  if (description !== "" && description !== props.item.description) {
    emit("update", props.item.id, description);
  }
}

/**
 * 取消编辑, 恢复原描述
 */
function cancelEdit(): void {
  editing.value = false;
  editDraft.value = props.item.description;
}
</script>

<template>
  <li
    class="todo-item"
    :class="{ 'todo-item--completed': item.isCompleted }"
    data-test="todo-item"
  >
    <label class="todo-item__toggle">
      <input
        type="checkbox"
        class="todo-item__checkbox"
        :checked="item.isCompleted"
        :aria-label="item.isCompleted ? '标记为未完成' : '标记为已完成'"
        data-test="toggle"
        @change="emit('toggle', item.id)"
      />
      <span class="todo-item__checkmark" aria-hidden="true"></span>
    </label>

    <div class="todo-item__body">
      <input
        v-if="editing"
        ref="editInput"
        v-model="editDraft"
        class="todo-item__edit"
        type="text"
        maxlength="500"
        data-test="edit-input"
        @keydown.enter.prevent="saveEdit"
        @keydown.esc.prevent="cancelEdit"
        @blur="saveEdit"
      />
      <template v-else>
        <p
          class="todo-item__description"
          data-test="description"
          @dblclick="startEdit"
        >
          {{ item.description }}
        </p>
        <p class="todo-item__meta">
          <span>创建于 {{ formatCreatedAt(item.createdAt) }}</span>
          <span class="todo-item__hint">双击可编辑</span>
        </p>
      </template>
    </div>

    <button
      class="todo-item__delete"
      type="button"
      :aria-label="`删除: ${item.description}`"
      data-test="delete"
      @click="emit('remove', item.id)"
    >
      <svg
        viewBox="0 0 24 24"
        width="16"
        height="16"
        fill="none"
        stroke="currentColor"
        stroke-width="2"
        stroke-linecap="round"
        stroke-linejoin="round"
        aria-hidden="true"
      >
        <path
          d="M3 6h18M8 6V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2m3 0v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6h14"
        />
      </svg>
    </button>
  </li>
</template>

<style scoped>
.todo-item {
  display: flex;
  align-items: center;
  gap: var(--space-md);
  padding: var(--space-md);
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  transition:
    background-color 0.2s ease,
    box-shadow 0.2s ease;
}

.todo-item:hover {
  background: var(--color-surface-hover);
  box-shadow: var(--shadow-card);
}

.todo-item--completed .todo-item__description {
  color: var(--color-text-secondary);
  text-decoration: line-through;
}

/* 自定义复选框 */
.todo-item__toggle {
  display: inline-flex;
  cursor: pointer;
}

.todo-item__checkbox {
  position: absolute;
  opacity: 0;
  pointer-events: none;
}

.todo-item__checkmark {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 22px;
  height: 22px;
  border: 2px solid var(--color-border);
  border-radius: 50%;
  transition:
    background-color 0.2s ease,
    border-color 0.2s ease;
}

.todo-item__checkbox:checked + .todo-item__checkmark {
  background: var(--color-primary);
  border-color: var(--color-primary);
}

.todo-item__checkbox:checked + .todo-item__checkmark::after {
  content: "";
  width: 10px;
  height: 5px;
  border-bottom: 2px solid #ffffff;
  border-left: 2px solid #ffffff;
  transform: translateY(-1px) rotate(-45deg);
}

.todo-item__checkbox:focus-visible + .todo-item__checkmark {
  box-shadow: 0 0 0 3px var(--color-primary-light);
}

.todo-item__body {
  flex: 1;
  min-width: 0;
}

.todo-item__description {
  font-size: 15px;
  line-height: 1.5;
  word-break: break-word;
  cursor: text;
}

.todo-item__meta {
  display: flex;
  gap: var(--space-md);
  margin-top: 2px;
  font-size: 12px;
  color: var(--color-text-secondary);
}

.todo-item__hint {
  display: none;
}

.todo-item:hover .todo-item__hint {
  display: inline;
}

.todo-item__edit {
  width: 100%;
  padding: 6px 10px;
  font-size: 15px;
  color: var(--color-text);
  background: var(--color-surface);
  border: 2px solid var(--color-primary);
  border-radius: var(--radius-sm);
  outline: none;
}

.todo-item__delete {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  color: var(--color-text-secondary);
  background: transparent;
  border: none;
  border-radius: var(--radius-sm);
  opacity: 0;
  transition:
    color 0.2s ease,
    background-color 0.2s ease,
    opacity 0.2s ease;
}

.todo-item:hover .todo-item__delete,
.todo-item__delete:focus-visible {
  opacity: 1;
}

.todo-item__delete:hover {
  color: var(--color-danger);
  background: var(--color-danger-light);
}
</style>
