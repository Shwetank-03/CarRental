/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [id]
      ,[name]
      ,[contactNumber]
      ,[email]
      ,[password]
      ,[status]
      ,[role]
  FROM [CarRental].[dbo].[Users]