CREATE OR ALTER PROCEDURE [dbo].[Sp_Dashboard_Daywise_Filter]
	@currentDt datetime,
	@filterUserType int,

	@onLeave bit,
	@locationId int,
	@jobId int,
	@filterUserIds nvarchar(max),

	--Authorization
	@loginUserType int,
	@loginUserId int
AS
begin
	declare @tblFilterUserIds table(id int)
	if LEN(coalesce(@filterUserIds,''))>0
	begin
		insert @tblFilterUserIds select cast(value as int) from  string_split(@filterUserIds,',')
	end
	select 
		vwDash.jobId,
		vwDash.bdmId,
		vwDash.recruiterID,
		vwDash.NoCVSRequired,
		vwDash.NoCVSUploadded,
		vwDash.NoOfFinalCVsFilled,
		vwDash.jobAssignmentDate
		--vwDash.PUID,
		--vwDash.BUID
		--vwDash.ClosedDate
	from 
		dbo.[vwDashboardDaywiseFilterData] vwDash with (nolock)
	where
		--Authorization
		( 
			(@loginUserType = 1) or --SuperAdmin
			(@loginUserType = 2 and vwDash.jobId in (select JOID from [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS] jbDtl with(nolock) inner join [dbo].[vwUserPuBu] vw with(nolock) on /*jbDtl.BUID=vw.[BusinessUnit] and*/ jbDtl.PUID=vw.[ProcessUnit] and vw.UserId=@loginUserId)) or --Admin
			(@loginUserType = 3 and @loginUserId = vwDash.bdmId) or --BDM
			(@loginUserType = 4 and @loginUserId = vwDash.recruiterID) or --Recruiter
			--Candidate 5
			(@loginUserType > 4 and 1 = 0)
			--Hire manager [dbo].[PH_JOB_OPENINGS_ADDL_DETAILS]
		)
		
		and (@onLeave is null 
			or (@onLeave=1 and 
					(
						(@filterUserType = 3 and vwDash.bdmId in (select usr.ID from [dbo].[PH_EMP_LEAVE_REQUEST] leave with(nolock) inner join [dbo].[PI_HIRE_USERS] usr with(nolock) on usr.ID=leave.empId where usr.Status != 5 and leave.[status]=2 and @currentDt between leave.LeaveStartDate and LeaveEndDate)) or
						(@filterUserType = 4 and vwDash.recruiterID in (select usr.ID from [dbo].[PH_EMP_LEAVE_REQUEST] leave with(nolock) inner join [dbo].[PI_HIRE_USERS] usr with(nolock) on usr.ID=leave.empId where usr.Status != 5 and leave.[status]=2 and @currentDt between leave.LeaveStartDate and LeaveEndDate))
					)
				)
			or (@onLeave=0 and 
					(
						(@filterUserType = 3 and vwDash.bdmId not in (select usr.ID from [dbo].[PH_EMP_LEAVE_REQUEST] leave with(nolock) inner join [dbo].[PI_HIRE_USERS] usr with(nolock) on usr.ID=leave.empId where usr.Status != 5 and leave.[status]=2 and @currentDt between leave.LeaveStartDate and LeaveEndDate)) or
						(@filterUserType = 4 and vwDash.recruiterID not in (select usr.ID from [dbo].[PH_EMP_LEAVE_REQUEST] leave with(nolock) inner join [dbo].[PI_HIRE_USERS] usr with(nolock) on usr.ID=leave.empId where usr.Status != 5 and leave.[status]=2 and @currentDt between leave.LeaveStartDate and LeaveEndDate))
					)
				)
			)
		and (@locationId is null or (
						(@filterUserType = 3 and vwDash.bdmId in (select usr.ID from [dbo].[PI_HIRE_USERS] usr with(nolock) where usr.Status != 5 and usr.UserType = 3 and usr.LocationID = @locationId)) or
						(@filterUserType = 4 and vwDash.recruiterID in (select usr.ID from [dbo].[PI_HIRE_USERS] usr with(nolock) where usr.Status != 5 and usr.UserType = 4  and usr.LocationID = @locationId))
					)
			)
		and (@jobId is null or @jobId = jobId)
		and (LEN(coalesce(@filterUserIds,'')) = 0 or (
						(@filterUserType = 3 and vwDash.bdmId in (select id from @tblFilterUserIds)) or
						(@filterUserType = 4 and vwDash.recruiterID in (select id from @tblFilterUserIds))
					)
			)
	order by 
		vwDash.closeddate desc
end
Go
