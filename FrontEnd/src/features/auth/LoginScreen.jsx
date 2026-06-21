import React, { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { loginStart, loginSuccess, loginFailure } from '../../store/slices/authSlice';
import apiClient from '../../services/apiClient';

const LoginScreen = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [validationError, setValidationError] = useState('');
  
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { loading, error } = useSelector((state) => state.auth);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setValidationError('');

    if (!email || !password) {
      setValidationError('Please enter both Email and Password.');
      return;
    }

    dispatch(loginStart());
    try {
      const response = await apiClient.post('/api/auth/login', { email, password });
      const { token } = response.data;
      dispatch(loginSuccess(token));
      navigate('/dashboard');
    } catch (err) {
      console.error(err);
      const errMsg = err.response?.data?.message || 'Invalid email or password.';
      dispatch(loginFailure(errMsg));
    }
  };

  return (
    <div className="card border-0 shadow-lg p-3 py-4" style={{ 
      borderRadius: 'var(--radius-lg)', 
      backgroundColor: 'var(--color-bg-surface)',
      boxShadow: '0 20px 25px -5px rgba(15, 23, 42, 0.05), 0 8px 10px -6px rgba(15, 23, 42, 0.03)'
    }}>
      <div className="card-body">
        {/* Branding header */}
        <div className="text-center mb-4">
          <div className="d-inline-flex align-items-center justify-content-center bg-primary text-white rounded-3 mb-3" style={{ width: '48px', height: '48px', backgroundColor: 'var(--color-primary)' }}>
            <span className="fs-5 fw-bold">EP</span>
          </div>
          <h4 className="fw-bold m-0" style={{ color: 'var(--color-text-primary)', letterSpacing: '-0.02em' }}>Welcome to EduPulse</h4>
          <p className="text-muted small mt-1">Enter your credentials to access the portal</p>
        </div>

        {/* Login Form */}
        <form onSubmit={handleSubmit} noValidate>
          {/* Email input */}
          <div className="mb-3">
            <label className="ep-form-label" htmlFor="email-address">Email Address</label>
            <input
              id="email-address"
              type="email"
              className={`ep-input ${validationError || error ? 'is-invalid' : ''}`}
              placeholder="name@schoolerp.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              disabled={loading}
              required
              style={{ borderRadius: 'var(--radius-xs)' }}
            />
          </div>

          {/* Password input */}
          <div className="mb-4">
            <div className="d-flex justify-content-between align-items-center">
              <label className="ep-form-label" htmlFor="user-password">Password</label>
            </div>
            <div className="position-relative">
              <input
                id="user-password"
                type={showPassword ? 'text' : 'password'}
                className={`ep-input ${validationError || error ? 'is-invalid' : ''}`}
                placeholder="••••••••"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                disabled={loading}
                required
                style={{ paddingRight: '45px', borderRadius: 'var(--radius-xs)' }}
              />
              <button
                type="button"
                className="btn btn-link position-absolute end-0 top-50 translate-middle-y text-decoration-none px-3"
                onClick={() => setShowPassword(!showPassword)}
                style={{ color: 'var(--color-text-muted)', fontSize: 'var(--font-size-xs)' }}
              >
                {showPassword ? 'Hide' : 'Show'}
              </button>
            </div>
          </div>

          {/* Validation/API error alerts */}
          {(validationError || error) && (
            <div className="alert alert-danger py-2 px-3 small border-0 mb-3 text-center" style={{ borderRadius: 'var(--radius-sm)' }}>
              {validationError || error}
            </div>
          )}

          {/* Submit button */}
          <button
            type="submit"
            className="btn btn-primary w-100 d-flex align-items-center justify-content-center py-2"
            disabled={loading}
            style={{ borderRadius: 'var(--radius-xs)', height: '44px', fontSize: 'var(--font-size-sm)' }}
          >
            {loading ? (
              <>
                <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                Validating...
              </>
            ) : (
              'Sign In'
            )}
          </button>
        </form>
      </div>
    </div>
  );
};

export default LoginScreen;
