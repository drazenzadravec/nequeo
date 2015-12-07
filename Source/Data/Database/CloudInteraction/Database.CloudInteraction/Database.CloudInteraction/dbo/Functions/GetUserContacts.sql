-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[GetUserContacts] 
(	
	-- Add the parameters for the function here
	@UserID BIGINT
)
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	(SELECT dbo.[User].UserID, dbo.[User].Username, dbo.[User].Password, dbo.[User].ApplicationLoginUserID, dbo.[User].UserSuspended, dbo.[User].LoggedIn, 
                      dbo.[User].LastLoginDate, dbo.[User].Email, dbo.[User].PasswordQuestion, dbo.[User].RoleID,
                      dbo.[User].PasswordAnswer, dbo.[User].ApplicationName, dbo.[User].FailedPasswordAttemptCount, dbo.[User].FailedPasswordAttemptWindowStart, 
                      dbo.[User].FailedPasswordAnswerAttemptCount, dbo.[User].FailedPasswordAnswerAttemptWindowStart, dbo.[User].UserSuspendedDate, 
                      dbo.[User].LastPasswordChangedDate, dbo.[User].LastActivityDate, dbo.[User].CreationDate, dbo.[User].IsApproved, dbo.[User].Comments, dbo.Contact.ContactID, 
                      dbo.Contact.UserID AS 'CUserID', dbo.Contact.UserContactID, dbo.Contact.ContactTypeID  AS 'CContactTypeID', dbo.ContactType.ContactTypeID, dbo.ContactType.CodeID, 
                      dbo.ContactType.Name, dbo.ContactType.Visible, dbo.ContactType.Description
	FROM [User] INNER JOIN [Contact] ON 
		[User].[UserID] = [Contact].[UserID] INNER JOIN [ContactType] ON
		[Contact].[ContactTypeID] = [ContactType].[ContactTypeID]
	WHERE ([Contact].[UserID] = @UserID))
	UNION
	(SELECT dbo.[User].UserID, dbo.[User].Username, dbo.[User].Password, dbo.[User].ApplicationLoginUserID, dbo.[User].UserSuspended, dbo.[User].LoggedIn, 
                      dbo.[User].LastLoginDate, dbo.[User].Email, dbo.[User].PasswordQuestion, dbo.[User].RoleID,
                      dbo.[User].PasswordAnswer, dbo.[User].ApplicationName, dbo.[User].FailedPasswordAttemptCount, dbo.[User].FailedPasswordAttemptWindowStart, 
                      dbo.[User].FailedPasswordAnswerAttemptCount, dbo.[User].FailedPasswordAnswerAttemptWindowStart, dbo.[User].UserSuspendedDate, 
                      dbo.[User].LastPasswordChangedDate, dbo.[User].LastActivityDate, dbo.[User].CreationDate, dbo.[User].IsApproved, dbo.[User].Comments, dbo.Contact.ContactID, 
                      dbo.Contact.UserID AS 'CUserID', dbo.Contact.UserContactID, dbo.Contact.ContactTypeID  AS 'CContactTypeID', dbo.ContactType.ContactTypeID, dbo.ContactType.CodeID, 
                      dbo.ContactType.Name, dbo.ContactType.Visible, dbo.ContactType.Description
	FROM [User] INNER JOIN [Contact] ON 
		[User].[UserID] = [Contact].[UserID] INNER JOIN [ContactType] ON
		[Contact].[ContactTypeID] = [ContactType].[ContactTypeID]
	WHERE ([Contact].[UserContactID] = @UserID))
)
