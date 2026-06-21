import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { logout } from '../store/slices/authSlice';
import { setActiveYear, setAvailableYears } from '../store/slices/academicContextSlice';
import { toggleSidebar, toggleMobileSidebar } from '../store/slices/uiSlice';
import apiClient from '../services/apiClient';

const Header = () => {
  const dispatch = useDispatch();
  const { user } = useSelector((state) => state.auth);
  const { activeYearLabel, availableYears } = useSelector((state) => state.academicContext);

  useEffect(() => {
    const fetchYears = async () => {
      try {
        const response = await apiClient.get('/api/academic-years');
        dispatch(setAvailableYears(response.data));
      } catch (err) {
        console.error('Failed to load academic years', err);
      }
    };
    fetchYears();
  }, [dispatch]);

  const handleYearChange = (yearId, yearName) => {
    dispatch(setActiveYear({ id: yearId, label: yearName }));
  };

  const handleLogout = () => {
    dispatch(logout());
  };

  const handleSidebarToggle = () => {
    // Check screen size
    if (window.innerWidth <= 768) {
      dispatch(toggleMobileSidebar());
    } else {
      dispatch(toggleSidebar());
    }
  };

  return (
    <header className="d-flex align-items-center justify-content-between px-3" style={{
      height: '70px',
      backgroundColor: '#ffffff',
      borderBottom: '1px solid rgba(226, 232, 240, 0.6)',
      boxShadow: 'var(--shadow-sm)',
      position: 'sticky',
      top: 0,
      zIndex: 100
    }}>
      {/* Search and Academic context */}
      <div className="d-flex align-items-center gap-3">
        {/* Sidebar Toggle Button */}
        <button 
          className="btn btn-light border-0 d-flex align-items-center justify-content-center" 
          onClick={handleSidebarToggle}
          style={{ width: '38px', height: '38px', borderRadius: 'var(--radius-xs)', backgroundColor: '#fafafa' }}
          title="Toggle Sidebar Menu"
          id="sidebar-toggle-btn"
        >
          <span>☰</span>
        </button>

        {/* School Name & Code */}
        <div className="d-none d-md-flex flex-column">
          <span className="fw-bold text-dark" style={{ fontSize: 'var(--font-size-sm)', lineHeight: '1.2' }}>
            EduPulse International Academy
          </span>
          <span className="text-muted font-monospace" style={{ fontSize: '10px' }}>
            School Code: EPA-2026
          </span>
        </div>
        
        <span className="text-muted d-none d-md-inline opacity-50">|</span>

        {/* Academic Year Dropdown */}
        <div className="dropdown">
          <button className="btn btn-sm dropdown-toggle text-dark border-0 fw-semibold bg-light" type="button" data-bs-toggle="dropdown" aria-expanded="false" style={{ borderRadius: 'var(--radius-xs)' }}>
            📅 {activeYearLabel}
          </button>
          <ul className="dropdown-menu border-0 shadow-lg mt-2" style={{ borderRadius: 'var(--radius-sm)' }}>
            {availableYears.length === 0 ? (
              <li><span className="dropdown-item disabled">No Years Found</span></li>
            ) : (
              availableYears.map((year) => (
                <li key={year.academicYearId}>
                  <button className="dropdown-item py-2" type="button" onClick={() => handleYearChange(year.academicYearId, year.name)}>
                    {year.name}
                  </button>
                </li>
              ))
            )}
          </ul>
        </div>
      </div>

      {/* User Actions & Utility Icons */}
      <div className="d-flex align-items-center gap-2">
        {/* Notifications Icon with Badge */}
        <div className="dropdown">
          <button className="btn btn-light border-0 position-relative d-flex align-items-center justify-content-center" data-bs-toggle="dropdown" style={{ width: '38px', height: '38px', borderRadius: 'var(--radius-xs)', backgroundColor: '#fafafa' }}>
            <span style={{ fontSize: '16px' }}>🔔</span>
            <span className="position-absolute top-1 start-100 translate-middle badge rounded-pill bg-danger border border-light" style={{ fontSize: '8px', padding: '3px 5px' }}>
              3
            </span>
          </button>
          <div className="dropdown-menu dropdown-menu-end border-0 shadow-lg p-3 mt-2" style={{ width: '280px', borderRadius: 'var(--radius-sm)' }}>
            <div className="d-flex justify-content-between align-items-center mb-2 pb-2 border-bottom">
              <span className="fw-bold text-dark small">Notifications</span>
              <span className="ep-badge ep-badge-success" style={{ fontSize: '8px' }}>3 New</span>
            </div>
            <div className="d-flex flex-column gap-2">
              <div className="p-2 bg-light rounded small" style={{ fontSize: '11px' }}>
                <div className="fw-semibold text-dark">PTA Meeting Scheduled</div>
                <div className="text-muted mt-1">Annual meet published in notifications board.</div>
              </div>
              <div className="p-2 bg-light rounded small" style={{ fontSize: '11px' }}>
                <div className="fw-semibold text-dark">Roster Upload Complete</div>
                <div className="text-muted mt-1">32 Grade 10 profiles verified successfully.</div>
              </div>
            </div>
          </div>
        </div>

        {/* Global Settings Trigger */}
        <button className="btn btn-light border-0 d-none d-sm-flex align-items-center justify-content-center" style={{ width: '38px', height: '38px', borderRadius: 'var(--radius-xs)', backgroundColor: '#fafafa' }} title="System Settings">
          <span style={{ fontSize: '16px' }}>⚙️</span>
        </button>

        {/* Profile Details Selector */}
        <div className="dropdown">
          <button className="btn btn-light border-0 d-flex align-items-center gap-2 px-2" data-bs-toggle="dropdown" style={{ height: '38px', borderRadius: 'var(--radius-xs)', backgroundColor: '#fafafa' }}>
            <div className="bg-primary text-white rounded-circle d-flex align-items-center justify-content-center fw-bold" style={{ width: '24px', height: '24px', fontSize: '11px', backgroundColor: 'var(--color-primary)' }}>
              {user?.firstName ? user.firstName[0].toUpperCase() : 'U'}
            </div>
            <span className="d-none d-sm-inline fw-semibold text-dark" style={{ fontSize: 'var(--font-size-xs)' }}>
              {user?.firstName}
            </span>
          </button>
          <ul className="dropdown-menu dropdown-menu-end border-0 shadow-lg mt-2" style={{ borderRadius: 'var(--radius-sm)', minWidth: '180px' }}>
            <li className="px-3 py-2 border-bottom">
              <div className="fw-semibold text-dark small">{user?.firstName} {user?.lastName}</div>
              <div className="text-muted xsmall mt-1">{user?.email}</div>
            </li>
            <li>
              <button className="dropdown-item py-2 text-danger small d-flex align-items-center gap-2" onClick={handleLogout}>
                <span>🚪</span> Sign Out
              </button>
            </li>
          </ul>
        </div>
      </div>
    </header>
  );
};

export default Header;
