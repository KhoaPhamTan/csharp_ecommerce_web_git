import React, { useState } from "react";
import axios from "axios"; // Import axios for making HTTP requests
import "../styles/EditPopup.css"; // Import the CSS for the popup

const EditPopup = ({ pet: initialPet, onSave, onCancel, onChange }) => {
  const [pet, setPet] = useState(initialPet);

  const handleImageChange = async (e) => {
    const file = e.target.files[0];
    if (file) {
      const formData = new FormData();
      formData.append("file", file);
      formData.append("petId", pet.id || 0); // Add petId to the form data, use 0 for new pets
      try {
        const response = await axios.post("http://localhost:5134/petStores/upload", formData, {
          headers: { "Content-Type": "multipart/form-data" },
        });
        const imageUrl = response.data.imageUrl; // Use the full image URL as returned by the server
        onChange({ ...pet, imageUrl: imageUrl });
      } catch (error) {
        console.error("Error uploading image:", error);
      }
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    const updatedPet = {
      ...pet,
      [name]: name === "birthDay" ? new Date(value).toISOString() : value,
    };
    setPet(updatedPet);
    onChange(updatedPet); // Ensure the parent component's state is updated
  };

  return (
    <div className="popup">
      <div className="popup-inner">
        <h2>{pet.id ? "Edit Pet Details" : "Add Pet Details"}</h2>
        <label>
          Category:
          <input
            type="text"
            name="categoryName"
            value={pet.categoryName}
            onChange={handleInputChange}
            disabled={!!pet.id} // Disable input if pet has an ID (editing)
            style={{ backgroundColor: pet.id ? "#d3d3d3" : "white" }} // Gray out when editing
          />
        </label>
        <label>
          Pet Name:
          <input
            type="text"
            name="petName"
            value={pet.petName}
            onChange={handleInputChange}
          />
        </label>
        <label>
          Gender:
          <input
            type="text"
            name="gender"
            value={pet.gender}
            onChange={handleInputChange}
          />
        </label>
        <label>
          Description:
          <input
            type="text"
            name="petDescription"
            value={pet.petDescription}
            onChange={handleInputChange}
          />
        </label>
        <label>
          Price:
          <input
            type="number"
            name="price"
            value={pet.price}
            onChange={handleInputChange}
          />
        </label>
        <label>
          Birth Date:
          <input
            type="date"
            name="birthDay"
            value={pet.birthDay ? new Date(pet.birthDay).toISOString().split('T')[0] : ''} // Ensure valid date format
            onChange={handleInputChange}
          />
        </label>
        <label>
          Image:
          <input
            type="file"
            accept="image/*"
            onChange={handleImageChange}
          />
        </label>
        <button onClick={onSave}>Save</button>
        <button onClick={onCancel}>Cancel</button>
      </div>
    </div>
  );
};

export default EditPopup;