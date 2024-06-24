CREATE OR ALTER PROCEDURE [dbo].[Sp_BroughtBy_Interview_ClientNames]
	@boughtBy int,
	--Authorization
	@loginUserType int,
	@loginUserId int
AS
BEGIN
	
	Select 
		client.Id as Id,
		client.organization_name as Name,
		client.acc_shortname as ShortName

	From 
		[pi_crm_qa].[dbo].[tbl_account_master] AS client WITH(NOLOCK) 
	WHERE
		client.Id in (
			select distinct ClientID from dbo.vwDashboardCandidateInterview as intrvw with(nolock) where intrvw.bdmId = @boughtBy 
		)

END