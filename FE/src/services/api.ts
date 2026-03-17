import axios from 'axios';

const api = axios.create({
    baseURL: 'https://smart-investigation-system.onrender.com/api/v1',
    headers: {
        'Content-Type': 'application/json',
    },
});

// Request interceptor for Auth
api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export default api;
