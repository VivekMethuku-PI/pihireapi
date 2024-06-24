CREATE OR ALTER PROCEDURE [dbo].[Sp_BroughtBy_daywiseJob_ClientNames]
	@boughtBy int,
	@puId	int,
	@FromDate Datetime,
	@ToDate Datetime,
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
			select distinct ClientID 
			from 
				dbo.PH_JOB_OPENINGS as job with(nolock) 
				JOIN  dbo.PH_DAY_WISE_JOB_ACTIONS AS JobActions with(nolock) ON job.Id = JobActions.JOID
			where Status = 1 
				and JobOpeningStatus in (select ID from dbo.PH_JOB_STATUS_S where Status = 1 and JSCode not in ('HLD','CLS','RJT')) 
				and coalesce(job.BroughtBy,job.createdby) = @boughtBy 
				and (@puId is null or exists(select 1 from dbo.PH_JOB_OPENINGS_ADDL_DETAILS adt where adt.JOID=job.ID and adt.PUID=@puId))
				and (@FromDate is null or (JobActions.CreatedDate >= @FromDate and JobActions.CreatedDate <= @ToDate))
		)

END