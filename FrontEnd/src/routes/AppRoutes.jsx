import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import ProtectedRoute from '../components/ProtectedRoute';
import AuthLayout from '../layouts/AuthLayout';
import MainLayout from '../layouts/MainLayout';
import LoginScreen from '../features/auth/LoginScreen';
import DashboardResolver from '../features/dashboard/DashboardResolver';
import AcademicYearManager from '../features/academics/AcademicYearManager';
import ClassManager from '../features/academics/ClassManager';
import SectionManager from '../features/academics/SectionManager';
import StaffDirectory from '../features/staff/StaffDirectory';

// Placeholder views for modules implemented in later phases
const StudentRosterPlaceholder = () => (
  <div className="ep-card text-center py-5">
    <h3>Student Directory</h3>
    <p className="text-muted">Student profiles will load here. Interface is configured.</p>
  </div>
);

const GuardiansPlaceholder = () => (
  <div className="ep-card text-center py-5">
    <h3>Guardian Directory</h3>
    <p className="text-muted">Guardian relationships will load here. Interface is configured.</p>
  </div>
);

const AttendancePlaceholder = () => (
  <div className="ep-card text-center py-5">
    <h3>Daily Attendance Sheet</h3>
    <p className="text-muted">Interactive attendance grid will load here. Interface is configured.</p>
  </div>
);

const ConfigPlaceholder = ({ moduleName }) => (
  <div className="ep-card text-center py-5">
    <h3>{moduleName} Configuration</h3>
    <p className="text-muted">Configuration settings parameters will load here. Interface is configured.</p>
  </div>
);

const AppRoutes = () => {
  return (
    <Routes>
      {/* Public Routes */}
      <Route element={<AuthLayout />}>
        <Route path="/login" element={<LoginScreen />} />
      </Route>

      {/* Protected Routes (Role Mapped) */}
      <Route element={
        <ProtectedRoute>
          <MainLayout />
        </ProtectedRoute>
      }>
        {/* Default route -> resolver redirects appropriately */}
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/dashboard" element={<DashboardResolver />} />

        {/* Students list */}
        <Route path="/students" element={<StudentRosterPlaceholder />} />
        
        {/* Admin only views */}
        <Route path="/guardians" element={
          <ProtectedRoute allowedRoles={['SchoolAdmin']}>
            <GuardiansPlaceholder />
          </ProtectedRoute>
        } />
        
        <Route path="/staff" element={
          <ProtectedRoute allowedRoles={['SchoolAdmin']}>
            <StaffDirectory />
          </ProtectedRoute>
        } />

        {/* Operations */}
        <Route path="/attendance/mark" element={<AttendancePlaceholder />} />
        <Route path="/attendance/analytics" element={<ConfigPlaceholder moduleName="Attendance Analytics" />} />

        {/* Configurations (Admin only) */}
        <Route path="/settings/academic-years" element={
          <ProtectedRoute allowedRoles={['SchoolAdmin']}>
            <AcademicYearManager />
          </ProtectedRoute>
        } />
        
        <Route path="/settings/classes" element={
          <ProtectedRoute allowedRoles={['SchoolAdmin']}>
            <ClassManager />
          </ProtectedRoute>
        } />
        
        <Route path="/settings/sections" element={
          <ProtectedRoute allowedRoles={['SchoolAdmin']}>
            <SectionManager />
          </ProtectedRoute>
        } />
      </Route>

      {/* Catch-all redirection */}
      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
};

export default AppRoutes;

