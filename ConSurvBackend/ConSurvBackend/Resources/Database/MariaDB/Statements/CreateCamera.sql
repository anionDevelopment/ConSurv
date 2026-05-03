insert into `Cameras`(
		`Id`,
		`Name`,
		`StreamURL`,
		`SupportsPTZViaONVIF`,
		`Certificate`,
		`RecordMode`,
		`Enabled`
	)
values (
		@Id,
		@Name,
		@StreamURL,
		@SupportsPTZViaONVIF,
		@Certificate,
		@RecordMode,
		@Enabled
	);
