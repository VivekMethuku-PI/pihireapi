--Drop table dbo.tmpAllCandidates
create table dbo.tmpAllCandidates(
	--[PH_CANDIDATE_PROFILES]
	[CandProfID] int not null primary key clustered with (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
	[SourceID] [smallint] NULL,
	[EmailID] [nvarchar](100) NOT NULL,
	[CandName] [nvarchar](150) NULL,
	[FullNameInPP] [nvarchar](150) NULL,
	[DOB] [datetime] NULL,
	[Gender] [char](1) NULL,
	[MaritalStatus] [char](1) NULL,
	[CurrOrganization] [nvarchar](100) NULL,
	[CurrLocation] [nvarchar](100) NULL,
	[CurrLocationID] [int] NULL,
	[NoticePeriod] [tinyint] NULL,
	[ReasonType] [int] NULL,
	[ReasonsForReloc] [nvarchar](max) NULL,
	[CountryID] [int] NULL,
	[Nationality] [varchar](150) NULL,
	[Experience] [nvarchar](100) NULL,
	[ExperienceInMonths] [int] NULL,
	[RelevantExperience] [nvarchar](20) NULL,
	[ReleExpeInMonths] [int] NULL,
	[ContactNo] [nvarchar](20) NULL,
	[AlteContactNo] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CPCurrency] [nvarchar](5) NULL,
	[CPTakeHomeSalPerMonth] [int] NULL,
	[CPGrossPayPerAnnum] [int] NULL,
	--[PH_JOB_CANDIDATES]
	[CandProfStatus] [tinyint] NULL,
	[UpdatedDate] [datetime] NULL,		
	[CandJobId] [int] NULL,
	[JoId] [int] NULL,
	[StageID] [int] NULL,
	[RecruiterId] [int] NULL,
	[EPCurrency] [nvarchar](10) NULL,
	[EPTakeHomePerMonth] [int] NULL,
	[OPCurrency] [nvarchar](10) NULL,
	[OpTakeHomePerMonth] [int] NULL,
	--PH_COUNTRY
	[CountryName] varchar(80) NULL,
	--PI_HIRE_USERS
	[RecName] nvarchar(max) NULL,
	--PH_CAND_STATUS_S
	[CsCode] nvarchar(max) NULL,
	[CandProfStatusName] nvarchar(max) NULL,
	--none(suggest)
	[TagWords] nvarchar(max) NULL,
	[SelfRating] decimal(10,2) NULL,
	[Evaluation] decimal(10,2),
	--none(pool)
	[AllTagWords] nvarchar(max) NULL,
	[AllSelfRating] decimal(10,2) NULL,
	[AllEvaluation] decimal(10,2)
) with (memory_optimized=on,DURABILITY=SCHEMA_AND_DATA);


Alter PROCEDURE [dbo].[pi_Sp_tmpAllCandidates_Update]
AS
begin
    SET NOCOUNT ON;
	BEGIN TRAN

	BEGIN TRY
	
		delete dbo.tmpAllCandidates;
		insert into dbo.tmpAllCandidates
		--SELECT 
		--	--[PH_CANDIDATE_PROFILES]
		--	c.ID as CandProfID,
		--	c.SourceID,
		--	c.EmailID,
		--	c.CandName,
		--	c.FullNameInPP,
		--	c.DOB, 
		--	c.Gender, 
		--	c.MaritalStatus,
		--	c.CandOverallStatus as CandProfStatus,
		--	c.CurrOrganization, c.CurrLocation, c.CurrLocationID,
		--	c.NoticePeriod, 
		--	c.ReasonType, c.ReasonsForReloc,
		--	c.CountryID as CountryID,
		--	c.Nationality,
		--	c.Experience, c.ExperienceInMonths,
		--	c.RelevantExperience, c.ReleExpeInMonths,
		--	c.ContactNo, c.AlteContactNo,
		--	c.CreatedDate,c.UpdatedDate,
		--	c.CPCurrency, c.CPTakeHomeSalPerMonth, c.CPGrossPayPerAnnum,
		--	--[PH_JOB_CANDIDATES]
		--	ph.Id as CandJobId,
		--	ph.JoId,
		--	ph.StageId as StageID,
		--	ph.RecruiterID as RecruiterId,
		--	ph.EPCurrency as EPCurrency, ph.EPTakeHomePerMonth as EPTakeHomePerMonth,  
		--	ph.OpCurrency as OpCurrency,ph.OPGrossPayPerMonth as OpTakeHomePerMonth,
		--	--PH_COUNTRY
		--	Cuntry.nicename as CountryName, 
		--	--vwALLRecruiters
		--	CONCAT(RecUser.FirstName,' ',RecUser.LastName) as RecName,
		--	--none
		--	'' as CsCode,
		--	CASE
		--		WHEN CandOverallStatus = 3 THEN 'Blacklisted'
		--		WHEN CandOverallStatus = 2 THEN 'Joined'
		--		ELSE 'Available' END AS CandProfStatusName,	
		--	(select STUFF((select ','+ TaggingWord from [dbo].[PH_CANDIDATE_TAGS] as Tags  WITH(NOLOCK)
		--		where Tags.CandProfID = c.ID  and Status !=5  for xml path('')),1,1,'')) as TagWords,
		--	(select Convert(decimal(10,2),SUM(pjc.SelfRating)/Count(*)) from PH_CANDIDATE_SKILLSET as pjc
		--		where pjc.CandProfId = c.ID) as SelfRating,
		--	(select Convert(decimal(10,2),SUM(pjce.Rating)/Count(*)) from PH_JOB_CANDIDATE_EVALUATION as pjce
		--		where pjce.CandProfId = c.ID) as Evaluation
	
		--FROM 
		--	dbo.PH_CANDIDATE_PROFILES c 
		--	left outer join (
		--	  SELECT  Id,JoId,RecruiterID,StageId,EPCurrency,CandProfId, ROW_NUMBER() OVER (PARTITION BY CandProfId ORDER BY ID DESC) AS RowNumber,
		--	  EPTakeHomePerMonth,OpCurrency,OPGrossPayPerMonth
		--	  FROM dbo.PH_JOB_CANDIDATES  
		--	  --WHERE CandProfId = c.Id
		--	  --ORDER BY Id desc
		--	  ) ph on ph.CandProfId = c.Id and ph.RowNumber=1
		--	left join dbo.vwALLRecruiters as RecUser WITH(NOLOCK) on ph.RecruiterID = RecUser.Id
		--	left join dbo.PH_COUNTRY as Cuntry  WITH(NOLOCK) on c.CountryID = Cuntry.Id   
		--order by ph.Id/*CandJobId*/ desc

		select 
			--[PH_CANDIDATE_PROFILES]		
			CANDIDATE_PROFILE.ID as CandProfID,        --JOB_CANDIDATES.CandProfID,
			CANDIDATE_PROFILE.SourceID,
			CANDIDATE_PROFILE.EmailID,
			CANDIDATE_PROFILE.CandName, 
			CANDIDATE_PROFILE.FullNameInPP,
			CANDIDATE_PROFILE.DOB,
			CANDIDATE_PROFILE.Gender,
			CANDIDATE_PROFILE.MaritalStatus,
			CANDIDATE_PROFILE.CurrOrganization,  CANDIDATE_PROFILE.CurrLocation, CANDIDATE_PROFILE.CurrLocationID,
			CANDIDATE_PROFILE.NoticePeriod,
			CANDIDATE_PROFILE.ReasonType, CANDIDATE_PROFILE.ReasonsForReloc,
			CANDIDATE_PROFILE.CountryID,
			CANDIDATE_PROFILE.Nationality,
			CANDIDATE_PROFILE.Experience, CANDIDATE_PROFILE.ExperienceInMonths,
			CANDIDATE_PROFILE.RelevantExperience, CANDIDATE_PROFILE.ReleExpeInMonths, 
			CANDIDATE_PROFILE.ContactNo, CANDIDATE_PROFILE.AlteContactNo,
			CANDIDATE_PROFILE.CreatedDate, 
			CANDIDATE_PROFILE.CPCurrency, CANDIDATE_PROFILE.CPTakeHomeSalPerMonth, CANDIDATE_PROFILE.CPGrossPayPerAnnum,
			--PH_JOB_CANDIDATES	
			JOB_CANDIDATES.CandProfStatus,
			JOB_CANDIDATES.UpdatedDate, 
			JOB_CANDIDATES.Id as CandJobId,	
			JOB_CANDIDATES.JoId,
			JOB_CANDIDATES.StageID,
			JOB_CANDIDATES.RecruiterId,
			JOB_CANDIDATES.EPCurrency as EPCurrency,
			JOB_CANDIDATES.EPTakeHomePerMonth as EPTakeHomePerMonth,
			JOB_CANDIDATES.OpCurrency as OpCurrency,
			JOB_CANDIDATES.OpGrossPayPerMonth as OpTakeHomePerMonth,
			--PH_COUNTRY
			Cuntry.nicename as CountryName,
			--PI_HIRE_USERS
			CONCAT(RecUser.FirstName,' ',RecUser.LastName) as RecName,
		
			--PH_CAND_STATUS_S CandProfStatusName 
			CAND_STATUS_S.CsCode,  
			CAND_STATUS_S.title as [CandProfStatusName],  -- no

			(select STUFF((select distinct ','+ TaggingWord from [dbo].[PH_CANDIDATE_TAGS] as Tags  WITH(NOLOCK)
				where Tags.JOID = JOB_CANDIDATES.JOID and Tags.CandProfID =JOB_CANDIDATES.CandProfID  
				and Status !=5  for xml path('')),1,1,'')) as TagWords,
			(select Convert(decimal(10,2),SUM(pjc.SelfRating)/Count(*)) from PH_JOB_CANDIDATES as pjc
				where pjc.CandProfId = CANDIDATE_PROFILE.ID) as SelfRating,
			(select Convert(decimal(10,2),SUM(pjce.Rating)/Count(*)) from PH_JOB_CANDIDATE_EVALUATION as pjce
				where  pjce.JoId = JOB_CANDIDATES.JoId and pjce.CandProfId = JOB_CANDIDATES.CandProfID) as Evaluation,

			(select STUFF((select distinct ','+ TaggingWord from [dbo].[PH_CANDIDATE_TAGS] as Tags  WITH(NOLOCK)
				where Tags.CandProfID = CANDIDATE_PROFILE.ID  and Status !=5  for xml path('')),1,1,'')) as [AllTagWords],
			(select Convert(decimal(10,2),SUM(pjc.SelfRating)/Count(*)) from PH_CANDIDATE_SKILLSET as pjc
				where pjc.CandProfId = CANDIDATE_PROFILE.ID) as [AllSelfRating],
			(select Convert(decimal(10,2),SUM(pjce.Rating)/Count(*)) from PH_JOB_CANDIDATE_EVALUATION as pjce
				where pjce.CandProfId = CANDIDATE_PROFILE.ID) as [AllEvaluation]
		from 
			dbo.PH_CANDIDATE_PROFILES as CANDIDATE_PROFILE 
			join (
				select * from (
					select *, row_number() over (
						partition by CandProfID
						order by CreatedDate desc
					) as row_num
					from dbo.PH_JOB_CANDIDATES
				) as ordered_widgets
				where ordered_widgets.row_num = 1
			) as JOB_CANDIDATES
				on CANDIDATE_PROFILE.ID = JOB_CANDIDATES.CandProfID
			join PH_CAND_STATUS_S as CAND_STATUS_S  WITH(NOLOCK) on JOB_CANDIDATES.CandProfStatus  = CAND_STATUS_S.Id
			left join PI_HIRE_USERS as RecUser on JOB_CANDIDATES.RecruiterID = RecUser.Id
			left join PH_COUNTRY as Cuntry  WITH(NOLOCK) on CANDIDATE_PROFILE.CountryID = Cuntry.Id 
		order by JOB_CANDIDATES.Id/*CandJobId*/ desc

		COMMIT TRAN
	END TRY
	BEGIN CATCH
	  ROLLBACK TRAN
	END CATCH
end


--CREATE TRIGGER dbo.trigger_Cand_Prof
--ON dbo.PH_CANDIDATE_PROFILES 
--AFTER INSERT, UPDATE, DELETE 
--AS 
--begin
--	exec [dbo].[pi_Sp_tmpAllCandidates_Update]
--end

--CREATE TRIGGER dbo.trigger_job_Cand_Prof
--ON dbo.PH_JOB_CANDIDATES 
--AFTER INSERT, UPDATE, DELETE 
--AS 
--begin
--	exec [dbo].[pi_Sp_tmpAllCandidates_Update]
--end
--drop table dbo.tmpAutoRunSp
create table dbo.tmpAutoRunSp
(
	isMain bit NOT NULL,
	isActive bit NOT NULL,
	isRunning bit NOT NULL,
	triggerSt datetime,
	triggerEnd datetime,
	triggerMins int NOT NULL,
	lastExecutionSt datetime,
	lastExecutionEnd datetime
);
insert [dbo].[tmpAutoRunSp] (ismain,isactive,triggerMins,isRunning) values(1,1,1,0);
--update dbo.tmpAutoRunSp set [isActive]=1,[triggerSt]=null,[triggerEnd]=null,[lastExecutionSt]=null,[lastExecutionEnd]=null

--use [master]
--go
Alter PROCEDURE dbo.pi_Sp_tmpAllCandidates_Update_trigger 
 AS
BEGIN  
    SET NOCOUNT ON;
    declare @delayTime nvarchar(50), @executedTime nvarchar(50)='';
	declare @dt datetime, @triggerMins int;
	if exists(select 1 from dbo.tmpAutoRunSp where isMain=1 and coalesce(isRunning,0)=0)
	begin
		update dbo.tmpAutoRunSp set triggerSt= getdate(),isRunning=1 where isMain=1;
		--while exists(select isMain from dbo.tmpAutoRunSp where isMain=1 and isActive=1)
		if exists(select isMain from dbo.tmpAutoRunSp where isMain=1 and isActive=1)
		begin
			select @triggerMins=triggerMins from dbo.tmpAutoRunSp where isMain=1 and isActive=1;
			set @dt= DATEADD(minute,@triggerMins,getdate());
			set @delayTime = 
				(case when datepart(hour, @dt)>9then '' else '0' end)+ cast(datepart(hour, @dt) as nvarchar(max)) +':'+ 
				   (case when datepart(minute, @dt)>9then '' else '0' end)+cast(datepart(minute, @dt) as nvarchar(max));
			print 'execute at '+@delayTime;
			--waitfor time @delayTime 
			begin
				update dbo.tmpAutoRunSp set lastExecutionSt= getdate() where isMain=1;
				if @executedTime!=@delayTime
				begin
					--Name for the stored proceduce you want to call on regular bases
					exec [dbo].[pi_Sp_tmpAllCandidates_Update];
					set @executedTime=@delayTime
					print 'Executed on:'+@executedTime;
				end
				else 
				begin 
					print 'Execution skipped on:'+@executedTime;
				end
				update dbo.tmpAutoRunSp set lastExecutionEnd= getdate() where isMain=1;
			end
		end
		update dbo.tmpAutoRunSp set triggerEnd= getdate(),isRunning=0 where isMain=1;
	end
END

--sp_procoption    @ProcName = 'pi_Sp_tmpAllCandidates_Update_trigger',
--                @OptionName = 'startup',
--                @OptionValue = 'on'
--                @OptionValue = 'off'

