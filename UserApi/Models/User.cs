using System;
using System.ComponentModel.DataAnnotations;
public class User
{
	public Guid Id { get; set; } = Guid.NewGuid();

	[Required]
	[RegularExpression(@"^[a-zA-Z0-9]+$")]
	public string Login { get; set; }

	[Required]
	[RegularExpression(@"^[a-zA-Z0-9]+$")]
	public string Password { get; set; }

	[Required]
	[RegularExpression(@"^[a-zA-Z�-��-�]+$")]
	public string Name { get; set; }

	[Range(0, 2)]
	public int Gender { get; set; }

	public DateTime? Birthday { get; set; }
	public bool Admin { get; set; }
	public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
	public string CreatedBy { get; set; }
	public DateTime ModifiedOn { get; set; }
	public string ModifiedBy { get; set; }
	public DateTime? RevokedOn { get; set; }
	public string RevokedBy { get; set; }
}