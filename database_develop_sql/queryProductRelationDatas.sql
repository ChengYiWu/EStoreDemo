
DECLARE @id INT;
DECLARE @itemId INT;
SET @id = 24;

SELECT *
FROM Product
WHERE Id = @id;


SELECT *
FROM Attachment
WHERE ProductId = @id;

SELECT * 
FROM ProductItem
WHERE ProductId IN (SELECT Id FROM Product WHERE Id = @id)

SELECT *
FROM Attachment
WHERE ProductItemId IN (
	SELECT Id
	FROM ProductItem
	WHERE ProductId IN (SELECT Id FROM Product WHERE Id = @id)
) 