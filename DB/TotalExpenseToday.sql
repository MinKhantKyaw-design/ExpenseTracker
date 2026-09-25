SELECT
    SUM(Amount) AS TotalSpentToday
FROM Expenses
WHERE UserID = 1
AND Date = CAST(GETDATE() AS DATE);