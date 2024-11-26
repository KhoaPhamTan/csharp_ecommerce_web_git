import React from "react";
import axios from "axios"; // Import axios for making HTTP requests
import "../styles/EditPopup.css"; // Import the CSS for the popup

const EditPopup = ({ pet, onSave, onCancel, onChange }) => {
  const handleImageChange = async (e) => {
    const file = e.target.files[0];
    if (file) {
      const formData = new FormData();
      formData.append("file", file);
      try {
        const response = await axios.post("http://localhost:5134/upload", formData, {
          headers: { "Content-Type": "multipart/form-data" },
        });
        onChange({ ...pet, imageUrl: response.data.imageUrl });
      } catch (error) {
        console.error("Error uploading image:", error);
      }
    }
  };

  return (
    <div className="popup">
      <div className="popup-inner">
        <h2>Edit Pet Details</h2>
        <label>
          Category:
          <input
            type="text"
            value={pet.categoryName}
            readOnly
          />
        </label>
        <label>
          Pet Name:
          <input
            type="text"
            value={pet.petName}
            onChange={(e) => onChange({ ...pet, petName: e.target.value })}
          />
        </label>
        <label>
          Gender:
          <input
            type="text"
            value={pet.gender}
            onChange={(e) => onChange({ ...pet, gender: e.target.value })}
          />
        </label>
        <label>
          Description:
          <input
            type="text"
            value={pet.petDescription}
            onChange={(e) => onChange({ ...pet, petDescription: e.target.value })}
          />
        </label>
        <label>
          Price:
          <input
            type="number"
            value={pet.price}
            onChange={(e) => onChange({ ...pet, price: parseFloat(e.target.value) })}
          />
        </label>
        <label>
          Birth Date:
          <input
            type="date"
            value={new Date(pet.birthDay).toISOString().split("T")[0]}
            onChange={(e) => onChange({ ...pet, birthDay: e.target.value })}
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