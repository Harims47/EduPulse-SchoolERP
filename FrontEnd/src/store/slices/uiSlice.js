import { createSlice } from '@reduxjs/toolkit';

const uiSlice = createSlice({
  name: 'ui',
  initialState: {
    sidebarCollapsed: false,
    mobileSidebarOpen: false,
    apiLoadingCount: 0,
    notifications: [],
    activeModalId: null
  },
  reducers: {
    toggleSidebar: (state) => {
      state.sidebarCollapsed = !state.sidebarCollapsed;
      state.mobileSidebarOpen = false;
    },
    setSidebarCollapsed: (state, action) => {
      state.sidebarCollapsed = action.payload;
    },
    toggleMobileSidebar: (state) => {
      state.mobileSidebarOpen = !state.mobileSidebarOpen;
    },
    setMobileSidebarOpen: (state, action) => {
      state.mobileSidebarOpen = action.payload;
    },
    startApiCall: (state) => {
      state.apiLoadingCount++;
    },
    completeApiCall: (state) => {
      state.apiLoadingCount = Math.max(0, state.apiLoadingCount - 1);
    },
    addNotification: (state, action) => {
      state.notifications.push({
        id: Date.now().toString() + Math.random().toString(),
        message: action.payload.message,
        type: action.payload.type || 'info' // success, warning, danger, info
      });
    },
    removeNotification: (state, action) => {
      state.notifications = state.notifications.filter(n => n.id !== action.payload);
    },
    setActiveModalId: (state, action) => {
      state.activeModalId = action.payload;
    }
  }
});

export const { 
  toggleSidebar, 
  setSidebarCollapsed, 
  toggleMobileSidebar, 
  setMobileSidebarOpen, 
  startApiCall,
  completeApiCall,
  addNotification, 
  removeNotification, 
  setActiveModalId 
} = uiSlice.actions;
export default uiSlice.reducer;
