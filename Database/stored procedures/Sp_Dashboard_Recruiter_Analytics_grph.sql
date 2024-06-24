USE [piHIRE1.0_QA]
GO
Alter PROCEDURE [dbo].[Sp_Dashboard_Recruiter_Analytics_grph]
	@fmDt datetime,
	@toDt datetime,
	--Authorization
	@userType int,
	@userId int 
AS
begin		
	select 
		JobId, JobTitle, ActivityDate,StatusCode,OPGrossPayPerMonth, CandProfId
	from 
		vwJobCandidateStatusHistory with(nolock) 
		--[dbo].[pH_job_openings] job with(nolock) 
		--inner join [dbo].[PH_JOB_CANDIDATES] jobCand with(nolock) on job.ID=jobCand.JoID
		--inner join [dbo].[PH_CAND_STATUS_S] candStatus with(nolock) on candStatus.ID=jobCand.candProfStatus	
	where 
		(@fmDt is null or @toDt is null or (ActivityDate between @fmDt and @toDt))
		and RecruiterID=@userId 
		and StatusCode in ('SUC','CRT','CDB')
		and ( 
				(@userType = 1) or --SuperAdmin
				(@userType = 2 and JobId in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@userId)) or --Admin
				(@userType = 3 and @userId = BroughtBy) or --BDM
				(@userType = 4 and @userId = RecruiterId)--Recruiter 4
				--Candidate 5
			)
		--(@fmDt is null or @toDt is null or (jobCand.ProfReceDate between @fmDt and @toDt))
		--and jobCand.RecruiterID=@userId 
		--and candStatus.cscode in ('SUC','CRT','CDB')
		--and  job.[status]!=5 
		--and ( 
		--		(@userType = 1) or --SuperAdmin
		--		(@userType = 2 and job.[ID] in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@userId)) or --Admin
		--		(@userType = 3 and @userId = coalesce(job.BroughtBy,job.createdby)) or --BDM
		--		(@userType = 4 and job.[ID] in (select JOID from [dbo].[PH_JOB_ASSIGNMENTS] where DeassignDate is null and AssignedTo =@userId))--Recruiter 4
		--		--Candidate 5
		--	)		
end



