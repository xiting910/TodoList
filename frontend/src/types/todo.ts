/**
 * 待办事项的数据模型, 与后端 TodoItem 保持一致
 */
export interface TodoItem {
  /** 待办事项的唯一标识 */
  id: number
  /** 是否已完成 */
  isCompleted: boolean
  /** 待办事项的描述 */
  description: string
  /** 创建时间 (ISO 8601 字符串) */
  createdAt: string
}

/**
 * 创建待办事项的请求体
 */
export interface CreateTodoRequest {
  description: string
}

/**
 * 更新待办事项的请求体, 字段均为可选, 只更新传入的字段
 */
export interface UpdateTodoRequest {
  description?: string
  isCompleted?: boolean
}

/**
 * 列表筛选条件
 */
export type TodoFilter = 'all' | 'active' | 'completed'
