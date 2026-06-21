import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { removeNotification } from '../../store/slices/uiSlice';

const ToastContainer = () => {
  const dispatch = useDispatch();
  const notifications = useSelector((state) => state.ui.notifications);

  return (
    <div style={{
      position: 'fixed',
      top: '24px',
      right: '24px',
      zIndex: 9999,
      display: 'flex',
      flexDirection: 'column',
      gap: '10px',
      maxWidth: '350px',
      width: '100%'
    }}>
      {notifications.map((toast) => (
        <ToastItem key={toast.id} toast={toast} onClose={(id) => dispatch(removeNotification(id))} />
      ))}
    </div>
  );
};

const ToastItem = ({ toast, onClose }) => {
  useEffect(() => {
    const timer = setTimeout(() => {
      onClose(toast.id);
    }, 4000);
    return () => clearTimeout(timer);
  }, [toast, onClose]);

  // Color maps based on toast type
  let bg = 'var(--color-bg-surface)';
  let border = 'rgba(226, 232, 240, 0.8)';
  let textColor = 'var(--color-text-primary)';
  let icon = 'ℹ️';

  if (toast.type === 'success') {
    bg = 'var(--color-success-bg)';
    border = '1px solid rgba(16, 185, 129, 0.15)';
    textColor = 'var(--color-success)';
    icon = '✅';
  } else if (toast.type === 'warning') {
    bg = 'var(--color-warning-bg)';
    border = '1px solid rgba(245, 158, 11, 0.15)';
    textColor = 'var(--color-warning)';
    icon = '⚠️';
  } else if (toast.type === 'danger') {
    bg = 'var(--color-danger-bg)';
    border = '1px solid rgba(239, 68, 68, 0.15)';
    textColor = 'var(--color-danger)';
    icon = '🛑';
  }

  return (
    <div 
      style={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        padding: '14px 18px',
        backgroundColor: bg,
        border: border,
        borderRadius: 'var(--radius-sm)',
        boxShadow: 'var(--shadow-lg)',
        animation: 'slideIn 0.25s cubic-bezier(0.16, 1, 0.3, 1)',
        color: textColor
      }}
    >
      <div className="d-flex align-items-center gap-2">
        <span style={{ fontSize: '16px' }}>{icon}</span>
        <span style={{ fontSize: '13px', fontWeight: '500', color: 'var(--color-text-primary)' }}>{toast.message}</span>
      </div>
      <button 
        type="button" 
        onClick={() => onClose(toast.id)} 
        style={{
          border: 'none',
          background: 'none',
          color: 'var(--color-text-muted)',
          fontSize: '16px',
          cursor: 'pointer',
          padding: '0 0 0 10px',
          lineHeight: 1
        }}
      >
        ×
      </button>

      <style dangerouslySetInnerHTML={{__html: `
        @keyframes slideIn {
          from {
            transform: translateY(-20px);
            opacity: 0;
          }
          to {
            transform: translateY(0);
            opacity: 1;
          }
        }
      `}} />
    </div>
  );
};

export default ToastContainer;
