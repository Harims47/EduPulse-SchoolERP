import React from 'react';
import { useSelector } from 'react-redux';

/**
 * PermissionGate conditionally renders children if the authenticated user's role is in the allowedRoles list.
 */
const PermissionGate = ({ children, allowedRoles = [], fallback = null }) => {
  const { user, isAuthenticated } = useSelector((state) => state.auth);

  if (!isAuthenticated || !user) {
    return fallback;
  }

  const hasPermission = allowedRoles.includes(user.role);

  if (!hasPermission) {
    return fallback;
  }

  return children;
};

export default PermissionGate;
