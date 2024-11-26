import { Link } from "react-router-dom";
import { useEffect, useState } from "react";
import axios from "axios";
import ConfirmPopup from "../components/ConfirmPopup";
import EditUserPopup from "../components/EditUserPopup";
import "../styles.css";

const ManageUsers = () => {
  const [users, setUsers] = useState([]);
  const [isPopupOpen, setIsPopupOpen] = useState(false);
  const [selectedUserId, setSelectedUserId] = useState(null);
  const [isEditPopupOpen, setIsEditPopupOpen] = useState(false);
  const [selectedUser, setSelectedUser] = useState(null);

  useEffect(() => {
    const fetchUsers = async () => {
      const token = localStorage.getItem("token");
      try {
        const response = await axios.get("http://localhost:5134/users", {
          headers: { Authorization: `Bearer ${token}` },
        });
        setUsers(response.data);
      } catch (error) {
        console.error("Error fetching users:", error);
      }
    };

    fetchUsers();
  }, []);

  const handleDeleteClick = (userId) => {
    setSelectedUserId(userId);
    setIsPopupOpen(true);
  };

  const handleConfirmDelete = async () => {
    const token = localStorage.getItem("token");
    try {
      await axios.delete(`http://localhost:5134/users/${selectedUserId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      // Update the list of users after deletion
      setUsers((prevUsers) => prevUsers.filter((user) => user.id !== selectedUserId));
    } catch (error) {
      console.error("Error deleting user:", error);
    }
    setIsPopupOpen(false); // Close the popup after deletion
  };

  const handleCancelDelete = () => {
    setIsPopupOpen(false);
  };

  const handleEditClick = (user) => {
    setSelectedUser(user);
    setIsEditPopupOpen(true);
  };

  const handleConfirmEdit = async (updatedUser) => {
    const token = localStorage.getItem("token");
    try {
      await axios.put(`http://localhost:5134/users/${updatedUser.id}`, updatedUser, {
        headers: { Authorization: `Bearer ${token}` },
      });
      // Update the list of users after editing
      setUsers((prevUsers) =>
        prevUsers.map((user) => (user.id === updatedUser.id ? updatedUser : user))
      );
    } catch (error) {
      console.error("Error editing user:", error);
    }
    setIsEditPopupOpen(false); // Close the popup after editing
  };

  const handleCancelEdit = () => {
    setIsEditPopupOpen(false);
  };

  const handleRefreshClick = async () => {
    const token = localStorage.getItem("token");
    try {
      const response = await axios.get("http://localhost:5134/users", {
        headers: { Authorization: `Bearer ${token}` },
      });
      setUsers(response.data);
    } catch (error) {
      console.error("Error refreshing users:", error);
    }
  };

  return (
    <div>
      <div>
        <Link to="/" className="back-to-dashboard">
          ← Back to Dashboard
        </Link>
      </div>
      <h2>Manage Users</h2>
      <button onClick={handleRefreshClick}>Refresh Users</button>
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Email</th>
            <th>Full Name</th>
            <th>Role</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr key={user.id}>
              <td>{user.id}</td>
              <td>{user.email}</td>
              <td>{user.fullName}</td>
              <td>{user.role}</td>
              <td>
                {user.role !== "Admin" && ( // Only show the buttons if the role is not Admin
                  <>
                    <button onClick={() => handleEditClick(user)}>Edit</button>
                    <button onClick={() => handleDeleteClick(user.id)}>Delete</button>
                  </>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {isPopupOpen && (
        <ConfirmPopup
          message="Are you sure you want to delete this user?"
          onConfirm={handleConfirmDelete}
          onCancel={handleCancelDelete}
        />
      )}

      {isEditPopupOpen && (
        <EditUserPopup
          user={selectedUser}
          onConfirm={handleConfirmEdit}
          onCancel={handleCancelEdit}
        />
      )}
    </div>
  );
};

export default ManageUsers;
