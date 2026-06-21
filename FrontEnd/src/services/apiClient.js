import axios from 'axios';
import { store } from '../store';
import { logout } from '../store/slices/authSlice';
import { startApiCall, completeApiCall, addNotification } from '../store/slices/uiSlice';

const apiClient = axios.create({
  baseURL: '/',
  headers: {
    'Content-Type': 'application/json'
  }
});

// Request Interceptor to automatically add JWT Bearer tokens and trigger loading bar
apiClient.interceptors.request.use(
  (config) => {
    store.dispatch(startApiCall());
    
    const state = store.getState();
    const token = state.auth.token;
    
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    return config;
  },
  (error) => {
    store.dispatch(completeApiCall());
    return Promise.reject(error);
  }
);

// Response Interceptor to intercept global status codes
apiClient.interceptors.response.use(
  (response) => {
    store.dispatch(completeApiCall());
    return response;
  },
  (error) => {
    store.dispatch(completeApiCall());
    
    if (error.response) {
      const { status, data } = error.response;
      
      if (status === 401) {
        // Token expired or invalid -> log out user
        store.dispatch(logout());
        store.dispatch(addNotification({
          message: 'Session expired. Please log in again.',
          type: 'warning'
        }));
        window.location.href = '/login';
      } else if (status === 403) {
        // Forbidden action -> alert or notify
        store.dispatch(addNotification({
          message: 'Access Denied: You do not have permissions to perform this action.',
          type: 'danger'
        }));
      } else if (status === 500) {
        // Internal Server Error -> friendly toast alert
        const msg = data?.message || 'An unexpected server error occurred. Please try again later.';
        store.dispatch(addNotification({
          message: msg,
          type: 'danger'
        }));
      } else if (status === 400) {
        // Bad Request -> show error payload if validation failed
        const msg = data?.message || 'Invalid input data. Please verify fields.';
        store.dispatch(addNotification({
          message: msg,
          type: 'warning'
        }));
      }
    } else {
      store.dispatch(addNotification({
        message: 'Network Error: Cannot connect to the server. Please check your network.',
        type: 'danger'
      }));
    }
    
    return Promise.reject(error);
  }
);

export default apiClient;
