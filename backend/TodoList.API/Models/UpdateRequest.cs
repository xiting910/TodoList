using System.ComponentModel.DataAnnotations;

namespace TodoList.API.Models;

/// <summary>
/// 表示更新待办事项的请求模型
/// </summary>
public sealed class UpdateRequest
{
    /// <summary>
    /// 获取或设置待办事项是否已完成
    /// </summary>
    public bool? IsCompleted { get; init; }

    /// <summary>
    /// 获取或设置待办事项的描述
    /// </summary>
    [StringLength(500)]
    public string? Description { get; init; }
}
