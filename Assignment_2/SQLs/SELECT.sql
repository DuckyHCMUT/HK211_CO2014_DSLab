SELECT OrderList.OrderCode AS ORDER_CODE, 
       OrderList.OrderCustomerCode AS CUSTOMER_CODE,
       OrderList.TotalPrice AS TOTAL_PRICE,
       OrderList.ProcessEmployeeCode AS PROCESS_BY,
       OrderList.ProcessDate AS PROCESS_DATE,
       OrderList.IsCancelled AS IS_CANCELLED,
       OrderList.CancelReason AS CANCEL_REASON
FROM BoltStock, OrderList, Supplier
WHERE 
(
    BoltStock.ContainInOrderCode = OrderList.OrderCode 
    AND BoltStock.ESupplierCode = Supplier.SupplierCode
)
AND Supplier.Name = 'Silk Agency';


