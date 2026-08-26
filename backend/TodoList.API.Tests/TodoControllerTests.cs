using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.API.Controllers;
using TodoList.API.Models;

namespace TodoList.API.Tests;

/// <summary>
/// <see cref="TodoController"/> 的单元测试
/// </summary>
public sealed class TodoControllerTests : IDisposable
{
    /// <summary>
    /// 用于测试的数据库上下文
    /// </summary>
    private readonly TodoDbContext _context;

    /// <summary>
    /// 用于测试的控制器实例
    /// </summary>
    private readonly TodoController _controller;

    /// <summary>
    /// 初始化测试所需的资源
    /// </summary>
    public TodoControllerTests()
    {
        var guid = Guid.NewGuid().ToString();
        var optionsBuilder = new DbContextOptionsBuilder<TodoDbContext>().UseInMemoryDatabase(guid);
        _context = new(optionsBuilder.Options);
        _controller = new(_context);
    }

    /// <summary>
    /// 释放测试资源
    /// </summary>
    public void Dispose()
    {
        _context.Dispose();
    }

    /// <summary>
    /// 测试 <see cref="TodoController.GetAll"/> 方法在数据库为空时返回空列表
    /// </summary>
    [Fact]
    public async Task GetAll_EmptyDatabase_ReturnsEmptyList()
    {
        var result = await _controller.GetAll(TestContext.Current.CancellationToken);

        var actionResult = Assert.IsType<ActionResult<List<TodoItem>>>(result);
        var items = Assert.IsType<List<TodoItem>>(actionResult.Value);
        Assert.Empty(items);
    }

    /// <summary>
    /// 测试 <see cref="TodoController.GetAll"/> 方法在数据库中有待办事项时返回所有事项
    /// </summary>
    [Fact]
    public async Task GetAll_WithItems_ReturnsAllItems()
    {
        var items = new List<TodoItem>
        {
            new() { Description = "测试项目1" },
            new() { Description = "测试项目2" },
            new() { Description = "测试项目3" }
        };
        await _context.TodoItems.AddRangeAsync(items, TestContext.Current.CancellationToken);
        _ = await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _controller.GetAll(TestContext.Current.CancellationToken);

        var actionResult = Assert.IsType<ActionResult<List<TodoItem>>>(result);
        var returnedItems = Assert.IsType<List<TodoItem>>(actionResult.Value);
        Assert.Equal(items.Count, returnedItems.Count);
    }

    /// <summary>
    /// 测试 <see cref="TodoController.GetById"/> 方法在待办事项存在时返回该事项
    /// </summary>
    [Fact]
    public async Task GetById_ExistingItem_ReturnsItem()
    {
        var item = new TodoItem { Description = "测试项目" };
        _ = await _context.TodoItems.AddAsync(item, TestContext.Current.CancellationToken);
        _ = await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _controller.GetById(item.Id, TestContext.Current.CancellationToken);

        var actionResult = Assert.IsType<ActionResult<TodoItem>>(result);
        var returnedItem = Assert.IsType<TodoItem>(actionResult.Value);
        Assert.Equal(item.Id, returnedItem.Id);
        Assert.Equal("测试项目", returnedItem.Description);
    }

    /// <summary>
    /// 测试 <see cref="TodoController.GetById"/> 方法在待办事项不存在时返回 <see cref="NotFoundResult"/>
    /// </summary>
    [Fact]
    public async Task GetById_NonExistingItem_ReturnsNotFound()
    {
        var result = await _controller.GetById(999, TestContext.Current.CancellationToken);

        var actionResult = Assert.IsType<ActionResult<TodoItem>>(result);
        _ = Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    /// <summary>
    /// 测试 <see cref="TodoController.Create"/> 方法在提供有效请求时成功创建待办事项
    /// </summary>
    [Fact]
    public async Task Create_ValidRequest_CreatesItem()
    {
        var request = new CreateRequest { Description = "新待办事项" };

        var result = await _controller.Create(request, TestContext.Current.CancellationToken);

        var actionResult = Assert.IsType<ActionResult<TodoItem>>(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        Assert.Equal(201, createdAtActionResult.StatusCode);

        var returnedItem = Assert.IsType<TodoItem>(createdAtActionResult.Value);
        Assert.Equal("新待办事项", returnedItem.Description);

        var savedItem = await _context.TodoItems.FindAsync(
            [returnedItem.Id], TestContext.Current.CancellationToken
        );
        Assert.NotNull(savedItem);
        Assert.Equal("新待办事项", savedItem.Description);
    }

    /// <summary>
    /// 测试 <see cref="TodoController.Update"/> 方法在待办事项存在时成功更新
    /// </summary>
    [Fact]
    public async Task Update_ExistingItem_UpdatesSuccessfully()
    {
        var item = new TodoItem { Description = "原始描述" };
        _ = await _context.TodoItems.AddAsync(item, TestContext.Current.CancellationToken);
        _ = await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new UpdateRequest
        {
            Description = "更新后的描述",
            IsCompleted = true
        };

        var result = await _controller.Update(item.Id, request, TestContext.Current.CancellationToken);

        _ = Assert.IsType<NoContentResult>(result);

        var updatedItem = await _context.TodoItems.FindAsync(
            [item.Id], TestContext.Current.CancellationToken
        );
        Assert.NotNull(updatedItem);
        Assert.Equal("更新后的描述", updatedItem.Description);
        Assert.True(updatedItem.IsCompleted);
    }

    /// <summary>
    /// 测试 <see cref="TodoController.Update"/> 方法在待办事项不存在时返回 <see cref="NotFoundResult"/>
    /// </summary>
    [Fact]
    public async Task Update_NonExistingItem_ReturnsNotFound()
    {
        var request = new UpdateRequest { Description = "更新描述" };

        var result = await _controller.Update(999, request, TestContext.Current.CancellationToken);

        _ = Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// 测试 <see cref="TodoController.Update"/> 方法只更新提供的字段
    /// </summary>
    [Fact]
    public async Task Update_PartialUpdate_OnlyUpdatesProvidedFields()
    {
        var item = new TodoItem { Description = "原始描述", IsCompleted = false };
        _ = await _context.TodoItems.AddAsync(item, TestContext.Current.CancellationToken);
        _ = await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new UpdateRequest { IsCompleted = true };

        var result = await _controller.Update(item.Id, request, TestContext.Current.CancellationToken);

        _ = Assert.IsType<NoContentResult>(result);

        var updatedItem = await _context.TodoItems.FindAsync(
            [item.Id], TestContext.Current.CancellationToken
        );
        Assert.NotNull(updatedItem);
        Assert.Equal("原始描述", updatedItem.Description);
        Assert.True(updatedItem.IsCompleted);
    }

    /// <summary>
    /// 测试 <see cref="TodoController.Delete"/> 方法在待办事项存在时成功删除
    /// </summary>
    [Fact]
    public async Task Delete_ExistingItem_DeletesSuccessfully()
    {
        var item = new TodoItem { Description = "待删除的项目" };
        _ = await _context.TodoItems.AddAsync(item, TestContext.Current.CancellationToken);
        _ = await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _controller.Delete(item.Id, TestContext.Current.CancellationToken);

        _ = Assert.IsType<NoContentResult>(result);

        var deletedItem = await _context.TodoItems.FindAsync(
            [item.Id], TestContext.Current.CancellationToken
        );
        Assert.Null(deletedItem);
    }

    /// <summary>
    /// 测试 <see cref="TodoController.Delete"/> 方法在待办事项不存在时返回 <see cref="NotFoundResult"/>
    /// </summary>
    [Fact]
    public async Task Delete_NonExistingItem_ReturnsNotFound()
    {
        var result = await _controller.Delete(999, TestContext.Current.CancellationToken);

        _ = Assert.IsType<NotFoundResult>(result);
    }
}
