update "Cameras"
	set "Name"=@Name,
		"StreamURL"=@StreamURL,
		"SupportsPTZViaONVIF"=@SupportsPTZViaONVIF,
		"Certificate"=@Certificate,
		"RecordMode"=@RecordMode,
		"Enabled"=@Enabled,
		"ONVIFUrl"=@ONVIFUrl,
		"ONVIFUsername"=@ONVIFUsername,
		"ONVIFPassword"=@ONVIFPassword
	where "Id"=@Id;
