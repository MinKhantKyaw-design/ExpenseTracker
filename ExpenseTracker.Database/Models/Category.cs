using System;
using System.Collections.Generic;

namespace ExpenseTracker.Database.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool DelFlg { get; set; }

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
