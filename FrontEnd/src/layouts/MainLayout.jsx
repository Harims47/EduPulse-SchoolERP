import React from 'react';
import { Outlet } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';
import Sidebar from './Sidebar';
import Header from './Header';
import { setMobileSidebarOpen } from '../store/slices/uiSlice';

const MainLayout = () => {
  const dispatch = useDispatch();
  const { sidebarCollapsed, mobileSidebarOpen } = useSelector((state) => state.ui);

  return (
    <div className={`ep-layout ${sidebarCollapsed ? 'sidebar-collapsed' : ''} ${mobileSidebarOpen ? 'mobile-sidebar-open' : ''}`}>
      {/* Sidebar Panel Navigation */}
      <Sidebar />

      {/* Backdrop overlay for mobile drawer */}
      {mobileSidebarOpen && (
        <div className="ep-backdrop" onClick={() => dispatch(setMobileSidebarOpen(false))} />
      )}

      {/* Main Content Frames */}
      <div className="ep-main">
        {/* Sticky Top Navbar */}
        <Header />

        {/* Dynamic Page Outlets Container */}
        <main className="flex-grow-1 p-4" style={{ overflowY: 'auto' }}>
          <div className="container-fluid p-0">
            <Outlet />
          </div>
        </main>
      </div>
    </div>
  );
};

export default MainLayout;
