using PetStoreLibrary.DTOs;
using System.Collections.Generic;

namespace PetStoreUI.Services
{
    public interface IPetService
    {
        List<PetTypeDTO> GetAllPetTypes();
    }
}