/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [id]
      ,[uuid]
      ,[name]
      ,[email]
      ,[contactNumber]
      ,[paymentMethod]
      ,[totalAmount]
      ,[productDetails]
      ,[createdBy]
  FROM [CarRental].[dbo].[Bill]