<script setup lang="ts">
import { ref } from "vue";

/**
 * 待办事项输入组件
 */
defineProps<{
  /** 提交过程中禁用输入 */
  disabled?: boolean;
}>();

const emit = defineEmits<{
  /** 提交新的待办描述 */
  (e: "submit", description: string): void;
}>();

const draft = ref("");

/**
 * 提交输入内容, 空内容不触发事件, 成功后清空输入框
 */
function handleSubmit(): void {
  const description = draft.value.trim();
  if (description === "") {
    return;
  }
  emit("submit", description);
  draft.value = "";
}
</script>

<template>
  <form class="todo-input" @submit.prevent="handleSubmit">
    <input
      v-model="draft"
      class="todo-input__field"
      type="text"
      maxlength="500"
      placeholder="添加新的待办事项, 按回车确认"
      :disabled="disabled"
      data-test="input"
      @keydown.enter="handleSubmit"
    />
    <button
      class="todo-input__submit"
      type="submit"
      :disabled="disabled"
      data-test="submit"
    >
      添加
    </button>
  </form>
</template>

<style scoped>
.todo-input {
  display: flex;
  gap: var(--space-sm);
}

.todo-input__field {
  flex: 1;
  padding: 12px 16px;
  font-size: 15px;
  color: var(--color-text);
  background: var(--color-surface);
  border: 2px solid var(--color-border);
  border-radius: var(--radius-md);
  outline: none;
  transition:
    border-color 0.2s ease,
    box-shadow 0.2s ease;
}

.todo-input__field::placeholder {
  color: var(--color-text-secondary);
}

.todo-input__field:focus {
  border-color: var(--color-primary);
  box-shadow: 0 0 0 3px var(--color-primary-light);
}

.todo-input__field:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.todo-input__submit {
  padding: 12px 24px;
  font-size: 15px;
  font-weight: 600;
  color: #ffffff;
  background: var(--color-primary);
  border: none;
  border-radius: var(--radius-md);
  transition: background-color 0.2s ease;
}

.todo-input__submit:hover:not(:disabled) {
  background: var(--color-primary-hover);
}

.todo-input__submit:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>
