﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkHub.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Display(Name = "Nome")]
        public required string Name { get; set; }

        public int PageId { get; set; }

        [Display(Name = "Página")]
        [ForeignKey("PageId")]
        public Page? Page { get; set; }
    }
}