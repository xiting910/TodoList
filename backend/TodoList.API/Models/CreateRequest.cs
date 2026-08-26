using System.ComponentModel.DataAnnotations;

namespace TodoList.API.Models;

/// <summary>
/// 创建待办事项的请求模型
/// </summary>
public sealed class CreateRequest
{
    /// <summary>
    /// 获取或设置待办事项的描述
    /// </summary>
    [Required]
    [StringLength(500)]
    public required string Description { get; init; }
}
