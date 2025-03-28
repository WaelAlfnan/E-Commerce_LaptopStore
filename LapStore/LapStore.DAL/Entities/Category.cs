﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        //public string ImageUrl { get; set; } // Optional
        //public int Order { get; set; }      // Optional

        [ForeignKey("parentCategory")]
        public int ParentCategoryId { get; set; }

        // Navigation Properties
        public virtual Category? parentCategory { get; set; }
        public virtual ICollection<Category>? childCategories { get; set; }
        public virtual ICollection<Product>? products { get; set; }


    }
}
