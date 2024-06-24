--created by Balaji N for dashboard daywise filter
CREATE OR ALTER view [dbo].[vwDashboardDaywiseFilterData]
as
select 
	job.ID as jobId,
	coalesce(job.BroughtBy,job.createdby) bdmId,
	jobRecr.AssignedTo recruiterID,
	jobRecr.NoCVSRequired,
	jobRecr.NoCVSUploadded,
	jobRecr.[AssignmentDate] as jobAssignmentDate,
	jobRecr.NoOfFinalCVsFilled,
	jobDtl.PUID,
	jobDtl.BUID,
	job.ClosedDate
from 
	[dbo].[PH_JOB_ASSIGNMENTS_DAY_WISE] jobRecr with (nolock)
	inner join [dbo].[PH_JOB_OPENINGS] job with (nolock) on jobRecr.JOID = job.ID  and jobRecr.Status=1 and job.Status!=5
	inner join [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] jobDtl with (nolock) on jobDtl.JOID = job.ID
	inner join [dbo].[PI_HIRE_USERS] usr with (nolock) on usr.ID = jobRecr.AssignedTo and usr.Status!=5

GO