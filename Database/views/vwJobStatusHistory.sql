USE [piHIRE1.0_QA]
GO

CREATE view [dbo].[vwJobStatusHistory]
AS
    select Distinct
		ActvyLog.joid as JobId
		--,job.JobTitle,job.ClientID,job.ClientName
		--,ActvyLog.CurrentStatusId,ActvyLog.UpdateStatusId
		,JobStatus.JSCode StatusCode,oldJobStatus.JSCode OldStatusCode
		, CAST(ActvyLog.CreatedDate as DATE) ActivityDate 
		--,Jobdtls.PUID, Jobdtls.BUID
		--,coalesce(job.BroughtBy,job.createdby) BroughtBy
	from   [dbo].[PH_ACTIVITY_LOG] as ActvyLog with(nolock)
		   join [dbo].[PH_JOB_STATUS_S] as JobStatus with(nolock) on ActvyLog.UpdateStatusId = JobStatus.Id
		   left outer join [dbo].[PH_JOB_STATUS_S] as oldJobStatus with(nolock) on ActvyLog.CurrentStatusId = oldJobStatus.Id
		   --join [dbo].[PH_JOB_OPENINGS] as job with(nolock) on ActvyLog.joid = job.id   
		   --join [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] as Jobdtls with(nolock) on ActvyLog.joid = Jobdtls.joid
   where
		-- Job
		ActvyLog.ActivityMode = 2 
		-- StatusUpdates
		and ActvyLog.ActivityType = 3 
		and ActvyLog.UpdateStatusId is not null




GO


