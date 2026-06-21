import React from 'react';

const ConfirmDeleteModal = ({ isOpen, onConfirm, onCancel, itemName }) => {
  if (!isOpen) return null;

  return (
    <div style={{
      position: 'fixed',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: 'rgba(15, 23, 42, 0.25)',
      backdropFilter: 'blur(3px)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      zIndex: 1100,
      padding: '20px'
    }}>
      <div 
        className="bg-white p-4 text-center" 
        style={{
          maxWidth: '400px',
          width: '100%',
          borderRadius: 'var(--radius-lg)',
          boxShadow: 'var(--shadow-xl)',
          animation: 'modalScale 0.2s cubic-bezier(0.16, 1, 0.3, 1)'
        }}
      >
        {/* Warning Icon Badge */}
        <div 
          className="d-inline-flex align-items-center justify-content-center bg-danger-subtle text-danger rounded-circle mb-3" 
          style={{ width: '48px', height: '48px', backgroundColor: 'var(--color-danger-bg)' }}
        >
          <span style={{ fontSize: '20px' }}>⚠️</span>
        </div>

        {/* Modal Copy */}
        <h5 className="fw-bold mb-2 text-dark">Confirm Delete</h5>
        <p className="text-muted small mb-4">
          Are you sure you want to delete <span className="fw-semibold text-dark">{itemName || 'this record'}</span>? 
          This action cannot be undone and will permanently remove the data.
        </p>

        {/* Buttons Actions */}
        <div className="d-flex justify-content-center gap-2">
          <button 
            type="button" 
            className="btn btn-secondary py-2 px-4" 
            style={{ borderRadius: 'var(--radius-xs)', fontSize: 'var(--font-size-xs)' }}
            onClick={onCancel}
          >
            Cancel
          </button>
          <button 
            type="button" 
            className="btn btn-danger py-2 px-4" 
            style={{ borderRadius: 'var(--radius-xs)', fontSize: 'var(--font-size-xs)', backgroundColor: 'var(--color-danger)', borderColor: 'var(--color-danger)' }}
            onClick={onConfirm}
          >
            Delete Record
          </button>
        </div>
      </div>

      <style dangerouslySetInnerHTML={{__html: `
        @keyframes modalScale {
          from {
            transform: scale(0.95);
            opacity: 0;
          }
          to {
            transform: scale(1);
            opacity: 1;
          }
        }
      `}} />
    </div>
  );
};

export default ConfirmDeleteModal;
