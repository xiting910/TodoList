import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'
import TodoFilter from '../TodoFilter.vue'

describe('TodoFilter', () => {
  it('渲染三个筛选按钮', () => {
    const wrapper = mount(TodoFilter, { props: { modelValue: 'all' } })
    expect(wrapper.findAll('[data-test^="filter-"]')).toHaveLength(3)
  })

  it('点击按钮触发 update:modelValue 事件', async () => {
    const wrapper = mount(TodoFilter, { props: { modelValue: 'all' } })
    await wrapper.get('[data-test="filter-active"]').trigger('click')
    expect(wrapper.emitted('update:modelValue')).toEqual([['active']])
  })

  it('当前选中的按钮具有 active 样式', () => {
    const wrapper = mount(TodoFilter, { props: { modelValue: 'completed' } })
    expect(wrapper.get('[data-test="filter-completed"]').classes()).toContain(
      'todo-filter__item--active',
    )
    expect(wrapper.get('[data-test="filter-all"]').classes()).not.toContain(
      'todo-filter__item--active',
    )
  })
})
