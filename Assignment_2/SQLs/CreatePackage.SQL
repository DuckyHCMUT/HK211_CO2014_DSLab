/
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