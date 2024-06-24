USE [piHIRE1.0_QA]
GO
CREATE PROCEDURE [dbo].[Sp_Dashboard_JobTime]
	@jobId int,
	--Authorization
	@userType int,
	@userId int 
AS
begin
	declare @workingJobCount int, @closedJobCount int, @totalCandidateCount int, @hiredCandidateCount int, @rejectedCandidateCount int,
	@recruitmentApplicantsCount int, @recruitmentPreScreenedCount int, @recruitmentInterviewedCount int, @recruitmentHiredCount int, 
	@offerAcceptedCount int, @offerProvidedCount int,
	@recruiterRejectCount int, @clientRejectCount int, @candidateRejectCount int;

	CREATE TABLE #TempDashboard_Recruiter_job ([ID] int not null, [JobOpeningStatus] int)
	CREATE CLUSTERED INDEX ix_tempDashboard_Recruiter_job ON #TempDashboard_Recruiter_job ([ID],[JobOpeningStatus]);
	INSERT INTO #TempDashboard_Recruiter_job 
		select distinct [ID], [JobOpeningStatus] 
		from pH_job_openings job with(nolock) 
		where status!=5 and 
			( 
				(@userType = 1) or --SuperAdmin
				(@userType = 2 and [ID] in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@userId)) or --Admin
				(@userType = 3 and @userId = coalesce(job.BroughtBy,job.createdby)) or --BDM
				(@userType = 4 and [ID] in (select JOID from [dbo].[PH_JOB_ASSIGNMENTS] where DeassignDate is null and AssignedTo =@userId))--Recruiter 4
				--Candidate 5
			)
			and (@fmDt is null or @toDt is null or (ClosedDate between @fmDt and @toDt));
		
	
	--Job Working -> not deassined jobs and job status is not closed
	select @workingJobCount = count(distinct JOID) from [dbo].[PH_JOB_ASSIGNMENTS] where DeassignDate is null and AssignedTo =@userId 
								and JOID in (select [ID] from #TempDashboard_Recruiter_job where JobOpeningStatus not in (select ID from [dbo].[PH_JOB_STATUS_S] where jsCode='CLS'))
	--Job closed -> assined jobs and job status is closed
	select @closedJobCount = count(distinct JOID) from [dbo].[PH_JOB_ASSIGNMENTS] where /*DeassignDate is null and*/ AssignedTo =@userId 
								and JOID in (select [ID] from #TempDashboard_Recruiter_job where JobOpeningStatus in (select ID from [dbo].[PH_JOB_STATUS_S] where jsCode='CLS'))
	--Total candidate -> job candidate -> profile recieved date filter and all assigned jobs(current & closed jobs)
	select @totalCandidateCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where ProfReceDate between @fmDt and @toDt and RecruiterID=@userId 
													and CandProfID in(select ID from [dbo].[PH_CANDIDATE_PROFILES] where status!=5) 
	--Hired Candidate -> job candidate -> status -> success ,date ?
	select @hiredCandidateCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where ProfReceDate between @fmDt and @toDt and RecruiterID=@userId 
													and candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode='SUC')
	--Rejected Candidate -> job candidate -> status -> all rejects(param, client etc),date ?
	select @rejectedCandidateCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where ProfReceDate between @fmDt and @toDt and RecruiterID=@userId 
													and candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode in ('PIR','IRT','CRT','RRT','SRT','RTD','HRR','ORD'))
	
	--Recruitment -> Applicants -> job candidate -> profile recieved date filter and all assigned jobs(current & closed jobs)
	select @recruitmentApplicantsCount= @totalCandidateCount
	--Recruitment -> Prescreened -> job candidate -> profile recieved date filter and stage 1(or)2(may vary dynamic data no codes)
	select @recruitmentPreScreenedCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where ProfReceDate between @fmDt and @toDt and RecruiterID=@userId 
													and StageID in (1,2)
	--Recruitment -> Interviewed -> job candidate -> profile recieved date filter and stage 4(may vary dynamic data no codes)
	select @recruitmentInterviewedCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where ProfReceDate between @fmDt and @toDt and RecruiterID=@userId 
													and StageID in (4)
	--Recruitment -> Hired -> job candidate -> profile recieved date filter and stage 6(may vary dynamic data no codes)
	select @recruitmentHiredCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where ProfReceDate between @fmDt and @toDt and RecruiterID=@userId 
													and StageID in (6)

	--Offer accepted -> job candidate -> profile recieved date filter and OpConfirmFlag =true -?
	select @offerAcceptedCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where ProfReceDate between @fmDt and @toDt and RecruiterID=@userId 
													and OpConfirmFlag =1
	--Offer provided -> job candidate -> profile recieved date filter -?
	select @offerProvidedCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where ProfReceDate between @fmDt and @toDt and RecruiterID=@userId 

	--Reject status -> Recruiter reject -> RRT/(ttagged+screeded) 
	select @recruiterRejectCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where ProfReceDate between @fmDt and @toDt and RecruiterID=@userId 
													and candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode in ('RRT'))
	--Reject status -> Client reject -> 
	select @clientRejectCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where ProfReceDate between @fmDt and @toDt and RecruiterID=@userId 
													and candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode in ('CRT'))
	--Reject status -> Candidate backout -> 
	select @candidateRejectCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where ProfReceDate between @fmDt and @toDt and RecruiterID=@userId 
													and candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode in ('ORD'))--JBO

	drop table #TempDashboard_Recruiter_job

	select @workingJobCount workingJobCount, @closedJobCount closedJobCount, @totalCandidateCount totalCandidateCount, @hiredCandidateCount hiredCandidateCount, @rejectedCandidateCount rejectedCandidateCount,
	@recruitmentApplicantsCount recruitmentApplicantsCount, @recruitmentPreScreenedCount recruitmentPreScreenedCount, @recruitmentInterviewedCount recruitmentInterviewedCount, @recruitmentHiredCount recruitmentHiredCount, 
	@offerAcceptedCount offerAcceptedCount, @offerProvidedCount offerProvidedCount,
	@recruiterRejectCount recruiterRejectCount, @clientRejectCount clientRejectCount, @candidateRejectCount candidateRejectCount
		
end



