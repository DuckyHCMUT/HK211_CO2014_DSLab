-- Search by ID
SELECT DISTINCT
                S.SupplierCode AS SupplierID, 
                S.Name AS SupplierName, 
                SN.PhoneNo AS PhoneNumber

FROM Assignment_2.Supplier S, Assignment_2.SupplierPhoneNo SN 

WHERE S.SupplierCode = SN.ESupplierCode
AND S.SupplierCode LIKE '%5%';
/

-- Search by Name
SELECT DISTINCT
                S.SupplierCode AS SupplierID, 
                S.Name AS SupplierName, 
                SN.PhoneNo AS PhoneNumber

FROM Assignment_2.Supplier S, Assignment_2.SupplierPhoneNo SN 
WHERE S.SupplierCode = SN.ESupplierCode
AND (LOWER(S.Name) LIKE LOWER('%Demin%') OR
                S.SupplierCode LIKE '%S002%');

-- Display Supply
SELECT DISTINCT
                B.BoltCode AS BoltCode,
                B.DateImported AS ImpDate,
                B.Length AS BoltLength,    
                B.Quantity AS Quantity,
                B.PurchasePrice AS PurchasePrice,
                F.FabricName AS FabricType,
                S.SupplierCode AS SupplierID,
                S.Name AS SupplierName
FROM 
                Assignment_2.Supplier S, 
                Assignment_2.BoltStock B, 
                Assignment_2.FabricCategory F
WHERE 
                S.SupplierCode = B.ESupplierCode AND
                B.CategoryCode = F.FabricCode AND
                (LOWER(S.Name) LIKE LOWER('%Demin%') OR
                S.SupplierCode LIKE '%2%');