import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'
import type { TodoItem } from '../../types/todo'
import TodoListItem from '../TodoListItem.vue'

const item: TodoItem = {
  id: 1,
  isCompleted: false,
  description: '写周报',
  createdAt: '2026-08-26T10:00:00Z',
}

describe('TodoListItem', () => {
  it('渲染描述和创建时间', () => {
    const wrapper = mount(TodoListItem, { props: { item } })
    expect(wrapper.get('[data-test="description"]').text()).toBe('写周报')
    expect(wrapper.text()).toContain('创建于')
  })

  it('已完成时添加完成样式', () => {
    const wrapper = mount(TodoListItem, {
      props: { item: { ...item, isCompleted: true } },
    })
    expect(wrapper.get('[data-test="todo-item"]').classes()).toContain(
      'todo-item--completed',
    )
  })

  it('勾选复选框触发 toggle 事件', async () => {
    const wrapper = mount(TodoListItem, { props: { item } })
    await wrapper.get('[data-test="toggle"]').trigger('change')
    expect(wrapper.emitted('toggle')).toEqual([[1]])
  })

  it('点击删除按钮触发 remove 事件', async () => {
    const wrapper = mount(TodoListItem, { props: { item } })
    await wrapper.get('[data-test="delete"]').trigger('click')
    expect(wrapper.emitted('remove')).toEqual([[1]])
  })

  it('双击进入编辑, 回车保存修改', async () => {
    const wrapper = mount(TodoListItem, { props: { item } })
    await wrapper.get('[data-test="description"]').trigger('dblclick')
    const input = wrapper.get('[data-test="edit-input"]')
    await input.setValue('写季度周报')
    await input.trigger('keydown', { key: 'Enter' })
    expect(wrapper.emitted('update')).toEqual([[1, '写季度周报']])
  })

  it('编辑时按 Esc 取消修改', async () => {
    const wrapper = mount(TodoListItem, { props: { item } })
    await wrapper.get('[data-test="description"]').trigger('dblclick')
    const input = wrapper.get('[data-test="edit-input"]')
    await input.setValue('不保存的内容')
    await input.trigger('keydown', { key: 'Escape' })
    expect(wrapper.emitted('update')).toBeUndefined()
    expect(wrapper.get('[data-test="description"]').text()).toBe('写周报')
  })
})
