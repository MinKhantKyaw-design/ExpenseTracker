using System;
using System.Collections.Generic;

namespace ExpenseTracker.Database.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool DelFlg { get; set; }

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
