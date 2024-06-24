USE [piHIRE1.0_QA]
GO
Alter PROCEDURE sp_customScheduler_candidate_jobOpenings
	@puIds nvarchar(max), @buIds nvarchar(max), @gndr nvarchar(1), @cntrIds nvarchar(max), @candStatus nvarchar(max), @jobId int
AS
BEGIN
with cte (TechnologyId) as (select distinct TechnologyId from [dbo].[PH_JOB_OPENING_SKILLS] where JOID=@jobId)
	select distinct candPrfl.ID from [dbo].[PH_CANDIDATE_PROFILES] as candPrfl 
						left outer join [dbo].[PH_JOB_CANDIDATES] as jbCand on candPrfl.ID = jbCand.CandProfID
						left outer join [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] as jbDtls on jbCand.JOID=jbDtls.JOID

						left outer join (
							SELECT DISTINCT candSkl2.CandProfID, 
								SUBSTRING(
									(
										SELECT ','+CAST(candSkl1.TechnologyId AS NVARCHAR(MAX)) 
										FROM (SELECT DISTINCT TechnologyID, CandProfID FROM [dbo].[PH_CANDIDATE_SKILLSET] WHERE SelfRating=5 AND TechnologyID in (select TechnologyID from cte) ) candSkl1
										WHERE candSkl1.CandProfID = candSkl2.CandProfID
										ORDER BY candSkl1.TechnologyID
										FOR XML PATH ('')
									), 2, 1000) skills
							FROM dbo.[PH_CANDIDATE_SKILLSET] candSkl2
						) as candSkill on candPrfl.ID =  candSkill.CandProfID

	where (candPrfl.[Status]!=5 and candPrfl.[Status]!=4) and
		candPrfl.ID not in (select CandProfID from [dbo].[PH_JOB_CANDIDATES] where joid=@jobId) and

		( substring((select ','+CAST(TechnologyID AS NVARCHAR(MAX)) from cte order by TechnologyID for XML PATH('')),2,1000)=candSkill.skills		) and

		 (len(@puIds)=0 or @puIds='0' or jbDtls.PUID in (select cast(value as int) val from string_split(@puIds,','))) and
		(len(@buIds)=0 or @buIds='0' or jbDtls.BUID in (select cast(value as int) val from string_split(@buIds,','))) and
		(len(@candStatus)=0 or @candStatus='0' or jbCand.CandProfStatus in (select cast(value as int) val from string_split(@candStatus,','))) and
		(len(@gndr)=0 or @gndr='A' or candPrfl.Gender in (select ID from dbo.PH_REF_MASTER_S where GroupID = 55 and RMValue = (case @gndr when 'M' then 'Male' when 'F' then 'Female' else '' end))) and
		 (len(@cntrIds)=0 or @cntrIds='0' or candPrfl.CountryID in (select cast(value as int) val from string_split(@cntrIds,',')))
END 