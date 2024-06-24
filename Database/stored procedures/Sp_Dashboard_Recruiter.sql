USE [piHIRE1.0_QA]
GO
Alter PROCEDURE [dbo].[Sp_Dashboard_Recruiter]
	@fmDt datetime,
	@toDt datetime,
	--Authorization
	@userType int,
	@userId int 
AS
begin
	CREATE TABLE #TempDashboard_Recruiter_job ([ID] int not null,ClosedDate datetime)
	CREATE CLUSTERED INDEX ix_tempDashboard_Recruiter_job ON #TempDashboard_Recruiter_job ([ID],ClosedDate);
	INSERT INTO #TempDashboard_Recruiter_job 
		select distinct job.[ID], job.ClosedDate 
		from pH_job_openings job with(nolock) 
		inner join [dbo].[PH_JOB_STATUS_S] jobStatus with(nolock) on job.JobOpeningStatus=jobStatus.ID
		where job.status!=5 and 
			( 
				--(@userType = 1) or --SuperAdmin
				--(@userType = 2 and [ID] in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@userId)) or --Admin
				--(@userType = 3 and @userId = coalesce(job.BroughtBy,job.createdby)) or --BDM
				(@userType = 4 and job.[ID] in (select JOID from [dbo].[PH_JOB_ASSIGNMENTS] where DeassignDate is null and AssignedTo =@userId))--Recruiter 4
				--Candidate 5
			)
			--and (@fmDt is null or @toDt is null or (ClosedDate between @fmDt and @toDt));
			and jobStatus.jsCode!='CLS';
		
	declare @highlightTrgtPeriodFilled int, @highlightTrgtPeriodRequired int, @highlightYetToJoin int, @highlightJoined int,
	@AssignedCount int, @SourcedCount int, @TaggedCount int, @ReqFinalCvsCount int, @FinalCvsCount int, @CvSubmissionCount int,
	@InterviewCount int, @AccountRejectedCount int, @InterviewBackoutCount int,	@JoinBackoutCount int;
	
	
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


	--Performance -> To be achieved(filled) 
	--select @highlightTrgtPeriodFilled= count(1) from [dbo].[PH_JOB_CANDIDATES] with(nolock) where JOID in (select ID from #TempDashboard_Recruiter_job) and candProfStatus in ( select ID from PH_CAND_STATUS_S with(nolock) where cscode in('SUC','PNS'))
	select @highlightTrgtPeriodFilled= count(distinct [CandProfId]) from #TempDashboard_Recruiter_job_candHistory with(nolock) where statusCode in ( 'SUC','PNS')
	--Performance -> To be achieved(required)
	--select Target_qty_set from [dbo].[tbl_param_targets] where Employee_id=@loginUserId and month_year=@currentYearMonth and Target_parameter_id in(select ID from [dbo].[tbl_param_reference_master] where groupId=81 and value in ('Recruitment') and description='Joinees')
	select @highlightTrgtPeriodRequired= sum(coalesce(Target_qty_set,0)) 
	from vwUserTargetsByMonthAndTarget with(nolock) 	
	where (month_year between @fmDt and @toDt) and 
	( 
		--   ((@userType = 1 or --SuperAdmin
		--	   @userType = 2) --Admin
		--		and UserId in (select AssignedTo from [dbo].[PH_JOB_ASSIGNMENTS] where JOID in (select ID from #TempDashboard_Recruiter_job) and DeassignDate is null)) or
		--	(@userType = 3 and @userId = UserId) --BDM
			(@userType = 4 and @userId = UserId) --Recruiter 4
		--	--Candidate 5
	)
	and targetValue='Recruitment' and targetDescription='Joinees'	
	--Performance -> Yet to join 
	--select @highlightYetToJoin= count(1) from [dbo].[PH_JOB_CANDIDATES] with(nolock) where candProfStatus in ( select ID from PH_CAND_STATUS_S with(nolock) where cscode='PNS') and JOID in (select ID from #TempDashboard_Recruiter_job)
	select @highlightYetToJoin = count(distinct [CandProfId]) from #TempDashboard_Recruiter_job_candHistory with(nolock) where statusCode in ( 'PNS') and [CandProfId] not in (select [CandProfId] from #TempDashboard_Recruiter_job_candHistory with(nolock) where statusCode in ( 'SUC'))
	--Performance -> Joined
	--select @highlightJoined= count(1) from [dbo].[PH_JOB_CANDIDATES] with(nolock) where candProfStatus in ( select ID from PH_CAND_STATUS_S with(nolock) where cscode='SUC') and JOID in (select ID from #TempDashboard_Recruiter_job)
	select @highlightJoined = count(distinct [CandProfId]) from #TempDashboard_Recruiter_job_candHistory with(nolock) where statusCode in ( 'SUC')

	--delete #TempDashboard_Recruiter_job
	--INSERT INTO #TempDashboard_Recruiter_job 
	--	select distinct [ID] 
	--	from [dbo].[PH_JOB_ASSIGNMENTS] with(nolock) 
	--	where status!=5 and DeassignDate is null and AssignedTo =@userId 			
	--		and (@fmDt is null or @toDt is null or (createdDate between @fmDt and @toDt));
	



	--Assigned -> job assigned to recruiter (select * from [dbo].[PH_JOB_ASSIGNMENTS] where createdDate between ''  and '' AND ASSIGNEDTO=@LOGIN)
	select @AssignedCount = count(distinct JOID) from [dbo].[PH_JOB_ASSIGNMENTS] where DeassignDate is null and AssignedTo =@userId and createdDate between @fmDt and @toDt and JOID in (select [ID] from #TempDashboard_Recruiter_job)
	
	--Sourced -> select * from [dbo].[PH_JOB_CANDIDATES] where SourceID in(2,3) and recr=@login
	--select @SourcedCount = count(1) from [dbo].[PH_JOB_CANDIDATES] where RecruiterID=@userId and createdDate between @fmDt and @toDt
	--												and CandProfID in(select ID from [dbo].[PH_CANDIDATE_PROFILES] where SourceID in(2,3) and status!=5)
	select @SourcedCount = count(distinct jobCand.CandProfID) from [dbo].[PH_JOB_CANDIDATES] jobCand where RecruiterID=@userId and createdDate between @fmDt and @toDt
													and Exists(select ID from [dbo].[PH_CANDIDATE_PROFILES] candPrfl where jobCand.CandProfID=candPrfl.ID and SourceID not in (4,1,6) and status!=5)
													and IsTagged = 0
	--Tagged -> select * from [dbo].[PH_JOB_CANDIDATES] where SourceID not in(2,3) and recr=@login
	--select @TaggedCount = count(1) from [dbo].[PH_JOB_CANDIDATES] where RecruiterID=@userId and createdDate between @fmDt and @toDt
	--												and CandProfID in(select ID from [dbo].[PH_CANDIDATE_PROFILES] where SourceID not in(2,3) and status!=5)
	select @TaggedCount = count(distinct CandProfID) from [dbo].[PH_JOB_CANDIDATES] where RecruiterID=@userId and createdDate between @fmDt and @toDt
													and IsTagged = 1
	--Final CVS -> sum of final cvs in recruiters
	select @ReqFinalCvsCount= SUM(COALESCE(NoCvsRequired,0)) from [dbo].[PH_JOB_ASSIGNMENTS] where DeassignDate is null and AssignedTo =@userId and JOID in (select [ID] from #TempDashboard_Recruiter_job)
	--select @FinalCvsCount= SUM(COALESCE(NoOfFinalCVsFilled,0)) from [dbo].[PH_JOB_ASSIGNMENTS] where DeassignDate is null and AssignedTo =@userId and JOID in (select [ID] from #TempDashboard_Recruiter_job)
	select @FinalCvsCount= count(1) from (select distinct [JobId], [CandProfId]	from #TempDashboard_Recruiter_job_candHistory where statusCode='FCV')inr 
	--cv submission -> distinct count of jobId, cand Id, clientId  from [PH_CANDIDATE_PROFILES_SHARED] table
	--select @CvSubmissionCount= count(1) from (
	--	select distinct candPrflShrd.JOID, candPrflShrd.CandProfId, candPrflShrd.ClientID 
	--	from [dbo].[PH_CANDIDATE_PROFILES_SHARED] candPrflShrd 
	--	inner join [dbo].[PH_JOB_CANDIDATES] jobCand on jobCand.JOID=candPrflShrd.JOID and candPrflShrd.CandProfId=jobCand.CandProfId
	--	and candPrflShrd.JOID in (select [ID] from #TempDashboard_Recruiter_job)
	--	where jobCand.RecruiterID=@userId and candPrflShrd.CreatedDate between @fmDt and @toDt
	--)inn
	select @CvSubmissionCount= count(1) from (select distinct [JobId], [CandProfId]	from #TempDashboard_Recruiter_job_candHistory where statusCode='CSB')inr 
	
	--Interview -> [PH_JOB_CANDIDATES] where candProfStatus -> cli1 
	--select @InterviewCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where JOID in (select ID from #TempDashboard_Recruiter_job) and RecruiterID=@userId and candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode in('CL1'))
	select @InterviewCount= count(1) from (select distinct [JobId], [CandProfId]	from #TempDashboard_Recruiter_job_candHistory where statusCode='CL1')inr 

	--Account  rejected -> select count(1) from [dbo].[PH_JOB_CANDIDATES] where candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode='CRT'		)
	--select @AccountRejectedCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where JOID in (select ID from #TempDashboard_Recruiter_job) and RecruiterID=@userId and candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode in('CRT'))
	select @AccountRejectedCount= count(1) from (select distinct [JobId], [CandProfId]	from #TempDashboard_Recruiter_job_candHistory where statusCode='CRT')inr 

	--Interview backout -> select count(1) from [dbo].[PH_JOB_CANDIDATES] where candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode='CBT'		)
	--select @InterviewBackoutCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where JOID in (select ID from #TempDashboard_Recruiter_job) and RecruiterID=@userId and candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode in('CBT'))
	select @InterviewBackoutCount= count(1) from (select distinct [JobId], [CandProfId]	from #TempDashboard_Recruiter_job_candHistory where statusCode='CBT')inr 

	--Join backout -> select count(1) from [dbo].[PH_JOB_CANDIDATES] where candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode='JBO'		)
	--select @JoinBackoutCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where JOID in (select ID from #TempDashboard_Recruiter_job) and RecruiterID=@userId and candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode in('JBO'))
	select @JoinBackoutCount= count(1) from (select distinct [JobId], [CandProfId]	from #TempDashboard_Recruiter_job_candHistory where statusCode='JBO')inr 
	
	drop table #TempDashboard_Recruiter_job;
	DROP TABLE #TempDashboard_Recruiter_job_candHistory;

	select @highlightTrgtPeriodFilled highlightTrgtPeriodFilled, @highlightTrgtPeriodRequired highlightTrgtPeriodRequired, @highlightYetToJoin highlightYetToJoin, @highlightJoined highlightJoined,
	@AssignedCount AssignedCount, @SourcedCount SourcedCount, @TaggedCount TaggedCount, @ReqFinalCvsCount ReqFinalCvsCount, @FinalCvsCount FinalCvsCount, @CvSubmissionCount CvSubmissionCount,
	@InterviewCount InterviewCount, @AccountRejectedCount AccountRejectedCount, @InterviewBackoutCount InterviewBackoutCount,	@JoinBackoutCount JoinBackoutCount
		
end



