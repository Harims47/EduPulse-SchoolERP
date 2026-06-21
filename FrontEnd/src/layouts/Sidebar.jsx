import React from 'react';
import { NavLink } from 'react-router-dom';
import { useSelector } from 'react-redux';

const Sidebar = () => {
  const { user } = useSelector((state) => state.auth);
  const isAdmin = user?.role === 'SchoolAdmin';

  const linkStyle = ({ isActive }) => ({
    display: 'flex',
    alignItems: 'center',
    padding: 'var(--space-sm) var(--space-md)',
    color: isActive ? 'var(--color-primary)' : 'var(--color-text-secondary)',
    textDecoration: 'none',
    fontWeight: isActive ? 'var(--font-weight-semibold)' : 'var(--font-weight-medium)',
    fontSize: 'var(--font-size-sm)',
    borderRadius: 'var(--radius-sm)',
    backgroundColor: isActive ? 'var(--color-primary-light)' : 'transparent',
    transition: 'all 0.2s cubic-bezier(0.16, 1, 0.3, 1)',
    marginBottom: 'var(--space-2xs)'
  });

  return (
    <aside className="ep-sidebar" style={{
      backgroundColor: '#ffffff',
      color: 'var(--color-text-primary)',
      display: 'flex',
      flexDirection: 'column',
      height: '100vh',
      position: 'sticky',
      top: 0,
      borderRight: '1px solid rgba(226, 232, 240, 0.8)',
      boxShadow: 'var(--shadow-sm)'
    }}>
      {/* Branding Logo Area */}
      <div className="d-flex align-items-center px-3" style={{ height: '70px', borderBottom: '1px solid rgba(226, 232, 240, 0.6)' }}>
        <div className="d-flex align-items-center gap-2 overflow-hidden" style={{ width: '100%' }}>
          <div className="bg-primary text-white rounded-3 d-flex align-items-center justify-content-center flex-shrink-0" style={{ width: '36px', height: '36px', backgroundColor: 'var(--color-primary)' }}>
            <span className="fw-bold">EP</span>
          </div>
          <div className="sidebar-brand-text d-flex flex-column" style={{ minWidth: 0 }}>
            <span className="fw-bold tracking-tight text-dark text-truncate" style={{ fontSize: '14px', lineHeight: '1.2' }}>EduPulse Acad.</span>
            <span className="text-muted xsmall font-monospace" style={{ fontSize: '10px' }}>EPA-2026</span>
          </div>
        </div>
      </div>

      {/* Navigation List */}
      <nav className="flex-grow-1 px-3 py-4 overflow-y-auto">
        <span className="sidebar-section-header text-uppercase fw-bold text-muted d-block mb-2 px-2" style={{ fontSize: '10px', letterSpacing: '0.08em' }}>
          Portal Core
        </span>
        
        <NavLink to="/dashboard" style={linkStyle}>
          <span className="sidebar-nav-icon me-2">📊</span>
          <span className="sidebar-nav-text">Dashboard</span>
        </NavLink>

        <NavLink to="/students" style={linkStyle}>
          <span className="sidebar-nav-icon me-2">👥</span>
          <span className="sidebar-nav-text">Students</span>
        </NavLink>

        {isAdmin && (
          <>
            <NavLink to="/guardians" style={linkStyle}>
              <span className="sidebar-nav-icon me-2">🛡️</span>
              <span className="sidebar-nav-text">Guardians</span>
            </NavLink>

            <NavLink to="/staff" style={linkStyle}>
              <span className="sidebar-nav-icon me-2">💼</span>
              <span className="sidebar-nav-text">Staff Directory</span>
            </NavLink>
          </>
        )}

        <span className="sidebar-section-header text-uppercase fw-bold text-muted d-block mt-4 mb-2 px-2" style={{ fontSize: '10px', letterSpacing: '0.08em' }}>
          Daily Routines
        </span>

        <NavLink to="/attendance/mark" style={linkStyle}>
          <span className="sidebar-nav-icon me-2">✅</span>
          <span className="sidebar-nav-text">Mark Attendance</span>
        </NavLink>

        <NavLink to="/attendance/analytics" style={linkStyle}>
          <span className="sidebar-nav-icon me-2">📈</span>
          <span className="sidebar-nav-text">Class Analytics</span>
        </NavLink>

        {isAdmin && (
          <>
            <span className="sidebar-section-header text-uppercase fw-bold text-muted d-block mt-4 mb-2 px-2" style={{ fontSize: '10px', letterSpacing: '0.08em' }}>
              System Configs
            </span>
            <NavLink to="/settings/academic-years" style={linkStyle}>
              <span className="sidebar-nav-icon me-2">📅</span>
              <span className="sidebar-nav-text">Academic Years</span>
            </NavLink>
            <NavLink to="/settings/classes" style={linkStyle}>
              <span className="sidebar-nav-icon me-2">🏫</span>
              <span className="sidebar-nav-text">Classes Config</span>
            </NavLink>
            <NavLink to="/settings/sections" style={linkStyle}>
              <span className="sidebar-nav-icon me-2">🧱</span>
              <span className="sidebar-nav-text">Sections Config</span>
            </NavLink>
          </>
        )}
      </nav>

      {/* Footer / Session indicator */}
      <div className="p-3 border-top border-light text-center text-muted" style={{ fontSize: 'var(--font-size-xs)', backgroundColor: '#fafafa', minHeight: '52px' }}>
        <span className="sidebar-footer-text">
          Portal: <span className="fw-semibold text-dark">{user?.role}</span>
        </span>
        <span className="sidebar-nav-icon d-none">🔐</span>
      </div>
    </aside>
  );
};

export default Sidebar;
