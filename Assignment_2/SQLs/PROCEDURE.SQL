--DROP PROCEDURE sort_Supplier;
CREATE OR REPLACE PROCEDURE sort_Supplier(
    lower_date IN DATE, 
    upper_date IN DATE
)
AS
BEGIN
    FOR supplier_record IN(
        -- sort supplier in increasing number of categories provide
        SELECT suppliercode AS Scode, COUNT(Esuppliercode) AS count_supply 
        FROM (
            SELECT DISTINCT S.suppliercode, B.Esuppliercode 
            FROM supplier S LEFT JOIN boltstock B
            ON S.suppliercode = B.esuppliercode
                AND CAST(dateimported AS DATE) < CAST(upper_date AS DATE)
                AND CAST(dateimported AS DATE) > CAST(lower_date AS DATE)
        )
        GROUP BY suppliercode
        ORDER BY count_supply
    )
    -- print out to console
    LOOP
        dbms_output.put_line('Supplier_id: ' || supplier_record.Scode 
                            || ' provide : ' || supplier_record.count_supply || ' categories');
    END LOOP;
END;
    



