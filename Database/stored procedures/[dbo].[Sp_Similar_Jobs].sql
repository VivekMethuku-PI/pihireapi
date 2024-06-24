CREATE OR ALTER PROCEDURE [dbo].[Sp_Similar_Jobs]
	@jobId int --= 256
AS
begin

	declare @tbl table(id int)
	insert @tbl
	select distinct TechnologyID from [dbo].[PH_JOB_OPENING_SKILLS] where [Status] = 1 and JOID = @jobId;
	
	declare @jobRole nvarchar(max), @skillCount int; 
	select @skillCount=count(1) from @tbl
	select @jobRole = JobRole from [dbo].[PH_JOB_OPENINGS] where ID = @jobId;


	select Job.ID as JobId
	from
		dbo.PH_JOB_OPENINGS as Job  WITH(NOLOCK) 
	where  1 = 1
		--and Job.ClosedDate >= GetDate() 
		and Job.[Status] = 1

		and ( 
			job.JobRole = @jobRole
			or 
			(case when @skillCount = 0 then 0 else 
				(select count(1)/@skillCount from [dbo].[PH_JOB_OPENING_SKILLS] as JobSkill  WITH(NOLOCK) where JobSkill.[Status] = 1 and JobSkill.JOID = Job.ID and  JobSkill.TechnologyID in (select id from @tbl))
			end) > 0.7
		)
end