using PetStoreLibrary.DTOs;
using System.Collections.Generic;

namespace PetStoreUI.Models // Ensure the namespace is correct
{
    public class PetStoreViewModel
    {
        public IEnumerable<CategoryDTO> Categories { get; set; }

        public PetStoreViewModel()
        {
            Categories = new List<CategoryDTO>(); // Initialize with empty list
        }
    }
}