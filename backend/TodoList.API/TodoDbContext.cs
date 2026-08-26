using Microsoft.EntityFrameworkCore;

namespace TodoList.API;

/// <summary>
/// 表示一个用于管理待办事项的数据库上下文
/// </summary>
/// <param name="options">用于配置数据库上下文的选项</param>
public sealed class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    /// <summary>
    /// 获取或设置待办事项的数据库集
    /// </summary>
    public DbSet<Models.TodoItem> TodoItems { get; set; }
}
