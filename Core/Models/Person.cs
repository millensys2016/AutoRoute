using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Persons")]
    public class Person
    {
        // Primary key: auto-incrementing integer starting from 1
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Name: nvarchar(256), not null
        [Required]
        [MaxLength(256)]
        [Column(TypeName = "nvarchar(256)")]
        public required string Name { get; set; }

        // Email: varchar(256), unique, not null
        [Required]
        [MaxLength(256)]
        [Column(TypeName = "varchar(256)")]
        public required string Email { get; set; }

        // Date of birth: date, not null
        [Required]
        [Column(TypeName = "date")]
        public required DateOnly DateOfBirth { get; set; }

        // Mobile number: varchar(15), unique, not null
        [Required]
        [MaxLength(15)]
        [Column(TypeName = "varchar(15)")]
        public required string MobileNumber { get; set; }

        // Role: enum [patient, doctor, admin], not null
        [Required]
        public PersonRole Role { get; set; }

        // CreatedAt: datetime with default value now (CLR default set here; DB default should be set in migration/EF config)
        [Required]
        [Column(TypeName = "datetime")]
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // UpdatedAt: datetime, nullable
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }
    }

    public enum PersonRole
    {
        Patient = 0,
        Doctor = 1,
        Admin = 2
    }
}
