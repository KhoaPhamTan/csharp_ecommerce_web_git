import { useState } from "react";
import "../styles/EditPopup.css"; // Import the CSS for the popup

const EditUserPopup = ({ user, onConfirm, onCancel }) => {
  const [username, setUsername] = useState(user.username || "");
  const [password, setPassword] = useState("");
  const [address, setAddress] = useState(user.address || "");
  const [fullName, setFullName] = useState(user.fullName || "");
  const [email, setEmail] = useState(user.email || "");

  const handleConfirm = () => {
    const updatedUser = { ...user, username, password, fullName, email, address };
    onConfirm(updatedUser);
  };

  return (
    <div className="popup">
      <div className="popup-content">
        <h3>{user.id ? "Edit User" : "Add User"}</h3>
        <label>
          Username:
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
          />
        </label>
        <label>
          Password:
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </label>
        <label>
          Full Name:
          <input
            type="text"
            value={fullName}
            onChange={(e) => setFullName(e.target.value)}
          />
        </label>
        <label>
          Email:
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </label>
        <label>
          Address:
          <input
            type="text"
            value={address}
            onChange={(e) => setAddress(e.target.value)}
          />
        </label>
        <button onClick={handleConfirm}>Save</button>
        <button onClick={onCancel}>Cancel</button>
      </div>
    </div>
  );
};

export default EditUserPopup;