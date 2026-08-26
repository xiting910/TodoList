using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoList.API.Models;

/// <summary>
/// 表示一个待办事项
/// </summary>
public sealed class TodoItem
{
    /// <summary>
    /// 获取或设置待办事项的唯一标识符
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    /// <summary>
    /// 获取或设置待办事项是否已完成
    /// </summary>
    public bool IsCompleted { get; internal set; }

    /// <summary>
    /// 获取或设置待办事项的描述
    /// </summary>
    [StringLength(500)]
    public string Description { get; internal set; } = string.Empty;

    /// <summary>
    /// 获取或设置待办事项的创建时间
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
