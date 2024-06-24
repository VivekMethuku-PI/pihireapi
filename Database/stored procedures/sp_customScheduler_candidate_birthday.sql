USE [piHIRE1.0_QA]
GO
Alter PROCEDURE sp_customScheduler_candidate_birthday
	@currentDate datetime,
	@puIds nvarchar(max), @buIds nvarchar(max), @gndr nvarchar(1), @cntrIds nvarchar(max), @candStatus nvarchar(max)
AS
BEGIN
	declare @curMnth int,@curDay int;
	set @curMnth = datepart(month, @currentDate);
	set @curDay = datepart(DAY, @currentDate);
	select distinct candPrfl.ID from [dbo].[PH_CANDIDATE_PROFILES] as candPrfl 
						left outer join [dbo].[PH_JOB_CANDIDATES] as jbCand on candPrfl.ID = jbCand.CandProfID
						left outer join [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] as jbDtls on jbCand.JOID=jbDtls.JOID

	where (candPrfl.[Status]!=5 and candPrfl.[Status]!=4) and 
		candPrfl.DOB is not null and datepart(month, candPrfl.DOB)=@curMnth and datepart(DAY, candPrfl.DOB)=@curDay and 
		(len(@puIds)=0 or @puIds='0' or jbDtls.PUID in (select cast(value as int) val from string_split(@puIds,','))) and
		(len(@buIds)=0 or @buIds='0' or jbDtls.BUID in (select cast(value as int) val from string_split(@buIds,','))) and
		(len(@candStatus)=0 or @candStatus='0' or jbCand.CandProfStatus in (select cast(value as int) val from string_split(@candStatus,','))) and
		(len(@gndr)=0 or @gndr='A' or candPrfl.Gender in (select ID from dbo.PH_REF_MASTER_S where GroupID = 55 and RMValue = (case @gndr when 'M' then 'Male' when 'F' then 'Female' else '' end))) and
		 (len(@cntrIds)=0 or @cntrIds='0' or candPrfl.CountryID in (select cast(value as int) val from string_split(@cntrIds,',')))
END 