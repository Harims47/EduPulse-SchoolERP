import { createSlice } from '@reduxjs/toolkit';

const parseJwt = (token) => {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      window
        .atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch (e) {
    return null;
  }
};

const initialToken = localStorage.getItem('ep_token');
let initialUser = null;
let initialIsAuthenticated = false;

if (initialToken) {
  const decoded = parseJwt(initialToken);
  if (decoded) {
    // Check if token is expired
    const curTime = Date.now() / 1000;
    if (decoded.exp && decoded.exp > curTime) {
      initialUser = {
        userId: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || decoded.sub || decoded.nameid,
        email: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] || decoded.email,
        role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || decoded.role,
        firstName: decoded.given_name || '',
        lastName: decoded.family_name || ''
      };
      initialIsAuthenticated = true;
    } else {
      localStorage.removeItem('ep_token');
    }
  }
}

const authSlice = createSlice({
  name: 'auth',
  initialState: {
    token: initialToken,
    user: initialUser,
    isAuthenticated: initialIsAuthenticated,
    loading: false,
    error: null
  },
  reducers: {
    loginStart: (state) => {
      state.loading = true;
      state.error = null;
    },
    loginSuccess: (state, action) => {
      const token = action.payload;
      const decoded = parseJwt(token);
      
      state.token = token;
      state.isAuthenticated = true;
      state.loading = false;
      state.error = null;
      
      if (decoded) {
        state.user = {
          userId: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || decoded.sub || decoded.nameid,
          email: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] || decoded.email,
          role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || decoded.role,
          firstName: decoded.given_name || '',
          lastName: decoded.family_name || ''
        };
      }
      localStorage.setItem('ep_token', token);
    },
    loginFailure: (state, action) => {
      state.loading = false;
      state.error = action.payload;
      state.isAuthenticated = false;
      state.token = null;
      state.user = null;
      localStorage.removeItem('ep_token');
    },
    logout: (state) => {
      state.token = null;
      state.user = null;
      state.isAuthenticated = false;
      state.loading = false;
      state.error = null;
      localStorage.removeItem('ep_token');
    }
  }
});

export const { loginStart, loginSuccess, loginFailure, logout } = authSlice.actions;
export default authSlice.reducer;
