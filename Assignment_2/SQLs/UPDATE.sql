SELECT * FROM CurrentPrice;

UPDATE CurrentPrice
SET Price = Price*1.1
WHERE CurrentPrice.FabricCode IN
(
    SELECT FabricCategory.FabricCode
    FROM FabricCategory
    WHERE FabricCategory.FabricName = 'Silk'
)
AND CurrentPrice.PricedTime > '01-Sep-2020'
AND CurrentPrice.PricedTime IS NOT NULL;

SELECT * FROM CurrentPrice;

