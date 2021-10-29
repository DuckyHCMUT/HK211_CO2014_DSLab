CREATE OR REPLACE FUNCTION supplier_payment(supplier_id in VARCHAR2)
RETURN DECIMAL
    AS TotalPrice DECIMAL(19, 4);
BEGIN
    SELECT 
        SUM(B.PurchasePrice) INTO TotalPrice
    FROM 
        Supplier S, BoltStock B
    WHERE
        S.SupplierCode = supplier_id AND
        S.SupplierCode = B.ESupplierCode;
    --GROUP BY S.SupplierCode;
    RETURN TotalPrice;
END;



