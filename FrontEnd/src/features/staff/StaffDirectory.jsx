import React, { useState, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import apiClient from '../../services/apiClient';
import { addNotification } from '../../store/slices/uiSlice';
import DataTable from '../../components/ui/DataTable';
import ConfirmDeleteModal from '../../components/feedback/ConfirmDeleteModal';
import PermissionGate from '../../components/PermissionGate';
import { validateForm, required, maxLength, phone as phoneValidator } from '../../utils/validation';

const StaffDirectory = () => {
  const dispatch = useDispatch();
  const { user } = useSelector((state) => state.auth);
  const isAdmin = user?.role === 'SchoolAdmin';

  const [staffList, setStaffList] = useState([]);
  const [loading, setLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('ALL'); // 'ALL' | 'ACTIVE' | 'INACTIVE'
  
  // Modals state
  const [showModal, setShowModal] = useState(false);
  const [modalMode, setModalMode] = useState('create'); // 'create' | 'edit'
  const [selectedStaff, setSelectedStaff] = useState(null);
  
  // Delete modal state
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [staffToDelete, setStaffToDelete] = useState(null);

  // Form state
  const [formData, setFormData] = useState({
    employeeCode: '',
    firstName: '',
    lastName: '',
    phone: '',
    designation: '',
    photoPath: '',
    isActive: true,
    userId: null
  });
  const [formErrors, setFormErrors] = useState({});

  // Fetch staff list
  const fetchStaff = async () => {
    setLoading(true);
    try {
      const response = await apiClient.get('/api/staff');
      setStaffList(response.data || []);
    } catch (error) {
      console.error('Error fetching staff directory:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchStaff();
  }, []);

  // Open modal for Create
  const handleOpenCreate = () => {
    setFormData({
      employeeCode: '',
      firstName: '',
      lastName: '',
      phone: '',
      designation: '',
      photoPath: '',
      isActive: true,
      userId: null
    });
    setFormErrors({});
    setModalMode('create');
    setShowModal(true);
  };

  // Open modal for Edit
  const handleOpenEdit = (staff) => {
    setSelectedStaff(staff);
    setFormData({
      employeeCode: staff.employeeCode || '',
      firstName: staff.firstName || '',
      lastName: staff.lastName || '',
      phone: staff.phone || '',
      designation: staff.designation || '',
      photoPath: staff.photoPath || '',
      isActive: staff.isActive ?? true,
      userId: staff.userId || null
    });
    setFormErrors({});
    setModalMode('edit');
    setShowModal(true);
  };

  // Open delete confirm modal
  const handleOpenDelete = (staff) => {
    setStaffToDelete(staff);
    setShowDeleteModal(true);
  };

  // Handle Form Change
  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
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
      employeeCode: [required, (val) => maxLength(val, 50)],
      firstName: [required, (val) => maxLength(val, 100)],
      lastName: [required, (val) => maxLength(val, 100)],
      phone: [required, phoneValidator],
      designation: [(val) => maxLength(val, 50)],
      photoPath: [(val) => maxLength(val, 500)]
    };

    const { isValid, errors } = validateForm(formData, rules);

    if (!isValid) {
      setFormErrors(errors);
      return;
    }

    try {
      if (modalMode === 'create') {
        await apiClient.post('/api/staff', formData);
        dispatch(addNotification({
          message: 'Staff member added successfully!',
          type: 'success'
        }));
      } else {
        await apiClient.put(`/api/staff/${selectedStaff.staffId}`, formData);
        dispatch(addNotification({
          message: 'Staff profile updated successfully!',
          type: 'success'
        }));
      }
      setShowModal(false);
      fetchStaff();
    } catch (error) {
      console.error('Error submitting staff form:', error);
    }
  };

  // Confirm delete
  const handleConfirmDelete = async () => {
    if (!staffToDelete) return;
    try {
      await apiClient.delete(`/api/staff/${staffToDelete.staffId}`);
      dispatch(addNotification({
        message: 'Staff record deleted successfully!',
        type: 'success'
      }));
      setShowDeleteModal(false);
      setStaffToDelete(null);
      fetchStaff();
    } catch (error) {
      console.error('Error deleting staff member:', error);
    }
  };

  // Filter staff list based on search term and active status filter
  const filteredStaff = staffList.filter((staff) => {
    const fullName = `${staff.firstName} ${staff.lastName}`.toLowerCase();
    const code = (staff.employeeCode || '').toLowerCase();
    const designation = (staff.designation || '').toLowerCase();
    const matchesSearch = 
      fullName.includes(searchTerm.toLowerCase()) || 
      code.includes(searchTerm.toLowerCase()) ||
      designation.includes(searchTerm.toLowerCase());
    
    const matchesStatus = 
      statusFilter === 'ALL' ||
      (statusFilter === 'ACTIVE' && staff.isActive) ||
      (statusFilter === 'INACTIVE' && !staff.isActive);

    return matchesSearch && matchesStatus;
  });

  const columns = [
    {
      header: 'Code',
      accessor: 'employeeCode',
      render: (row) => <span className="fw-semibold text-secondary">{row.employeeCode}</span>
    },
    {
      header: 'Name',
      accessor: 'firstName',
      render: (row) => (
        <div>
          <span className="fw-semibold text-dark">{row.firstName} {row.lastName}</span>
        </div>
      )
    },
    {
      header: 'Designation',
      accessor: 'designation',
      render: (row) => row.designation || <span className="text-muted small">-</span>
    },
    {
      header: 'Phone Number',
      accessor: 'phone',
      render: (row) => row.phone
    },
    {
      header: 'Status',
      accessor: 'isActive',
      render: (row) => (
        <span 
          className="ep-badge px-2.5 py-1 text-xs fw-semibold rounded-pill"
          style={{
            backgroundColor: row.isActive ? 'var(--color-success-bg)' : 'var(--color-danger-bg)',
            color: row.isActive ? 'var(--color-success)' : 'var(--color-danger)',
            fontSize: 'var(--font-size-xs)'
          }}
        >
          {row.isActive ? 'ACTIVE' : 'INACTIVE'}
        </span>
      )
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
            title="Edit Staff Details"
          >
            ✏️
          </button>
          <button
            type="button"
            className="btn btn-sm btn-outline-danger py-1 px-2 border-0"
            onClick={() => handleOpenDelete(row)}
            title="Delete Staff Record"
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
          <h4 className="fw-bold text-dark mb-1" style={{ fontSize: 'var(--font-size-2xl)' }}>Staff Directory</h4>
          <p className="text-muted mb-0" style={{ fontSize: 'var(--font-size-sm)' }}>
            Manage staff members, employee profile files, departments, and active statuses.
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
            <span>+</span> Add Staff Member
          </button>
        </PermissionGate>
      </div>

      {/* Main Roster Card */}
      <div className="ep-card bg-white p-4" style={{ borderRadius: 'var(--radius-lg)', boxShadow: 'var(--shadow-sm)' }}>
        {/* Extra Status Filter Select Tool */}
        <div className="d-flex flex-wrap align-items-center gap-2 mb-4">
          <label className="text-uppercase fw-semibold text-muted small me-2" style={{ letterSpacing: '0.05em' }}>
            Filter Status:
          </label>
          <div className="btn-group" role="group">
            {['ALL', 'ACTIVE', 'INACTIVE'].map((status) => (
              <button
                key={status}
                type="button"
                className={`btn btn-sm px-3 py-1.5 fw-medium ${statusFilter === status ? 'btn-secondary' : 'btn-outline-secondary'}`}
                onClick={() => setStatusFilter(status)}
                style={{
                  fontSize: 'var(--font-size-xs)',
                  borderRadius: 'var(--radius-xs)',
                  marginRight: '4px'
                }}
              >
                {status}
              </button>
            ))}
          </div>
        </div>

        <DataTable
          columns={columns}
          data={filteredStaff}
          loading={loading}
          searchValue={searchTerm}
          onSearchChange={setSearchTerm}
          searchPlaceholder="Search by name, code or title..."
          emptyMessage="No staff members registered matching the criteria."
          emptyActionLabel={isAdmin ? "Add Staff Member" : null}
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
              maxWidth: '600px',
              width: '100%',
              borderRadius: 'var(--radius-lg)',
              boxShadow: 'var(--shadow-xl)',
              animation: 'modalScale 0.2s cubic-bezier(0.16, 1, 0.3, 1)',
              overflow: 'hidden'
            }}
          >
            <div className="px-4 py-3 border-bottom d-flex justify-content-between align-items-center bg-light">
              <h5 className="mb-0 fw-bold text-dark">
                {modalMode === 'create' ? 'Add Staff Member' : 'Edit Staff Member'}
              </h5>
              <button 
                type="button" 
                className="btn-close" 
                onClick={() => setShowModal(false)}
                aria-label="Close"
              />
            </div>
            
            <form onSubmit={handleSubmit}>
              <div className="p-4" style={{ maxHeight: 'calc(80vh - 100px)', overflowY: 'auto' }}>
                
                {/* Employee Code */}
                <div className="mb-3">
                  <label className="ep-form-label mb-1 d-block text-uppercase fw-semibold text-muted" style={{ fontSize: 'var(--font-size-xs)' }}>
                    Employee Code / ID *
                  </label>
                  <input
                    type="text"
                    name="employeeCode"
                    className={`form-control ep-input ${formErrors.employeeCode ? 'is-invalid' : ''}`}
                    placeholder="e.g., EMP-0012"
                    value={formData.employeeCode}
                    onChange={handleChange}
                    style={{ height: '44px', borderRadius: 'var(--radius-xs)' }}
                  />
                  {formErrors.employeeCode && (
                    <div className="invalid-feedback text-danger small mt-1">{formErrors.employeeCode}</div>
                  )}
                  <small className="text-muted">Unique code code identifier for the employee.</small>
                </div>

                {/* Names Row */}
                <div className="row">
                  <div className="col-md-6 mb-3">
                    <label className="ep-form-label mb-1 d-block text-uppercase fw-semibold text-muted" style={{ fontSize: 'var(--font-size-xs)' }}>
                      First Name *
                    </label>
                    <input
                      type="text"
                      name="firstName"
                      className={`form-control ep-input ${formErrors.firstName ? 'is-invalid' : ''}`}
                      placeholder="e.g., John"
                      value={formData.firstName}
                      onChange={handleChange}
                      style={{ height: '44px', borderRadius: 'var(--radius-xs)' }}
                    />
                    {formErrors.firstName && (
                      <div className="invalid-feedback text-danger small mt-1">{formErrors.firstName}</div>
                    )}
                  </div>
                  <div className="col-md-6 mb-3">
                    <label className="ep-form-label mb-1 d-block text-uppercase fw-semibold text-muted" style={{ fontSize: 'var(--font-size-xs)' }}>
                      Last Name *
                    </label>
                    <input
                      type="text"
                      name="lastName"
                      className={`form-control ep-input ${formErrors.lastName ? 'is-invalid' : ''}`}
                      placeholder="e.g., Doe"
                      value={formData.lastName}
                      onChange={handleChange}
                      style={{ height: '44px', borderRadius: 'var(--radius-xs)' }}
                    />
                    {formErrors.lastName && (
                      <div className="invalid-feedback text-danger small mt-1">{formErrors.lastName}</div>
                    )}
                  </div>
                </div>

                {/* Phone & Designation */}
                <div className="row">
                  <div className="col-md-6 mb-3">
                    <label className="ep-form-label mb-1 d-block text-uppercase fw-semibold text-muted" style={{ fontSize: 'var(--font-size-xs)' }}>
                      Phone Number *
                    </label>
                    <input
                      type="text"
                      name="phone"
                      className={`form-control ep-input ${formErrors.phone ? 'is-invalid' : ''}`}
                      placeholder="e.g., +1234567890"
                      value={formData.phone}
                      onChange={handleChange}
                      style={{ height: '44px', borderRadius: 'var(--radius-xs)' }}
                    />
                    {formErrors.phone && (
                      <div className="invalid-feedback text-danger small mt-1">{formErrors.phone}</div>
                    )}
                  </div>
                  <div className="col-md-6 mb-3">
                    <label className="ep-form-label mb-1 d-block text-uppercase fw-semibold text-muted" style={{ fontSize: 'var(--font-size-xs)' }}>
                      Designation / Role Title
                    </label>
                    <input
                      type="text"
                      name="designation"
                      className={`form-control ep-input ${formErrors.designation ? 'is-invalid' : ''}`}
                      placeholder="e.g., Math Teacher, Principal"
                      value={formData.designation}
                      onChange={handleChange}
                      style={{ height: '44px', borderRadius: 'var(--radius-xs)' }}
                    />
                    {formErrors.designation && (
                      <div className="invalid-feedback text-danger small mt-1">{formErrors.designation}</div>
                    )}
                  </div>
                </div>

                {/* Optional Photo Path */}
                <div className="mb-3">
                  <label className="ep-form-label mb-1 d-block text-uppercase fw-semibold text-muted" style={{ fontSize: 'var(--font-size-xs)' }}>
                    Photo URL (Optional)
                  </label>
                  <input
                    type="text"
                    name="photoPath"
                    className={`form-control ep-input ${formErrors.photoPath ? 'is-invalid' : ''}`}
                    placeholder="https://example.com/avatar.jpg"
                    value={formData.photoPath}
                    onChange={handleChange}
                    style={{ height: '44px', borderRadius: 'var(--radius-xs)' }}
                  />
                  {formErrors.photoPath && (
                    <div className="invalid-feedback text-danger small mt-1">{formErrors.photoPath}</div>
                  )}
                </div>

                {/* Is Active Toggle Switch */}
                <div className="form-check form-switch mb-2 pt-2">
                  <input
                    className="form-check-input"
                    type="checkbox"
                    role="switch"
                    id="isActiveSwitch"
                    name="isActive"
                    checked={formData.isActive}
                    onChange={handleChange}
                    style={{ cursor: 'pointer' }}
                  />
                  <label className="form-check-label fw-medium text-dark ms-2" htmlFor="isActiveSwitch" style={{ cursor: 'pointer' }}>
                    Active Status (Checked indicates active staff)
                  </label>
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
                  {modalMode === 'create' ? 'Create Staff Record' : 'Save Changes'}
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
        itemName={staffToDelete ? `${staffToDelete.firstName} ${staffToDelete.lastName}` : ''}
      />
    </div>
  );
};

export default StaffDirectory;
