insert into "Cameras"(
		"Id",
		"Name",
		"StreamURL",
		"SupportsPTZViaONVIF",
		"Certificate",
		"RecordMode",
		"Enabled",
		"ONVIFUrl",
		"ONVIFUsername",
		"ONVIFPassword"
	)
values (
		@Id,
		@Name,
		@StreamURL,
		@SupportsPTZViaONVIF,
		@Certificate,
		@RecordMode,
		@Enabled,
		@ONVIFUrl,
		@ONVIFUsername,
		@ONVIFPassword
	);
