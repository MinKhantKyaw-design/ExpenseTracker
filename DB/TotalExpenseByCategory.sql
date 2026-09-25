SELECT
    cat.CategoryID,
    cat.Name AS CategoryName,
    SUM(exp.Amount) AS TotalAmount
FROM Expenses as exp
INNER JOIN Categories as cat
    ON exp.CategoryID = cat.CategoryID
WHERE exp.UserID = 1
GROUP BY
    cat.CategoryID,
    cat.Name
ORDER BY TotalAmount DESC;