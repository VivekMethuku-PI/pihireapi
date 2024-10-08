USE [piHIRE1.0_QA]
GO
Alter PROCEDURE [dbo].[Sp_Dashboard_JobStage]
	@puIds nvarchar(max), 
	@buIds nvarchar(max), 
	--pagination
	@fetchCount int,--0 if no pagination
	@offsetCount int,
	--Authorization
	@userType int,
	@userId int
AS
begin
if(@fetchCount != 0)
	select 
		* 
	from 
		vwDashboardJobStage with (nolock)
	where JobOpeningStatus not in (select ID from [dbo].[PH_JOB_STATUS_S] where jscode in ('CLS')) and  
		( 
			(@userType = 1) or --SuperAdmin
			(@userType = 2 and jobId in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@userId)) or --Admin
			(@userType = 3 and @userId = bdmId) or --BDM
			(@userType = 4 and jobId in (SELECT [JOID]  FROM [dbo].[PH_JOB_ASSIGNMENTS] where [DeassignDate] is null and [status]!=5 and [AssignedTo] = @userId )) --Recruiter
			--Candidate - 5
			--Hire manager PH_JOB_OPENINGS_ADDL_DETAILS
		)
		and (LEN(coalesce(@puIds,''))=0 or (jobId in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) where PUID in (select cast(value as int) from string_split(@puIds,',')))))
		and (LEN(coalesce(@buIds,''))=0 or (jobId in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) where BUID in (select cast(value as int) from string_split(@buIds,',')))))
	order by jobId desc offset @offsetCount rows fetch next @fetchCount rows only;

else
	select 
		* 
	from 
		vwDashboardJobStage with (nolock)
	where JobOpeningStatus not in (select ID from [dbo].[PH_JOB_STATUS_S] where jscode in ('CLS')) and  
		( 
			(@userType = 1) or --SuperAdmin
			(@userType = 2 and jobId in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@userId)) or --Admin
			(@userType = 3 and @userId = bdmId) or --BDM
			(@userType = 4 and jobId in (SELECT [JOID]  FROM [dbo].[PH_JOB_ASSIGNMENTS] where [DeassignDate] is null and [status]!=5 and [AssignedTo] = @userId )) --Recruiter
			--Candidate - 5
			--Hire manager PH_JOB_OPENINGS_ADDL_DETAILS
		)
		and (LEN(coalesce(@puIds,''))=0 or (jobId in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) where PUID in (select cast(value as int) from string_split(@puIds,',')))))
		and (LEN(coalesce(@buIds,''))=0 or (jobId in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) where BUID in (select cast(value as int) from string_split(@buIds,',')))))
	order by jobId desc
end
Go

Alter PROCEDURE [dbo].[Sp_Dashboard_JobStageCount]
	@puIds nvarchar(max), 
	@buIds nvarchar(max), 
	--Authorization
	@userType int,
	@userId int
AS
begin
	select 
		count(1) TotCnt 
	from 
		vwDashboardJobStage with (nolock)
	where JobOpeningStatus not in (select ID from [dbo].[PH_JOB_STATUS_S] where jscode in ('CLS')) and 
		( 
			(@userType = 1) or --SuperAdmin
			(@userType = 2 and jobId in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@userId)) or --Admin
			(@userType = 3 and @userId = bdmId) or --BDM
			(@userType = 4 and jobId in (SELECT [JOID]  FROM [dbo].[PH_JOB_ASSIGNMENTS] where [DeassignDate] is null and [status]!=5 and [AssignedTo] = @userId )) --Recruiter
			--Candidate - 5
			--Hire manager PH_JOB_OPENINGS_ADDL_DETAILS
		)
		and (LEN(coalesce(@puIds,''))=0 or (jobId in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) where PUID in (select cast(value as int) from string_split(@puIds,',')))))
		and (LEN(coalesce(@buIds,''))=0 or (jobId in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) where BUID in (select cast(value as int) from string_split(@buIds,',')))))
end
Go




Alter view vwDashboardJobStage
as
select distinct
	job.clientName, job.JobTitle,job.ID as jobId, coalesce(job.BroughtBy,job.createdby) bdmId, job.PostedDate, job.ClosedDate,
	job.JobLocationID jobCityId, job.CountryID jobCountryId, job.JobOpeningStatus,
	jbDtl.NoOfCvsRequired, job.ReopenedDate,
	(select SUM(coalesce(NoOfFinalCVsFilled,0)) from [dbo].[PH_JOB_ASSIGNMENTS] jobRecr with (nolock) where Status!=5 and JOID=job.ID) NoOfCvsFullfilled
	--stageCnt.Sourcing, stageCnt.Screening, stageCnt.Submission, stageCnt.Interview,  stageCnt.Offered, stageCnt.Hired
from  
	[dbo].[PH_JOB_OPENINGS] job with (nolock)
	inner join PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with (nolock) on jbDtl.JOID=job.ID and job.status!=5
	--inner join (
	--	select piv.JOID,piv.[1] as Sourcing, [2] as Screening, [3] as Submission, [4] as Interview, [5] as Offered, [6] as Hired
	--	from
	--	(
	--	  select JOID,StageID,SUM([Counter]) as [Counter] from [dbo].[PH_JOB_OPENING_STATUS_COUNTER] with (nolock) group by JOID,StageID
	--	) d
	--	pivot
	--	(
	--	  max([Counter])
	--	  for StageID in ([1], [2], [3], [4], [5], [6])
	--	) piv
	--) stageCnt on stageCnt.JOID=job.ID 