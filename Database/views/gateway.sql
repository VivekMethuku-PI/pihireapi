Alter view vwUserPuBu
as
select 
	AppUser.ID as UserId,
	Pus.ProcessUnit,Pus.BusinessUnit
from
	[dbo].[PI_HIRE_USERS] AppUser with (nolock) 
	join [gateway_uat].[dbo].[PI_APP_USERS] GwUser with (nolock)  on AppUser.UserId = GwUser.ID  
	join [gateway_uat].[dbo].[PI_APP_USER_PU_BU] Pus with (nolock)  on GwUser.ID = Pus.AppUserID 
where AppUser.Status != 5 and Pus.Status != 5 and GwUser.Status !=5 and GwUser.applicationID=2 and Pus.applicationID=2

Create view vwUserTargetsByMonthAndTarget
AS
select 
	AppUser.ID as UserId,
	targ.Target_qty_set, targ.month_year, 
	refMstr.value targetValue, refMstr.description targetDescription
from 
	[dbo].[PI_HIRE_USERS] AppUser with (nolock) 
	join [gateway_uat].[dbo].[tbl_param_targets] targ with (nolock) on AppUser.EmployID = targ.Employee_id  
	left outer join [gateway_uat].[dbo].[tbl_param_reference_master] refMstr with (nolock)  on targ.Target_parameter_id=refMstr.ID and refMstr.groupId=81
where AppUser.Status != 5 


