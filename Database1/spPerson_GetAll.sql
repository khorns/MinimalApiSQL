CREATE PROCEDURE [dbo].[spPerson_GetAll]
AS
begin
	SELECT [Id], [FirstName], [LastName] 
	from [dbo].[Person];

end