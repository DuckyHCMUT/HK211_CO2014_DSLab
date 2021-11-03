DROP VIEW order_detail_views;
DROP PACKAGE pkg_global_params;
DROP PACKAGE pkg_customer_params;

CREATE OR REPLACE PACKAGE pkg_global_params AS
    g_customerID VARCHAR(10);
    g_orderID VARCHAR(10);
END pkg_global_params;
/

-- Interface Package
CREATE OR REPLACE PACKAGE pkg_customer_params AS
    -- OrderID
    PROCEDURE set_orderID(p_orderID IN VARCHAR);
    FUNCTION get_orderID RETURN VARCHAR;
    -- CustomerID
    PROCEDURE set_customerID(p_customerID IN VARCHAR);
    FUNCTION get_customerID RETURN VARCHAR;
END pkg_customer_params;

/
-- Redefined Interface
CREATE OR REPLACE PACKAGE BODY pkg_customer_params AS
    -- Order ID
    PROCEDURE set_orderID(p_orderID IN VARCHAR)
    AS
    BEGIN
        pkg_global_params.g_orderID := p_orderID;
    END set_orderID;
    
    FUNCTION get_orderID RETURN VARCHAR
    IS
    BEGIN
        RETURN pkg_global_params.g_orderID;
    END get_orderID;
    
    --  Customer ID
    PROCEDURE set_customerID(p_customerID IN VARCHAR)
    AS
    BEGIN
        pkg_global_params.g_customerID := p_customerID;
    END set_customerID;
    
    FUNCTION get_customerID RETURN VARCHAR
    IS
    BEGIN
        RETURN pkg_global_params.g_customerID;
    END get_customerID;
END pkg_customer_params;
/

CREATE OR REPLACE VIEW order_detail_views AS
SELECT
    C.CustomerCode AS "Customer Code",
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
AND C.CustomerCode = nvl(pkg_customer_params.get_customerID, 0);
/