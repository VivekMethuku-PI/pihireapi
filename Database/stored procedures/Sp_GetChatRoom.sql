USE [piHIRE1.0_DEV]
GO
alter PROCEDURE [dbo].[Sp_GetChatRoom]
	--pagination
	@fetchCount int,--0 if no pagination
	@offsetCount int,
	--Authorization
	@userId int
AS
begin
	
	if(@fetchCount != 0)
		select 
			room.ID as [RoomId],
			room.JoID as [JobId],
			room.UpdatedDate as [RoomUpdatedDate],
			chat.[Count],
			chat.UnreadCount,
			chat.LatestMessageDt,
			(case when room.UserID = @userId then room.CreatedBy else room.UserID end) ReceiverId
		from (
			select 
				ChatRoomID, max(CreatedDate) LatestMessageDt,count(1) [Count], SUM(unread) as UnreadCount
			from (
				select 
					ChatRoomID,CreatedDate,(case when (ReadStatus=0 and ReceiverID=@userId) then 1 else 0 end) unread
				from [dbo].[ph_chat_messages]
				where (SenderID=@userId or ReceiverID=@userId) and [Status]=1) chat 
			group by ChatRoomID
		)chat
		inner join [dbo].[ph_chat_rooms] room on room.ID= chat.ChatRoomID and room.[Status]=1
		order by chat.LatestMessageDt desc offset @offsetCount rows fetch next @fetchCount rows only;
	else
		select 
			room.ID as [RoomId],
			room.JoID as [JobId],
			room.UpdatedDate as [RoomUpdatedDate],
			chat.[Count],
			chat.UnreadCount,
			chat.LatestMessageDt,
			(case when room.UserID = @userId then room.CreatedBy else room.UserID end) ReceiverId
		from (
			select 
				ChatRoomID, max(CreatedDate) LatestMessageDt,count(1) [Count], SUM(unread) as UnreadCount
			from (
				select 
					ChatRoomID,CreatedDate,(case when (ReadStatus=0 and ReceiverID=@userId) then 1 else 0 end) unread/*, row_number() over (partition by ChatRoomID order by CreatedDate desc) as rowNo*/ 
				from [dbo].[ph_chat_messages]
				where (SenderID=@userId or ReceiverID=@userId) and [Status]=1) chat 
			group by ChatRoomID
		)chat
		inner join [dbo].[ph_chat_rooms] room on room.ID= chat.ChatRoomID and room.[Status]=1	
end


Create PROCEDURE [dbo].[Sp_GetChatRoomCount]
	--Authorization
	@userId int
AS
begin
		select 
			count(1) RoomsCount
		from (
			select 
				distinct ChatRoomID 
			from 
				[dbo].[ph_chat_messages]
			where 
				(SenderID=@userId or ReceiverID=@userId) and [Status]=1		
		)chat
		inner join [dbo].[ph_chat_rooms] room on room.ID= chat.ChatRoomID and room.[Status]=1	
end