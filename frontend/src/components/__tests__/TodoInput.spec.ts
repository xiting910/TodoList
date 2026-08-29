import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'
import TodoInput from '../TodoInput.vue'

describe('TodoInput', () => {
  it('渲染输入框和提交按钮', () => {
    const wrapper = mount(TodoInput)
    expect(wrapper.find('[data-test="input"]').exists()).toBe(true)
    expect(wrapper.get('[data-test="submit"]').text()).toBe('添加')
  })

  it('输入内容后回车触发 submit 事件并清空输入框', async () => {
    const wrapper = mount(TodoInput)
    const input = wrapper.get('[data-test="input"]')
    await input.setValue('  写周报  ')
    await input.trigger('keydown', { key: 'Enter' })
    expect(wrapper.emitted('submit')).toEqual([['写周报']])
    expect((input.element as HTMLInputElement).value).toBe('')
  })

  it('点击按钮提交内容', async () => {
    const wrapper = mount(TodoInput)
    await wrapper.get('[data-test="input"]').setValue('买菜')
    await wrapper.get('form').trigger('submit')
    expect(wrapper.emitted('submit')).toEqual([['买菜']])
  })

  it('空白内容不触发 submit 事件', async () => {
    const wrapper = mount(TodoInput)
    await wrapper.get('[data-test="input"]').setValue('   ')
    await wrapper.get('[data-test="submit"]').trigger('click')
    expect(wrapper.emitted('submit')).toBeUndefined()
  })

  it('disabled 时禁用输入框和按钮', () => {
    const wrapper = mount(TodoInput, { props: { disabled: true } })
    expect((wrapper.get('[data-test="input"]').element as HTMLInputElement).disabled).toBe(true)
    expect((wrapper.get('[data-test="submit"]').element as HTMLButtonElement).disabled).toBe(true)
  })
})
