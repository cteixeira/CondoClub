EXECUTE sp_addsrvrolemember @loginame = N'CT\c.teixeira', @rolename = N'sysadmin';


GO
EXECUTE sp_addsrvrolemember @loginame = N'NT AUTHORITY\SYSTEM', @rolename = N'sysadmin';

