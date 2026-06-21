import React from 'react';

const DataTable = ({
  columns,
  data = [],
  loading = false,
  searchPlaceholder = 'Search...',
  searchValue = '',
  onSearchChange,
  currentPage = 1,
  totalPages = 1,
  onPageChange,
  emptyMessage = 'No records found.',
  emptyActionLabel,
  onEmptyAction,
  actionButton // Optional component or button to render next to search
}) => {
  const startIndex = (currentPage - 1) * 10 + 1;
  const endIndex = Math.min(currentPage * 10, data.length); // local page count or handle external page count calculation
  
  return (
    <div className="ep-table-wrapper">
      {/* Table Toolbar */}
      {(onSearchChange || actionButton) && (
        <div className="d-flex flex-column flex-sm-row justify-content-between align-items-stretch align-items-sm-center gap-3 mb-4">
          {onSearchChange ? (
            <div className="position-relative flex-grow-1" style={{ maxWidth: '320px' }}>
              <span 
                className="position-absolute top-50 start-0 translate-middle-y ms-3 text-muted" 
                style={{ fontSize: '14px', pointerEvents: 'none' }}
              >
                🔍
              </span>
              <input
                type="text"
                className="form-control ep-input ps-5"
                placeholder={searchPlaceholder}
                value={searchValue}
                onChange={(e) => onSearchChange(e.target.value)}
                style={{
                  height: '40px',
                  borderRadius: 'var(--radius-xs)',
                  border: '1px solid var(--color-border-default)',
                  fontSize: 'var(--font-size-sm)',
                  backgroundColor: 'var(--color-bg-surface)'
                }}
              />
            </div>
          ) : (
            <div />
          )}
          {actionButton && (
            <div className="d-flex align-items-center gap-2">
              {actionButton}
            </div>
          )}
        </div>
      )}

      {/* Table Container Card */}
      <div 
        className="ep-table-container border bg-white" 
        style={{
          borderRadius: 'var(--radius-lg)',
          borderColor: 'rgba(226, 232, 240, 0.8)',
          boxShadow: 'var(--shadow-sm)',
          overflow: 'hidden'
        }}
      >
        <div className="table-responsive" style={{ overflowX: 'auto' }}>
          <table className="table mb-0 align-middle" style={{ width: '100%' }}>
            <thead>
              <tr style={{ height: '44px', backgroundColor: '#fafafa', borderBottom: '1px solid var(--color-border-default)' }}>
                {columns.map((col, idx) => (
                  <th 
                    key={idx}
                    className="text-uppercase fw-semibold text-muted px-4 py-2"
                    style={{
                      fontSize: 'var(--font-size-xs)',
                      letterSpacing: '0.05em',
                      textAlign: col.align || 'left',
                      width: col.width || 'auto',
                      borderBottom: 'none'
                    }}
                  >
                    {col.header}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {loading ? (
                // Loading Skeleton rows
                Array.from({ length: 5 }).map((_, rIdx) => (
                  <tr key={rIdx} style={{ height: '52px', borderBottom: '1px solid var(--color-border-subtle)' }}>
                    {columns.map((col, cIdx) => (
                      <td key={cIdx} className="px-4 py-3">
                        <div className="skeleton-line" style={{ height: '16px', background: '#f1f5f9', borderRadius: '4px', width: '80%', animation: 'pulse 1.5s infinite' }} />
                      </td>
                    ))}
                  </tr>
                ))
              ) : data.length === 0 ? (
                // Empty state block
                <tr>
                  <td colSpan={columns.length} className="text-center py-5 px-4">
                    <div className="d-flex flex-column align-items-center justify-content-center py-4">
                      <div 
                        className="d-flex align-items-center justify-content-center text-muted mb-3" 
                        style={{ fontSize: '32px', opacity: 0.4 }}
                      >
                        📂
                      </div>
                      <h6 className="fw-semibold text-dark mb-1" style={{ fontSize: 'var(--font-size-md)' }}>
                        No Records Found
                      </h6>
                      <p className="text-muted small mb-3" style={{ maxWidth: '300px', fontSize: 'var(--font-size-sm)' }}>
                        {emptyMessage}
                      </p>
                      {emptyActionLabel && onEmptyAction && (
                        <button
                          type="button"
                          className="btn btn-primary btn-sm py-2 px-3 fw-medium"
                          onClick={onEmptyAction}
                          style={{
                            borderRadius: 'var(--radius-xs)',
                            fontSize: 'var(--font-size-xs)',
                            backgroundColor: 'var(--color-primary)',
                            borderColor: 'var(--color-primary)'
                          }}
                        >
                          {emptyActionLabel}
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ) : (
                // Data rows
                data.map((row, rIdx) => (
                  <tr 
                    key={row.id || rIdx} 
                    className="ep-table-row"
                    style={{ 
                      height: '52px', 
                      borderBottom: '1px solid var(--color-border-subtle)',
                      transition: 'background-color 0.15s ease'
                    }}
                  >
                    {columns.map((col, cIdx) => {
                      const value = col.accessor ? row[col.accessor] : null;
                      return (
                        <td 
                          key={cIdx} 
                          className="px-4 py-2 text-secondary"
                          style={{ 
                            fontSize: 'var(--font-size-sm)',
                            textAlign: col.align || 'left',
                            color: 'var(--color-text-secondary)'
                          }}
                        >
                          {col.render ? col.render(row, value, rIdx) : (value !== null && value !== undefined ? String(value) : '-')}
                        </td>
                      );
                    })}
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {/* Table Footer with Pagination */}
        {!loading && data.length > 0 && totalPages > 1 && (
          <div 
            className="d-flex flex-column flex-sm-row justify-content-between align-items-center gap-3 px-4 py-3 border-top"
            style={{ borderColor: 'var(--color-border-default)' }}
          >
            <div className="text-muted small" style={{ fontSize: 'var(--font-size-xs)' }}>
              Page <span className="fw-semibold text-dark">{currentPage}</span> of <span className="fw-semibold text-dark">{totalPages}</span>
            </div>
            <div className="d-flex gap-2">
              <button
                type="button"
                className="btn btn-outline btn-sm py-1 px-3"
                disabled={currentPage <= 1}
                onClick={() => onPageChange(currentPage - 1)}
                style={{
                  borderRadius: 'var(--radius-xs)',
                  fontSize: 'var(--font-size-xs)',
                  border: '1px solid var(--color-border-default)',
                  backgroundColor: currentPage <= 1 ? '#fafafa' : '#ffffff',
                  color: currentPage <= 1 ? 'var(--color-text-muted)' : 'var(--color-text-secondary)'
                }}
              >
                Previous
              </button>
              <button
                type="button"
                className="btn btn-outline btn-sm py-1 px-3"
                disabled={currentPage >= totalPages}
                onClick={() => onPageChange(currentPage + 1)}
                style={{
                  borderRadius: 'var(--radius-xs)',
                  fontSize: 'var(--font-size-xs)',
                  border: '1px solid var(--color-border-default)',
                  backgroundColor: currentPage >= totalPages ? '#fafafa' : '#ffffff',
                  color: currentPage >= totalPages ? 'var(--color-text-muted)' : 'var(--color-text-secondary)'
                }}
              >
                Next
              </button>
            </div>
          </div>
        )}
      </div>

      <style dangerouslySetInnerHTML={{__html: `
        .ep-table-row:hover {
          background-color: #fafafa !important;
        }
        @keyframes pulse {
          0%, 100% { opacity: 1; }
          50% { opacity: 0.5; }
        }
      `}} />
    </div>
  );
};

export default DataTable;
