import React, { useState, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import apiClient from '../../services/apiClient';
import { addNotification } from '../../store/slices/uiSlice';
import DataTable from '../../components/ui/DataTable';
import ConfirmDeleteModal from '../../components/feedback/ConfirmDeleteModal';
import PermissionGate from '../../components/PermissionGate';
import { validateForm, required, maxLength } from '../../utils/validation';

const AcademicYearManager = () => {
  const dispatch = useDispatch();
  const { user } = useSelector((state) => state.auth);
  const isAdmin = user?.role === 'SchoolAdmin';

  const [academicYears, setAcademicYears] = useState([]);
  const [loading, setLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  
  // Modals state
  const [showModal, setShowModal] = useState(false);
  const [modalMode, setModalMode] = useState('create'); // 'create' | 'edit'
  const [selectedYear, setSelectedYear] = useState(null);
  
  // Delete modal state
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [yearToDelete, setYearToDelete] = useState(null);

  // Form state
  const [formData, setFormData] = useState({
    name: '',
    startDate: '',
    endDate: ''
  });
  const [formErrors, setFormErrors] = useState({});

  // Fetch academic years
  const fetchAcademicYears = async () => {
    setLoading(true);
    try {
      const response = await apiClient.get('/api/academic-years');
      setAcademicYears(response.data || []);
    } catch (error) {
      console.error('Error fetching academic years:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAcademicYears();
  }, []);

  // Format date for displaying in table
  const formatDate = (dateStr) => {
    if (!dateStr) return '-';
    try {
      const date = new Date(dateStr);
      return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
    } catch {
      return dateStr;
    }
  };

  // Format date to YYYY-MM-DD for input field
  const formatDateForInput = (dateStr) => {
    if (!dateStr) return '';
    try {
      return new Date(dateStr).toISOString().split('T')[0];
    } catch {
      return '';
    }
  };

  // Open modal for Create
  const handleOpenCreate = () => {
    setFormData({
      name: '',
      startDate: '',
      endDate: ''
    });
    setFormErrors({});
    setModalMode('create');
    setShowModal(true);
  };

  // Open modal for Edit
  const handleOpenEdit = (year) => {
    setSelectedYear(year);
    setFormData({
      name: year.name || '',
      startDate: formatDateForInput(year.startDate),
      endDate: formatDateForInput(year.endDate)
    });
    setFormErrors({});
    setModalMode('edit');
    setShowModal(true);
  };

  // Open delete confirm modal
  const handleOpenDelete = (year) => {
    setYearToDelete(year);
    setShowDeleteModal(true);
  };

  // Handle Form Change
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value
    }));
    // Clear validation error when editing
    if (formErrors[name]) {
      setFormErrors((prev) => ({
        ...prev,
        [name]: null
      }));
    }
  };

  // Submit Form
  const handleSubmit = async (e) => {
    e.preventDefault();

    // Validation rules
    const rules = {
      name: [required, (val) => maxLength(val, 20)],
      startDate: [required],
      endDate: [
        required,
        (val) => {
          if (formData.startDate && val && new Date(val) <= new Date(formData.startDate)) {
            return 'End date must be after start date.';
          }
          return null;
        }
      ]
    };

    const { isValid, errors } = validateForm(formData, rules);

    if (!isValid) {
      setFormErrors(errors);
      return;
    }

    try {
      if (modalMode === 'create') {
        await apiClient.post('/api/academic-years', formData);
        dispatch(addNotification({
          message: 'Academic Year created successfully!',
          type: 'success'
        }));
      } else {
        await apiClient.put(`/api/academic-years/${selectedYear.academicYearId}`, formData);
        dispatch(addNotification({
          message: 'Academic Year updated successfully!',
          type: 'success'
        }));
      }
      setShowModal(false);
      fetchAcademicYears();
    } catch (error) {
      console.error('Error submitting form:', error);
    }
  };

  // Confirm delete
  const handleConfirmDelete = async () => {
    if (!yearToDelete) return;
    try {
      await apiClient.delete(`/api/academic-years/${yearToDelete.academicYearId}`);
      dispatch(addNotification({
        message: 'Academic Year deleted successfully!',
        type: 'success'
      }));
      setShowDeleteModal(false);
      setYearToDelete(null);
      fetchAcademicYears();
    } catch (error) {
      console.error('Error deleting academic year:', error);
    }
  };

  // Filter academic years based on search term
  const filteredYears = academicYears.filter((year) =>
    year.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const columns = [
    {
      header: 'Academic Year Name',
      accessor: 'name',
      render: (row) => <span className="fw-semibold text-dark">{row.name}</span>
    },
    {
      header: 'Start Date',
      accessor: 'startDate',
      render: (row) => formatDate(row.startDate)
    },
    {
      header: 'End Date',
      accessor: 'endDate',
      render: (row) => formatDate(row.endDate)
    },
    ...(isAdmin ? [{
      header: 'Actions',
      align: 'right',
      width: '120px',
      render: (row) => (
        <div className="d-flex justify-content-end gap-2">
          <button
            type="button"
            className="btn btn-sm btn-outline-secondary py-1 px-2 border-0"
            onClick={() => handleOpenEdit(row)}
            title="Edit Academic Year"
          >
            ✏️
          </button>
          <button
            type="button"
            className="btn btn-sm btn-outline-danger py-1 px-2 border-0"
            onClick={() => handleOpenDelete(row)}
            title="Delete Academic Year"
          >
            🗑️
          </button>
        </div>
      )
    }] : [])
  ];

  return (
    <div className="p-4" style={{ backgroundColor: 'var(--color-bg-main)', minHeight: '100%' }}>
      {/* Page Header */}
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h4 className="fw-bold text-dark mb-1" style={{ fontSize: 'var(--font-size-2xl)' }}>Academic Years</h4>
          <p className="text-muted mb-0" style={{ fontSize: 'var(--font-size-sm)' }}>
            Manage school academic terms, durations, and scheduler calendars.
          </p>
        </div>
        <PermissionGate allowedRoles={['SchoolAdmin']}>
          <button
            type="button"
            className="btn btn-primary d-flex align-items-center gap-2 py-2 px-3 fw-medium"
            onClick={handleOpenCreate}
            style={{
              backgroundColor: 'var(--color-primary)',
              borderColor: 'var(--color-primary)',
              borderRadius: 'var(--radius-xs)',
              fontSize: 'var(--font-size-sm)'
            }}
          >
            <span>+</span> Add Academic Year
          </button>
        </PermissionGate>
      </div>

      {/* Main Roster Card */}
      <div className="ep-card bg-white p-4" style={{ borderRadius: 'var(--radius-lg)', boxShadow: 'var(--shadow-sm)' }}>
        <DataTable
          columns={columns}
          data={filteredYears}
          loading={loading}
          searchValue={searchTerm}
          onSearchChange={setSearchTerm}
          searchPlaceholder="Search academic years..."
          emptyMessage="No academic years defined yet. Click Add Academic Year to register a term."
          emptyActionLabel={isAdmin ? "Add Academic Year" : null}
          onEmptyAction={handleOpenCreate}
        />
      </div>

      {/* Form Modal */}
      {showModal && (
        <div style={{
          position: 'fixed',
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          backgroundColor: 'rgba(15, 23, 42, 0.3)',
          backdropFilter: 'blur(3px)',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          zIndex: 1050,
          padding: '20px'
        }}>
          <div 
            className="bg-white" 
            style={{
              maxWidth: '500px',
              width: '100%',
              borderRadius: 'var(--radius-lg)',
              boxShadow: 'var(--shadow-xl)',
              animation: 'modalScale 0.2s cubic-bezier(0.16, 1, 0.3, 1)',
              overflow: 'hidden'
            }}
          >
            <div className="px-4 py-3 border-bottom d-flex justify-content-between align-items-center bg-light">
              <h5 className="mb-0 fw-bold text-dark">
                {modalMode === 'create' ? 'Add Academic Year' : 'Edit Academic Year'}
              </h5>
              <button 
                type="button" 
                className="btn-close" 
                onClick={() => setShowModal(false)}
                aria-label="Close"
              />
            </div>
            
            <form onSubmit={handleSubmit}>
              <div className="p-4">
                {/* Year Name */}
                <div className="mb-3">
                  <label className="ep-form-label mb-1 d-block text-uppercase fw-semibold text-muted" style={{ fontSize: 'var(--font-size-xs)' }}>
                    Academic Year Name *
                  </label>
                  <input
                    type="text"
                    name="name"
                    className={`form-control ep-input ${formErrors.name ? 'is-invalid' : ''}`}
                    placeholder="e.g., 2026-2027"
                    value={formData.name}
                    onChange={handleChange}
                    style={{ height: '44px', borderRadius: 'var(--radius-xs)' }}
                  />
                  {formErrors.name && (
                    <div className="invalid-feedback text-danger small mt-1">{formErrors.name}</div>
                  )}
                  <small className="text-muted">Maximum length is 20 characters.</small>
                </div>

                {/* Dates Row */}
                <div className="row">
                  <div className="col-md-6 mb-3">
                    <label className="ep-form-label mb-1 d-block text-uppercase fw-semibold text-muted" style={{ fontSize: 'var(--font-size-xs)' }}>
                      Start Date *
                    </label>
                    <input
                      type="date"
                      name="startDate"
                      className={`form-control ep-input ${formErrors.startDate ? 'is-invalid' : ''}`}
                      value={formData.startDate}
                      onChange={handleChange}
                      style={{ height: '44px', borderRadius: 'var(--radius-xs)' }}
                    />
                    {formErrors.startDate && (
                      <div className="invalid-feedback text-danger small mt-1">{formErrors.startDate}</div>
                    )}
                  </div>
                  <div className="col-md-6 mb-3">
                    <label className="ep-form-label mb-1 d-block text-uppercase fw-semibold text-muted" style={{ fontSize: 'var(--font-size-xs)' }}>
                      End Date *
                    </label>
                    <input
                      type="date"
                      name="endDate"
                      className={`form-control ep-input ${formErrors.endDate ? 'is-invalid' : ''}`}
                      value={formData.endDate}
                      onChange={handleChange}
                      style={{ height: '44px', borderRadius: 'var(--radius-xs)' }}
                    />
                    {formErrors.endDate && (
                      <div className="invalid-feedback text-danger small mt-1">{formErrors.endDate}</div>
                    )}
                  </div>
                </div>
              </div>

              <div className="px-4 py-3 bg-light border-top d-flex justify-content-end gap-2">
                <button
                  type="button"
                  className="btn btn-outline py-2 px-4"
                  onClick={() => setShowModal(false)}
                  style={{ borderRadius: 'var(--radius-xs)', fontSize: 'var(--font-size-sm)' }}
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="btn btn-primary py-2 px-4"
                  style={{
                    borderRadius: 'var(--radius-xs)',
                    fontSize: 'var(--font-size-sm)',
                    backgroundColor: 'var(--color-primary)',
                    borderColor: 'var(--color-primary)'
                  }}
                >
                  {modalMode === 'create' ? 'Create Term' : 'Save Changes'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Reusable Confirm Delete Modal */}
      <ConfirmDeleteModal
        isOpen={showDeleteModal}
        onConfirm={handleConfirmDelete}
        onCancel={() => setShowDeleteModal(false)}
        itemName={yearToDelete ? yearToDelete.name : ''}
      />
    </div>
  );
};

export default AcademicYearManager;
