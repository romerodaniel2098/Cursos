import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import { Layout } from './components/Layout';
import { LoginPage } from './pages/LoginPage';
import { CoursesPage } from './pages/CoursesPage';
import { CourseDetailsPage } from './pages/CourseDetailsPage';

const PrivateRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated } = useAuth();
  // Simple check. In real app, might wait for initial load.
  // But our AuthProvider loads sync from localStorage in useEffect, 
  // actually useEffect is async. Initial render user is null.
  // We need a clearer loading state in AuthProvider or allow null initially?
  // Let's check AuthContext again.
  // It uses `useEffect` to set user. So initial render is unauthenticated.
  // We should probably allow a brief loading or check token in localStorage directly for initial state.

  // For this assessment, if we have a token in localStorage, we assume authenticated until proven otherwise.
  const token = localStorage.getItem('token');
  if (!token && !isAuthenticated) {
    return <Navigate to="/login" />;
  }

  return <Layout>{children}</Layout>;
};

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/" element={
            <PrivateRoute>
              <CoursesPage />
            </PrivateRoute>
          } />
          <Route path="/courses/:id" element={
            <PrivateRoute>
              <CourseDetailsPage />
            </PrivateRoute>
          } />
          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
