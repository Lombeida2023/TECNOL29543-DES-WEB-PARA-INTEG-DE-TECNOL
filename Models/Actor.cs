using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SakilaApp.Models
{
    [Table("actor")]
    public class Actor
    {
        [Key]
        [Column("actor_id")]
        public int ActorId { get; set; }

        [Required]
        [Column("first_name")]
        [StringLength(45)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Column("last_name")]
        [StringLength(45)]
        public string LastName { get; set; } = string.Empty;

        [Column("last_update")]
        public DateTime LastUpdate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        public virtual ICollection<FilmActor> FilmActors { get; set; } = new List<FilmActor>();
    }
}