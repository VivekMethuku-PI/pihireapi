USE [piHIRE1.0_DEV]
GO
Alter PROCEDURE [dbo].[Sp_Dashboard_Recruiter_JobCategory]
@fmDt datetime,
@toDt datetime,
@typeId int,
--Authorization
@userType int,
@userId int 
AS
begin
	select JobId, [JobCategory],[ClosedDate],sum(cnt) resumeCount from (
		--select distinct
		--	job.[ID] JobId, job.[JobCategory],job.[ClosedDate],jobCand.[ID] 
		--from 
		--	pH_job_openings job with(nolock) 
		--	inner join [PH_JOB_CANDIDATES] jobCand  with(nolock) on jobCand.JOID =job.[ID] and RecruiterID=@userId and candProfStatus in (select ID from PH_CAND_STATUS_S where cscode in('SUC'))
		--where 
		--	job.status!=5 and jobCand.status!=5 and 
		--	( 
		--		--(@userType = 1) or --SuperAdmin
		--		--(@userType = 2 and job.[ID] in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@userId)) or --Admin
		--		--(@userType = 3 and @userId = coalesce(job.BroughtBy,job.createdby)) or --BDM
		--		(@userType = 4 and job.[ID] in (select JOID from [dbo].[PH_JOB_ASSIGNMENTS] where /*DeassignDate is null and*/ AssignedTo =@userId and (@fmDt is null or @toDt is null or (CreatedDate between @fmDt and @toDt))))--Recruiter 4
		--		--Candidate 5
		--	)
		--	--and (@fmDt is null or @toDt is null or (ClosedDate between @fmDt and @toDt))


		select 
			distinct job.[ID] JobId, job.[JobCategory],job.[ClosedDate], (case when vw.RecruiterId is not null and vw.RecruiterId =@userId and (vw.statusCode='SUC' or (@typeId=2 and vw.statusCode='PNS')) and (@fmDt is null or @toDt is null or (activityDate between @fmDt and @toDt)) then 1 else 0 end)  cnt
		from 
			[dbo].[PH_JOB_OPENINGS] job with(nolock) 
			left outer join [dbo].[vwJobCandidateStatusHistory] vw with(nolock) on vw.[JobId] =job.[ID]
		where 
			( 
				--(@userType = 1) or --SuperAdmin
				--(@userType = 2 and [JobId] in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw on jbDtl.BUID=vw.[BusinessUnit] and jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@userId)) or --Admin
				--(@userType = 3 and @userId = BroughtBy) or --BDM
				(@userType = 4)--Recruiter 4
				--Candidate 5
			)			

	)inr
	group by inr.jobId,	inr.[JobCategory],inr.[ClosedDate]
end



