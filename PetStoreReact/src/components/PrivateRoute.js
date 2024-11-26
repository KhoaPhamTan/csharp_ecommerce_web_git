import { useContext } from "react";
import { Navigate } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";

const PrivateRoute = ({ element }) => {
  const { isAuthenticated } = useContext(AuthContext);

  if (isAuthenticated === null) {
    return <div>Loading...</div>; // or a loading spinner
  }

  return isAuthenticated ? element : <Navigate to="/login" />;
};

export default PrivateRoute;