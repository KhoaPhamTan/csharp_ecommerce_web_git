import { useEffect, useState } from "react";
import axios from "axios";
import { Link } from "react-router-dom";
import DeletePopup from "../components/DeletePopup"; // Import the new DeletePopup component
import EditPopup from "../components/EditPopup"; // Import the new EditPopup component
import "../styles/ManagePets.css"; // Import the specific CSS file for ManagePets

const ManagePets = () => {
  const [pets, setPets] = useState([]);
  const [isPopupOpen, setIsPopupOpen] = useState(false);
  const [selectedPetId, setSelectedPetId] = useState(null);
  const [isEditPopupOpen, setIsEditPopupOpen] = useState(false);
  const [editPet, setEditPet] = useState(null);

  useEffect(() => {
    const fetchPets = async () => {
      const token = localStorage.getItem("token");
      try {
        const response = await axios.get("http://localhost:5134/petstores", {
          headers: { Authorization: `Bearer ${token}` },
        });
        setPets(response.data);
      } catch (error) {
        console.error("Error fetching pets:", error);
      }
    };

    fetchPets();
  }, []);

  const handleDeleteClick = (petId) => {
    setSelectedPetId(petId);
    setIsPopupOpen(true);
  };

  const handleConfirmDelete = async () => {
    const token = localStorage.getItem("token");
    try {
      await axios.delete(`http://localhost:5134/petstores/${selectedPetId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      // Update the list of pets after deletion
      setPets((prevPets) => prevPets.filter((pet) => pet.id !== selectedPetId));
    } catch (error) {
      console.error("Error deleting pet:", error);
    }
    setIsPopupOpen(false); // Close the popup after deletion
  };

  const handleCancelDelete = () => {
    setIsPopupOpen(false);
    setSelectedPetId(null);
  };

  const handleEditClick = (pet) => {
    setEditPet(pet);
    setIsEditPopupOpen(true);
  };

  const handleSaveEdit = async () => {
    const token = localStorage.getItem("token");
    try {
      await axios.put(`http://localhost:5134/petstores/${editPet.id}`, editPet, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setPets((prevPets) =>
        prevPets.map((pet) => (pet.id === editPet.id ? editPet : pet))
      );
    } catch (error) {
      console.error("Error updating pet:", error);
    }
    setIsEditPopupOpen(false);
    setEditPet(null);
  };

  const handleSaveAdd = async () => {
    const token = localStorage.getItem("token");
    if (!editPet.petName) {
      console.error("Pet name is required.");
      return;
    }
    if (!editPet.gender) {
      console.error("Gender is required.");
      return;
    }
    if (!editPet.petDescription) {
      console.error("Description is required.");
      return;
    }
    if (!editPet.price || editPet.price <= 0) {
      console.error("Price must be positive.");
      return;
    }
    if (!editPet.categoryName) {
      console.error("Category name is required.");
      return;
    }
    if (!editPet.birthDay) {
      console.error("Birth date is required.");
      return;
    }

    const formattedPet = {
      ...editPet,
      birthDay: new Date(editPet.birthDay).toISOString().split('T')[0], // Format birthDay as DateOnly string
    };

    try {
      const response = await axios.post("http://localhost:5134/petstores", formattedPet, {
        headers: { Authorization: `Bearer ${token}` },
      });
      const newPet = response.data;
      newPet.categoryName = editPet.categoryName; // Ensure categoryName is set correctly
      setPets((prevPets) => [...prevPets, newPet]);
    } catch (error) {
      console.error("Error adding pet:", error.response ? error.response.data : error.message);
    }
    setIsEditPopupOpen(false);
    setEditPet(null);
  };

  const handleCancelEdit = () => {
    setIsEditPopupOpen(false);
    setEditPet(null);
  };

  const handleAddClick = () => {
    setEditPet({}); // Initialize with an empty object for adding a new pet
    setIsEditPopupOpen(true);
  };

  return (
    <div>
      <div>
        <Link to="/" className="back-to-dashboard">
          ← Back to Dashboard
        </Link>
      </div>
      <h2>Manage Pets</h2>
      <button className="add-button" onClick={handleAddClick}>Add Pet</button>
      <table className="manage-pets-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Category</th>
            <th>Pet Name</th>
            <th>Gender</th>
            <th>Description</th>
            <th>Price</th>
            <th>Birth Date</th>
            <th>Image</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {pets.map((pet) => (
            <tr key={pet.id}>
              <td>{pet.id}</td>
              <td>{pet.categoryName ? pet.categoryName : "No Category"}</td>
              <td>{pet.petName}</td>
              <td>{pet.gender}</td>
              <td>{pet.petDescription}</td>
              <td>${pet.price.toLocaleString()}</td>
              <td>{new Date(pet.birthDay).toLocaleDateString()}</td>
              <td>
                <img
                  src={pet.imageUrl ? `http://localhost:5135${pet.imageUrl}` : "/placeholder-image.png"} // Ensure this matches the property name in your database
                  alt={pet.petName}
                  onError={(e) => {
                    console.error(`Image load error for ${pet.petName}: ${pet.imageUrl}`);
                    e.target.src = "/placeholder-image.jpg";
                  }}
                  onLoad={() => {
                    console.log(`Image loaded for ${pet.petName}: ${pet.imageUrl}`);
                  }}
                />
                {console.log(`Debug: pet.imageUrl = ${pet.imageUrl}`)} {/* Debug logging */}
              </td>
              <td>
                <button className="edit-button" onClick={() => handleEditClick(pet)}>Edit</button>
                <button className="delete-button" onClick={() => handleDeleteClick(pet.id)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      {isPopupOpen && (
        <DeletePopup
          message="Are you sure you want to delete this pet?"
          onConfirm={handleConfirmDelete}
          onCancel={handleCancelDelete}
        />
      )}
      {isEditPopupOpen && (
        <EditPopup
          pet={editPet}
          onSave={editPet.id ? handleSaveEdit : handleSaveAdd}
          onCancel={handleCancelEdit}
          onChange={(updatedPet) => setEditPet(updatedPet)}
        />
      )}
    </div>
  );
};

export default ManagePets;
