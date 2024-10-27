/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [id]
      ,[name]
      ,[categoryId]
      ,[description]
      ,[price]
      ,[status]
  FROM [CarRental].[dbo].[Product]