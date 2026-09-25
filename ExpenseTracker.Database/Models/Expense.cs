using System;
using System.Collections.Generic;

namespace ExpenseTracker.Database.Models;

public partial class Expense
{
    public int ExpenseId { get; set; }

    public int UserId { get; set; }

    public int CategoryId { get; set; }

    public decimal Amount { get; set; }

    public DateOnly Date { get; set; }

    public string? Notes { get; set; }

    public bool DelFlg { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
