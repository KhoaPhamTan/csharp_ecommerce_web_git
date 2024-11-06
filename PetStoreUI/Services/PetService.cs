using PetStoreLibrary.DTOs;
using PetStoreAPI.Data;
using System.Collections.Generic;
using System.Linq;

namespace PetStoreUI.Services
{
    public class PetService : IPetService
    {
        private readonly PetStoreDbContext _context;

        public PetService(PetStoreDbContext context)
        {
            _context = context;
        }

        public List<PetTypeDTO> GetAllPetTypes()
        {
            return _context.PetTypes.Select(pt => new PetTypeDTO(
                pt.Id,
                pt.Name
            )).ToList();
        }
    }
}