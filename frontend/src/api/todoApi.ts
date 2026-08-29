import type { CreateTodoRequest, TodoItem, UpdateTodoRequest } from '../types/todo'

/**
 * API 基础地址: 默认使用同源 /api (开发环境由 Vite 代理转发到后端),
 * 部署到独立域名时可通过环境变量 VITE_API_BASE_URL 覆盖
 */
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '/api'

/**
 * 发起请求并解析 JSON 响应, 非 2xx 状态码抛出错误
 * @param path 请求路径, 以 / 开头
 * @param init 请求配置
 * @returns 解析后的响应数据
 */
async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...init,
  })
  if (!response.ok) {
    throw new Error(`请求失败 (${response.status})`)
  }
  // 204 No Content 没有响应体, 直接返回 undefined
  if (response.status === 204) {
    return undefined as T
  }
  return (await response.json()) as T
}

/**
 * 待办事项 API 封装
 */
export const todoApi = {
  /**
   * 获取所有待办事项
   */
  list(): Promise<TodoItem[]> {
    return request('/todo')
  },

  /**
   * 创建待办事项
   * @param input 创建请求
   */
  create(input: CreateTodoRequest): Promise<TodoItem> {
    return request('/todo', {
      method: 'POST',
      body: JSON.stringify(input),
    })
  },

  /**
   * 更新待办事项
   * @param id 待办事项 ID
   * @param input 更新请求
   */
  update(id: number, input: UpdateTodoRequest): Promise<void> {
    return request(`/todo/${id}`, {
      method: 'PUT',
      body: JSON.stringify(input),
    })
  },

  /**
   * 删除待办事项
   * @param id 待办事项 ID
   */
  remove(id: number): Promise<void> {
    return request(`/todo/${id}`, {
      method: 'DELETE',
    })
  },
}
