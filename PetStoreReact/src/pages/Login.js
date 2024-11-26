import { useContext, useState } from "react";
import axios from "axios";
import { AuthContext } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";
import "../styles/login.css"; // Import the new CSS file

const Login = () => {
  const { login } = useContext(AuthContext);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    console.log(`Login attempt for email: ${email}`);
    try {
      const response = await axios.post("http://localhost:5134/Users/login", {
        email,
        password,
      });
      console.log("Response data:", response.data); // Add this line to log the response data
      const { token, role } = response.data;

      if (role !== "Admin") {
        setError("Access denied. Admins only.");
        return;
      }

      login(token); 
      navigate("/"); 
    } catch (err) {
      if (err.response) {
        console.error("Login failed:", err.response.data, "Status code:", err.response.status);
        console.error("Request payload:", { email, password });
        console.error("Request headers:", err.response.config.headers);
      } else {
        console.error("Login failed:", err.message);
      }
      setError("Invalid email or password");
    }
  };

  return (
    <div className="login-container">
      <div className="login-card">
        <h2>Admin Login</h2>
        <form onSubmit={handleSubmit} className="login-form">
          <div className="form-group">
            <label className="form-label">Email:</label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="form-control"
            />
          </div>
          <div className="form-group">
            <label className="form-label">Password:</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="form-control"
            />
          </div>
          {error && <p style={{ color: "red" }}>{error}</p>}
          <button type="submit" className="btn">Login</button>
        </form>
      </div>
    </div>
  );
};

export default Login;
