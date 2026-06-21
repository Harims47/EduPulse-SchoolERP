import React, { useState, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import apiClient from '../../services/apiClient';
import { addNotification } from '../../store/slices/uiSlice';
import DataTable from '../../components/ui/DataTable';
import ConfirmDeleteModal from '../../components/feedback/ConfirmDeleteModal';
import PermissionGate from '../../components/PermissionGate';
import { validateForm, required, maxLength } from '../../utils/validation';

const SectionManager = () => {
  const dispatch = useDispatch();
  const { user } = useSelector((state) => state.auth);
  const isAdmin = user?.role === 'SchoolAdmin';

  const [sections, setSections] = useState([]);
  const [loading, setLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  
  // Modals state
  const [showModal, setShowModal] = useState(false);
  const [modalMode, setModalMode] = useState('create'); // 'create' | 'edit'
  const [selectedSection, setSelectedSection] = useState(null);
  
  // Delete modal state
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [sectionToDelete, setSectionToDelete] = useState(null);

  // Form state
  const [formData, setFormData] = useState({
    name: ''
  });
  const [formErrors, setFormErrors] = useState({});

  // Fetch sections
  const fetchSections = async () => {
    setLoading(true);
    try {
      const response = await apiClient.get('/api/sections');
      setSections(response.data || []);
    } catch (error) {
      console.error('Error fetching sections:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSections();
  }, []);

  // Open modal for Create
  const handleOpenCreate = () => {
    setFormData({
      name: ''
    });
    setFormErrors({});
    setModalMode('create');
    setShowModal(true);
  };

  // Open modal for Edit
  const handleOpenEdit = (section) => {
    setSelectedSection(section);
    setFormData({
      name: section.name || ''
    });
    setFormErrors({});
    setModalMode('edit');
    setShowModal(true);
  };

  // Open delete confirm modal
  const handleOpenDelete = (section) => {
    setSectionToDelete(section);
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
      name: [required, (val) => maxLength(val, 10)]
    };

    const { isValid, errors } = validateForm(formData, rules);

    if (!isValid) {
      setFormErrors(errors);
      return;
    }

    try {
      if (modalMode === 'create') {
        await apiClient.post('/api/sections', formData);
        dispatch(addNotification({
          message: 'Section created successfully!',
          type: 'success'
        }));
      } else {
        await apiClient.put(`/api/sections/${selectedSection.sectionId}`, formData);
        dispatch(addNotification({
          message: 'Section updated successfully!',
          type: 'success'
        }));
      }
      setShowModal(false);
      fetchSections();
    } catch (error) {
      console.error('Error submitting form:', error);
    }
  };

  // Confirm delete
  const handleConfirmDelete = async () => {
    if (!sectionToDelete) return;
    try {
      await apiClient.delete(`/api/sections/${sectionToDelete.sectionId}`);
      dispatch(addNotification({
        message: 'Section deleted successfully!',
        type: 'success'
      }));
      setShowDeleteModal(false);
      setSectionToDelete(null);
      fetchSections();
    } catch (error) {
      console.error('Error deleting section:', error);
    }
  };

  // Filter sections based on search term
  const filteredSections = sections.filter((s) =>
    s.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const columns = [
    {
      header: 'Section Name',
      accessor: 'name',
      render: (row) => <span className="fw-semibold text-dark">{row.name}</span>
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
            title="Edit Section"
          >
            ✏️
          </button>
          <button
            type="button"
            className="btn btn-sm btn-outline-danger py-1 px-2 border-0"
            onClick={() => handleOpenDelete(row)}
            title="Delete Section"
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
          <h4 className="fw-bold text-dark mb-1" style={{ fontSize: 'var(--font-size-2xl)' }}>Sections & Divisions</h4>
          <p className="text-muted mb-0" style={{ fontSize: 'var(--font-size-sm)' }}>
            Configure classroom sections, sub-divisions, and labels.
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
            <span>+</span> Add Section
          </button>
        </PermissionGate>
      </div>

      {/* Main Roster Card */}
      <div className="ep-card bg-white p-4" style={{ borderRadius: 'var(--radius-lg)', boxShadow: 'var(--shadow-sm)' }}>
        <DataTable
          columns={columns}
          data={filteredSections}
          loading={loading}
          searchValue={searchTerm}
          onSearchChange={setSearchTerm}
          searchPlaceholder="Search sections..."
          emptyMessage="No sections created yet. Click Add Section to register divisions."
          emptyActionLabel={isAdmin ? "Add Section" : null}
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
                {modalMode === 'create' ? 'Add Section' : 'Edit Section'}
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
                {/* Section Name */}
                <div className="mb-3">
                  <label className="ep-form-label mb-1 d-block text-uppercase fw-semibold text-muted" style={{ fontSize: 'var(--font-size-xs)' }}>
                    Section Name *
                  </label>
                  <input
                    type="text"
                    name="name"
                    className={`form-control ep-input ${formErrors.name ? 'is-invalid' : ''}`}
                    placeholder="e.g., A, B, Red, Gold"
                    value={formData.name}
                    onChange={handleChange}
                    style={{ height: '44px', borderRadius: 'var(--radius-xs)' }}
                  />
                  {formErrors.name && (
                    <div className="invalid-feedback text-danger small mt-1">{formErrors.name}</div>
                  )}
                  <small className="text-muted">Maximum length is 10 characters.</small>
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
                  {modalMode === 'create' ? 'Create Section' : 'Save Changes'}
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
        itemName={sectionToDelete ? sectionToDelete.name : ''}
      />
    </div>
  );
};

export default SectionManager;
