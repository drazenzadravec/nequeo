EXECUTE sp_addrolemember @rolename = N'db_owner', @membername = N'nequeo';


GO
EXECUTE sp_addrolemember @rolename = N'db_owner', @membername = N'DEVELOPMENT\PC';


GO
EXECUTE sp_addrolemember @rolename = N'db_accessadmin', @membername = N'nequeo';


GO
EXECUTE sp_addrolemember @rolename = N'db_securityadmin', @membername = N'nequeo';


GO
EXECUTE sp_addrolemember @rolename = N'db_ddladmin', @membername = N'nequeo';


GO
EXECUTE sp_addrolemember @rolename = N'db_backupoperator', @membername = N'nequeo';


GO
EXECUTE sp_addrolemember @rolename = N'db_datareader', @membername = N'nequeo';


GO
EXECUTE sp_addrolemember @rolename = N'db_datawriter', @membername = N'nequeo';


GO
EXECUTE sp_addrolemember @rolename = N'db_denydatareader', @membername = N'nequeo';


GO
EXECUTE sp_addrolemember @rolename = N'db_denydatawriter', @membername = N'nequeo';

