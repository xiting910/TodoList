using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.API.Models;
using Token = System.Threading.CancellationToken;

namespace TodoList.API.Controllers;

/// <summary>
/// 表示一个用于处理待办事项的控制器
/// </summary>
/// <param name="context">数据库上下文</param>
[ApiController]
[Route("api/[controller]")]
public sealed class TodoController(TodoDbContext context) : ControllerBase
{
    /// <summary>
    /// 表示操作失败时的错误状态码
    /// </summary>
    internal const int ErrorStatusCode = 500;

    /// <summary>
    /// 获取所有待办事项
    /// </summary>
    /// <param name="token">用于取消操作的令牌</param>
    /// <returns>包含所有待办事项的列表</returns>
    /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
    [HttpGet]
    public async Task<ActionResult<List<TodoItem>>> GetAll(Token token)
    {
        return await context.TodoItems.ToListAsync(token);
    }

    /// <summary>
    /// 根据ID获取指定的待办事项
    /// </summary>
    /// <param name="id">待办事项的 ID </param>
    /// <param name="token">用于取消操作的令牌</param>
    /// <returns>指定 ID 的待办事项, 如果不存在则返回 <see cref="NotFoundResult"/> </returns>
    /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetById(int id, Token token)
    {
        var item = await context.TodoItems.FindAsync([id], token);
        return item is null ? NotFound() : item;
    }

    /// <summary>
    /// 创建一个新的待办事项
    /// </summary>
    /// <param name="request">创建请求</param>
    /// <param name="token">用于取消操作的令牌</param>
    /// <returns>创建的待办事项</returns>
    /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create([FromBody] CreateRequest request, Token token)
    {
        var item = new TodoItem
        {
            Description = request.Description,
        };

        try
        {
            _ = context.TodoItems.Add(item);
            _ = await context.SaveChangesAsync(token);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
        catch (DbUpdateException)
        {
            return Problem("An error occurred while creating the todo item.", statusCode: ErrorStatusCode);
        }
    }

    /// <summary>
    /// 更新指定的待办事项
    /// </summary>
    /// <param name="id">待办事项的 ID</param>
    /// <param name="request">更新请求</param>
    /// <param name="token">用于取消操作的令牌</param>
    /// <returns>如果更新成功则返回 <see cref="NoContentResult"/>, 如果待办事项不存在则返回
    /// <see cref="NotFoundResult"/></returns>
    /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRequest request, Token token)
    {
        var existing = await context.TodoItems.FindAsync([id], token);
        if (existing is null) { return NotFound(); }

        if (request.Description is not null)
        {
            existing.Description = request.Description;
        }
        if (request.IsCompleted.HasValue)
        {
            existing.IsCompleted = request.IsCompleted.Value;
        }

        try
        {
            _ = await context.SaveChangesAsync(token);
            return NoContent();
        }
        catch (DbUpdateException)
        {
            return Problem("An error occurred while updating the todo item.", statusCode: ErrorStatusCode);
        }
    }

    /// <summary>
    /// 删除指定的待办事项
    /// </summary>
    /// <param name="id">待办事项的ID</param>
    /// <param name="token">用于取消操作的令牌</param>
    /// <returns>如果删除成功则返回 <see cref="NoContentResult"/>, 如果待办事项不存在则返回
    /// <see cref="NotFoundResult"/></returns>
    /// <exception cref="OperationCanceledException">当操作被取消时抛出</exception>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, Token token)
    {
        var item = await context.TodoItems.FindAsync([id], token);
        if (item is null) { return NotFound(); }

        try
        {
            _ = context.TodoItems.Remove(item);
            _ = await context.SaveChangesAsync(token);
            return NoContent();
        }
        catch (DbUpdateException)
        {
            return Problem("An error occurred while deleting the todo item.", statusCode: ErrorStatusCode);
        }
    }
}
