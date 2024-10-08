CREATE OR ALTER PROCEDURE [dbo].[Sp_Day_Wise_Assignment_Jobs_List_Count]

	@FilterKey nvarchar(max),
	@puId int,
	@bdmId int,
	@JobPriority int,
	@FromDate Datetime,
	@ToDate Datetime,

	@Assign bit,
	@PriorityUpdate bit,
	@Note bit,
	@Interviews bit,
	@JobStatus bit,
	@clientId int,

	--Authorization
	@loginUserType int,
	@loginUserId int

AS
BEGIN   
 

	SET NOCOUNT ON;

   SELECT Job.ID,Job.ClosedDate,Job.CreatedDate from PH_JOB_OPENINGS as Job  
         JOIN  dbo.PH_JOB_OPENINGS_ADDL_DETAILS as jobAddl  on Job.Id = jobAddl.JOID 
		 JOIN  dbo.PH_DAY_WISE_JOB_ACTIONS AS JobActions ON Job.Id = JobActions.JOID
         LEFT JOIN dbo.PH_COUNTRY as Cuntry  on Job.CountryID = Cuntry.Id 
         LEFT JOIN dbo.PH_CITY as city  on Job.JobLocationID = city.Id 
         LEFT JOIN dbo.PH_JOB_STATUS_S as JobStatus  on Job.JobOpeningStatus = JobStatus.Id 
         LEFT JOIN dbo.PI_HIRE_USERS as HireUser on Job.CreatedBy = HireUser.UserId 
         LEFT OUTER JOIN dbo.Ph_Job_Opening_Actv_Counter as Counter on Job.Id = Counter.JOID and Counter.Status !=5
		
 	Where 
		--Authorization
		( 
			(@loginUserType = 1) or --SuperAdmin
			(@loginUserType = 2 and Job.ID in (select JOID from [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw with(nolock) on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@loginUserId)) or --Admin
			(@loginUserType = 3 and @loginUserId = coalesce(Job.BroughtBy,Job.CreatedBy)) or --BDM
			(@loginUserType = 4 and Job.ID in (select asgn.JOID from dbo.PH_JOB_ASSIGNMENTS asgn where asgn.AssignedTo = @loginUserId and asgn.DeassignDate is null)) or --Recruiter
			--Candidate 5
			(@loginUserType > 4 and 1 = 0)
			--Hire manager [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS]
		)	AND
		JobStatus.JSCode !='CLS' AND 

	    (@FilterKey is null or (Job.Id like '%'+@FilterKey+'%' or Job.JobTitle like '%'+@FilterKey+'%' or Job.JobRole like '%'+@FilterKey+'%')) and  

		(@puId is null or (jobAddl.PUID = @puId)) AND 
		(@bdmId is null or (coalesce(Job.BroughtBy,Job.createdby) = @bdmId)) AND
		(@JobPriority is null or Job.[Priority] = @JobPriority) AND
	    (@FromDate is null or (JobActions.CreatedDate >= @FromDate and JobActions.CreatedDate <= @ToDate)) AND

		(@Assign is null or JobActions.[Assign] = @Assign) AND
		(@PriorityUpdate is null or JobActions.[Priority] = @PriorityUpdate) AND
		(@Note is null or JobActions.Note = @Note) AND
		(@Interviews is null or JobActions.Interviews = @Interviews) AND
		(@JobStatus is null or JobActions.JobStatus = @JobStatus) AND
		(@clientId is null or (Job.ClientID = @clientId)) 
	   

 END
 Go

CREATE OR ALTER PROCEDURE [dbo].[Sp_Day_Wise_Assignment_Jobs_List]

	@FilterKey nvarchar(max),
	@puId int,
	@bdmId int,
	@JobPriority int,
	@FromDate Datetime,
	@ToDate Datetime,
	@PerPage int,
	@CurrentPage int,

	@Assign bit,
	@PriorityUpdate bit,
	@Note bit,
	@Interviews bit,
	@JobStatus bit,
	@clientId int,

	--Authorization
	@loginUserType int,
	@loginUserId int

AS
BEGIN   

	SET NOCOUNT ON 

	SELECT
		Job.JobLocationID,
		city.id AS CityId,
	
	    dbo.fn_titlecase(city.name) AS CityName,
		Cuntry.nicename AS CountryName,
		CountryID,

		Job.ClientId,
		Job.ClientName,
		Job.ClosedDate,
		Job.Id AS Id,
		Job.JobRole,
		Job.JobTitle,
		Job.JobDescription,
		Job.PostedDate AS StartDate,
		Job.JobOpeningStatus,
		JobStatus.Title AS JobOpeningStatusName,
		JobStatus.JSCode,
		Job.[status],
		Job.MinExpeInMonths / 12 AS MinExp,
		Job.MaxExpeInMonths / 12 AS MaxExp,
		Job.CreatedDate,
		Job.CreatedBy,
		HireUser.FirstName AS CreatedByName,
		Job.ShortJobDesc,

		JobActions.Assign,
		JobActions.[Priority] AS PriorityUpdate,
		JobActions.Note,
		JobActions.Interviews,
		JobActions.JobStatus,
		JobActions.CandStatus,


		[Counter].AsmtCounter,
		[Counter].JobPostingCounter,
		[Counter].ClientViewsCounter,
		[Counter].EmailsCounter,

		jobAddl.NoOfCvsRequired,

		ACTIVITY_LOG.ModificationOn,
		ACTIVITY_LOG.ModificationBy,	
		
		Job.[PRIORITY] AS [Priority],
		Ref.RMValue As PriorityName,
		
		CAST(DATEDIFF(DAY, CASE
         WHEN Job.ReopenedDate is null THEN Job.CreatedDate
         ELSE Job.ReopenedDate END, GETDATE()) AS VARCHAR(10)) AS Age,

		(SELECT COUNT(id) FROM dbo.Ph_Candidate_Profiles_Shared cps with (nolock) WHERE cps.Joid = Job.id) AS ProfilesSharedToClientCounter,

		(SELECT COUNT(id) FROM dbo.PH_JOB_ASSIGNMENTS JA with (nolock) WHERE JA.Joid = Job.id and JA.DeassignDate is null) AS Assinged, 

		(select SUM(coalesce(NoOfFinalCVsFilled,0)) from [dbo].[PH_JOB_ASSIGNMENTS] jobRecr with (nolock)
		where Status!=5 and jobRecr.JOID=job.ID) AS NoOfCvsFullfilled

	FROM
		dbo.PH_JOB_OPENINGS AS Job with (nolock)
		JOIN
			dbo.PH_JOB_OPENINGS_ADDL_DETAILS AS jobAddl with (nolock) ON Job.Id = jobAddl.JOID
		JOIN
			dbo.PH_DAY_WISE_JOB_ACTIONS AS JobActions with (nolock) ON Job.Id = JobActions.JOID
		LEFT JOIN
			dbo.PH_REF_MASTER_S AS Ref with (nolock) ON Job.Priority = Ref.Id
		LEFT JOIN
			dbo.PH_COUNTRY AS Cuntry with (nolock) ON Job.CountryID = Cuntry.Id
		LEFT JOIN
			dbo.PH_CITY AS city with (nolock) ON Job.JobLocationID = city.Id
		LEFT JOIN
			dbo.PH_JOB_STATUS_S AS JobStatus with (nolock) ON Job.JobOpeningStatus = JobStatus.Id
		LEFT JOIN
			dbo.PI_HIRE_USERS AS HireUser with (nolock) ON Job.CreatedBy = HireUser.id and HireUser.UserType !=5  -- candidate
		LEFT OUTER JOIN
			dbo.Ph_Job_Opening_Actv_Counter AS [Counter] with (nolock) ON Job.Id = [Counter].JOID AND [Counter].Status != 5
		CROSS APPLY (
			SELECT TOP 1
				Logs.CreatedDate AS ModificationOn,CONCAT(HireUser.FirstName , HireUser.LastName) AS ModificationBy
			FROM
				dbo.PH_ACTIVITY_LOG AS Logs with (nolock)
			JOIN
				dbo.PI_HIRE_USERS AS HireUser with (nolock) ON Logs.Createdby = HireUser.Id and UserType in (1,2,3)
			WHERE
				ActivityMode = 2
				AND ActivityType IN (7, 2)
				AND Logs.JoId = Job.Id
			ORDER BY
				Logs.id DESC
		) ACTIVITY_LOG
	Where 
		--Authorization
		( 
			(@loginUserType = 1) or --SuperAdmin
			(@loginUserType = 2 and Job.ID in (select JOID from [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw with(nolock) on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@loginUserId)) or --Admin
			(@loginUserType = 3 and @loginUserId = coalesce(Job.BroughtBy,Job.CreatedBy)) or --BDM
			(@loginUserType = 4 and Job.ID in (select asgn.JOID from dbo.PH_JOB_ASSIGNMENTS asgn where asgn.AssignedTo = @loginUserId and asgn.DeassignDate is null)) or --Recruiter
			--Candidate 5
			(@loginUserType > 4 and 1 = 0)
			--Hire manager [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS]
		)	AND
		JobStatus.JSCode !='CLS'  AND

	    (@FilterKey is null or (Job.Id like '%'+@FilterKey+'%' or Job.JobTitle like '%'+@FilterKey+'%' or Job.JobRole like '%'+@FilterKey+'%')) and  
		
		(@puId is null or (jobAddl.PUID = @puId)) and 
		(@bdmId is null or (coalesce(Job.BroughtBy,Job.createdby) = @bdmId)) and 
		(@JobPriority is null or Job.[Priority] = @JobPriority) and 
		(@FromDate is null or (JobActions.CreatedDate >= @FromDate and JobActions.CreatedDate <= @ToDate)) AND

		(@Assign is null or JobActions.[Assign] = @Assign) and
		(@PriorityUpdate is null or JobActions.[Priority] = @PriorityUpdate) and
		(@Note is null or JobActions.Note = @Note) and
		(@Interviews is null or JobActions.Interviews = @Interviews) and
		(@JobStatus is null or JobActions.JobStatus = @JobStatus) AND
		(@clientId is null or (Job.ClientID = @clientId))

		ORDER by Job.CreatedDate desc offset @CurrentPage rows fetch next @PerPage rows only

END



