import React from 'react';
import { Outlet, Navigate } from 'react-router-dom';
import { useSelector } from 'react-redux';

const AuthLayout = () => {
  const { isAuthenticated } = useSelector((state) => state.auth);

  if (isAuthenticated) {
    // Redirect authenticated users trying to hit login back to dashboard
    return <Navigate to="/dashboard" replace />;
  }

  return (
    <div className="d-flex align-items-center justify-content-center min-vh-100 py-5" style={{ backgroundColor: 'var(--color-bg-main)' }}>
      <div className="container" style={{ maxWidth: '480px' }}>
        <Outlet />
      </div>
    </div>
  );
};

export default AuthLayout;
