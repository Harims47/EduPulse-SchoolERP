import { configureStore } from '@reduxjs/toolkit';
import authReducer from './slices/authSlice';
import uiReducer from './slices/uiSlice';
import academicContextReducer from './slices/academicContextSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    ui: uiReducer,
    academicContext: academicContextReducer
  }
});
