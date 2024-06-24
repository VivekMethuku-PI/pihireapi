USE [piHIRE1.0_QA]
GO
Alter PROCEDURE [dbo].[Sp_Dashboard_Recruiter_Analytics]
	@fmDt datetime,
	@toDt datetime,
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
				--(@userType = 1) or --SuperAdmin
				--(@userType = 2 and [ID] in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@userId)) or --Admin
				--(@userType = 3 and @userId = coalesce(job.BroughtBy,job.createdby)) or --BDM
				(@userType = 4 and [ID] in (select JOID from [dbo].[PH_JOB_ASSIGNMENTS] where /*DeassignDate is null and*/ AssignedTo =@userId))--Recruiter 4
				--Candidate 5
			)
			--and (@fmDt is null or @toDt is null or (ClosedDate between @fmDt and @toDt));
	
	--Job Working -> not deassined jobs and job status is not closed
	select @workingJobCount = count(distinct JOID) from [dbo].[PH_JOB_ASSIGNMENTS] where /*DeassignDate is null and*/ AssignedTo =@userId 
								and (@fmDt is null or @toDt is null or (CreatedDate between @fmDt and @toDt)) 
								and JOID in (select [ID] from #TempDashboard_Recruiter_job where JobOpeningStatus not in (select ID from [dbo].[PH_JOB_STATUS_S] where jsCode='CLS'))
	--Job closed -> assined jobs and job status is closed
	select @closedJobCount = count(distinct JOID) from [dbo].[PH_JOB_ASSIGNMENTS] where /*DeassignDate is null and*/ AssignedTo =@userId
								and (@fmDt is null or @toDt is null or (CreatedDate between @fmDt and @toDt))  
								and JOID in (select [ID] from #TempDashboard_Recruiter_job where JobOpeningStatus in (select ID from [dbo].[PH_JOB_STATUS_S] where jsCode='CLS'))

	CREATE TABLE #TempDashboard_Recruiter_job_candHistory ([JobId] int not null, [CandProfId] int, statusCode nvarchar(max), activityDate datetime, RecruiterId int,BroughtBy int)
	--CREATE CLUSTERED INDEX ix_tempDashboard_Recruiter_job_candHistory ON #TempDashboard_Recruiter_job_candHistory ([JobId], [CandProfId], statusCode, activityDate, RecruiterId,BroughtBy);
	INSERT INTO #TempDashboard_Recruiter_job_candHistory
	select [JobId], [CandProfId], statusCode, activityDate, RecruiterId,BroughtBy from [dbo].[vwJobCandidateStatusHistory] with(nolock) 
		where 
			( 
				--(@userType = 1) or --SuperAdmin
				--(@userType = 2 and [JobId] in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@userId)) or --Admin
				--(@userType = 3 and @userId = BroughtBy) or --BDM
				(@userType = 4 and RecruiterId =@userId)--Recruiter 4
				--Candidate 5
			)
			and (@fmDt is null or @toDt is null or (activityDate between @fmDt and @toDt));


	--Total candidate -> job candidate -> profile recieved date filter and all assigned jobs(current & closed jobs)
	select @totalCandidateCount= count(1) from (select distinct [JobId], [CandProfId]	from #TempDashboard_Recruiter_job_candHistory)inr 
	--Hired Candidate -> job candidate -> status -> success ,date ?
	select @hiredCandidateCount= count(1) from (select distinct [JobId], [CandProfId]	from #TempDashboard_Recruiter_job_candHistory where statusCode='SUC')inr 
	--Rejected Candidate -> job candidate -> status -> all rejects(param, client etc),date ?
	select @rejectedCandidateCount= count(1) from (select distinct [JobId], [CandProfId]	from #TempDashboard_Recruiter_job_candHistory where statusCode in ('PIR','IRT','CRT','RRT','SRT','RTD','HRR','ORD'))inr 
	
	--Recruitment -> Applicants -> job candidate -> profile recieved date filter and all assigned jobs(current & closed jobs)
	select @recruitmentApplicantsCount= @totalCandidateCount
	--Recruitment -> Prescreened -> job candidate -> profile recieved date filter and stage 1(or)2(may vary dynamic data no codes)
	select @recruitmentPreScreenedCount= count(1) from (select distinct [JobId], [CandProfId]	from #TempDashboard_Recruiter_job_candHistory where statusCode in 
	('SVD','YSP','SCD','CFD','FCV','OHD'))inr 
	--Recruitment -> Interviewed -> job candidate -> profile recieved date filter and stage 4(may vary dynamic data no codes)
	select @recruitmentInterviewedCount= count(1) from (select distinct [JobId], [CandProfId]	from #TempDashboard_Recruiter_job_candHistory where statusCode in 
	('CL1','CL2','CFF','CIB','CRT','CRD','CHD','CBT','CSN','CDB','WFO'))inr 
	--Recruitment -> Hired -> job candidate -> profile recieved date filter and stage 6(may vary dynamic data no codes)
	select @recruitmentHiredCount= count(1) from (select distinct [JobId], [CandProfId]	from #TempDashboard_Recruiter_job_candHistory where statusCode in 
	('JBO','CJS','CTE','SUC'))inr 

	--Offer accepted -> job candidate -> status success and [PH_JOB_OFFER_LETTERS] -> joiningDate date filter
	select 
		@offerAcceptedCount= count(1) 
	from (
		select distinct
			jobOfferLtr.JOID, jobOfferLtr.CandProfID
		from
			#TempDashboard_Recruiter_job_candHistory tmp 
			inner join [dbo].[PH_JOB_OFFER_LETTERS] jobOfferLtr on jobOfferLtr.JOID=tmp.[JobId] and jobOfferLtr.CandProfID=tmp.[CandProfId] 
			and jobOfferLtr.status!=5
		where 
			tmp.statusCode='SUC'
	)inr

	--Offer provided -> job candidate & last one in [PH_JOB_OFFER_LETTERS]
	select 
		@offerProvidedCount= count(1) 
	from (
		select distinct
			jobOfferLtr.JOID, jobOfferLtr.CandProfID
		from
			#TempDashboard_Recruiter_job_candHistory tmp 
			inner join [dbo].[PH_JOB_OFFER_LETTERS] jobOfferLtr on jobOfferLtr.JOID=tmp.[JobId] and jobOfferLtr.CandProfID=tmp.[CandProfId] 
			and jobOfferLtr.status!=5
	)inr

	--Reject status -> Recruiter reject -> RRT/(ttagged+screeded) 
	select @recruiterRejectCount= count(1) from (select distinct [JobId], [CandProfId]	from #TempDashboard_Recruiter_job_candHistory where statusCode in ('RRT','NSB'))inr

	--Reject status -> Client reject -> 
	select @clientRejectCount= count(1) from (select distinct [JobId], [CandProfId]	from #TempDashboard_Recruiter_job_candHistory where statusCode='CRT')inr
	
	--Reject status -> Candidate backout -> 
	select @candidateRejectCount= count(1) from (select distinct [JobId], [CandProfId]	from #TempDashboard_Recruiter_job_candHistory where statusCode='CDB')inr
	
	drop table #TempDashboard_Recruiter_job
	drop table #TempDashboard_Recruiter_job_candHistory

	select @workingJobCount workingJobCount, @closedJobCount closedJobCount, @totalCandidateCount totalCandidateCount, @hiredCandidateCount hiredCandidateCount, @rejectedCandidateCount rejectedCandidateCount,
	@recruitmentApplicantsCount recruitmentApplicantsCount, @recruitmentPreScreenedCount recruitmentPreScreenedCount, @recruitmentInterviewedCount recruitmentInterviewedCount, @recruitmentHiredCount recruitmentHiredCount, 
	@offerAcceptedCount offerAcceptedCount, @offerProvidedCount offerProvidedCount,
	@recruiterRejectCount recruiterRejectCount, @clientRejectCount clientRejectCount, @candidateRejectCount candidateRejectCount
		
end



