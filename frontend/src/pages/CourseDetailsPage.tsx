import React, { useEffect, useState } from 'react';
import api from '../services/api';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Save, Trash2, Plus, CheckCircle, XCircle } from 'lucide-react';

interface Lesson {
    id: string;
    title: string;
    order: number;
}

interface Course {
    id: string;
    title: string;
    status: string;
}

export const CourseDetailsPage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();

    const [course, setCourse] = useState<Course | null>(null);
    const [lessons, setLessons] = useState<Lesson[]>([]);
    const [loading, setLoading] = useState(true);

    // Modal State
    const [showLessonModal, setShowLessonModal] = useState(false);
    const [currentLesson, setCurrentLesson] = useState<{ id?: string; title: string; order: number } | null>(null);

    const fetchData = async () => {
        try {
            const [courseRes, lessonsRes] = await Promise.all([
                api.get(`/courses/${id}`),
                api.get(`/lessons/course/${id}`)
            ]);
            setCourse(courseRes.data);
            setLessons(lessonsRes.data);
        } catch (error) {
            console.error(error);
            navigate('/');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (id) fetchData();
    }, [id]);

    const handlePublishToggle = async () => {
        if (!course) return;
        try {
            if (course.status === 'Published') {
                await api.patch(`/courses/${id}/unpublish`);
            } else {
                await api.patch(`/courses/${id}/publish`);
            }
            fetchData();
        } catch (error: any) {
            alert(error.response?.data?.message || 'Error toggling publish status');
        }
    };

    const handleDeleteLesson = async (lessonId: string) => {
        if (!confirm('Delete this lesson?')) return;
        try {
            await api.delete(`/lessons/${lessonId}`);
            fetchData();
        } catch (error) {
            alert('Error deleting lesson');
        }
    };

    const handleReorder = async (lessonId: string, direction: 'up' | 'down') => {
        try {
            await api.patch(`/lessons/${lessonId}/reorder?direction=${direction}`);
            fetchData();
        } catch (error) {
            console.error('Reorder failed', error);
        }
    };

    const handleSaveLesson = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!currentLesson) return;

        try {
            if (currentLesson.id) {
                await api.put(`/lessons/${currentLesson.id}`, { title: currentLesson.title, order: Number(currentLesson.order) });
            } else {
                await api.post('/lessons', { courseId: id, title: currentLesson.title, order: Number(currentLesson.order) });
            }
            setShowLessonModal(false);
            setCurrentLesson(null);
            fetchData();
        } catch (error: any) {
            alert(error.response?.data?.message || 'Error saving lesson');
        }
    };

    const openAddModal = () => {
        const nextOrder = lessons.length > 0 ? Math.max(...lessons.map(l => l.order)) + 1 : 1;
        setCurrentLesson({ title: '', order: nextOrder });
        setShowLessonModal(true);
    };

    const openEditModal = (lesson: Lesson) => {
        setCurrentLesson({ id: lesson.id, title: lesson.title, order: lesson.order });
        setShowLessonModal(true);
    };

    if (loading || !course) return <div className="p-10">Loading...</div>;

    return (
        <div className="max-w-4xl mx-auto">
            <button onClick={() => navigate('/')} className="btn btn-outline mb-6 gap-2">
                <ArrowLeft size={16} /> Back to Courses
            </button>

            <div className="bg-white rounded-xl p-6 shadow-sm mb-8">
                <div className="flex justify-between items-start">
                    <div>
                        <h1 className="text-3xl font-bold mb-2">{course.title}</h1>
                        <span className={`badge ${course.status === 'Published' ? 'badge-published' : 'badge-draft'}`}>
                            {course.status}
                        </span>
                    </div>
                    <div className="flex gap-2">
                        <button
                            onClick={handlePublishToggle}
                            className={`btn ${course.status === 'Published' ? 'btn-danger' : 'btn-success'}`}
                        >
                            {course.status === 'Published' ? <><XCircle size={18} /> Unpublish</> : <><CheckCircle size={18} /> Publish</>}
                        </button>
                    </div>
                </div>
            </div>

            <div className="flex justify-between items-center mb-4">
                <h2 className="text-xl font-bold">Lessons ({lessons.length})</h2>
                <button onClick={openAddModal} className="btn btn-primary">
                    <Plus size={18} /> Add Lesson
                </button>
            </div>

            <div className="space-y-4">
                {lessons.length === 0 ? (
                    <div className="text-gray-500 text-center py-8 bg-white rounded-xl">No lessons yet. Add one!</div>
                ) : (
                    lessons.map((lesson) => (
                        <div key={lesson.id} className="bg-white p-4 rounded-lg shadow-sm border border-gray-100 flex items-center justify-between hover:shadow-md transition-shadow">
                            <div className="flex items-center gap-4">
                                <div className="w-8 h-8 rounded-full bg-gray-100 flex items-center justify-center font-bold text-gray-600">
                                    {lesson.order}
                                </div>
                                <h3 className="font-medium text-lg">{lesson.title}</h3>
                            </div>
                            <div className="flex gap-2">
                                <button onClick={() => handleReorder(lesson.id, 'up')} className="p-2 text-gray-400 hover:text-blue-600" title="Move Up">↑</button>
                                <button onClick={() => handleReorder(lesson.id, 'down')} className="p-2 text-gray-400 hover:text-blue-600" title="Move Down">↓</button>
                                <button onClick={() => openEditModal(lesson)} className="text-gray-600 hover:text-blue-600 p-2">
                                    Edit
                                </button>
                                <button onClick={() => handleDeleteLesson(lesson.id)} className="text-gray-400 hover:text-red-500 p-2">
                                    <Trash2 size={18} />
                                </button>
                            </div>
                        </div>
                    ))
                )}
            </div>

            {showLessonModal && currentLesson && (
                <div className="fixed inset-0 bg-black/50 flex items-center justify-center fade-in z-50">
                    <div className="bg-white p-6 rounded-xl w-96 shadow-2xl">
                        <h2 className="text-xl font-bold mb-4">{currentLesson.id ? 'Edit Lesson' : 'Add Lesson'}</h2>
                        <form onSubmit={handleSaveLesson}>
                            <div className="mb-4">
                                <label className="block text-sm font-medium mb-1">Title</label>
                                <input
                                    type="text"
                                    className="input"
                                    value={currentLesson.title}
                                    onChange={(e) => setCurrentLesson({ ...currentLesson, title: e.target.value })}
                                    required
                                />
                            </div>
                            <div className="mb-4">
                                <label className="block text-sm font-medium mb-1">Order</label>
                                <input
                                    type="number"
                                    className="input"
                                    value={currentLesson.order}
                                    onChange={(e) => setCurrentLesson({ ...currentLesson, order: parseInt(e.target.value) })}
                                    required
                                />
                            </div>
                            <div className="flex justify-end gap-2">
                                <button type="button" className="btn" onClick={() => setShowLessonModal(false)}>Cancel</button>
                                <button type="submit" className="btn btn-primary gap-2"><Save size={18} /> Save</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
};
