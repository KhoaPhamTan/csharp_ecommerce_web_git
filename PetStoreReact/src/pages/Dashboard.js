import { useContext } from "react";
import { AuthContext } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";
import { Link } from "react-router-dom";
import "../App.css"; // Add this import

const Dashboard = () => {
  const navigate = useNavigate();
  const { isAuthenticated, user, logout } = useContext(AuthContext);

  const handleLoginClick = () => {
    navigate("/login");
  };

  const handleLogoutClick = () => {
    logout();
    navigate("/login");
  };

  return (
    <div>
      <h1>Admin Dashboard</h1>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <p>Welcome to the store!</p>
        {!isAuthenticated ? (
          <button onClick={handleLoginClick} className="button">Login</button>
        ) : (
          <button onClick={handleLogoutClick} className="button">Logout</button>
        )}
      </div>
      <nav>
        <ul>
          <li><Link to="/manage-pets">Manage Pets</Link></li>
          <li><Link to="/manage-users">Manage Users</Link></li>
        </ul>
      </nav>
    </div>
  );
};

export default Dashboard;