-- Create the user that will be used
ALTER SESSION SET "_oracle_script" = TRUE;

-- Drop user if already existed
DROP USER Assignment_2 CASCADE;

CREATE USER Assignment_2 IDENTIFIED BY "123456";
GRANT DBA TO Assignment_2;