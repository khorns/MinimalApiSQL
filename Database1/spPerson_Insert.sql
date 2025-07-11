CREATE PROCEDURE [dbo].[spPerson_Insert]
	@FirstName NVARCHAR(100),
	@LastName NVARCHAR(100)
AS
BEGIN
	INSERT INTO Person (FirstName, LastName)
    VALUES (@FirstName, @LastName);
END	
