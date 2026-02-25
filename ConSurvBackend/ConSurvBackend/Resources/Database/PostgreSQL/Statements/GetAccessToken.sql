
select "UserId", "ExpiredMoment"
	from "AccessToken"
	where "Value"=@Value;
