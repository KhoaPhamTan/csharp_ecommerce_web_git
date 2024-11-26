import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Dashboard from "./pages/Dashboard";
import ManagePets from "./pages/ManagePets";
import ManageUsers from "./pages/ManageUsers";
import Login from "./pages/Login";
import { AuthProvider } from "./context/AuthContext"; // Import AuthProvider
import PrivateRoute from "./components/PrivateRoute"; // Import PrivateRoute

function App() {
  return (
    <AuthProvider> {/* Wrap Router with AuthProvider */}
      <Router>
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/manage-pets" element={<PrivateRoute element={<ManagePets />} />} />
          <Route path="/manage-users" element={<PrivateRoute element={<ManageUsers />} />} />
          <Route path="/login" element={<Login />} />
          {/* ...other routes... */}
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
