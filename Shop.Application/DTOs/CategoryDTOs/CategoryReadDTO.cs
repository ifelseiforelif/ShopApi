<<<<<<< HEAD
﻿using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
=======
﻿using System;
using System.Collections.Generic;
>>>>>>> dtos
using System.Text;

namespace Shop.Application.DTOs.CategoryDTOs;

public class CategoryReadDTO
{
    public int Id { get; set; }
<<<<<<< HEAD

    public string? Name { get; set; }

    
    public string? Slug { get; set; }


 
    public string? Url { get; set; }

    
    public bool IsActive { get; set; } 

   
    public int? ParentId { get; set; }

    public ICollection<int>? Products { get; set; }
=======
    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public int? ParentId { get; set; } = null;
>>>>>>> dtos
}
