-- Order detail - For part 4
CREATE OR REPLACE VIEW order_detail_views AS
SELECT
    C.CustomerCode AS "Customer Code"
    O.OrderCode AS "Order Code",
    C.FName AS "FName",
    C.LName AS "LName",
    O.TotalPrice AS "Price",
    O.ProcessDate AS "Process Date",
    O.OrderStatus AS "Status",
    B.BoltCode AS "Bolt",
    F.FabricName AS "Fabric Name"
FROM OrderList O, Customer C, BoltStock B, FabricCategory F
WHERE 
    O.OrderCustomerCode = C.CustomerCode
AND B.ContainInOrderCode = O.OrderCode
AND F.FabricCode = B.CategoryCode
AND C.CustomerCode = nvl(pkg_customer_params.get_customerID, 0)
;