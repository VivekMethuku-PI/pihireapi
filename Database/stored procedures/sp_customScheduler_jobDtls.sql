USE [piHIRE1.0_DEV]
GO
Alter PROCEDURE sp_customScheduler_jobDtls
	@jobId int
AS
BEGIN
	select distinct
		job.Id,job.JobDescription,job.MaxExpeInMonths,job.MinExpeInMonths, job.JobTitle, job.ClosedDate, job.PostedDate, job.ClientName,
		coalesce(cntry.Name,'') JobCountry, coalesce(cty.Name,'') JobLocation, coalesce(jbStatus.Title,'') JobStatus, coalesce(refMstr.Rmvalue,'') JobCurrencyName,
		jobAdl.PUID,jobAdl.BUID,
		(recruiter.FirstName +' '+recruiter.LastName) recruiterName, recruiter.MobileNumber recruiterMobileNumber,recruiter.EmailSignature recruiterEmailSignature,recruiter.EmailID recruiterEmailID, recruiter.UserRoleName recruiterPosition,
		(bdm.FirstName +' '+bdm.LastName) bdmName
	from [dbo].[PH_JOB_OPENINGS] as job 
	left outer join [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] as jobAdl on job.ID= jobAdl.JOID
	left outer join [dbo].[PH_COUNTRY] as cntry on cntry.ID=job.CountryId
	left outer join [dbo].[PH_CITY] as cty on cty.id= job.JobLocationId
	left outer join [dbo].[PH_JOB_STATUS_S] as jbStatus on jbStatus.id =job.JobOpeningStatus
	left outer join [dbo].[PH_REF_MASTER_S] as refMstr on refMstr.GroupId = 13 and refMstr.Id = jobAdl.CurrencyId

	left outer join (select * from(select JOID,AssignedTo,ROW_NUMBER() OVER(PARTITION BY JOID ORDER BY CreatedDate ASC) AS rk from [dbo].[PH_JOB_ASSIGNMENTS] where DeassignDate is null)a where rk=1) as RecruiterId on RecruiterId.JOID=job.ID
	left outer join [dbo].[PI_HIRE_USERS] as recruiter on recruiter.ID=RecruiterId.AssignedTo
	left outer join [dbo].[PI_HIRE_USERS] as bdm on bdm.ID=coalesce(job.BroughtBy,job.createdby)

	where job.ID=@jobId
end