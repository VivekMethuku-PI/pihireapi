USE [piHIRE1.0_QA]
GO
CREATE OR ALTER PROCEDURE [dbo].[Sp_Dashboard_HireAdmin]
	@fmDt datetime,
	@toDt datetime,
	@puIds nvarchar(max), 
	@buIds nvarchar(max), 
	@currentDt datetime,
	--Authorization
	@userType int,
	@userId int 
AS
begin
	
	CREATE TABLE #TempDashboard_HireAdmin_job ([ID] int not null,[JobOpeningStatus] smallint,[ClosedDate] datetime,[reopenedDate] datetime)
	CREATE CLUSTERED INDEX ix_tempDashboard_HireAdmin_job ON #TempDashboard_HireAdmin_job ([ID],[JobOpeningStatus],[ClosedDate],[reopenedDate]);
	INSERT INTO #TempDashboard_HireAdmin_job 
	select distinct [ID],[JobOpeningStatus],[ClosedDate], [reopenedDate] 
	from [dbo].pH_job_openings job with(nolock) 
	where status!=5 and 
		( 
			(@userType = 1) or --SuperAdmin
			(@userType = 2 and [ID] in (select JOID from [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw with(nolock) on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@userId)) or --Admin
			(@userType = 3 and @userId = coalesce(job.BroughtBy,job.createdby)) --BDM
			--Recruiter 4
			--Candidate 5
		)
		and (@fmDt is null or @toDt is null or (ClosedDate between @fmDt and @toDt));

	Declare @TempDashboard_Recruiter_job_candHistory TABLE([JobId] int not null, [CandProfId] int, statusCode nvarchar(max), activityDate datetime, RecruiterId int,BroughtBy int)
	--CREATE CLUSTERED INDEX ix_tempDashboard_Recruiter_job_candHistory ON @TempDashboard_Recruiter_job_candHistory ([JobId], [CandProfId], statusCode, activityDate, RecruiterId,BroughtBy);
	INSERT INTO @TempDashboard_Recruiter_job_candHistory
	select [JobId], [CandProfId], statusCode, activityDate, RecruiterId,BroughtBy from [dbo].[vwJobCandidateStatusHistory] with(nolock) 
		where 
			( 
				(@userType = 1) or --SuperAdmin
				(@userType = 2 and [JobId] in (select JOID from [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw with(nolock) on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@userId)) or --Admin
				(@userType = 3 and @userId = BroughtBy) --BDM
				--(@userType = 4 and RecruiterId =@userId)--Recruiter 4
				--Candidate 5
			)
			and (@fmDt is null or @toDt is null or (activityDate between @fmDt and @toDt));
		
	declare @activeJobCount int, @newJobCount int, @holdJobCount int, @reopenJobCount int, @morecvsJobCount int, @submittedJobsFilled int, @submittedJobsRequired int,
	@highlightTrgtPeriodFilled int, @highlightTrgtPeriodRequired int, @highlightYetToJoin int, @highlightJoined int;
	--active jobs
	select @activeJobCount= count(1) from #TempDashboard_HireAdmin_job where JobOpeningStatus not in (select ID from [dbo].[PH_JOB_STATUS_S] with(nolock) where jscode in ('CLS'))
	--new jobs
	select @newJobCount= count(1) from #TempDashboard_HireAdmin_job where JobOpeningStatus in (select ID from [dbo].[PH_JOB_STATUS_S] with(nolock) where jscode ='WIP') 
	--hold jobs
	select @holdJobCount= count(1) from #TempDashboard_HireAdmin_job where JobOpeningStatus in (select ID from [dbo].[PH_JOB_STATUS_S] with(nolock) where jscode ='HLD') 
	--reopend date not null and JobOpeningStatus != CLS
	select @reopenJobCount= count(1) from #TempDashboard_HireAdmin_job where [reopenedDate] is not null and JobOpeningStatus not in (select ID from [dbo].[PH_JOB_STATUS_S] with(nolock) where jscode ='CLS')
	--Submitted
	select @submittedJobsRequired=count(1) from #TempDashboard_HireAdmin_job where JobOpeningStatus in (select ID from [dbo].[PH_JOB_STATUS_S] with(nolock) where jsCode='SUB')
	select @submittedJobsFilled=count(1) from #TempDashboard_HireAdmin_job tmp inner join [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] jobDtls with(nolock) on jobDtls.JOID=tmp.ID and jobDtls.clientReviewFlag=1 where tmp.JobOpeningStatus in (select ID from [dbo].[PH_JOB_STATUS_S] with(nolock) where jsCode='SUB')
 

	--more cvs -MCV
	select @morecvsJobCount= count(1) from #TempDashboard_HireAdmin_job where JobOpeningStatus in (select ID from [dbo].[PH_JOB_STATUS_S] with(nolock) where jscode ='MCV')

	declare @currentDtYr int = case when @currentDt is not null then datepart(year,@currentDt) else 0 end;
	declare @currentDtMnth int = case when @currentDt is not null then datepart(month,@currentDt) else 0 end;
	--highlight -> TargetThisPeriod(filled) cv 
	--select @highlightTrgtPeriodFilled= count(1) from [dbo].[PH_JOB_CANDIDATES] where JOID in (select ID from #TempDashboard_HireAdmin_job) and candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode in('SUC','PNS'))
	select @highlightTrgtPeriodFilled = count(distinct [CandProfId]) from @TempDashboard_Recruiter_job_candHistory where statusCode in ( 'SUC','PNS')
		and (@currentDt is null or (datepart(year,activityDate) = @currentDtYr and datepart(month,activityDate) = @currentDtMnth))
		and (LEN(coalesce(@puIds,''))=0 or ([JobId] in (select JOID from [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] jbDtl with(nolock) where PUID in (select cast(value as int) from string_split(@puIds,',')))))
		and (LEN(coalesce(@buIds,''))=0 or ([JobId] in (select JOID from [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] jbDtl with(nolock) where BUID in (select cast(value as int) from string_split(@buIds,',')))));
	-- highlight -> submitted(required)
	select @highlightTrgtPeriodRequired= sum(coalesce(Target_qty_set,0)) 
	from vwUserTargetsByMonthAndTarget	
	where (@currentDt is null or (datepart(year,month_year) = @currentDtYr and datepart(month,month_year) = @currentDtMnth)) and 
	( 
			(@userType = 1 or --SuperAdmin
			(@userType = 2 or --Admin
			(@userType = 3 --BDM
				and UserId in (select AssignedTo from [dbo].[PH_JOB_ASSIGNMENTS] with(nolock) where JOID in (select ID from #TempDashboard_HireAdmin_job) and DeassignDate is null
		and (LEN(coalesce(@puIds,''))=0 or (JOID in (select JOID from [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] jbDtl with(nolock) where PUID in (select cast(value as int) from string_split(@puIds,',')))))
		and (LEN(coalesce(@buIds,''))=0 or (JOID in (select JOID from [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] jbDtl with(nolock) where BUID in (select cast(value as int) from string_split(@buIds,','))))))))) or 
			(@userType = 4 and @userId = UserId) --Recruiter 4
			--Candidate 5
	)
	and targetValue='Recruitment' and targetDescription='Joinees'	
	--highlight -> Yet to join 
	--select @highlightYetToJoin= count(1) from [dbo].[PH_JOB_CANDIDATES] with(nolock) where candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode='PNS') and JOID in (select ID from #TempDashboard_HireAdmin_job)
	select @highlightYetToJoin = count(distinct [CandProfId]) from @TempDashboard_Recruiter_job_candHistory where statusCode in ( 'PNS') and [CandProfId] not in (select [CandProfId] from @TempDashboard_Recruiter_job_candHistory where statusCode in ( 'SUC') and cast (datediff (day, 0, activityDate) as datetime) = @currentDt)
		and (@currentDt is null or (datepart(year,activityDate) = @currentDtYr and datepart(month,activityDate) = @currentDtMnth))
		and (LEN(coalesce(@puIds,''))=0 or ([JobId] in (select JOID from [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] jbDtl with(nolock) where PUID in (select cast(value as int) from string_split(@puIds,',')))))
		and (LEN(coalesce(@buIds,''))=0 or ([JobId] in (select JOID from [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] jbDtl with(nolock) where BUID in (select cast(value as int) from string_split(@buIds,',')))));
	--highlight -> Joined 
	--select @highlightJoined= count(1) from [dbo].[PH_JOB_CANDIDATES] with(nolock) where candProfStatus in ( select ID from PH_CAND_STATUS_S where cscode='SUC') and JOID in (select ID from #TempDashboard_HireAdmin_job)
	select @highlightJoined = count(distinct [CandProfId]) from @TempDashboard_Recruiter_job_candHistory where statusCode in ( 'SUC')
		and (@currentDt is null or (datepart(year,activityDate) = @currentDtYr and datepart(month,activityDate) = @currentDtMnth))
		and (LEN(coalesce(@puIds,''))=0 or ([JobId] in (select JOID from [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] jbDtl with(nolock) where PUID in (select cast(value as int) from string_split(@puIds,',')))))
		and (LEN(coalesce(@buIds,''))=0 or ([JobId] in (select JOID from [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] jbDtl with(nolock) where BUID in (select cast(value as int) from string_split(@buIds,',')))));
	
	DROP table #TempDashboard_HireAdmin_job;
	select @activeJobCount activeJobCount, @newJobCount newJobCount, @holdJobCount holdJobCount, @reopenJobCount reopenJobCount, @morecvsJobCount morecvsJobCount, @submittedJobsFilled submittedJobsFilled, @submittedJobsRequired submittedJobsRequired,
	@highlightTrgtPeriodFilled highlightTrgtPeriodFilled, @highlightTrgtPeriodRequired highlightTrgtPeriodRequired, @highlightYetToJoin highlightYetToJoin, @highlightJoined highlightJoined
		
end



