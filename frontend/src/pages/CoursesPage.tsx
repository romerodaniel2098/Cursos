import React, { useEffect, useState } from 'react';
import api from '../services/api';
import { useNavigate } from 'react-router-dom';
import { Plus, Book, Trash2 } from 'lucide-react';

interface Course {
    id: string;
    title: string;
    status: string;
    lessonCount: number;
}

export const CoursesPage: React.FC = () => {
    const [courses, setCourses] = useState<Course[]>([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [newTitle, setNewTitle] = useState('');
    const navigate = useNavigate();

    const fetchCourses = async () => {
        try {
            const response = await api.get('/courses/search?pageSize=100'); // Simple pagination for now
            setCourses(response.data.items);
        } catch (error) {
            console.error(error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchCourses();
    }, []);

    const handleCreate = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            const response = await api.post('/courses', { title: newTitle });
            navigate(`/courses/${response.data.id}`);
        } catch (error) {
            alert('Error creating course');
        }
    };

    const handleDelete = async (e: React.MouseEvent, id: string) => {
        e.stopPropagation();
        if (!confirm('Are you sure you want to delete this course?')) return;
        try {
            await api.delete(`/courses/${id}`);
            fetchCourses();
        } catch (error) {
            alert('Error deleting course');
        }
    };

    if (loading) return <div className="text-center mt-10">Loading...</div>;

    return (
        <div>
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-3xl font-bold title-gradient">My Courses</h1>
                <button className="btn btn-primary" onClick={() => setShowModal(true)}>
                    <Plus size={20} /> Create Course
                </button>
            </div>

            {courses.length === 0 ? (
                <div className="text-center text-gray-500 mt-10">No courses found. Create one to get started!</div>
            ) : (
                <div className="grid">
                    {courses.map((course) => (
                        <div
                            key={course.id}
                            className="card cursor-pointer"
                            onClick={() => navigate(`/courses/${course.id}`)}
                        >
                            <div className="flex justify-between items-start mb-4">
                                <div className="p-3 bg-indigo-100 rounded-lg text-indigo-600">
                                    <Book size={24} />
                                </div>
                                <span className={`badge ${course.status === 'Published' ? 'badge-published' : 'badge-draft'}`}>
                                    {course.status}
                                </span>
                            </div>
                            <h3 className="text-xl font-semibold mb-2">{course.title}</h3>
                            <p className="text-gray-500 text-sm mb-4">{course.lessonCount} Lessons</p>

                            <div className="flex justify-end gap-2 mt-4 pt-4 border-t border-gray-100">
                                <button className="btn btn-outline p-2" onClick={(e) => handleDelete(e, course.id)}>
                                    <Trash2 size={16} />
                                </button>
                            </div>
                        </div>
                    ))}
                </div>
            )}

            {showModal && (
                <div className="fixed inset-0 bg-black/50 flex items-center justify-center fade-in">
                    <div className="bg-white p-6 rounded-xl w-96 shadow-2xl relative">
                        <h2 className="text-xl font-bold mb-4">Create New Course</h2>
                        <form onSubmit={handleCreate}>
                            <input
                                type="text"
                                className="input"
                                placeholder="Course Title"
                                value={newTitle}
                                onChange={(e) => setNewTitle(e.target.value)}
                                required
                            />
                            <div className="flex justify-end gap-2">
                                <button type="button" className="btn" onClick={() => setShowModal(false)}>Cancel</button>
                                <button type="submit" className="btn btn-primary">Create</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
};
