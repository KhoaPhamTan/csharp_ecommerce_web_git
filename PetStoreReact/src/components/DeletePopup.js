import React from "react";
import "../styles/DeletePopup.css"; // Import the CSS for the popup

const DeletePopup = ({ message, onConfirm, onCancel }) => {
  return (
    <div className="delete-popup">
      <div className="delete-popup-inner">
        <h2>{message}</h2>
        <div className="buttons">
          <button onClick={onConfirm}>Confirm</button>
          <button onClick={onCancel}>Cancel</button>
        </div>
      </div>
    </div>
  );
};

export default DeletePopup;