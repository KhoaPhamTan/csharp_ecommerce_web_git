using WebStore.DTOs;

namespace WebStore.EndPoints;
public static class StoresEndPoints
{
    const string GetStoreEndpointName = "GetStore";

    private static readonly List<PetStoreDTO> petStores = [
        new (
        1,
        "F-001",
        "F-M-001",
        "Fish",
        "Male",
        "Large",
        8.01m,
        new DateOnly(2023,09,09)),
    new (
        2,
        "D-001",
        "D-M-001",
        "Dog",
        "Male",
        "Tall",
        18.91m,
        new DateOnly(2024,1,1)),
    new (
        3,
        "C-001",
        "C-FM-001",
        "Fish",
        "Female",
        "Small",
        38.77m,
        new DateOnly(2023,12,12)),

];

    public static WebApplication MapStoresEndPoints(this WebApplication app)
    {
        // Get /stores
        app.MapGet("petStores", () => petStores);

        // Get /stores/1
        app.MapGet("petStores/{id}", (int id) =>
        {
            PetStoreDTO? petStore = petStores.Find(petStores => petStores.id == id);
            return petStore is null ? Results.NotFound() : Results.Ok(petStore);
        })
        .WithName(GetStoreEndpointName);


        // Post /stores/1
        app.MapPost("petStores", (CreatePetDTO newPetStore) =>
        {
            PetStoreDTO store = new(
                petStores.Count + 1,
                newPetStore.itemId,
                newPetStore.productId,
                newPetStore.petType,
                newPetStore.gender,
                newPetStore.petDescription,
                newPetStore.price,
                newPetStore.birthDay
            );
            petStores.Add(store);
            return Results.CreatedAtRoute(GetStoreEndpointName, new { id = store.id }, store);
        });




        // PUT
        app.MapPut("/petStores/{id}", (int id, UpdatedPetStoreDTO updatedPetStoreDTO) =>
        {
            var index = petStores.FindIndex(store => store.id == id); // Correctly compare the id property

            if (index == -1)
            {
                return Results.NotFound();
            }

            // Update the store
            petStores[index] = new PetStoreDTO(
                id,
                updatedPetStoreDTO.itemId,
                updatedPetStoreDTO.productId,
                updatedPetStoreDTO.petType,
                updatedPetStoreDTO.gender,
                updatedPetStoreDTO.petDescription,
                updatedPetStoreDTO.price,
                updatedPetStoreDTO.birthDay
            );

            return Results.NoContent();
        });

        // DELETE /

        app.MapDelete("/petStores/{id}", (int id) =>
        {
            petStores.RemoveAll(store => store.id == id);
            return Results.NoContent();
        });
        return app;
    }
}