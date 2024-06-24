USE [piHIRE1.0_QA]
GO
CREATE OR ALTER PROCEDURE [dbo].[Sp_Dashboard_BDM]
	@fmDt datetime,
	@toDt datetime,
	@JobCategory nvarchar(max),
	--Authorization
	@userType int,
	@userId int 
AS
begin

	--select job.* 
	--from 
	--	[dbo].[PH_JOB_OPENINGS] job 
	--	where Job.Id not in (
	--		select chkCls.JobId 
	--		from [dbo].[vwJobStatusHistory] chkCls 
	--		left outer join [dbo].[vwJobStatusHistory] chkReopen on chkCls.JobId=chkReopen.JobId and chkReopen.ActivityDate>chkCls.ActivityDate and chkReopen.ActivityDate<@toDt and chkReopen.statusCode ='RPN' 
	--		where chkCls.ActivityDate<@fmDt and chkCls.statusCode in ('HLD','CLS') and chkReopen.JobId is null
	--	) and job.PostedDate<@toDt

	CREATE TABLE #TempDashboard_BDM_job ([ID] int not null, ClosedDate datetime,JobCategory nvarchar(max),jsCode nvarchar(max))
	CREATE CLUSTERED INDEX ix_tempDashboard_BDM_job ON #TempDashboard_BDM_job ([ID],ClosedDate);
	INSERT INTO #TempDashboard_BDM_job 
		select distinct job.[ID], job.ClosedDate, job.JobCategory,jobStatus.jsCode
		from dbo.pH_job_openings job with(nolock) 
		left outer join [dbo].[PH_JOB_STATUS_S] jobStatus with(nolock) on job.JobOpeningStatus=jobStatus.ID
		where job.status!=5 and 
			( 
				--(@userType = 1) or --SuperAdmin
				--(@userType = 2 and [ID] in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@userId)) or --Admin
				(@userType = 3 and @userId = coalesce(job.BroughtBy,job.createdby)) --BDM
				--Recruiter 4
				--Candidate 5
			)
			--and (@fmDt is null or @toDt is null or (ClosedDate between @fmDt and @toDt))
			--and (@JobCategory is null or len(@JobCategory)=0 or JobCategory =@JobCategory);

	CREATE TABLE #TempDashboard_Recruiter_job_candHistory ([JobId] int not null, [CandProfId] int, statusCode nvarchar(max), activityDate datetime, RecruiterId int,BroughtBy int)
	--CREATE CLUSTERED INDEX ix_tempDashboard_Recruiter_job_candHistory ON #TempDashboard_Recruiter_job_candHistory ([JobId], [CandProfId], statusCode, activityDate, RecruiterId,BroughtBy);
	INSERT INTO #TempDashboard_Recruiter_job_candHistory
	select [JobId], [CandProfId], statusCode, activityDate, RecruiterId,BroughtBy from [dbo].[vwJobCandidateStatusHistory] with(nolock) 
		where 
			( 
				--(@userType = 1) or --SuperAdmin
				--(@userType = 2 and [JobId] in (select JOID from PH_JOB_OPENINGS_ADDL_DETAILS jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@userId)) or --Admin
				(@userType = 3 and @userId = BroughtBy) --BDM
				--(@userType = 4 and RecruiterId =@userId)--Recruiter 4
				--Candidate 5
			)
			and (@fmDt is null or @toDt is null or (activityDate between @fmDt and @toDt));
		
	declare @ReqFinalCvsCount int, @FinalCvsCount int, @CvSubmissionCount int, @InterviewCount int, @ResultDueCount int, @pf2fCount int,
	@highlightTrgtPeriodFilled int, @highlightTrgtPeriodRequired int, @highlightYetToJoin int, @highlightJoined int,
	@NewJobCount int, @ClosedJobCount int;
	--Req final cvs -> req cvs in jobOpenings 
	select @ReqFinalCvsCount= SUM(COALESCE(NoOfCvsRequired,0)) from [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] where JOID in (select [ID] from #TempDashboard_BDM_job where jsCode!='CLS')
	--Final cvs -> sum of final cvs in recruiters 
	select @FinalCvsCount= SUM(COALESCE(NoOfFinalCVsFilled,0)) from [dbo].[PH_JOB_ASSIGNMENTS] where JOID in (select [ID] from #TempDashboard_BDM_job where jsCode!='CLS') --and DeassignDate is null

	--cv submission -> distinct count of jobId, cand Id, clientId  from [PH_CANDIDATE_PROFILES_SHARED] table
	select @CvSubmissionCount= count(1) from (select distinct JOID, CandProfId, ClientID from [dbo].[PH_CANDIDATE_PROFILES_SHARED] where JOID in (select [ID] from #TempDashboard_BDM_job))inn
	--Interview -> [PH_JOB_CANDIDATES] where candProfStatus -> cli1 
	select @InterviewCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where JOID in (select ID from #TempDashboard_BDM_job) and candProfStatus in ( select ID from dbo.PH_CAND_STATUS_S where cscode in('CL1'))
	--Result Due -> [PH_JOB_CANDIDATES] where candProfStatus -> client due 
	select @ResultDueCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where JOID in (select ID from #TempDashboard_BDM_job) and candProfStatus in ( select ID from dbo.PH_CAND_STATUS_S where cscode in('CRD'))
	--PF2F -> count of [PH_JOB_CANDIDATES] where candProfStatus in (select ID from [dbo].[PH_CAND_STATUS_S] where cscode ='PFF') 
	select @pf2fCount= count(1) from [dbo].[PH_JOB_CANDIDATES] where JOID in (select ID from #TempDashboard_BDM_job) and candProfStatus in ( select ID from dbo.PH_CAND_STATUS_S where cscode in('PFF'))
	
	--New Openings
	select @NewJobCount = count(1) from dbo.pH_job_openings with(nolock) where ID in (select ID from #TempDashboard_BDM_job) and (@fmDt is null or @toDt is null or (CreatedDate between @fmDt and @toDt)) 
	--Openings Closed
	select @ClosedJobCount = count(distinct JobId) from [dbo].[vwJobStatusHistory] with(nolock) where JobId in (select ID from #TempDashboard_BDM_job) and (@fmDt is null or @toDt is null or (ActivityDate between @fmDt and @toDt)) and statusCode in ('CLS')

	--Performance -> To be achieved(filled) 
	--select @highlightTrgtPeriodFilled= count(1) from [dbo].[PH_JOB_CANDIDATES] where JOID in (select ID from #TempDashboard_BDM_job where jsCode!='CLS' and (@fmDt is null or @toDt is null or (ClosedDate between @fmDt and @toDt)) and (@JobCategory is null or len(@JobCategory)=0 or JobCategory =@JobCategory)) and candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode in('SUC','PNS'))
	select @highlightTrgtPeriodFilled = count(distinct [CandProfId]) from #TempDashboard_Recruiter_job_candHistory with(nolock) where statusCode in ('SUC','PNS') and (@JobCategory is null or len(@JobCategory)=0 or JobId in (select ID from #TempDashboard_BDM_job where (@JobCategory is null or len(@JobCategory)=0 or JobCategory =@JobCategory)))
	--Performance -> To be achieved(required)
	--select Target_qty_set from [dbo].[tbl_param_targets] where Employee_id=@loginUserId and month_year=@currentYearMonth and Target_parameter_id in(select ID from [dbo].[tbl_param_reference_master] where groupId=81 and value in ('Business Development') and description='Joinees')
	select @highlightTrgtPeriodRequired= sum(coalesce(Target_qty_set,0)) 
	from dbo.vwUserTargetsByMonthAndTarget	
	where (month_year between @fmDt and @toDt) and 
	( 
		  -- ((@userType = 1 or --SuperAdmin
			 --@userType = 2) --Admin
				--and UserId in (select AssignedTo from [dbo].[PH_JOB_ASSIGNMENTS] where JOID in (select ID from #TempDashboard_BDM_job where (@fmDt is null or @toDt is null or (ClosedDate between @fmDt and @toDt))and (@JobCategory is null or len(@JobCategory)=0 or JobCategory =@JobCategory)) and DeassignDate is null)) or
			(@userType = 3 and @userId = UserId) --BDM
			--(@userType = 4 and @userId = UserId) --Recruiter 4
			--Candidate 5
	)
	and targetValue='Business Development' and targetDescription='Joiners'	
	--Performance -> Yet to join 
	--select @highlightYetToJoin= count(1) from [dbo].[PH_JOB_CANDIDATES] where candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode='PNS') and JOID in (select ID from #TempDashboard_BDM_job where jsCode!='CLS' and (@fmDt is null or @toDt is null or (ClosedDate between @fmDt and @toDt))and (@JobCategory is null or len(@JobCategory)=0 or JobCategory =@JobCategory))
	select @highlightYetToJoin = count(distinct [CandProfId]) from #TempDashboard_Recruiter_job_candHistory with(nolock) where statusCode in ( 'PNS') and (@JobCategory is null or len(@JobCategory)=0 or JobId in (select ID from #TempDashboard_BDM_job where (@JobCategory is null or len(@JobCategory)=0 or JobCategory =@JobCategory)))
	 and [CandProfId] not in (select [CandProfId] from #TempDashboard_Recruiter_job_candHistory with(nolock) where statusCode in ( 'SUC') and (@JobCategory is null or len(@JobCategory)=0 or JobId in (select ID from #TempDashboard_BDM_job where (@JobCategory is null or len(@JobCategory)=0 or JobCategory =@JobCategory))))
	--Performance -> Joined
	--select @highlightJoined= count(1) from [dbo].[PH_JOB_CANDIDATES] where candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode='SUC') and JOID in (select ID from #TempDashboard_BDM_job where jsCode!='CLS' and (@fmDt is null or @toDt is null or (ClosedDate between @fmDt and @toDt))and (@JobCategory is null or len(@JobCategory)=0 or JobCategory =@JobCategory))
	select @highlightJoined = count(distinct [CandProfId]) from #TempDashboard_Recruiter_job_candHistory with(nolock) where statusCode in ( 'SUC') and (@JobCategory is null or len(@JobCategory)=0 or JobId in (select ID from #TempDashboard_BDM_job where (@JobCategory is null or len(@JobCategory)=0 or JobCategory =@JobCategory)))

	DROP TABLE #TempDashboard_BDM_job 
	DROP TABLE #TempDashboard_Recruiter_job_candHistory

	select @ReqFinalCvsCount ReqFinalCvsCount, @FinalCvsCount FinalCvsCount, @CvSubmissionCount CvSubmissionCount, @InterviewCount InterviewCount, @ResultDueCount ResultDueCount, @pf2fCount pf2fCount,
	@highlightTrgtPeriodFilled highlightTrgtPeriodFilled, @highlightTrgtPeriodRequired highlightTrgtPeriodRequired, @highlightYetToJoin highlightYetToJoin, @highlightJoined highlightJoined,
	@NewJobCount newJobCount, @ClosedJobCount closedJobCount
		
end



