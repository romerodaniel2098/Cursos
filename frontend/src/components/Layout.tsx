import React from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import { LogOut, BookOpen } from 'lucide-react';

export const Layout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const { user, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    return (
        <div className="layout">
            <nav className="navbar">
                <div className="nav-brand flex items-center gap-2 cursor-pointer" onClick={() => navigate('/')}>
                    <BookOpen /> Online Courses
                </div>
                <div className="flex items-center gap-4">
                    {user && (
                        <>
                            <span className="text-sm text-gray-600">{user.email}</span>
                            <button onClick={handleLogout} className="btn btn-outline">
                                <LogOut size={16} /> Logout
                            </button>
                        </>
                    )}
                </div>
            </nav>
            <main className="container fade-in">
                {children}
            </main>
        </div>
    );
};
