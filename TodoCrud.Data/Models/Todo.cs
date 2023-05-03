namespace TodoCrud.Data.Models;

using System;

public class Todo : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
