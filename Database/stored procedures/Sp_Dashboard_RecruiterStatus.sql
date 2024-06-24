USE [piHIRE1.0_QA]
GO
CREATE OR ALTER PROCEDURE [dbo].[Sp_Dashboard_RecruiterStatus]
	@currentDt datetime,
	@fmDt datetime,
	@toDt datetime,
	@onLeave bit,
	--Authorization
	@userType int,
	@userId int
AS
begin
	select 
		* 
	from 
		vwDashboardRecruiterStatus with (nolock)
	where (@onLeave is null or JobOpeningStatus not in (select ID from [dbo].[PH_JOB_STATUS_S] with(nolock) where jscode in ('CLS'))) and 
		( 
			(@userType = 1) or --SuperAdmin
			(@userType = 2 and jobId in (select JOID from [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw with(nolock) on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@userId)) or --Admin
			(@userType = 3 and @userId = bdmId) or --BDM
			(@userType = 4 and @userId = recruiterID) --Recruiter
			--Candidate 5
			--Hire manager [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS]
		)
		and ((ClosedDate >= @fmDt) or (ClosedDate<@fmDt and NoCVSRequired>NoOfFinalCVsFilled))
		and (@onLeave is null 
			or (@onLeave=1 and recruiterID in (select usr.ID from [dbo].[PH_EMP_LEAVE_REQUEST] leave with(nolock) inner join [dbo].[PI_HIRE_USERS] usr with(nolock) on usr.ID=leave.empId where leave.[status]=2 and @currentDt between leave.LeaveStartDate and LeaveEndDate))
			or (@onLeave=0 and recruiterID not in (select usr.ID from [dbo].[PH_EMP_LEAVE_REQUEST] leave with(nolock) inner join [dbo].[PI_HIRE_USERS] usr with(nolock) on usr.ID=leave.empId where leave.[status]=2 and @currentDt between leave.LeaveStartDate and LeaveEndDate)))
	order by closeddate desc
end
Go

CREATE OR ALTER view vwDashboardRecruiterStatus
as
select 
	job.ID as jobId,
	job.ClientName, job.JobTitle, job.PostedDate, job.ClosedDate,
	coalesce(job.BroughtBy,job.createdby) bdmId,
	job.JobLocationID jobCityId, job.CountryID jobCountryId,
	job.JobOpeningStatus,
	jobRecr.AssignedTo recruiterID,
	jobRecr.NoCVSRequired,
	jobRecr.NoOfFinalCVsFilled
from 
	[dbo].[PH_JOB_ASSIGNMENTS] jobRecr with (nolock)
	inner join [dbo].[PH_JOB_OPENINGS] job with (nolock) on jobRecr.JOID = job.ID  and jobRecr.DeassignDate is null and jobRecr.Status!=5 and job.Status!=5
	inner join [dbo].[PI_HIRE_USERS] usr with (nolock) on usr.ID = jobRecr.AssignedTo and usr.Status!=5