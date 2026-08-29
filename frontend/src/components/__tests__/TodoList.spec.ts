import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'
import type { TodoItem } from '../../types/todo'
import TodoList from '../TodoList.vue'

const items: TodoItem[] = [
  { id: 1, isCompleted: false, description: '任务一', createdAt: '2026-08-26T10:00:00Z' },
  { id: 2, isCompleted: true, description: '任务二', createdAt: '2026-08-26T11:00:00Z' },
]

describe('TodoList', () => {
  it('渲染待办事项列表', () => {
    const wrapper = mount(TodoList, { props: { items } })
    expect(wrapper.findAll('[data-test="todo-item"]')).toHaveLength(2)
    expect(wrapper.text()).toContain('任务一')
    expect(wrapper.text()).toContain('任务二')
  })

  it('加载中显示加载状态', () => {
    const wrapper = mount(TodoList, { props: { items: [], loading: true } })
    expect(wrapper.find('[data-test="loading"]').exists()).toBe(true)
  })

  it('空列表显示空状态提示', () => {
    const wrapper = mount(TodoList, { props: { items: [] } })
    expect(wrapper.find('[data-test="empty"]').exists()).toBe(true)
  })

  it('转发子组件的 toggle 事件', async () => {
    const wrapper = mount(TodoList, { props: { items } })
    await wrapper.get('[data-test="toggle"]').trigger('change')
    expect(wrapper.emitted('toggle')).toEqual([[1]])
  })
})
