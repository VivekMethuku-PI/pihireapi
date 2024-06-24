
Alter view vwJobCandidateStatusHistory
AS
    select Distinct
		job.ID as JobId,job.JobTitle,job.ClientID,job.ClientName,
		ActvyLog.CurrentStatusId,ActvyLog.UpdateStatusId,
		ActvyLog.ActivityOn CandProfId, CandProfile.CandName,
		CandStatus.CSCode StatusCode, CAST(ActvyLog.CreatedDate as DATE) ActivityDate, 
		Jobdtls.PUID, Jobdtls.BUID,
		JobCand.RecruiterId,jobCand.OPGrossPayPerMonth, coalesce(job.BroughtBy,job.createdby) BroughtBy
	from   [dbo].[PH_ACTIVITY_LOG] as ActvyLog with(nolock)
		   join [dbo].[PH_CAND_STATUS_S] as CandStatus with(nolock) on ActvyLog.UpdateStatusId = CandStatus.Id
		   join [dbo].[PH_JOB_OPENINGS] as job with(nolock) on ActvyLog.joid = job.id   
		   join [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] as Jobdtls with(nolock) on ActvyLog.joid = Jobdtls.joid
		   join [dbo].[PH_CANDIDATE_PROFILES] as CandProfile with(nolock) on ActvyLog.ActivityOn = CandProfile.Id 
		   join [dbo].[PH_JOB_CANDIDATES] as JobCand with(nolock) on ActvyLog.ActivityOn = JobCand.CandProfID and JobCand.joid = ActvyLog.joid
   where
		-- Candidate
		ActvyLog.ActivityMode = 1 
		-- StatusUpdates
		and ActvyLog.ActivityType = 3 
		and ActvyLog.UpdateStatusId is not null
	
--CREATE unique CLUSTERED INDEX ix_vwJobCandidateStatusHistory ON vwJobCandidateStatusHistory (JobId,CandProfId,StatusCode,ActivityDate);

--CREATE unique CLUSTERED INDEX ix_vwJobCandidateStatusHistory ON vwJobCandidateStatusHistory (JobId,CandProfId,StatusCode,ActivityDate,PUID,BUID,RecruiterId,BroughtBy);